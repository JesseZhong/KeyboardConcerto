#region Usings
using System;
using System.Runtime.InteropServices;
#endregion

namespace KeyboardConcerto.Theme {
	public class Win32Interop {

		public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
		public const int DWM_BB_ENABLE = 0x1; 

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern void DwmEnableBlurBehindWindow(IntPtr hwnd, ref DWM_BLURBEHIND blurBehind);

		[DllImport("dwmapi.dll")]
		public static extern int DwmExtendFrameIntoClientArea(IntPtr hWnd, ref MARGINS pMargins);

		[DllImport("dwmapi.dll", PreserveSig = false)]
		public static extern bool DwmIsCompositionEnabled();

		[DllImport("dwmapi.dll", PreserveSig = true)]
		public static extern int DwmSetWindowAttribute(IntPtr hwnd, int attr, ref int attrValue, int attrSize);

		[DllImport("dwmapi.dll", PreserveSig = true)]
		public static extern IntPtr DwmGetColorizationParameters(out COLORIZATIONPARAMS colorParams);

		[DllImport("dwmapi.dll", PreserveSig = true)]
		public static extern IntPtr DwmGetColorizationParameters(out COLORIZATIONPARAMS colorParams, uint unknown);
	}
}
