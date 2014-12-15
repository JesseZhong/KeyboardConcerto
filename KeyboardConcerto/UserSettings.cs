// UserSettings.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows.Forms;
using System.Xml.Serialization;
using System.Collections.Generic;
using InterceptInput;
using RawInput_dll;
using Common;
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="arguments"></param>
		/// <returns></returns>
		public bool ProcessInput(InputEventArg arguments) {


			return true;
		}
	}
}
