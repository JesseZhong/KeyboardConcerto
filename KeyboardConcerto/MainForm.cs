// MainForm.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Linq;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using RawInput_dll;
#endregion

namespace KeyboardConcerto {
	public partial class MainForm : Form {

		#region Members
		private readonly RawInput mRawInput;
		private UserSettings mUserSettings;
		#endregion

		public MainForm() {
			this.InitializeComponent();
			AppDomain.CurrentDomain.UnhandledException += this.CurrentDomain_UnhandledException;

			this.mRawInput = new RawInput(this.Handle);
			this.mRawInput.CaptureOnlyIfTopMostWindow = false;
			this.mRawInput.AddMessageFilter();
			this.mRawInput.KeyPressed += this.OnKeyPressed;

			this.mUserSettings = new UserSettings();

			Win32.DeviceAudit();
		}


		private void OnKeyPressed(object sender, InputEventArg e) {
			if (this.mUserSettings.ProcessInput())
				return;
			
		}

		#region Program Entry Point
		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() {
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
