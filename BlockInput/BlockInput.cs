// BlockInput.cs
// Original base code by Agha Usman Ahmed
// Article: http://geekswithblogs.net/aghausman/archive/2009/04/26/disable-special-keys-in-win-app-c.aspx
// Modified by Jesse Z. Zhong
#region Usings
using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion

namespace BlockInput {
	public class BlockInput {

		#region Members
		private IntPtr mPtrHook;
		private LowLevelKeyboardProc mObjKeyboardProcess;
		#endregion

		#region Initialization
		public BlockInput() {
			ProcessModule objCurrentModule = Process.GetCurrentProcess().MainModule;
			this.mObjKeyboardProcess = new LowLevelKeyboardProc(CaptureKey);
			this.mPtrHook = SetWindowsHookEx(13, this.mObjKeyboardProcess, GetModuleHandle(objCurrentModule.ModuleName), 0);
		}
		#endregion

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
