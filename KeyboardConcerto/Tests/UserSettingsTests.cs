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
			execSeq.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.VK_F, ExecVKey.KeyState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_I, ExecVKey.KeyState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_R, ExecVKey.KeyState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_S, ExecVKey.KeyState.Press));
			execSeq.AddLast(new ExecVKey(WindowsInput.VirtualKeyCode.VK_T, ExecVKey.KeyState.Press));

			const string logitechKeyboardName = @"\\?\HID#VID_046D&PID_C31D&MI_00#8&133de0dc&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";
			const string asusKeyboardName = @"\\?\ACPI#PNP0303#4&16cfe3e0&0#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";
			const string duckyKeyboardName = @"\\?\HID#VID_04D9&PID_0183&MI_00#8&392462f2&0&0000#{884b96c3-56ef-11d1-bc8c-00a0c91405dd}";

			// [Control] Fake keyboard input to function as the test control.
			us.AddEntry(@"Fake Keyboard", RawInput.VirtualKeys.S, "MAKE", execSeq);

			// For ASUS laptop's keyboard. 'A' key triggers macro sequence.
			us.AddEntry(asusKeyboardName, RawInput.VirtualKeys.A, "MAKE", execSeq);

			// For Logitech K200 Keyboard. '4' key triggers "first" to be typed.
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D4, "MAKE", execSeq);

			// For Logitech K200 Keyboard. '1' key triggers 'Previous Media Track'.
			LinkedList<ExecNode> execSeq0 = new LinkedList<ExecNode>();
			execSeq0.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.MEDIA_PREV_TRACK, ExecVKey.KeyState.Press));
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D1, "MAKE", execSeq0);

			// For Logitech K200 Keyboard. '2' key triggers 'Next Media Track.
			LinkedList<ExecNode> execSeq1 = new LinkedList<ExecNode>();
			execSeq1.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.MEDIA_NEXT_TRACK, ExecVKey.KeyState.Press));
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D2, "MAKE", execSeq1);

			// For Logitech K200 Keyboard. '3' key triggers 'Launch Media'.
			LinkedList<ExecNode> execSeq2 = new LinkedList<ExecNode>();
			execSeq2.AddFirst(new ExecVKey(WindowsInput.VirtualKeyCode.LAUNCH_MEDIA_SELECT, ExecVKey.KeyState.Press));
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D3, "MAKE", execSeq2);

			// For Logitech K200 Keyboard. '5' key triggers 'Launch Media'.
			LinkedList<ExecNode> execSeq3 = new LinkedList<ExecNode>();
			execSeq3.AddFirst(new ExecCommand(Command.APP_LAUNCH, "C:/Program Files/iTunes/iTunes.exe"));
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D5, "MAKE", execSeq3);

			// For Logitech K200 Keyboard. '6' key triggers 'Launch Media'.
			LinkedList<ExecNode> execSeq4 = new LinkedList<ExecNode>();
			execSeq4.AddFirst(new ExecCommand(Command.HTTPS_LAUNCH, "www.reddit.com"));
			us.AddEntry(logitechKeyboardName, RawInput.VirtualKeys.D6, "MAKE", execSeq4);

			// For Ducky Shine II. '3' key triggers "first" to be typed.
			us.AddEntry(duckyKeyboardName, RawInput.VirtualKeys.D3, "MAKE", execSeq);

			return us;
		}

	}
}
