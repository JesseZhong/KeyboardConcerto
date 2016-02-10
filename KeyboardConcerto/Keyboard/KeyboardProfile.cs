// KeyboardMacros.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Collections.Generic;
using KeyboardConcerto.RawInput;
#endregion

namespace KeyboardConcerto {

	/// <summary>
	/// Alias and expansion for storing keys, their key states, and their macros.
	/// In the order they are nested: key code, key state, and key macro.
	/// (Sanity Check) Having the key code the wrapping dictionary key not only makes 
	/// logical sense, but it allows fewer lookups than putting it any other order in 
	/// most cases.
	/// Example: If you placed key state first, all key presses will make it to the
	/// key code lookup stage, therefore wasting resources and time based on the 
	/// frequency of key presses.
	/// </summary>
	public class KeyboardProfile : Dictionary<VirtualKeys, KeyMacroSet> {

	}

	/// <summary>
	/// Alias and expansion for storing key states and macros.
	/// </summary>
	public class KeyMacroSet : Dictionary<string, KeyMacro> {

	}
}
