using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using KeyboardConcerto.CustomWindow;

namespace KeyboardConcerto {
	public partial class WindowTemplate : ResourceDictionary {

		private void WindowLoaded(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.StateChanged += new EventHandler(this.StandardWindow_StateChanged);
		}

		// called when state of the window changed to minimized, normal or maximized
		private void StandardWindow_StateChanged(object sender, EventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			
			if (window.WindowState == WindowState.Normal) {
				WindowRestoreButton restoreButton = (WindowRestoreButton)this["RestoreButton"];
				restoreButton.Visibility = Visibility.Collapsed;
			} else if (window.WindowState == WindowState.Maximized) {
				WindowMaximizeButton maximizeButton = (WindowMaximizeButton)this["MaximizeButton"];
				maximizeButton.Visibility = Visibility.Collapsed;
			}
		}

		private void Header_MouseMove(object sender, System.Windows.Input.MouseEventArgs e) {
			if (e.LeftButton == System.Windows.Input.MouseButtonState.Pressed) {
				Window window = (Window)((FrameworkElement)sender).TemplatedParent;
				window.DragMove();
			}
		}

		#region Window Button Event Handling
		/// <summary>
		/// Procures when Minimize button clicked.
		/// </summary>
		/// <param name="sender">Minimize button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonMinimize_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = WindowState.Minimized;
		}

		/// <summary>
		/// Procures when Restore button clicked.
		/// </summary>
		/// <param name="sender">Restore button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonRestore_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = WindowState.Normal;
		}

		/// <summary>
		/// Procures when Maximize button clicked.
		/// </summary>
		/// <param name="sender">Maximize button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonMaximize_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.WindowState = WindowState.Maximized;
		}

		/// <summary>
		/// Procures when Close button clicked.
		/// </summary>
		/// <param name="sender">Close button.</param>
		/// <param name="e">Event arguments.</param>
		private void OnButtonClose_Click(object sender, RoutedEventArgs e) {
			Window window = (Window)((FrameworkElement)sender).TemplatedParent;
			window.Close();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="state"></param>
		/// <param name="button"></param>
		private void OnWindowButtonStateChange(WindowButtonState state, WindowButton button) {
			switch (state) {
				case WindowButtonState.Normal:
					button.Visibility = Visibility.Visible;
					button.IsEnabled = true;
					break;

				case WindowButtonState.Disabled:
					button.Visibility = Visibility.Visible;
					button.IsEnabled = false;
					break;

				case WindowButtonState.None:
					button.Visibility = Visibility.Collapsed;
					break;
			}
		}
		#endregion
	}
}
