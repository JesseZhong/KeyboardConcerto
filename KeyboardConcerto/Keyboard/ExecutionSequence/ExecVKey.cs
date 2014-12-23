// ExecVKey.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
using WindowsInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Execute virtual key.
	/// </summary>
	public class ExecVKey : ExecNode {

		public enum EState {
			Press = 0,
			Down,
			Up
		}

		#region Members
		private VirtualKeyCode mKey = VirtualKeyCode.NONAME;
		private EState mState;
		#endregion

		#region Properties
		/// <summary>
		/// Virtual key that needs to be simulated.
		/// </summary>
		public VirtualKeyCode Key {
			get {
				return this.mKey;
			}
			set {
				this.mKey = value;
			}
		}

		/// <summary>
		/// Whether the key is pressed, down, or up.
		/// </summary>
		public EState State {
			get {
				return this.mState;
			}
			set {
				this.mState = value;
			}
		}
		#endregion

		/// <summary>
		/// Execute the key.
		/// </summary>
		/// <returns>True if execution was successful.</returns>
		public override bool Execute() {
			switch (this.mState) {
				case EState.Press:
					InputSimulator.SimulateKeyPress(this.mKey);
					break;
				case EState.Down:
					InputSimulator.SimulateKeyDown(this.mKey);
					break;
				case EState.Up:
					InputSimulator.SimulateKeyUp(this.mKey);
					break;
				default:
					return false;
			}
			return true;
		}
	}
}
