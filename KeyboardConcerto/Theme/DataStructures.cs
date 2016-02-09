#region Usings
using System;
using System.Drawing;
using System.Runtime.InteropServices;
#endregion

namespace KeyboardConcerto.Theme {
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
 
	[StructLayout(LayoutKind.Sequential)]
	public struct COLORIZATIONPARAMS {
		public Color BaseColor;
		public Color AfterGlowColor;
		public uint Intensity;
		public uint AfterGlowBalance;
		public uint BlurBalance;
		public uint GlassReflectionIntensity;
		public bool IsOpaque;
	}
}
