// UserSettings.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using RawInput_dll;
using BlockInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class UserSettings {

		#region Members
		private Dictionary<string, KeyboardMacros> mKeyboardProfiles;
		#endregion

		#region Properties
		/// <summary>
		/// 
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

		/// <summary>
		/// 
		/// </summary>
		public UserSettings() {
			this.mKeyboardProfiles = new Dictionary<string, KeyboardMacros>();
		}

		public bool ProcessInput(InputEventArg arguments) {

			return true;
		}
	}
}
