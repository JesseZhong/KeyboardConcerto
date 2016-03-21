
using System.Windows;

namespace KeyboardConcerto {
	/// <summary>
	/// Interaction logic for MacrosWindow.xaml
	/// </summary>
	public partial class MacrosWindow : Window {
		public MacrosWindow() {
			this.InitializeComponent();

			this.Closing += this.Window_Closing;

			// Test code: Simulate keyboard input when buttons are clicked.
			#if DEBUG
			TestKeyboard.ForwardedKey += new OnScreenKeyboardClick((object sender, OnScreenKeyboardClickEventArgs e) => {
				WindowsInput.InputSimulator.SimulateKeyPress((WindowsInput.VirtualKeyCode)e.Key);
			});
			#endif
		}

		/// <summary>
		/// Intercept close button to only hide the window instead of destroying it.
		/// </summary>
		/// <param name="sender">This window.</param>
		/// <param name="e">Event arguments.</param>
		private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
			e.Cancel = true;
			this.Hide();
		}
	}
}