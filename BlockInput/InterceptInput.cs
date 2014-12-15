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

		#region Constants
		/// <summary>
		/// The size of the memory-mapped file.
		/// </summary>
		private const long MEMORY_MAPPED_FILE_SIZE = 1024;
		#endregion

		#region Members
		private IntPtr mPtrHook;
		private KeyboardProc mKeyboardProcess;
		#endregion

		#region Initialization
		/// <summary>
		/// Set up hooks for the DLL's process.
		/// </summary>
		public InterceptInput() {
			ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
			this.mKeyboardProcess = new KeyboardProc(ProcessKeyboard);
			this.mPtrHook = SetWindowsHookEx(WH_KEYBOARD, this.mKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
		}
		#endregion

		/// <summary>
		/// Handle the windows messages.
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wp"></param>
		/// <param name="lp"></param>
		/// <returns></returns>
		private IntPtr ProcessKeyboard(int code, IntPtr wParam, IntPtr lParam) {
			if (code >= 0) {
				

				bool response = false;

				// Block input if the response is true.
				if(response)
					return (IntPtr)1;
			}
			return CallNextHookEx(mPtrHook, code, wParam, lParam);
		}

		#region Windows API
		// idHooks: List of hook IDs in Windows API.
		private const int WH_KEYBOARD = 2;
		private const int WH_KEYBOARD_LL = 13;

		private delegate IntPtr KeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);

		// System level functions to be used for hook and unhook keyboard input.
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int id, KeyboardProc callback, IntPtr hMod, uint dwThreadId);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern bool UnhookWindowsHookEx(IntPtr hook);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr CallNextHookEx(IntPtr hook, int nCode, IntPtr wp, IntPtr lp);
		[DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr GetModuleHandle(string name);
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		private static extern short GetAsyncKeyState(Keys key);
		#endregion
	}
}
