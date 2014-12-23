// ExecCommand.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Diagnostics;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Execute command.
	/// </summary>
	public class ExecCommand : ExecNode {

		#region Members
		private string mCommand;
		#endregion

		#region Properties
		/// <summary>
		/// Command that needs to be run.
		/// </summary>
		public string Command {
			get {
				return this.mCommand;
			}
			set {
				this.mCommand = value;
			}
		}
		#endregion

		/// <summary>
		/// Attempts to run the command.
		/// </summary>
		/// <returns>True if execution was successful.</returns>
		public override bool Execute() {
			if ((this.mCommand == null) || (this.mCommand.Length < 1))
				return false;

			Process.Start(this.mCommand);
			return true;
		}
	}
}
