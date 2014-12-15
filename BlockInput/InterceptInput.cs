// InterceptInput.cs
// Original base code by Agha Usman Ahmed
// Article: http://geekswithblogs.net/aghausman/archive/2009/04/26/disable-special-keys-in-win-app-c.aspx
// Concept borrowed from Petre Medek
// From comments section in http://www.codeproject.com/Articles/17123/Using-Raw-Input-from-C-to-handle-multiple-keyboard?fid=375378&fr=226#xx0xx
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
	public class InterceptInput : IDisposable {

		#region Public Members
		/// <summary>
		/// Global name of the memory-mapped file being shared.
		/// </summary>
		/// <remarks>
		/// Included process ID in the event there's another instance.
		/// </remarks>
		public readonly string MappedFileName = "InterceptInputMemoryMappedFile" 
			+ Process.GetCurrentProcess().Id.ToString();

		/// <summary>
		/// Global name of the Mutex used to safely access the shared file.
		/// </summary>
		/// <remarks>
		/// Included process ID in the event there's another instance.
		/// </remarks>
		public readonly string SharedMutexName = "InterceptInputSharedMutex"
			+ Process.GetCurrentProcess().Id.ToString();
		#endregion

		#region Constants
		/// <summary>
		/// The size of the memory-mapped file.
		/// </summary>
		private const long MEMORY_MAPPED_FILE_SIZE = 1024;
		#endregion

		#region Members
		private bool mDisposed;

		private IntPtr mPtrHook;
		private LowLevelKeyboardProc mObjKeyboardProcess;

		private Mutex mMutex;
		private bool mMutexInitialized;
		private MemoryMappedFile mMemMappedFile;
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

			this.mMemMappedFile = MemoryMappedFile.CreateNew(this.MappedFileName, MEMORY_MAPPED_FILE_SIZE);
			this.mMutex = new Mutex(true, this.SharedMutexName, out this.mMutexInitialized);

			using (MemoryMappedViewStream stream = this.mMemMappedFile.CreateViewStream(0, 0)) {
				BinaryWriter writer = new BinaryWriter(stream);
				writer.Write((new KBDLLHOOKSTRUCT()).ToBytes());
				writer.Write(false);
				writer.Write(false);
			}
			this.mMutex.ReleaseMutex();
		}
		#endregion

		/// <summary>
		/// Handle the windows messages.
		/// </summary>
		/// <param name="nCode"></param>
		/// <param name="wp"></param>
		/// <param name="lp"></param>
		/// <returns></returns>
		private IntPtr CaptureKey(int nCode, IntPtr wp, IntPtr lp) {
			const int boolSize = sizeof(bool);
			if (nCode >= 0) {
				KBDLLHOOKSTRUCT objKeyInfo = (KBDLLHOOKSTRUCT)Marshal.PtrToStructure(lp, typeof(KBDLLHOOKSTRUCT));

				// Store the key info in the shared file.
				this.mMutex.WaitOne();
				using (MemoryMappedViewStream stream = this.mMemMappedFile.CreateViewStream(0, 0)) {
					BinaryWriter writer = new BinaryWriter(stream);
					writer.Write(objKeyInfo.ToBytes());
				}
				this.mMutex.ReleaseMutex();

				bool response = false;
				bool responseReady = false;

				// Wait for the main process to respond.
				while (!responseReady) {
					this.mMutex.WaitOne();
					using (MemoryMappedViewStream stream = this.mMemMappedFile.CreateViewStream(1, boolSize)) {
						BinaryReader reader = new BinaryReader(stream);
						responseReady = reader.ReadBoolean();
					}
					this.mMutex.ReleaseMutex();
				}

				// Read the response.
				this.mMutex.WaitOne();
				using (MemoryMappedViewStream stream = this.mMemMappedFile.CreateViewStream(2, boolSize)) {
					BinaryReader reader = new BinaryReader(stream);
					response = reader.ReadBoolean();
				}
				this.mMutex.ReleaseMutex();

				// Block input if the response is true.
				if(response)
					return (IntPtr)1;
			}
			return CallNextHookEx(mPtrHook, nCode, wp, lp);
		}

		#region Destruction
		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		public void Dispose() {
			if (this.mDisposed)
				return;

			this.mMutex.Dispose();
			this.mMemMappedFile.Dispose();
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
