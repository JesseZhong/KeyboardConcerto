// UserSettings.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Linq;
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

		#region Methods

		/// <summary>
		/// Attempts to add a new execution sequence given the device name, key, and sequence.
		/// If an entry already exists, it will overwrite the existing execution sequence.
		/// </summary>
		/// <param name="deviceName"></param>
		/// <param name="key"></param>
		/// <param name="executionSequence"></param>
		public void AddEntry(string deviceName, VirtualKeys key, LinkedList<ExecNode> executionSequence) {

			System.Diagnostics.Debug.Assert(this.mKeyboardProfiles != null);
			if (this.mKeyboardProfiles.ContainsKey(deviceName)) {

				KeyboardMacros macros = this.mKeyboardProfiles[deviceName];

				System.Diagnostics.Debug.Assert(macros != null);
				if(macros.ContainsKey(key)) {

					KeyMacro macro = macros[key];

					System.Diagnostics.Debug.Assert(macro != null);
					macro.ExecutionSequence = executionSequence;

				} else {

					macros.Add(key, new KeyMacro(executionSequence));
				}

			} else {

				KeyboardMacros macros = new KeyboardMacros();
				macros.Add(key, new KeyMacro(executionSequence));
				this.mKeyboardProfiles.Add(deviceName, macros);
			}
		}

		/// <summary>
		/// Attempts to locate the execution sequence for a specific key and device.
		/// </summary>
		/// <param name="deviceName">Name of the keyboard device.</param>
		/// <param name="key">Virtual key in question.</param>
		/// <param name="executionSequence">Returned execution sequence.</param>
		/// <returns>True if the device name and key were found.</returns>
		public bool FindEntry(string deviceName, VirtualKeys key, out LinkedList<ExecNode> executionSequence) {

			// Checks if the device name exists. Return false and null if it doesn't.
			System.Diagnostics.Debug.Assert(this.mKeyboardProfiles != null);
			if (!this.mKeyboardProfiles.ContainsKey(deviceName)) {
				executionSequence = null;
				return false;
			}

			// Load macros for the found device name.
			KeyboardMacros macros = this.mKeyboardProfiles[deviceName];

			// Checks if the macro exists for the virtual key. Return false and null if it doesn't.
			System.Diagnostics.Debug.Assert(macros != null);
			if(!macros.ContainsKey(key)) {
				executionSequence = null;
				return false;
			}

			// Load the macro for the specific key.
			KeyMacro macro = macros[key];

			// Out the execution sequence.
			System.Diagnostics.Debug.Assert(macro != null);
			System.Diagnostics.Debug.Assert(macro.ExecutionSequence != null);
			executionSequence = macro.ExecutionSequence;

			return true;
		}

		/// <summary>
		/// Attempts to remove a macro given a device name and key.
		/// Remove any empty macro listings.
		/// </summary>
		/// <param name="deviceName">Name of the keyboard device.</param>
		/// <param name="key">Virtual key in question.</param>
		/// <returns>True if the macro was successfully removed.</returns>
		public bool RemoveEntry(string deviceName, VirtualKeys key) {

			// Check if there are any keyboard macros under the given device name.
			System.Diagnostics.Debug.Assert(this.mKeyboardProfiles != null);
			if (!this.mKeyboardProfiles.ContainsKey(deviceName))
				return false;

			// Load the macros if they exist.
			KeyboardMacros macros = this.mKeyboardProfiles[deviceName];

			// Check if there is a macro for the specified key.
			System.Diagnostics.Debug.Assert(macros != null);
			if (!macros.ContainsKey(key))
				return false;

			// Remove the corresponding macro for the key.
			macros.Remove(key);

			// Remove macros dictionary for the specific device if there are no macros left.
			if (!macros.Any()) {
				this.mKeyboardProfiles.Remove(deviceName);
			}
			
			return true;
		}
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
				if (macroSet.TryGetValue((VirtualKeys)keyPressEvent.VKey, out macro)) {

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
