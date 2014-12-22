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
using System.Runtime.InteropServices;
using RawInput_dll;
using Common;
#endregion

namespace KeyboardConcerto {
	public partial class MainForm : Form {

		[DllImport("Interceptor.dll")]
		private static extern bool InstallHook(IntPtr hWndParent);

		[DllImport("Interceptor.dll")]
		private static extern bool UninstallHook();

		public struct HookParams {
			public string State;
			public string PrevState;
			public string AltState;
			public bool ExtendedKey;
			public short ScanCode;
			public short RepeatCount;
		}

		#region Constants
		/// <summary>
		/// The size of the memory-mapped file.
		/// </summary>
		private const long MEMORY_MAPPED_FILE_SIZE = 1024;

		/// <summary>
		/// The amount of time the hook can wait for its respective Raw Input before timing out.
		/// </summary>
		/// <remarks>
		/// Measured in milliseconds.
		/// </remarks>
		private const long MAX_WAIT_TIME = 1000;
		#endregion

		#region Members
		private readonly RawInput mRawInput;
		private UserSettings mUserSettings;

		//private MemoryMappedFile mMemoryMappedFile;

		private Queue<Decision> mDecisionQueue;
		#endregion

		#region Initialization
		/// <summary>
		/// Initialize the form's components, input handling, and memory.
		/// </summary>
		public MainForm() {
			this.InitializeComponent();
			this.mDecisionQueue = new Queue<Decision>();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;
			IntPtr accessHandle = this.Handle; // Ensure that the handle is created.

			this.mRawInput = new RawInput(this.Handle);
			this.mRawInput.CaptureOnlyIfTopMostWindow = false;
			this.mRawInput.AddMessageFilter();
			this.mRawInput.KeyPressed += this.OnKeyPressed;

			this.mUserSettings = new UserSettings();

// 			this.mMemoryMappedFile = MemoryMappedFile.CreateNew(Sharing.MMF_NAME, MEMORY_MAPPED_FILE_SIZE);
// 			this.InitializeHandleMMF();

			InstallHook(this.Handle);

			Win32.DeviceAudit();
		}

		/// <summary>
		/// Initialize the memory-mapped file for sharing the form's handle.
		/// </summary>
// 		private void InitializeHandleMMF() {
// 			bool mutexCreated = false;
// 			Mutex mutex = new Mutex(true, Sharing.MMF_MUTEX_NAME, out mutexCreated);
// 			using (MemoryMappedViewStream stream = this.mMemoryMappedFile.CreateViewStream()) {
// 				BinaryWriter writer = new BinaryWriter(stream);
// 				byte[] buffer = Conversion.ToBytes(this.Handle);
// 				writer.Write(buffer.Length);
// 				writer.Write(buffer);
// 			}
// 			mutex.ReleaseMutex();
// 		}

		/// <summary>
		/// Ensure that the messages are still received when the form is minimized.
		/// </summary>
		/// <param name="e"></param>
		protected override void OnHandleCreated(EventArgs e) {
			base.OnHandleCreated(e);
			IntPtr HWND_MESSAGE = new IntPtr(-3);
			SetParent(this.Handle, HWND_MESSAGE);
		}
		#endregion

		#region Keyboard Handling
		/// <summary>
		/// Processes the input and enqueues the decision to be executed later.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void OnKeyPressed(object sender, InputEventArg e) {
			Decision dc;
			this.mDecisionQueue.Enqueue(dc = new Decision() {
				Key = (Keys)e.KeyPressEvent.VKey,
				State = e.KeyPressEvent.KeyPressState,
				Allow = this.mUserSettings.ProcessInput(e)
			});

			//this.TextBox.AppendText(String.Format("{0} {1} {2} {3} {4} {5}\n", dc.Key, dc.State, dc.Allow, silliness, stk.State, stk.PrevState));
			this.TextBox.AppendText(String.Format("{0} {1} {2} {3}\n", dc.Key, dc.State, dc.Allow, sacred));

			string name = "./output.txt";
			if (!File.Exists(name)) {
				using (File.Create(name)) {

				}
			}
			using (StreamWriter file = new StreamWriter(name, true)) {
				file.WriteLine(String.Format("{0} {1} {2} {3}\n", dc.Key, dc.State, dc.Allow, sacred));
			}
		}

		uint silliness;
		//HookParams stk;
		string sacred;
		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
		protected override void WndProc(ref Message msg) {
			const string makeStr = "MAKE";
			const string breakStr = "BREAK";

			base.WndProc(ref msg);

			//const uint keyStateMask = 0x80000000;
			switch (msg.Msg) {  
				case 32769: {
						bool block = false;
						bool decisionFound = false;
						//Keys key = (Keys)Marshal.ReadInt32(msg.LParam);
						//string state = (param.State & keyStateMask) == keyStateMask ? "BREAK" : "MAKE";
						silliness = (uint)msg.WParam;
						long lparam = (long)msg.LParam;
						sacred = GetIntBinaryString(lparam);
// 						stk = new HookParams() {
// 							State = lparam >>
// 						};
// 
// 						Stopwatch timer = new Stopwatch();
// 						timer.Start();
						//while (!decisionFound) {

							// Time out if no matching Raw Input is found after a while.
// 							if (timer.ElapsedMilliseconds > MAX_WAIT_TIME) {
// 								msg.Result = (IntPtr)0;
// 								return;
// 							}

							// Search the queue for matching input.
// 							int index = 1;
// 							foreach (Decision decision in this.mDecisionQueue) {
// 
// 								if ((decision.Key == key)/* && (decision.State == state)*/) {
// 									block = decision.Allow;
// 									decisionFound = true;
// 									//mahNigga++;
// 									break;
// 								}
// 								index++;
// 							}

							// Remove the current and all preceding messages from the queue.
// 							for (int i = 0, count = this.mDecisionQueue.Count; (i < index) && (i < count); i++) {
// 								this.mDecisionQueue.Dequeue();
// 							}
						//}

						// Reply with the decision.
						if (block)
							msg.Result = (IntPtr)1;
					}
					break;
			}
		}

		static string GetIntBinaryString(long n) {
			char[] b = new char[64];
			int pos = 63;
			int i = 0;

			while (i < 64) {
				if ((n & (1 << i)) != 0) {
					b[pos] = '1';
				} else {
					b[pos] = '0';
				}
				pos--;
				i++;
			}
			return new string(b);
		}
		#endregion

		#region Managing Resources
		/// <summary>
		/// Release unmanaged resources.
		/// </summary>
		public new void Dispose() {
			if (this.IsDisposed)
				return;

			//this.mMemoryMappedFile.Dispose();
			UninstallHook();

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

		#region Windows API
		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);
		#endregion
	}
}
