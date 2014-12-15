// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using RawInput_dll;
using Common;
#endregion

namespace KeyboardConcerto {
	public partial class MainForm : Form {

		#region Constants
		/// <summary>
		/// The size of the memory-mapped file.
		/// </summary>
		private const long MEMORY_MAPPED_FILE_SIZE = 1024;
		#endregion

		#region Members
		private readonly RawInput mRawInput;
		private UserSettings mUserSettings;

		private MemoryMappedFile mMemoryMappedFile;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainForm() {
			this.InitializeComponent();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

			this.mRawInput = new RawInput(this.Handle);
			this.mRawInput.CaptureOnlyIfTopMostWindow = false;
			this.mRawInput.AddMessageFilter();
			this.mRawInput.KeyPressed += this.OnKeyPressed;

			this.mUserSettings = new UserSettings();

			this.mMemoryMappedFile = MemoryMappedFile.CreateNew(Sharing.MMF_NAME, MEMORY_MAPPED_FILE_SIZE);
			this.InitializeHandleMMF();

			Win32.DeviceAudit();
		}

		/// <summary>
		/// Initialize the memory-mapped file for sharing the form's handle.
		/// </summary>
		private void InitializeHandleMMF() {
			bool mutexCreated = false;
			Mutex mutex = new Mutex(true, Sharing.MMF_MUTEX_NAME, out mutexCreated);
			using (MemoryMappedViewStream stream = this.mMemoryMappedFile.CreateViewStream()) {
				BinaryWriter writer = new BinaryWriter(stream);
				byte[] buffer = Conversion.ToBytes(this.Handle);
				writer.Write(buffer.Length);
				writer.Write(buffer);
			}
			mutex.ReleaseMutex();
		}
		#endregion

		#region Keyboard Handling


		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPressed(object sender, InputEventArg e) {
			this.mUserSettings.ProcessInput(e);
		}
		#endregion

		#region Managing Resources
		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		public new void Dispose() {
			if (this.IsDisposed)
				return;

			this.mMemoryMappedFile.Dispose();

			base.Dispose();
		}
		#endregion

		#region Program Entry Point
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		public static void Main() {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new MainForm());
		}
		#endregion

		#region Exception Handling
		private void CurrentDomain_UnhandledException(Object sender, UnhandledExceptionEventArgs e) {
			var ex = e.ExceptionObject as Exception;

			if (null == ex) return;

			// Log this error. Logging the exception doesn't correct the problem but at least now
			// you may have more insight as to why the exception is being thrown.
			Debug.WriteLine("Unhandled Exception: " + ex.Message);
			Debug.WriteLine("Unhandled Exception: " + ex);
			MessageBox.Show(ex.Message);
		}
		#endregion
	}
}
