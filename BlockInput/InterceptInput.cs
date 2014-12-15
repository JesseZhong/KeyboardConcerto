// InterceptInput.cs
// Original base code by Agha Usman Ahmed
// Article: http://geekswithblogs.net/aghausman/archive/2009/04/26/disable-special-keys-in-win-app-c.aspx
// Concept borrowed from Petre Medek
// From comments section in http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard?fid=375378&fr=226#xx0xx
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion

namespace InterceptInput {

	/// <summary>
	/// Intercepts user input, broadcasts to subscribers and waits for callback.
	/// If the input is to be allowed, the D
	/// </summary>
	public class InterceptInput : IDisposable {

		#region Global Constants
		/// <summary>
		/// Global name of the memory-mapped file being shared.
		/// </summary>
		public const string MAPPED_FILE_NAME = "BlockInputMemoryMapped";
		#endregion

		#region Members
		private bool mDisposed;
		private IntPtr mPtrHook;
		private LowLevelKeyboardProc mObjKeyboardProcess;
		#endregion

		#region Initialization
		/// <summary>
		/// Set up hooks for the DLL's process.
		/// </summary>
		public InterceptInput() {
			this.mDisposed = false;
			ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
			this.mObjKeyboardProcess = new LowLevelKeyboardProc(CaptureKey);
			this.mPtrHook = SetWindowsHookEx(13, this.mObjKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
		}
		#endregion

		/// <summary>
		/// 
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wp"></param>
		/// <param name="lp"></param>
		/// <returns></returns>
		private IntPtr CaptureKey(int nCode, IntPtr wp, IntPtr lp) {
			if (nCode >= 0) {
				KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

				if ((this.ProcessInput != null) && this.ProcessInput(objKeyInfo.Key))
					// Block the input from being passed on.
					return (IntPtr)1;
			}
			return CallNextHookEx(mPtrHook, nCode, wp, lp);
		}

		public delegate bool ProcessInputDelegate(Keys keystroke);
		public ProcessInputDelegate ProcessInput;

		#region Destruction
		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (this.mDisposed)
				return;


		}
		#endregion

		#region Assembly Entry
		[STAThread]
		public static void Main() {
			using (InterceptInput interceptInput = new InterceptInput())
				Application.Run();
		}
		#endregion

		#region Windows API
		// Structure contain information about low-level keyboard input event.
		[StructLayout(LayoutKind.Sequential)]
		private struct KBDLLHOOKSTRUCT {
			public Keys Key;
			public int ScanCode;
			public int Flags;
			public int Time;
			public IntPtr Extra;
		}

		//System level functions to be used for hook and unhook keyboard input.
		private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		private static extern IntPtr SetWindowsHookEx(int id, LowLevelKeyboardProc callback, IntPtr hMod, uint dwThreadId);
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
