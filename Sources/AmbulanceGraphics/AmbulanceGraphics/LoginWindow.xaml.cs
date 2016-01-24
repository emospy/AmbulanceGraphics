using BL.Logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace AmbulanceGraphics
{
	/// <summary>
	/// Interaction logic for LoginWindow.xaml
	/// </summary>
	public partial class LoginWindow : Window
	{
		public UserInfo currentUser;
		public LoginWindow()
		{
			InitializeComponent();			
		}

		private void btnLogin_Click(object sender, RoutedEventArgs e)
		{
			LoginLogic logic = new LoginLogic();

			try
			{
				logic.Login(this.txtUserName.Text, this.txtPassword.Password);
				this.DialogResult = true;
				currentUser = logic.currentUser;
				this.Close();				
			}
			catch(Zora.Core.Exceptions.ZoraException ex)
			{
				MessageBox.Show(ex.Result.ErrorCodeMessage);
			}
		}

		private void btnCancel_Click(object sender, RoutedEventArgs e)
		{
			this.DialogResult = false;
			this.Close();
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			this.txtUserName.Focus();
			//this.DialogResult = false;
		}
	}
}
