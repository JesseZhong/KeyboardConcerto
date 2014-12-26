#region Usings
using System;
using System.Runtime.InteropServices;
#endregion

namespace KeyboardConcerto.CustomWindow {
	[StructLayout( LayoutKind.Sequential )]
	public struct DWM_BLURBEHIND {
		public int dwFlags;
		public bool fEnable;
		public IntPtr hRgnBlur;
		public bool fTransitionOnMaximized;
	}

	[StructLayout(LayoutKind.Sequential)]
	public struct MARGINS {
		public int cxLeftWidth;
		public int cxRightWidth;
		public int cyTopHeight;
		public int cyBottomHeight;
	} 
}
