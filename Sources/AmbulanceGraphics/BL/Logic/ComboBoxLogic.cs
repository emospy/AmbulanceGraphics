using BL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using BL.DB;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class ComboBoxLogic : BaseLogic, IDisposable
	{
		public enum ArticleGroups
		{
			Stock = 1,
			Scrap = 2,
			Materials = 3,
			ScrapMaterials = 4,
			ScrapReceivers = 5,
		}

		//public List<GR_Ambulances> GetAmbulances(int id_ambulance)
		//{
		//	List<GR_Ambulances> result = new List<GR_Ambulances>();
		//	result.Add(new GR_Ambulances() { id_ambulance = 0, Name = "", IsActive = true });
		//	var queryResult = this._databaseContext.GR_Ambulances.Where(c => c.IsActive == true).ToList();
		//	result.AddRange(queryResult);
		//	if (id_ambulance != 0)
		//	{
		//		if (result.Any(c => c.id_ambulance == id_ambulance) == false)
		//		{
		//			var ambulance = this._databaseContext.GR_Ambulances.FirstOrDefault(c => c.id_ambulance == id_ambulance);
		//			if (ambulance != null)
		//			{
		//				result.Add(ambulance);
		//			}
		//		}
		//	}
		//	return result;
		//}

		public List<ComboBoxModel> ReadAllDepartments()
		{
			var queryResult = this._databaseContext.UN_Departments.Select(c => new ComboBoxModel { id = c.id_department, IsActive = c.IsActive, Name = c.Name }).ToList();
			return queryResult;
		}

		public List<HR_GlobalPositions> ReadGlobalPositions(int id_globalPosition)
		{			
            List<HR_GlobalPositions> result = new List<HR_GlobalPositions>();
			result.Add(new HR_GlobalPositions() { id_globalPosition = 0, Name = "", IsActive = true });
			var queryResult = this._databaseContext.HR_GlobalPositions.Where(c => c.IsActive == true).Select(c => c).ToList();
			result.AddRange(queryResult);
			if (id_globalPosition != 0)
			{
				if (result.Any(c => c.id_globalPosition == id_globalPosition) == false)
				{
					var globalPosition = this._databaseContext.HR_GlobalPositions.FirstOrDefault(c => c.id_globalPosition == id_globalPosition);
					if (globalPosition != null)
					{
						result.Add(globalPosition);
					}
				}
			}
			return result;
		}

		public List<ComboBoxModel> GetPersonalYearHolidays(int id_contract)
		{
			List<ComboBoxModel> result = new List<ComboBoxModel>();
			
			var res = this._databaseContext.HR_YearHolidays.Where(a => a.id_contract == id_contract)
				.Select(a => new ComboBoxModel { id = a.id_contract, Name = a.Name.ToString(), IsActive = true }).ToList().OrderByDescending(a => a.Name);
			result.AddRange(res);

			return result;
		}

		//public void AddYearHolidays()
		//{
		//	var lstContracts = this._databaseContext.HR_Contracts.Where(c => c.IsFired == false).ToList();
		//	foreach(var con in lstContracts)
		//	{
		//		HR_YearHolidays yh = new HR_YearHolidays();
		//		yh.id_contract = con.id_contract;
		//		yh.IsActive = true;
		//		yh.Leftover = 20;
		//		yh.Total = 20;
		//		yh.Name = 2016;
		//		this._databaseContext.HR_YearHolidays.Add(yh);
		//	}
		//	this.Save();
		//}

		public List<ComboBoxModel> GetLevel(int? id_level, int id_parent = 0)
		{
			List<ComboBoxModel> result = new List<ComboBoxModel>();
			result.Add(new ComboBoxModel() { id = 0, Name = "", IsActive = true });
			if (id_parent == 0)
			{
				var res = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == a.id_department)
					.Select(a => new ComboBoxModel { id = a.id_department, Name = a.Name, IsActive = a.IsActive }).ToList();
				result.AddRange(res);
			}
			else
			{
				var res = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == id_parent && a.id_departmentParent != a.id_department)
					.Select(a=> new ComboBoxModel { id = a.id_department, Name = a.Name, IsActive = a.IsActive }).ToList();
				result.AddRange(res);
            }

			if(id_level != 0)
			{
				if (result.Any(c => c.id == id_level) == false)
				{
					var department = this._databaseContext.UN_Departments.FirstOrDefault(c => c.id_department == id_level);
					if (department != null)
					{
						result.Add(new ComboBoxModel { id = department.id_department, Name = department.Name, IsActive = department.IsActive });
					}
				}
			}
			
			return result;
		}

		public List<ComboBoxModel> GetStructurePositions(int id_structurePosition, int id_department)
		{
			List<ComboBoxModel> result = new List<ComboBoxModel>();
			result.Add(new ComboBoxModel() { id = 0, Name = "", IsActive = true });
			
			var res = this._databaseContext.HR_StructurePositions.Where(a => a.id_department == id_department)
				.Select(a => new ComboBoxModel { id = a.id_structurePosition, Name = a.HR_GlobalPositions.Name, IsActive = a.IsActive }).ToList();
			
			result.AddRange(res);
			
			if (id_structurePosition != 0)
			{
				if (result.Any(c => c.id == id_structurePosition) == false)
				{
					var sp = this._databaseContext.HR_StructurePositions.FirstOrDefault(c => c.id_structurePosition == id_structurePosition);
					if (sp != null)
					{
						result.Add(new ComboBoxModel { id = sp.id_structurePosition, Name = sp.HR_GlobalPositions.Name, IsActive = sp.IsActive });
					}
				}
			}

			return result;
		}
	}
}