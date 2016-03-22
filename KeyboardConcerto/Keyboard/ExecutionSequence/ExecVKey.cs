// ExecVKey.cs
// Authored by Jesse Z. Zhong
#region Usings
using WindowsInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Execute virtual key.
	/// </summary>
	public class ExecVKey : ExecNode {

		public enum KeyState {
			Press = 0,
			Down,
			Up
		}

		#region Members
		private VirtualKeyCode mKey = VirtualKeyCode.NONAME;
		private KeyState mState;
		#endregion

		#region Constructor
		/// <summary>
		/// Initialization constructor.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="state"></param>
		public ExecVKey(VirtualKeyCode key, KeyState state) {
			this.mKey = key;
			this.mState = state;
		}
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
		public KeyState State {
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
				case KeyState.Press:
					InputSimulator.SimulateKeyPress(this.mKey);
					break;
				case KeyState.Down:
					InputSimulator.SimulateKeyDown(this.mKey);
					break;
				case KeyState.Up:
					InputSimulator.SimulateKeyUp(this.mKey);
					break;
				default:
					return false;
			}
			return true;
		}
	}
}
