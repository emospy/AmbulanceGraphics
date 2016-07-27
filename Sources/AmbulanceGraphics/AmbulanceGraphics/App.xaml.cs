using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Threading;
using System.Globalization;
using Telerik.Windows.Controls;

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		private void ApplicationStart(object sender, StartupEventArgs e)
		{
			EventManager.RegisterClassHandler(typeof(TextBox), TextBox.KeyDownEvent, new KeyEventHandler(TextBox_KeyDown));
			EventManager.RegisterClassHandler(typeof(CheckBox), CheckBox.KeyDownEvent, new KeyEventHandler(CheckBox_KeyDown));
			//Disable shutdown when the dialog closes
			Current.ShutdownMode = ShutdownMode.OnExplicitShutdown;

			var dialog = new LoginWindow();

			if (dialog.ShowDialog() == true)
			{
				var mainWindow = new MainWindow(dialog.currentUser);
				//Re-enable normal shutdown mode.
				Current.ShutdownMode = ShutdownMode.OnMainWindowClose;
				Current.MainWindow = mainWindow;

				Thread.CurrentThread.CurrentCulture = new CultureInfo("bg");
				Thread.CurrentThread.CurrentUICulture = new CultureInfo("bg");

				LocalizationManager.Manager = new CustomLocalizationManager();
				//LocalizationManager.Manager = new LocalizationManager()
				//{
				//	ResourceManager = Gr GridViewResources.ResourceManager 
				//};

				mainWindow.Show();
			}
			else
			{
				MessageBox.Show("Отказан достъп.", "Грешка", MessageBoxButton.OK);
				Current.Shutdown(-1);
			}
		}

		

		void TextBox_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter & (sender as TextBox).AcceptsReturn == false) MoveToNextUIElement(e);
		}

		void CheckBox_KeyDown(object sender, KeyEventArgs e)
		{
			MoveToNextUIElement(e);
			//Sucessfully moved on and marked key as handled.
			//Toggle check box since the key was handled and
			//the checkbox will never receive it.
			if (e.Handled == true)
			{
				CheckBox cb = (CheckBox)sender;
				cb.IsChecked = !cb.IsChecked;
			}

		}

		void MoveToNextUIElement(KeyEventArgs e)
		{
			// Creating a FocusNavigationDirection object and setting it to a
			// local field that contains the direction selected.
			FocusNavigationDirection focusDirection = FocusNavigationDirection.Next;

			// MoveFocus takes a TraveralReqest as its argument.
			TraversalRequest request = new TraversalRequest(focusDirection);

			// Gets the element with keyboard focus.
			UIElement elementWithFocus = Keyboard.FocusedElement as UIElement;

			// Change keyboard focus.
			if (elementWithFocus != null)
			{
				if (elementWithFocus.MoveFocus(request)) e.Handled = true;
			}
		}
	}

	public class CustomLocalizationManager : LocalizationManager
	{
		public override string GetStringOverride(string key)
		{
			switch (key)
			{
				case "GridViewGroupPanelText":
					return "Zum gruppieren ziehen Sie den Spaltenkopf in diesen Bereich.";
				//---------------------- RadGridView Filter Dropdown items texts:
				case "GridViewClearFilter":
					return "Изчисти филтъра";
				case "GridViewFilterShowRowsWithValueThat":
					return "Филтрирай редове със стойност която:";
				case "GridViewFilterSelectAll":
					return "Избери всичко";
				case "GridViewFilterContains":
					return "Съдържа";
				case "GridViewFilterEndsWith":
					return "Завършва с";
				case "GridViewFilterIsContainedIn":
					return "Се съдържа в";
				case "GridViewFilterIsEqualTo":
					return "Равно на";
				case "GridViewFilterIsGreaterThan":
					return "По-голямо от ";
				case "GridViewFilterIsGreaterThanOrEqualTo":
					return "По-голямо или равно";
				case "GridViewFilterIsLessThan":
					return "По-малко";
				case "GridViewFilterIsLessThanOrEqualTo":
					return "По-малко или равно";
				case "GridViewFilterIsNotEqualTo":
					return "Различно";
				case "GridViewFilterStartsWith":
					return "Започва с";
				case "GridViewFilterAnd":
					return "И";
				case "GridViewFilter":
					return "Филтрирай";
			}
			return base.GetStringOverride(key);
		}
	}
}
