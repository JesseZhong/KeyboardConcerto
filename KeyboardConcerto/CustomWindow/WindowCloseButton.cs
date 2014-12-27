using System;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;

namespace KeyboardConcerto.CustomWindow {
	public class WindowCloseButton : WindowButton {

		public WindowCloseButton() {
			this.Width = 43;

			// open resource where in XAML are defined some required stuff such as icons and colors
			Stream resourceStream = Application.GetResourceStream(new Uri("pack://application:,,,/KeyboardConcerto;Component/CustomWindow/ButtonIcons.xaml")).Stream;
			ResourceDictionary resourceDictionary = (ResourceDictionary)XamlReader.Load(resourceStream);

			//
			// Foreground (represents a backgroundcolor when Mouse is over)
			this.Foreground = (Brush)resourceDictionary["RedButtonMouseOverBackground"];

			// set icon
			this.Content = resourceDictionary["WindowButtonCloseIcon"];

			// radius
			this.CornerRadius = new CornerRadius(0, 0, 3, 0);
		}
	}
}
