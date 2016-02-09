#region Usings
using System.Collections.Generic;
#endregion

namespace KeyboardConcerto.Tests {
	public static class UserSettingsTests {

		/// <summary>
		/// Creates a simple user settings with just VKeys.
		/// </summary>
		/// <returns>The created user settings.</returns>
		public static UserSettings BasicUserSettings() {
			UserSettings us = new UserSettings();

			LinkedList<ExecNode> execSeq = new LinkedList<ExecNode>();
			execSeq.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.VK_F, ExecVKey.EState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_I, ExecVKey.EState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_R, ExecVKey.EState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_S, ExecVKey.EState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_T, ExecVKey.EState.Press));

			// For ASUS laptop's keyboard. 'A' key triggers macro sequence.
			us.AddEntry(@"\\?\ACPI#PNP0303#4&16cfe3e0&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}", RawInput.VirtualKeys.A, execSeq);

			return us;
		}

	}
}
