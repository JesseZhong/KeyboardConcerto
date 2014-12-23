// UserSettings.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using KeyboardConcerto.RawInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Contains the user defined settings for each keyboard device.
	/// This generally includes all the hot keys, shortcuts, and macros.
	/// </summary>
	[Serializable]
	public class UserSettings {

		#region Members
		private Dictionary<string, KeyboardMacros> mKeyboardProfiles;
		#endregion

		#region Properties
		/// <summary>
		/// Stores all of the user defined settings for their keyboards.
		/// </summary>
		public Dictionary<string, KeyboardMacros> KeyboardProfiles {
			get {
				return this.mKeyboardProfiles;
			}
			set {
				this.mKeyboardProfiles = value;
			}
		}
		#endregion

		#region Initialization
		/// <summary>
		/// Initializes keyboard profile container.
		/// </summary>
		public UserSettings() {
			this.mKeyboardProfiles = new Dictionary<string, KeyboardMacros>();
		}
		#endregion

		#region Process Input
		/// <summary>
		/// Checks if there is a hot key for the passed input.
		/// Execute the hot key if it is found.
		/// </summary>
		/// <param name="arguments">User input information.</param>
		/// <returns>True if a macro is available for the key event.</returns>
		public bool ProcessInput(KeyPressEvent keyPressEvent) {

			// Check for keyboard name match.
			KeyboardMacros macroSet;
			if (this.mKeyboardProfiles.TryGetValue(keyPressEvent.DeviceName, out macroSet)) {

				// Check for a key and macro match.
				KeyMacro macro;
				if (macroSet.TryGetValue((Keys)keyPressEvent.VKey, out macro)) {

					// Execute macro and return true.
					macro.Execute();
					return true;
				}
			}

			return false;
		}
		#endregion
	}
}
