// Sharing.cs
#region Usings
using System;
#endregion

namespace Common {
	public class Sharing {
		/// <summary>
		/// Global name of the memory-mapped file being shared.
		/// </summary>
		public const string MMF_NAME = "KeyboardConcertoMemoryMappedFile";

		/// <summary>
		/// Global name of the Mutex used to safely access the shared file.
		/// </summary>
		public const string MMF_MUTEX_NAME = "KeyboardConcertoSharedMutex";
	}
}
