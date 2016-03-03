using System;
using System.Windows.Controls;

namespace KeyboardConcerto {

	using VKey = WindowsInput.VirtualKeyCode;

	public class OnScreenKeyboardClickEventArgs : EventArgs {

	}

	public delegate void OnScreenKeyboardClick(object sender, OnScreenKeyboardClickEventArgs e);

	/// <summary>
	/// Interaction logic for OnScreenKeyboard.xaml
	/// </summary>
	public partial class OnScreenKeyboard : UserControl {
		public OnScreenKeyboard() {
			this.InitializeComponent();
		}

		public OnScreenKeyboardClickEventArgs ForwardedKey;

		private void ForwardVKey(VKey key) {
			if(this.ForwardedKey != null) {

			}
		}

		private void KeyNumDecimal_Click(object sender, System.Windows.RoutedEventArgs e) {

		}
	}
}
