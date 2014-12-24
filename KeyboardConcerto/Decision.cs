// Decision.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using KeyboardConcerto.RawInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Represents the decision by the macro system
	/// whether or not to allow an input to pass.
	/// </summary>
	public struct Decision {
		public VirtualKeys Key;
		public string State;
		public bool Allow;

		public Decision(VirtualKeys key, string state, bool allow) {
			this.Key = key;
			this.State = state;
			this.Allow = allow;
		}
	}
}
