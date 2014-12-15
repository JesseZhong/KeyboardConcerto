// Conversion.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Runtime.InteropServices;
#endregion

namespace Common {
	public class Conversion {

		/// <summary>
		/// Converts object to binary form and stores into byte array.
		/// </summary>
		/// <returns>Resulting object in byte array.</returns>
		public static byte[] ToBytes<T>(T obj) {
			int size = Marshal.SizeOf(obj);
			IntPtr intPtr = Marshal.AllocHGlobal(size);
			byte[] buffer = new byte[size];

			Marshal.StructureToPtr(obj, intPtr, true);
			Marshal.Copy(intPtr, buffer, 0, size);
			Marshal.FreeHGlobal(intPtr);

			return buffer;
		}

		/// <summary>
		/// Converts byte array back into object.
		/// </summary>
		public static T FromBytes<T>(byte[] buffer) where T : new() {
			T obj = new T();
			int size = Marshal.SizeOf(obj);
			IntPtr intPtr = Marshal.AllocHGlobal(size);

			Marshal.Copy(buffer, 0, intPtr, size);

			obj = (T)Marshal.PtrToStructure(intPtr, obj.GetType());
			Marshal.FreeHGlobal(intPtr);

			return obj;
		}
	}
}
