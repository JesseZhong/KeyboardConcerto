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

			// [Control] Fake keyboard input to function as the test control.
			us.AddEntry(@"Fake Keyboard", RawInput.VirtualKeys.S, "MAKE", execSeq);

			// For ASUS laptop's keyboard. 'A' key triggers macro sequence.
			us.AddEntry(@"\\?\ACPI#PNP0303#4&16cfe3e0&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}", RawInput.VirtualKeys.A, "MAKE", execSeq);

			// For Logitech K200 Keyboard. '1' key triggers macro sequence.
			LinkedList<ExecNode> execSeq0 = new LinkedList<ExecNode>();
			execSeq0.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.MEDIA_PREV_TRACK, ExecVKey.EState.Press));
			us.AddEntry(@"\\?\HID#VID_046D&PID_C31D&MI_00#9&38b6029d&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}", RawInput.VirtualKeys.D1, "MAKE", execSeq0);

			// For Logitech K200 Keyboard. '2' key triggers macro sequence.
			LinkedList<ExecNode> execSeq1 = new LinkedList<ExecNode>();
			execSeq1.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.MEDIA_NEXT_TRACK, ExecVKey.EState.Press));
			us.AddEntry(@"\\?\HID#VID_046D&PID_C31D&MI_00#9&38b6029d&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}", RawInput.VirtualKeys.D2, "MAKE", execSeq1);

			// For Ducky Shine II. '2' key triggers macro sequence.
			us.AddEntry(@"\\?\HID#VID_04D9&PID_0183&MI_00#8&6d34b6b&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}", RawInput.VirtualKeys.D3, "MAKE", execSeq);

			return us;
		}

	}
}
