#region Usings
using System;
using WindowsInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// 
	/// </summary>
	public class ExecText : ExecNode {

		#region Members
		private string mText = "";
		#endregion

		#region Constructor
		/// <summary>
		/// 
		/// </summary>
		/// <param name="text"></param>
		public ExecText(string text) {
			this.mText = text;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Gets or sets the text to be executed.
		/// </summary>
		public string Text {
			get {
				return this.mText;
			}
			set {
				this.mText = value;
			}
		}
		#endregion

		/// <summary>
		/// Executes a line of text.
		/// </summary>
		/// <returns>True if the text was executed.</returns>
		public override bool Execute() {
			if (this.mText == "")
				return false;

			InputSimulator.SimulateTextEntry(this.mText);

			return true;
		}
	}
}
