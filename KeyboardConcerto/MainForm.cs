// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
#endregion

namespace KeyboardConcerto {
	public partial class MainForm : Form {

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
		private readonly IntPtr mDeviceNotifyHandle;
		private static readonly Guid mDeviceInterfaceHID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
		private PreMessageFilter mFilter;

		private UserSettings mUserSettings;

		private Deque<Decision> mDecisionQueue;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainForm() {
			this.InitializeComponent();
			this.mDecisionQueue = new Deque<Decision>();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			IntPtr accessHandle = this.Handle; // Ensure that the handle is created.

			mKeyboardDriver = new RawKeyboard(this.Handle);
			mKeyboardDriver.EnumerateDevices();
			mKeyboardDriver.CaptureOnlyIfTopMostWindow = false;
			mDeviceNotifyHandle = RegisterForDeviceNotifications(this.Handle);
			Application.AddMessageFilter(this.mFilter = new PreMessageFilter(this.ProcessKeyboard));

			this.mUserSettings = new UserSettings();

			InstallHook(this.Handle);

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

		/// <summary>
		/// Ensure that the messages are still received when the form is minimized.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			IntPtr HWND_MESSAGE = new IntPtr(-3);
			SetParent(this.Handle, HWND_MESSAGE);
		}
		#endregion
		
		#region Keyboard Handling
		/// <summary>
		/// Tests raw input and stores the result to be applied later.
		/// </summary>
		/// <param name="keyPressEvent">Raw input event.</param>
		private void ProcessKeyboard(KeyPressEvent keyPressEvent) {
			this.mDecisionQueue.AddToBack(new Decision() {
				Key = (Keys)keyPressEvent.VKey,
				State = keyPressEvent.KeyPressState,
				Allow = !this.mUserSettings.ProcessInput(keyPressEvent)
			});
		}

		/// <summary>
		/// Pre-filters windows messages for WM_INPUT/raw input.
		/// </summary>
		private class PreMessageFilter : IMessageFilter {

			public delegate void ProcessKeyboard(KeyPressEvent keyPressEvent);
			private ProcessKeyboard mProcessKeyboard;

			/// <summary>
			/// Initializes the filter with the keyboard processing method.
			/// </summary>
			/// <param name="processKeyboard">Raw input process method.</param>
			public PreMessageFilter(ProcessKeyboard processKeyboard) {
				this.mProcessKeyboard = processKeyboard;
			}

			/// <summary>
			/// Pre-filters message for raw input and processes it.
			/// </summary>
			/// <param name="msg">Message that needs processing.</param>
			/// <returns>True if the message is a WM_INPUT message.</returns>
			public bool PreFilterMessage(ref Message msg) {
				if (msg.Msg != Win32.WM_INPUT) {
					// Allow any non WM_INPUT message to pass through
					return false;
				}
				KeyPressEvent keyPressEvent;
				bool result = mKeyboardDriver.ProcessRawInput(msg.LParam, out keyPressEvent);
				if (mKeyboardDriver.ProcessRawInput(msg.LParam, out keyPressEvent)) {
					if (this.mProcessKeyboard != null)
						this.mProcessKeyboard(keyPressEvent);
				}
				return result;
			}
		}

		/// <summary>
		/// Processes and intercepts user input.
		/// </summary>
		/// <param name="msg">Windows message.</param>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message msg) {
			base.WndProc(ref msg);

			switch (msg.Msg) {
				case Win32.WM_INPUT: {
						// Should never get here if you are using PreMessageFiltering
						KeyPressEvent keyPressEvent;
						if (mKeyboardDriver.ProcessRawInput(msg.LParam, out keyPressEvent)) {
							this.ProcessKeyboard(keyPressEvent);
						}
						return;
					}

				case Win32.WM_USB_DEVICECHANGE: {
						Debug.WriteLine("USB Device Arrival / Removal");
						mKeyboardDriver.EnumerateDevices();
						return;
					}

				case WM_HOOK: {
						const string makeStr = "MAKE";
						const string breakStr = "BREAK";
						long lparam = (long)msg.LParam;

						Keys key = (Keys)(uint)msg.WParam;
						string state = ((lparam >> 31 & 0x1) == 1) ? breakStr : makeStr;		// WM_DOWN is 0; WM_UP is 1

						// Search if there's a corresponding raw input decision.
						// Remove the current and all preceding messages from the queue.
						for (int i = 0, count = this.mDecisionQueue.Count; i < count; i++) {
							Decision decision = this.mDecisionQueue.RemoveFromFront();
							if ((decision.Key == key) && (decision.State == state)) {
								msg.Result = decision.Allow ? (IntPtr)0 : (IntPtr)1;
								return;
							}
						}

						Stopwatch sw = new Stopwatch();
						sw.Start();
						while (true) {
							Message rawMsg = new Message();
							while (!Win32.PeekMessage(ref rawMsg, this.Handle, Win32.WM_INPUT, Win32.WM_INPUT, Win32.PM_REMOVE)) {
								if (MAX_WAIT_TIME < sw.ElapsedMilliseconds) {
									sw.Stop();
									return;
								}
							}

							KeyPressEvent keyPressEvent;
							if (mKeyboardDriver.ProcessRawInput(rawMsg.LParam, out keyPressEvent)) {
								Keys riKey = (Keys)keyPressEvent.VKey;
								string riState = keyPressEvent.KeyPressState;
								bool riAllow = !this.mUserSettings.ProcessInput(keyPressEvent);

								if ((riKey == key) && (riState == state)) {
									msg.Result = riAllow ? (IntPtr)0 : (IntPtr)1;
									sw.Stop();
									return;
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
					return;
			}
		}
		#endregion

		#region Managing Resources
		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		public new void Dispose() {
			if (this.IsDisposed)
				return;

			Win32.UnregisterDeviceNotification(mDeviceNotifyHandle);
			UninstallHook();

			base.Dispose();
		}
		#endregion

		#region Program Entry Point
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
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
			MessageBox.Show(ex.Message);
		}
		#endregion

		#region Windows API
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		#endregion

		#region Hooking
		/// <summary>
		/// Global keyboard hook.
		/// </summary>
		private const int WM_HOOK = 0x8001;

		[DllImport("Interceptor.dll")]
		private static extern bool InstallHook(IntPtr hWndParent);

		[DllImport("Interceptor.dll")]
		private static extern bool UninstallHook();
		#endregion
	}
}
