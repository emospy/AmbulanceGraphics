using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public class Settings
	{
		public static int id_userLogin;
		public static string UserName;
		public static string ConnectionString;

		private const string dbUser = "sa";
		private const string password = "teSSla56";

		public const string AdministratorRole = "Administrator";
		public const string OperatorRole = "Operator";
		public const string CustomsRole = "Customs";
		public const string ClientRole = "Operator";
		public static string DefaultLanguage = "bg-BG";

		public static void SetOwnConnectionStringEntity(string address = "localhost", string database = "Ambulance")
		{
			string model = "DB.AmbulanceModel";
			ConnectionString = string.Format(
@"metadata=res://*/{0}.csdl|res://*/{0}.ssdl|res://*/{0}.msl;provider=System.Data.SqlClient;provider connection string=
'Data Source={1};Initial Catalog={2};Persist Security Info=True;User ID={3};Password={4};MultipleActiveResultSets=True'", model, address, database, dbUser, password);
		}

		public enum Roles
		{
			Administrator = 1,
			Operator,
			Client
		};
	}
}
