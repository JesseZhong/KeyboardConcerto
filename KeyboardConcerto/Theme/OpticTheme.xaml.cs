// WindowTemplate.cs
// Authored by Jesse Z. Zhong
#region Usings
using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
#endregion

namespace KeyboardConcerto.Theme {

	/// <summary>
	/// Enumerates the user interaction events for the windows theme.
	/// </summary>
	public partial class OpticTheme : ResourceDictionary {

		#region Mouse Interaction Event Handling
		/// <summary>
		/// Drags the window with the mouse when the top bar is pressed.
		/// </summary>
		/// <param name="sender">TopBar</param>
		/// <param name="e">Event arguments.</param>
		private void TopBar_MouseMove(object sender, MouseEventArgs e) {
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
				Window window = (Window)((FrameworkElement)sender).TemplatedParent;
				window.DragMove();
			}
		}
		#endregion

		#region Window Button Event Handling
		/// <summary>
		/// Procures when Minimize button clicked.
		/// </summary>
		/// <param name="sender">Minimize button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnMinimizeButton_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Procures when Close button clicked.
		/// </summary>
		/// <param name="sender">Close button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnCloseButton_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.Close();
		}
		#endregion
	}
}
