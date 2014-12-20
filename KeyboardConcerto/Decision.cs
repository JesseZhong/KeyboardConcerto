// Decision.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Represents the decision by the macro system
	/// whether or not to allow an input to pass.
	/// </summary>
	public struct Decision {
		public Keys Key;
		public string State;
		public bool Allow;

		public Decision(Keys key, string state, bool allow) {
			this.Key = key;
			this.State = state;
			this.Allow = allow;
		}
	}
}
