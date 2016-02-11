// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Interop;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeyboardConcerto.Theme;
using KeyboardConcerto.RawInput;
using System.Reflection;
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
		private IntPtr mHandle;

		private static RawKeyboard mKeyboardDriver;
		private IntPtr mDeviceNotifyHandle;
		private static readonly Guid mDeviceInterfaceHID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

		private UserSettings mUserSettings;
		private Deque<Decision> mDecisionQueue;

		private MacrosWindow mMacrosWindow;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainWindow() {
			this.InitializeComponent();
			this.mDecisionQueue = new Deque<Decision>();
			this.mUserSettings = new UserSettings();

			this.Closing += new CancelEventHandler(this.OnWindowClosing);

			AppDomain.CurrentDomain.UnhandledException 
				+= new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

			Win32.DeviceAudit();

			this.InitializeMacrosWindow();

			this.mUserSettings = KeyboardConcerto.Tests.UserSettingsTests.BasicUserSettings();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hwnd"></param>
		private void InitializeGlass(IntPtr hWnd) {
			if (!Win32Interop.DwmIsCompositionEnabled())
				return;

			// fill the background with glass
			var margins = new MARGINS();
			margins.cxLeftWidth = margins.cxRightWidth = margins.cyBottomHeight = margins.cyTopHeight = -1;
			Win32Interop.DwmExtendFrameIntoClientArea(hWnd, ref margins);
		}

		/// <summary>
		/// Initialize the hooks for keyboard and wndproc. Grabs the window handle as well.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnSourceInitialized(EventArgs e) {
			base.OnSourceInitialized(e);
			HwndSource source = PresentationSource.FromVisual(this) as HwndSource;

			source.AddHook(WndProc);

			this.mHandle = source.Handle;

			if (Environment.OSVersion.Version.Major >= 6) {
				source.CompositionTarget.BackgroundColor = Colors.Transparent;
				this.InitializeGlass(this.mHandle);
			}

			mKeyboardDriver = new RawKeyboard(this.mHandle);
			mKeyboardDriver.EnumerateDevices();
			mKeyboardDriver.CaptureOnlyIfTopMostWindow = false;
			mDeviceNotifyHandle = RegisterForDeviceNotifications(this.mHandle);

			InstallHook(this.mHandle);
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

		/// <summary>
		/// 
		/// </summary>
		protected void InitializeMacrosWindow() {
			this.mMacrosWindow = new MacrosWindow();
			this.mMacrosWindow.InitializeComponent();
			this.mMacrosWindow.Hide();
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

			// [TESTING] Prints out the user input's origin device's name.
			Debug.WriteLine(keyPressEvent.DeviceName);

			// [TESTING] Prints out the state of the key.
			Debug.WriteLine(keyPressEvent.KeyPressState);
		}

		/// <summary>
		/// Processes all WM_INPUT and WM_HOOK messages sent from windows and the interceptor, respectively.
		/// <para>Determines whether certain user input will be allowed or blocked depending on UserSettings.</para>
		/// </summary>
		/// <param name="hWnd">The window handle.</param>
		/// <param name="msg">A windows message.</param>
		/// <param name="wParam">
		/// Word param for the message. 
		/// Carries the input code (irrelevant to this implementation) for WM_INPUT.
		/// Carries the virtual key of the hooked user input/key press.
		/// </param>
		/// <param name="lParam">
		/// Long param for the message.
		/// Carries the RAWINPUT structure that contains raw input information.
		/// Carries the flags of the hooked user input/key press, such as key state.
		/// </param>
		/// <param name="handled">
		/// Indicates if the message is handled by this method call. Must be set 
		/// to true in order for the return value to be considered by the callback.
		/// </param>
		/// <returns>0 to allow input and 1 to deny input.</returns>
		private IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled) {
			switch (msg) {
				#region Win32.WM_INPUT
				case Win32.WM_INPUT: {
						// Should never get here if you are using PreMessageFiltering
						KeyPressEvent keyPressEvent;
						if (mKeyboardDriver.ProcessRawInput(lParam, out keyPressEvent)) {
							this.ProcessKeyboard(keyPressEvent);
						}
						return (IntPtr)0;
					}
				#endregion
				#region Win32.WM_USB_DEVICECHANGE
				case Win32.WM_USB_DEVICECHANGE: {
						Debug.WriteLine("USB Device Arrival / Removal");
						mKeyboardDriver.EnumerateDevices();
						return (IntPtr)0;
					}
				#endregion
				#region WM_HOOK
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
				#endregion
				#region Win32Interop.WM_DWMCOMPOSITIONCHANGED
				case Win32Interop.WM_DWMCOMPOSITIONCHANGED:
					this.InitializeGlass(hWnd);
					handled = false;
					return (IntPtr)0;
				#endregion
				default:
					return (IntPtr)0;
			}
		}
		#endregion

		#region Button Event Handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnMacrosButton_Click(object sender, System.Windows.RoutedEventArgs e) {
			// TODO: Add event handler implementation here.
			if (!this.mMacrosWindow.IsVisible)
				this.mMacrosWindow.Show();
			else
				this.mMacrosWindow.Activate();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnSettingsButton_Click(object sender, System.Windows.RoutedEventArgs e) {
			// TODO: Add event handler implementation here.
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

		#region Program Entry
		[STAThreadAttribute]
		public static void Main() {
			var assemblies = new Dictionary<string, Assembly>();
			var executingAssembly = Assembly.GetExecutingAssembly();
			var resources = executingAssembly.GetManifestResourceNames().Where(n => n.EndsWith(".dll"));

			foreach (string resource in resources) {
				using (var stream = executingAssembly.GetManifestResourceStream(resource)) {
					if (stream == null)
						continue;

					var bytes = new byte[stream.Length];
					stream.Read(bytes, 0, bytes.Length);
					try {
						assemblies.Add(resource, Assembly.Load(bytes));
					} catch (Exception ex) {
						System.Diagnostics.Debug.Print(string.Format("Failed to load: {0}, Exception: {1}", resource, ex.Message));
					}
				}
			}

			AppDomain.CurrentDomain.AssemblyResolve += (s, e) => {
				var assemblyName = new AssemblyName(e.Name);

				var path = string.Format("{0}.dll", assemblyName.Name);

				if (assemblies.ContainsKey(path)) {
					return assemblies[path];
				}

				return null;
			};

			App.Main();
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
