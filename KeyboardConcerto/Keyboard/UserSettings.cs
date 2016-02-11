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
		private Dictionary<string, KeyboardProfile> mKeyboardProfiles;
		#endregion

		#region Methods

		/// <summary>
		/// Attempts to add a new execution sequence given the device name, key, and sequence.
		/// If an entry already exists, it will overwrite the existing execution sequence.
		/// </summary>
		/// <param name="deviceName">The name of the keyboard device.</param>
		/// <param name="key">The virtual key for the macro.</param>
		/// <param name="keyState">The key state for the macro.</param>
		/// <param name="executionSequence">The execution sequence for the macro.</param>
		public void AddEntry(string deviceName, VirtualKeys key, string keyState, LinkedList<ExecNode> executionSequence) {

			// Check if the device exists in the profile dictionary.
			KeyboardProfile keyboardProfile;
			if(this.mKeyboardProfiles.TryGetValue(deviceName, out keyboardProfile)) {

				// Check if the macro set exists in the current keyboard profile.
				KeyMacroSet macroSet;
				if (keyboardProfile.TryGetValue(key, out macroSet)) {

					// Check if the macro exists in the current macro set.
					KeyMacro macro;
					if(macroSet.TryGetValue(keyState, out macro)) {

					} else {

						// Add a new macro to the current macro set.
						macroSet.Add(keyState, new KeyMacro(executionSequence));
					}

				} else {

					// Create a new macro set to add to the current keyboard profile.
					macroSet = new KeyMacroSet();

					// Add a new macro to the macro set.
					macroSet.Add(keyState, new KeyMacro(executionSequence));

					// Add the macro set to the current profile.
					keyboardProfile.Add(key, macroSet);
				}

			} else {

				// Create a new profile to add the profiles dictionary.
				keyboardProfile = new KeyboardProfile();

				// Create a new macro set to add to the profile.
				KeyMacroSet macroSet = new KeyMacroSet();

				// Add the new entry <KeyState, KeyMacro> to the macro set.
				macroSet.Add(keyState, new KeyMacro(executionSequence));

				// Add the macro set to a newly created profile for the given key. 
				keyboardProfile.Add(key, macroSet);

				// Add the new profile to the profile dictionary.
				this.mKeyboardProfiles.Add(deviceName, keyboardProfile);
			}
		}

		/// <summary>
		/// Attempts to locate the execution sequence for a specific key and device.
		/// </summary>
		/// <param name="deviceName">Name of the keyboard device.</param>
		/// <param name="key">Virtual key in question.</param>
		/// <param name="keyState">Key state in question.</param>
		/// <param name="executionSequence">Returned execution sequence.</param>
		/// <returns>True if the device name and key were found.</returns>
		public bool FindEntry(string deviceName, VirtualKeys key, string keyState, out LinkedList<ExecNode> executionSequence) {

			// Check if the device exists in the profile dictionary.
			KeyboardProfile keyboardProfile;
			if (this.mKeyboardProfiles.TryGetValue(deviceName, out keyboardProfile)) {

				// Check if the macro set exists in the current keyboard profile.
				KeyMacroSet macroSet;
				if (keyboardProfile.TryGetValue(key, out macroSet)) {

					// Check if the macro exists in the current macro set.
					KeyMacro macro;
					if (macroSet.TryGetValue(keyState, out macro)) {

						// Return the execution sequence and true if the macro is found.
						executionSequence = macro.ExecutionSequence;
						return true;
					}
				}
			}

			// Return null and false if the macro is not found.
			executionSequence = null;
			return false;
		}

		/// <summary>
		/// Attempts to remove a macro given a device name and key.
		/// Remove any empty macro listings.
		/// </summary>
		/// <param name="deviceName">Name of the keyboard device.</param>
		/// <param name="key">Virtual key in question.</param>
		/// <returns>True if the macro was successfully removed.</returns>
		public bool RemoveEntry(string deviceName, VirtualKeys key, string keyState) {

			// Check if the device exists in the profile dictionary.
			KeyboardProfile keyboardProfile;
			if (this.mKeyboardProfiles.TryGetValue(deviceName, out keyboardProfile)) {

				// Check if the macro set exists in the current keyboard profile.
				KeyMacroSet macroSet;
				if (keyboardProfile.TryGetValue(key, out macroSet)) {

					// Attempt to remove the macro.
					if(macroSet.Remove(keyState)) {

						// If macro was found and remove, check if there are any more macros.
						if(!macroSet.Any()) {

							// Remove the macro set from the keyboard profile.
							keyboardProfile.Remove(key);

							// Check if there are any macros left in the profile.
							if(!keyboardProfile.Any()) {

								// Remove the profile if there are no profiles left.
								this.mKeyboardProfiles.Remove(deviceName);
							}
						}

						// Return true for a successful removal.
						return true;
					}
				}
			}

			// Return false if the macro is not found.
			return false;
		}
		#endregion

		#region Properties
		/// <summary>
		/// Stores all of the user defined settings for their keyboards.
		/// </summary>
		public Dictionary<string, KeyboardProfile> KeyboardProfiles {
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
			this.mKeyboardProfiles = new Dictionary<string, KeyboardProfile>();
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
			KeyboardProfile keyboardProfile;
			if (this.mKeyboardProfiles.TryGetValue(keyPressEvent.DeviceName, out keyboardProfile)) {

				// Check for a key and macros match.
				KeyMacroSet macroSet;
				if (keyboardProfile.TryGetValue((VirtualKeys)keyPressEvent.VKey, out macroSet)) {

					// Check if a macro can be found based on key state.
					KeyMacro macro;
					if(macroSet.TryGetValue(keyPressEvent.KeyPressState, out macro)) {

						// Execute macro and return true.
						macro.Execute();
						return true;
					}
				}
			}

			return false;
		}
		#endregion
	}
}
