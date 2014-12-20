// InterceptInput.cs
// Original base code by Agha Usman Ahmed
// Article: http://geekswithblogs.net/aghausman/archive/2009/04/26/disable-special-keys-in-win-app-c.aspx
// Concept from Petre Medek
// From comments section in http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard?fid=375378&fr=226#xx0xx
// Detailed explanation and C++ example by Vit Blecha
// Article: http://www.codeproject.com/Articles/716591/Combining-Raw-Input-and-keyboard-Hook-to-selective
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using Common;
#endregion

namespace InterceptInput {

	/// <summary>
	/// Hooks standard keyboard input from the main application,
	/// checks against raw input and user settings, and determines
	/// whether to allow or block the input.
	/// </summary>
	public class Interceptor {

		#region Global Constants
		/// <summary>
		/// Global indicator for the hook.
		/// </summary>
		public const int WM_HOOK = 0x8000 + 1;

		public readonly static IntPtr hInstance = LoadLibrary("User32");
		#endregion

		#region Members
		private IntPtr mHookHandle;
		private IntPtr mWindowHandle;
		private KeyboardProc mObjKeyboardProcess;
		#endregion

		#region Initialization
		/// <summary>
		/// Hook DLL to main program's window handle.
		/// </summary>
		/// <param name="hWndParent">Window handle.</param>
		public Interceptor(IntPtr hWndParent) {
			this.InstallHook(hWndParent);

			// Attempt to retrieve the window's handle from MMF and register hook with its process.
// 			try {
// 				using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.OpenExisting(Sharing.MMF_NAME)) {
// 
// 					Mutex mutex = Mutex.OpenExisting(Sharing.MMF_MUTEX_NAME);
// 					mutex.WaitOne();
// 
// 					using (MemoryMappedViewStream stream = memoryMappedFile.CreateViewStream(0, 0)) {
// 						BinaryReader reader = new BinaryReader(stream);
// 						int length = reader.ReadInt32();
// 
// 						if (length > 0) {
// 							this.mWindowHandle = Conversion.FromBytes<IntPtr>(reader.ReadBytes(length));
// 							if(IsWindow(this.mWindowHandle))
// 								this.mHookHandle = SetWindowsHookEx(WH_KEYBOARD, ProcessKeyboard, this.mWindowHandle, 0);
// 						}
// 					}
// 				}
// 			} catch(FileNotFoundException) {
// 				Console.WriteLine("Memory-mapped file does not exist.");
// 			}
		}
		#endregion

		#region Hooking and Unhooking
		/// <summary>
		/// Installs the keyboard hook onto an existing window.
		/// </summary>
		/// <param name="hWndParent">Window handle.</param>
		/// <returns>True if successful.</returns>
		public bool InstallHook(IntPtr hWndParent) {

			// Check if hook is already installed or not.
			if (this.mWindowHandle != IntPtr.Zero)
				return false;
				
			// Install the hook.
			this.mObjKeyboardProcess = new KeyboardProc(ProcessKeyboard);
			this.mHookHandle = SetWindowsHookEx(WH_KEYBOARD, this.mObjKeyboardProcess, hInstance, 0);

			//int number = Marshal.GetLastWin32Error();

			// Check if the hook handle is valid.
			if (this.mHookHandle == IntPtr.Zero)
				return false;

			this.mWindowHandle = hWndParent;
			return true;
		}

		/// <summary>
		/// Uninstalls the keyboard hook.
		/// </summary>
		/// <returns></returns>
		public bool UninstallHook() {

			// Check if the hook as already been uninstalled or not.
			if (this.mHookHandle == IntPtr.Zero)
				return true;

			// Attempt to uninstall hook.
			if (!UnhookWindowsHookEx(this.mHookHandle)) {
				Console.WriteLine("Error: Invalid hook handle; could not uninstall hook.");
				return false;
			}

			this.mWindowHandle = IntPtr.Zero;
			this.mHookHandle = IntPtr.Zero;
			return true;
		}
		#endregion

		#region Keyboard Handling
		/// <summary>
		/// Handle the windows messages.
		/// </summary>
		/// <param name="code"></param>
		/// <param name="wParam"></param>
		/// <param name="lParam"></param>
		/// <returns></returns>
		private IntPtr ProcessKeyboard(int code, IntPtr wParam, IntPtr lParam) {

			// If the code is less than zero, the message must be passed on.
			// See: http://msdn.microsoft.com/en-us/library/windows/desktop/ms644984%28v=vs.85%29.aspx
			if (code < 0)
				return CallNextHookEx(mHookHandle, code, wParam, lParam);

			// Block input if the call back requests it.
			if ((IntPtr)1 == SendMessage(this.mWindowHandle, WM_HOOK, wParam, lParam))
				return (IntPtr)1;

			// Otherwise, pass input on.
			return CallNextHookEx(mHookHandle, code, wParam, lParam);
		}
		#endregion

		#region Windows API
		// idHooks: List of hook IDs in Windows API.
		private const int WH_KEYBOARD = 2;

		private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		// System level functions to be used for hook and unhook keyboard input.
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern IntPtr SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int id, KeyboardProc callback, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hook);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr LoadLibrary(string lpFileName);
		#endregion
	}
}
