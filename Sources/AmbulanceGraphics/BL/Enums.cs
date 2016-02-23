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

	public enum PosiitonTypes
	{
		Driver = 1, //	Шофьор  
		Medic = 2,	//Лекар   
		Nurse = 3,	//Медицинска сестра   
		Janitor = 4,	//Хигиенист   
		Mechanic = 5,	//Механик 
		Administration = 6,	//Администрация   
		Pharmacist = 7,	//Фармацевт   
		Paramedic = 8,	//Фелдшер 
		Security = 9,	//Охрана  			
	}
}
