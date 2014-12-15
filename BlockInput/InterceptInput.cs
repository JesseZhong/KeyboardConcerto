// InterceptInput.cs
// Original base code by Agha Usman Ahmed
// Article: http://geekswithblogs.net/aghausman/archive/2009/04/26/disable-special-keys-in-win-app-c.aspx
// Concept borrowed from Petre Medek
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
	/// Intercepts user input, broadcasts to subscribers and waits for callback.
	/// If the input is to be allowed, the D
	/// </summary>
	public class InterceptInput {

		#region Global Constants
		/// <summary>
		/// Global indicator for the hook.
		/// </summary>
		public const int WM_HOOK = 0x8000 + 1;
		#endregion

		#region Members
		private IntPtr mHookHandle;
		private IntPtr mWindowHandle;
		#endregion

		#region Initialization
		/// <summary>
		/// Hook DLL to main program's window handle.
		/// </summary>
		public InterceptInput() {

			// Attempt to retrieve the window's handle from MMF and register hook with its process.
			try {
				using (MemoryMappedFile memoryMappedFile = MemoryMappedFile.OpenExisting(Sharing.MMF_NAME)) {

					Mutex mutex = Mutex.OpenExisting(Sharing.MMF_MUTEX_NAME);
					mutex.WaitOne();

					using (MemoryMappedViewStream stream = memoryMappedFile.CreateViewStream(0, 0)) {
						BinaryReader reader = new BinaryReader(stream);
						int length = reader.ReadInt32();

						if (length > 0) {
							this.mWindowHandle = Conversion.FromBytes<IntPtr>(reader.ReadBytes(length));
							if(IsWindow(this.mWindowHandle))
								this.mHookHandle = SetWindowsHookEx(WH_KEYBOARD, ProcessKeyboard, this.mWindowHandle, 0);
						}
					}
				}
			} catch(FileNotFoundException) {
				Console.WriteLine("Memory-mapped file does not exist.");
			}
		}
		#endregion

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
			if(code < 0)
				return CallNextHookEx(mHookHandle, code, wParam, lParam);

			// Block input if the call back requests it.
			if (IsWindow(this.mWindowHandle) && ((IntPtr)1 == SendMessage(this.mWindowHandle, WM_HOOK, wParam, lParam)))
				return (IntPtr)1;

			// Otherwise, pass input on.
			return CallNextHookEx(mHookHandle, code, wParam, lParam);
		}

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
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool IsWindow(IntPtr hWnd);
		#endregion
	}
}
