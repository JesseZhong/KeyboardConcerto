// KBDLLHOOKSTRUCT.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
using System.Runtime.InteropServices;
#endregion

namespace Common {

	/// <summary>
	/// Structure contain information about low-level keyboard input event.
	/// </summary>
	[StructLayout(LayoutKind.Sequential)]
	public struct KBDLLHOOKSTRUCT {
		public Keys Key;
		public int ScanCode;
		public int Flags;
		public int Time;
		public IntPtr Extra;

		/// <summary>
		/// Converts hook structure to binary form and stores into byte array.
		/// </summary>
		/// <returns>Resulting struct in byte array.</returns>
		public byte[] ToBytes() {
			int size = Marshal.SizeOf(this);
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			byte[] buffer = new byte[size];

			Marshal.StructureToPtr(this, intPtr, true);
			Marshal.Copy(intPtr, buffer, 0, size);
			Marshal.FreeHGlobal(intPtr);

			return buffer;
		}

		/// <summary>
		/// Converts byte array back into hook structure.
		/// </summary>
		public void FromBytes(byte[] buffer) {
			int size = Marshal.SizeOf(this);
			IntPtr intPtr = Marshal.AllocHGlobal(size);

			Marshal.Copy(buffer, 0, intPtr, size);

			this = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(intPtr, this.GetType());
			Marshal.FreeHGlobal(intPtr);
		}
	}
}
