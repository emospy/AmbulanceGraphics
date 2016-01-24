using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public enum MeasurementUnits
	{
		Kg = 1,
		Lt = 2,
		m3,
	}

	public enum UserAuthenticationResult
	{
		Success = 1,
		WrongName = -1,
		WrongPassword = -2,
	}

	public enum ArticleGroups
	{
		Stock = 1,
		Scrap = 2,
		Materials = 3,
		ScrapMaterials = 4,
		ScrapReceivers = 5,
	}
}
