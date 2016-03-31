using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public static class Constants
	{
		public static string EventLogName = "BoklukLog";
		public const string AdminUser = "998";
		public const int AdminUserID = 998;
		public const string AdminPassword = "889";
		//public const string SelectCustomer = "Моля изберете клиент";
		//public const string SelectCustomerSite = "Моля изберете обект";

		//public const string SelectScrapType = "Моля, изберете клиент";

		//public const string Client = "Client";

		public static DateTime StartScheduleDate = new DateTime(2016, 4, 1);

		public const int StartShift = 3;

		public static int IndexNotSet = -1;

		public static double DefaultShiftHours = 8;

		public static double DefaultNightHours = 8;
	}
}
