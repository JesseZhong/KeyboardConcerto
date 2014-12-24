// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeyboardConcerto.RawInput;
#endregion

namespace KeyboardConcerto {
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window {

		#region Constants
		/// <summary>
		/// The amount of time the hook can wait for its respective Raw Input before timing out.
		/// </summary>
		/// <remarks>
		/// Measured in milliseconds.
		/// </remarks>
		private const long MAX_WAIT_TIME = 60;
		#endregion

		#region Members
		private static RawKeyboard mKeyboardDriver;
		private IntPtr mDeviceNotifyHandle;
		private static readonly Guid mDeviceInterfaceHID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

		private UserSettings mUserSettings;

		private Deque<Decision> mDecisionQueue;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainWindow() {
			this.InitializeComponent();
			this.Closing += this.OnWindowClosing;

			this.mDecisionQueue = new Deque<Decision>();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			this.mUserSettings = new UserSettings();

			Win32.DeviceAudit();
		}

		/// <summary>
		/// Registers window handle for device notifications.
		/// </summary>
		/// <param name="parent">Window handle.</param>
		/// <returns></returns>
		static IntPtr RegisterForDeviceNotifications(IntPtr parent) {
			var usbNotifyHandle = IntPtr.Zero;
			var bdi = new BroadcastDeviceInterface();
			bdi.dbcc_size = Marshal.SizeOf(bdi);
			bdi.BroadcastDeviceType = BroadcastDeviceType.DBT_DEVTYP_DEVICEINTERFACE;
			bdi.dbcc_classguid = mDeviceInterfaceHID;

			var mem = IntPtr.Zero;
			try {
				mem = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(BroadcastDeviceInterface)));
				Marshal.StructureToPtr(bdi, mem, false);
				usbNotifyHandle = Win32.RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
			} catch (Exception e) {
				Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
				Debug.Print(e.StackTrace);
			} finally {
				Marshal.FreeHGlobal(mem);
			}

			if (usbNotifyHandle == IntPtr.Zero) {
				Debug.Print("Registration for device notifications Failed. Error: {0}", Marshal.GetLastWin32Error());
			}

			return usbNotifyHandle;
		}

		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

			source.AddHook(WndProc);

			mKeyboardDriver = new RawKeyboard(source.Handle);
			mKeyboardDriver.EnumerateDevices();
			mKeyboardDriver.CaptureOnlyIfTopMostWindow = false;
			mDeviceNotifyHandle = RegisterForDeviceNotifications(source.Handle);

			InstallHook(source.Handle);
		}
		#endregion

		#region Keyboard Processing
		/// <summary>
		/// Tests raw input and stores the result to be applied later.
		/// <para><b>WARNING: DO NOT USE/ATTACH DEBUGGER!! THE APPLICATION WILL HANG INDEFINITELY.</b></para>
		/// </summary>
		/// <param name="keyPressEvent">Raw input event.</param>
		private void ProcessKeyboard(KeyPressEvent keyPressEvent) {
			this.mDecisionQueue.AddToBack(new Decision() {
				Key = (VirtualKeys)keyPressEvent.VKey,
				State = keyPressEvent.KeyPressState,
				Allow = !this.mUserSettings.ProcessInput(keyPressEvent)
			});
		}

		private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch (msg) {
				case Win32.WM_INPUT: {
						// Should never get here if you are using PreMessageFiltering
						KeyPressEvent keyPressEvent;
						if (mKeyboardDriver.ProcessRawInput(lParam, out keyPressEvent)) {
							this.ProcessKeyboard(keyPressEvent);
						}
						return (IntPtr)0;
					}

				case Win32.WM_USB_DEVICECHANGE: {
						Debug.WriteLine("USB Device Arrival / Removal");
						mKeyboardDriver.EnumerateDevices();
						return (IntPtr)0;
					}

				case WM_HOOK: {
					handled = true;
						const string makeStr = "MAKE";
						const string breakStr = "BREAK";
						long lparam = (long)lParam;

						VirtualKeys key = (VirtualKeys)(uint)wParam;
						string state = ((lparam >> 31 & 0x1) == 1) ? breakStr : makeStr;		// WM_DOWN is 0; WM_UP is 1

						// Search if there's a corresponding raw input decision.
						// Remove the current and all preceding messages from the queue.
						for (int i = 0, count = this.mDecisionQueue.Count; i < count; i++) {
							Decision decision = this.mDecisionQueue.RemoveFromFront();
							if ((decision.Key == key) && (decision.State == state)) {
								return decision.Allow ? (IntPtr)0 : (IntPtr)1;
							}
						}

						Stopwatch sw = new Stopwatch();
						sw.Start();
						while (true) {
							NativeMessage rawMsg = new NativeMessage();
							while (!Win32.PeekMessage(out rawMsg, hWnd, Win32.WM_INPUT, Win32.WM_INPUT, Win32.PM_REMOVE)) {
								if (MAX_WAIT_TIME < sw.ElapsedMilliseconds) {
									sw.Stop();
									return (IntPtr)0;
								}
							}

							KeyPressEvent keyPressEvent;
							if (mKeyboardDriver.ProcessRawInput(rawMsg.LParam, out keyPressEvent)) {
								VirtualKeys riKey = (VirtualKeys)keyPressEvent.VKey;
								string riState = keyPressEvent.KeyPressState;
								bool riAllow = !this.mUserSettings.ProcessInput(keyPressEvent);

								if ((riKey == key) && (riState == state)) {
									sw.Stop();
									return riAllow ? (IntPtr)0 : (IntPtr)1;
								} else {
									this.mDecisionQueue.AddToBack(new Decision() {
										Key = riKey,
										State = riState,
										Allow = riAllow
									});
								}
							}
						}
					}

				default:
					return (IntPtr)0;
			}
		}
		#endregion

		#region Program Entry Point
		/// <summary>
		/// Application Entry Point.
		/// </summary>
		[System.STAThreadAttribute()]
		[System.Diagnostics.DebuggerNonUserCodeAttribute()]
		public static void Main() {
			KeyboardConcerto.App app = new KeyboardConcerto.App();
			app.InitializeComponent();
			app.Run();
		}
		#endregion

		#region Exception Handling
		private void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e) {
			var ex = e.ExceptionObject as Exception;

			if (null == ex) return;

			// Log this error. Logging the exception doesn't correct the problem but at least now
			// you may have more insight as to why the exception is being thrown.
			Debug.WriteLine("Unhandled Exception: " + ex.Message);
			Debug.WriteLine("Unhandled Exception: " + ex);
		}
		#endregion

		#region Destruction
		private void OnWindowClosing(object sender, EventArgs e) {

			Win32.UnregisterDeviceNotification(mDeviceNotifyHandle);
			UninstallHook();

			IntPtr windowHandle = (new WindowInteropHelper(this)).Handle;
			HwndSource src = HwndSource.FromHwnd(windowHandle);
			src.RemoveHook(new HwndSourceHook(this.WndProc));
		}
		#endregion

		#region Hooking
		/// <summary>
		/// Global keyboard hook.
		/// </summary>
		private const int WM_HOOK = 0x8001;

		[DllImport("Interceptor.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool InstallHook(IntPtr hWndParent);

		[DllImport("Interceptor.dll", CallingConvention = CallingConvention.Cdecl)]
		private static extern bool UninstallHook();
		#endregion
	}
}
