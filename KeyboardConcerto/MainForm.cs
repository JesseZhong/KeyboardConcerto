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
using RawInput_dll;
using Common;
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
		private class PreMessageFilter : IMessageFilter {
			public bool PreFilterMessage(ref Message m) {
				if (m.Msg != WM_INPUT) {
					// Allow any non WM_INPUT message to pass through
					return false;
				}

				return mKeyboardDriver.ProcessRawInput(m.LParam);
			}
		}

		private static RawKeyboard mKeyboardDriver;
		private readonly IntPtr mDeviceNotifyHandle;
		private static readonly Guid mDeviceInterfaceHID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");
		private PreMessageFilter mFilter;

		private UserSettings mUserSettings;

		private Queue<Decision> mDecisionQueue;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainForm() {
			this.InitializeComponent();
			this.mDecisionQueue = new Queue<Decision>();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			IntPtr accessHandle = this.Handle; // Ensure that the handle is created.

			mKeyboardDriver = new RawKeyboard(this.Handle);
			mKeyboardDriver.EnumerateDevices();
			mKeyboardDriver.CaptureOnlyIfTopMostWindow = false;
			mDeviceNotifyHandle = RegisterForDeviceNotifications(this.Handle);
			Application.AddMessageFilter(this.mFilter = new PreMessageFilter());
			mKeyboardDriver.KeyPressed += OnKeyPressed;

			this.mUserSettings = new UserSettings();

			InstallHook(this.Handle);

			Win32.DeviceAudit();
		}

		struct BroadcastDeviceInterface {
			public Int32 dbcc_size;
			public BroadcastDeviceType BroadcastDeviceType;
			public Guid dbcc_classguid;
		}

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
				usbNotifyHandle = RegisterDeviceNotification(parent, mem, DeviceNotification.DEVICE_NOTIFY_WINDOW_HANDLE);
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
		/// Processes the input and enqueues the decision to be executed later.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPressed(object sender, InputEventArg e) {
			Decision dc;
			this.mDecisionQueue.Enqueue(dc = new Decision() {
				Key = (Keys)e.KeyPressEvent.VKey,
				State = e.KeyPressEvent.KeyPressState,
				Allow = !this.mUserSettings.ProcessInput(e)
			});
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message msg) {
			const string makeStr = "MAKE";
			const string breakStr = "BREAK";

			base.WndProc(ref msg);

			switch (msg.Msg) {
				case WM_INPUT: {
						// Should never get here if you are using PreMessageFiltering
						mKeyboardDriver.ProcessRawInput(msg.LParam);
					}
					return;

				case WM_USB_DEVICECHANGE: {
						Debug.WriteLine("USB Device Arrival / Removal");
						mKeyboardDriver.EnumerateDevices();
					}
					return;

				case WM_HOOK: {
						long lparam = (long)msg.LParam;
						HookParams hParams = new HookParams() {
							Key = (Keys)(uint)msg.WParam,
							State = ((lparam >> 31 & 0x1) == 1) ? breakStr : makeStr,		// WM_DOWN is 0; WM_UP is 1
							PrevState = ((lparam >> 30 & 0x1) == 1) ? makeStr : breakStr,	// WM_UP is 0; WM_DOWN is 0 (super confusing)
							AltState = ((lparam >> 29 & 0x1) == 1) ? makeStr : breakStr,	// WM_UP is 0; WM_DOWN is 0
							ExtendedKey = (lparam >> 24 & 0x1) == 1,
							ScanCode = (byte)(lparam >> 16 & 0xF),
							RepeatCount = (short)(lparam & 0xFF)
						};

						Stopwatch timer = new Stopwatch();
						timer.Start();
						while (true) {

							// Time out if no matching Raw Input is found after a while.
							if (timer.ElapsedMilliseconds > MAX_WAIT_TIME) {
								msg.Result = (IntPtr)0;
								timer.Stop();
								return;
							}

							// Search if there's a corresponding raw input decision.
							// Remove the current and all preceding messages from the queue.
							for (int i = 0, count = this.mDecisionQueue.Count; i < count; i++) {
								Decision decision = this.mDecisionQueue.Dequeue();
								if ((decision.Key == hParams.Key) && (decision.State == hParams.State)) {
									msg.Result = decision.Allow ? (IntPtr)0 : (IntPtr)1;
									return;
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

			UnregisterDeviceNotification(mDeviceNotifyHandle);
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

		#region Raw Input
		private const int WM_INPUT = 0x00FF;
		private const int WM_USB_DEVICECHANGE = 0x0219;

		enum BroadcastDeviceType {
			DBT_DEVTYP_OEM = 0,
			DBT_DEVTYP_DEVNODE = 1,
			DBT_DEVTYP_VOLUME = 2,
			DBT_DEVTYP_PORT = 3,
			DBT_DEVTYP_NET = 4,
			DBT_DEVTYP_DEVICEINTERFACE = 5,
			DBT_DEVTYP_HANDLE = 6,
		}

		enum DeviceNotification {
			/// <summary>The hRecipient parameter is a window handle.</summary>
			DEVICE_NOTIFY_WINDOW_HANDLE = 0x00000000,
			/// <summary>The hRecipient parameter is a service status handle.</summary>
			DEVICE_NOTIFY_SERVICE_HANDLE = 0x00000001,
			/// <summary>
			/// Notifies the recipient of device interface events for all device interface classes. (The dbcc_classguid member is ignored.)
			/// This value can be used only if the dbch_devicetype member is DBT_DEVTYP_DEVICEINTERFACE.
			///</summary>
			DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 0x00000004
		}

		[DllImport("user32.dll", SetLastError = true)]
		private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr notificationFilter, DeviceNotification flags);

		[DllImport("user32.dll", SetLastError = true)]
		private static extern bool UnregisterDeviceNotification(IntPtr handle);
		#endregion

		#region Hooking
		/// <summary>
		/// Global keyboard hook.
		/// </summary>
		private const int WM_HOOK = 0x8001;

		/// <summary>
		/// Structure used to deserialize hooking messages for WH_KEYBOARD.
		/// </summary>
		private struct HookParams {
			public Keys Key;
			public string State;
			public string PrevState;
			public string AltState;
			public bool ExtendedKey;
			public byte ScanCode;
			public short RepeatCount;
		}

		[DllImport("Interceptor.dll")]
		private static extern bool InstallHook(IntPtr hWndParent);

		[DllImport("Interceptor.dll")]
		private static extern bool UninstallHook();
		#endregion
	}
}
