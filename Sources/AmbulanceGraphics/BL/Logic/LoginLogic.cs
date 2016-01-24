using BL.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Zora.Core;

namespace BL.Logic
{
	public class LoginLogic : BaseLogic
	{
		private SHA1CryptoServiceProvider sha1;
		public UserInfo currentUser;

		//private List<SitesModel> lstSitesModel;

		internal static Dictionary<string, UserInfo> usersDict = new Dictionary<string, UserInfo>();

		public LoginLogic()
		{
			usersDict = new Dictionary<string, UserInfo>();
			usersDict.Add(Constants.AdminUser, new UserInfo() { Name = "Developer-Tester", Password = Constants.AdminPassword, id_user = Constants.AdminUserID, IsAdmin = true, Role = Settings.AdministratorRole, RealName = Settings.AdministratorRole });
			//_databaseContext.Database.Connection.ConnectionString = cs;
		}

		public bool CheckUser(string userName, string password)
		{
			UserInfo info = usersDict[userName];

			if (password.ToLower().Trim() == info.Password.ToLower().Trim())
			{
				this.currentUser.Name = info.Name;
				this.currentUser.Password = info.Password;
				this.currentUser.id_user = info.id_user;
				this.currentUser.IsAdmin = info.IsAdmin;
				this.currentUser.RealName = info.RealName;
				this.currentUser.Role = info.Role;
				return true;
			}
			return false;
		}

		internal bool CheckUserPassword(string userName, string password)
		{
			if (LoginLogic.usersDict.ContainsKey(userName) == true)
			{
				if (CheckUser(userName, password) != true)
				{
					ThrowZoraException(ErrorCodes.WrongPassword);
				}
				Settings.id_userLogin = 1;
				return this.currentUser.IsAdmin;
			}

			var currentUserDb = this._databaseContext.UN_UserLogins.FirstOrDefault(u => u.UserName == userName);
			if (currentUserDb == null)
			{
				ThrowZoraException(ErrorCodes.UserNotFound);
				return false;
			}
			string pass;
			byte[] tempByteArr = Encoding.UTF8.GetBytes(password);
			pass = BitConverter.ToString(sha1.ComputeHash(tempByteArr)).Replace("-", "");

			if (currentUserDb.Password != pass)
			{
				ThrowZoraException(ErrorCodes.WrongPassword);
			}

			currentUser.id_user = currentUserDb.id_userLogin;
			currentUser.Name = currentUserDb.UserName;
			currentUser.Password = currentUserDb.Password;
			currentUser.Role = currentUserDb.NM_Roles.Name;
			currentUser.RealName = currentUserDb.UN_Persons.Name;
			if (currentUserDb.id_role == (int)Settings.Roles.Administrator)
			{
				currentUser.IsAdmin = true;
			}
			Settings.id_userLogin = currentUser.id_user;
			return currentUser.IsAdmin;
		}

		public bool Login(string user, string password)
		{
			return this.CheckUserPassword(user, password);
		}
	}

	public struct UserInfo
	{
		public string Password;
		public string Name;
		public int id_user;
		public bool IsAdmin;
		public string RealName;
		public string Role;
	}
}
