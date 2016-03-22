﻿using System;
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

	public enum PositionTypes
	{
		Driver = 1, //	Шофьор  
		Doctor = 2,	//Лекар   
		MedicalStaff = 3,	//Медицинска сестра   
		Janitor = 4,	//Хигиенист   
		Mechanic = 5,	//Механик 
		Administration = 6,	//Администрация   
		Pharmacist = 7,	//Фармацевт   
		Paramedic = 8,	//Фелдшер 
		Security = 9,	//Охрана  			
	}

	public enum AbsenceTypes
	{
		YearPaidHoliday = 1, //	Полагаем годишен отпуск	  
		UnpaidHoliday = 2, //Неплатен отпуск   
		Sickness = 3,   //Болнични  
		BusinessTrip = 4,    //Командировка   
		Education = 5,   //Обучение 
		MotherhoodSickness = 6, //болнични при майчинство   
		Motherhood = 7, //Майчинство   
		MotherhoodExtend = 8,  //Удължено майчинство 
		OtherPaidHoliday =9 , //Друг платен отпуск
	}

	public enum ScheduleTypes
	{
		PresenceForm = 1,
		DailySchedule = 2,
		FinalMonthSchedule = 3,
		ForecastMonthSchedule = 4,
		SixMonthSchedule = 5
	}

	public enum SicknessTypes
	{
		Primary = 1,
		Continuaton = 2,
	}

	public enum PresenceTypes
	{
		Nothing = 0,
		DayShift = 1, //	дневна 12	  
		NightShift = 2, //нощна 12 
		RegularShift = 3,   //редовна  
		YearPaidHoliday = 4,    //годишен отпуск   
		Sickness = 5,   //болнични 
		Education = 6, //обучение  
		BusinessTrip = 7, //командировка  
		Motherhood = 8,  //майчинство 
		OtherPaidHoliday = 9, //Друг платен отпуск
		UnpaidHoliday = 10, //неплатен отпуск
	}

	public enum CrewTypes
	{
		Reanimation = 1,
		Doctor = 2,
		Paramedic = 3,
	}
}