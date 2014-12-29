// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
using System.Linq;
using System.Windows;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using KeyboardConcerto.CustomWindow;
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
		private IntPtr mHandle;

		private static RawKeyboard mKeyboardDriver;
		private IntPtr mDeviceNotifyHandle;
		private static readonly Guid mDeviceInterfaceHID = new Guid("4D1E55B2-F16F-11CF-88CB-001111000030");

		private UserSettings mUserSettings;
		private Deque<Decision> mDecisionQueue;

		private WindowMinimizeButton mMinimizeButton;
		private WindowRestoreButton mRestoreButton;
		private WindowMaximizeButton mMaximizeButton;
		private WindowCloseButton mCloseButton;

 		private UIElement mWindowButtons;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainWindow() {
			this.InitializeComponent();
			this.mDecisionQueue = new Deque<Decision>();
			this.mUserSettings = new UserSettings();

			this.WindowStyle = WindowStyle.None; 
			this.ResizeMode = ResizeMode.NoResize;
			this.Background = Brushes.Transparent;

			mWindowButtons = this.GenerateWindowButtons();

			this.Loaded += new RoutedEventHandler(this.OnLoaded);
			this.Closing += new CancelEventHandler(this.OnWindowClosing);
			this.StateChanged += new EventHandler(this.StandardWindow_StateChanged);
			this.Activated += new EventHandler(this.OnStandardWindowActivated);
			this.Deactivated += new EventHandler(this.OnStandardWindowDeactivated);

			AppDomain.CurrentDomain.UnhandledException 
				+= new UnhandledExceptionEventHandler(this.CurrentDomain_UnhandledException);

			Win32.DeviceAudit();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		private Decorator GetWindowButtonsPlaceholder() {
			return WindowButtonsPlaceholder;
		}

		/// <summary>
		/// Initialize windows border buttons.
		/// </summary>
		/// <returns></returns>
		private UIElement GenerateWindowButtons() {
			// Buttons
			this.mMinimizeButton = new WindowMinimizeButton();
			this.mMinimizeButton.Click += new RoutedEventHandler(this.OnButtonMinimize_Click);

			this.mRestoreButton = new WindowRestoreButton();
			this.mRestoreButton.Click += new RoutedEventHandler(this.OnButtonRestore_Click);
			this.mRestoreButton.Margin = new Thickness(-1, 0, 0, 0);

			this.mMaximizeButton = new WindowMaximizeButton();
			this.mMaximizeButton.Click += new RoutedEventHandler(this.OnButtonMaximize_Click);
			this.mMaximizeButton.Margin = new Thickness(-1, 0, 0, 0);

			this.mCloseButton = new WindowCloseButton();
			this.mCloseButton.Click += new RoutedEventHandler(this.OnButtonClose_Click);
			this.mCloseButton.Margin = new Thickness(-1, 0, 0, 0);

			// put buttons into StackPanel
			StackPanel buttonsStackPanel = new StackPanel();
			buttonsStackPanel.Orientation = Orientation.Horizontal;
			buttonsStackPanel.Children.Add(this.mMinimizeButton);
			buttonsStackPanel.Children.Add(this.mRestoreButton);
			buttonsStackPanel.Children.Add(this.mMaximizeButton);
			buttonsStackPanel.Children.Add(this.mCloseButton);

			return buttonsStackPanel;
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
		/// Extends windows glass.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnLoaded(object sender, RoutedEventArgs e) {
			Decorator placeholder = GetWindowButtonsPlaceholder();

			if (placeholder == null)
				throw new NotSupportedException("Placeholder must be created already in the initialization of the Window");

			placeholder.Child = mWindowButtons;
			this.OnStateChanged(new EventArgs());
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

		#region Mouse Move Event Handling
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Header_MouseMove(object sender, MouseEventArgs e) {
			if (e.LeftButton == MouseButtonState.Pressed)
				this.DragMove();
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

		#region Window Button Event Handling
		/// <summary>
		/// Procures when Minimize button clicked.
		/// </summary>
		/// <param name="sender">Minimize button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonMinimize_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Procures when Restore button clicked.
		/// </summary>
		/// <param name="sender">Restore button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonRestore_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Normal;
		}

		/// <summary>
		/// Procures when Maximize button clicked.
		/// </summary>
		/// <param name="sender">Maximize button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonMaximize_Click(object sender, RoutedEventArgs e) {
			this.WindowState = WindowState.Maximized;
		}

		/// <summary>
		/// Procures when Close button clicked.
		/// </summary>
		/// <param name="sender">Close button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonClose_Click(object sender, RoutedEventArgs e) {
			this.Close();
		}


		// called when state of the window changed to minimized, normal or maximized
		private void StandardWindow_StateChanged(object sender, EventArgs e) {
			if (this.WindowState == WindowState.Normal) {
				this.mRestoreButton.Visibility = Visibility.Collapsed;
			} else if (this.WindowState == WindowState.Maximized) {
				this.mMaximizeButton.Visibility = Visibility.Collapsed;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="button"></param>
		private void OnWindowButtonStateChange(WindowButtonState state, WindowButton button) {
			switch (state) {
				case WindowButtonState.Normal:
					button.Visibility = Visibility.Visible;
					button.IsEnabled = true;
					break;

				case WindowButtonState.Disabled:
					button.Visibility = Visibility.Visible;
					button.IsEnabled = false;
					break;

				case WindowButtonState.None:
					button.Visibility = Visibility.Collapsed;
					break;
			}
		}

		//
		// Active / Not active Window
		// manages window buttons that differ in those states
		//
		private void OnStandardWindowActivated(object sender, EventArgs e) {
			mMinimizeButton.Background = mMinimizeButton.BackgroundDefaultValue;
			mMaximizeButton.Background = mMaximizeButton.BackgroundDefaultValue;
			mRestoreButton.Background = mRestoreButton.BackgroundDefaultValue;
			mCloseButton.Background = mCloseButton.BackgroundDefaultValue;
		}

		private void OnStandardWindowDeactivated(object sender, EventArgs e) {
			mMinimizeButton.Background = Brushes.Transparent;
			mMaximizeButton.Background = Brushes.Transparent;
			mRestoreButton.Background = Brushes.Transparent;
			mCloseButton.Background = Brushes.Transparent;
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
