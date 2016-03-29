using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class NomenclaturesLogic : BaseLogic
	{
		public NomenclaturesLogic()
		{
			//_databaseContext = new AmbulanceEntities();
			//_databaseContext.Database.Connection.ConnectionString = cs;
		}

		#region Service Methods
		public List<GR_Ambulances> GetAmbulances(bool IsActiveOnly)
		{
			var query = this._databaseContext.GR_Ambulances.Select(a => a);
			if (IsActiveOnly == true)
			{
				query = query.Where(a => a.IsActive == true);
			}
			return query.OrderBy(a => a.Name).ToList();
		}
		public List<NM_AmbulanceTypes> GetAmbulanceTypes(bool v)
		{
			throw new NotImplementedException();
		}
		public UN_Departments GetDepartmentByName(string name)
		{
			var result = this._databaseContext.UN_Departments.Where(a => a.Name == name).Single();
			return result;
		}
		public CalendarRow FillCalendarRow(DateTime date)
		{
			CalendarRow row = new CalendarRow(date);
			var lstWorkDays = this._databaseContext.HR_YearWorkDays.Where(wd => wd.Date.Year == date.Year && wd.Date.Month == date.Month).ToList();

			foreach (var hrYearWorkdayse in lstWorkDays)
			{
				row[hrYearWorkdayse.Date.Day] = (bool)hrYearWorkdayse.IsWorkDay;
			}
			return row;
		}
		public CalendarRow FillCalendarRowNH(DateTime date)
		{
			CalendarRow row = new CalendarRow(date, true);
			var lstWorkDays = this._databaseContext.HR_YearWorkDays.Where(wd => wd.Date.Year == date.Year && wd.Date.Month == date.Month).ToList();

			foreach (var hrYearWorkdayse in lstWorkDays)
			{
				row[hrYearWorkdayse.Date.Day] = (bool)hrYearWorkdayse.IsNationalHoliday;
			}
			return row;
		}
		public UN_Departments GetDepartmentShiftByName(string gstr, int id_currentDepartment)
		{
			//var res = this._databaseContext.UN_Departments.Where(a => a.Name == gstr).ToList();
			var result = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == id_currentDepartment
																		&& a.Name == gstr).Single();
			return result;
		}
		public List<HR_GlobalPositions> GetGlobalPositions(bool IsActiveOnly)
		{
			using (var data = new AmbulanceEntities())
			{
				var query = data.HR_GlobalPositions.Select(a => a);

				if (IsActiveOnly == true)
				{
					query = query.Where(a => a.IsActive == true);
				}
				return query.OrderBy(a => a.Name).ToList();
			}
		}
		public bool MoveStructurePositionUp(int id_structurePosition)
		{
			var oSource = this.HR_StructurePositions.GetById(id_structurePosition);

			var oDest = this._databaseContext.HR_StructurePositions
				.Where(a => a.id_department == oSource.id_department && a.Order < oSource.Order)
				.OrderByDescending(a => a.Order).FirstOrDefault();
			if (oSource != null && oDest != null)
			{
				int tmp = oSource.Order;
				oSource.Order = oDest.Order;
				oDest.Order = tmp;
				this.Save();
				return true;
			}
			return false;
		}
		public void SaveCalendarRow(CalendarRow cal, DateTime CurrentDate)
		{
			var lstWorkDays = this._databaseContext.HR_YearWorkDays.Where(a => a.Date.Year == CurrentDate.Year && a.Date.Month == CurrentDate.Month).ToList();

			for (int i = 1; i < DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month) + 1; i++)
			{
				DateTime CD = new DateTime(CurrentDate.Year, CurrentDate.Month, i);
				var day = lstWorkDays.Find(wd => wd.Date == CD);

				if ((day == null) && (((CD.DayOfWeek == DayOfWeek.Saturday || CD.DayOfWeek == DayOfWeek.Sunday) && cal[i]) || ((CD.DayOfWeek != DayOfWeek.Saturday && CD.DayOfWeek != DayOfWeek.Sunday) && !cal[i])))
				{ //if it is an exception
					day = new HR_YearWorkDays();
					day.Date = new DateTime(CurrentDate.Year, CurrentDate.Month, i);
					day.IsWorkDay = cal[i];

					this._databaseContext.HR_YearWorkDays.Add(day);
				}
				else if ((day != null) && (((CD.DayOfWeek == DayOfWeek.Saturday || CD.DayOfWeek == DayOfWeek.Sunday) && !cal[i]) || ((CD.DayOfWeek != DayOfWeek.Saturday && CD.DayOfWeek != DayOfWeek.Sunday) && cal[i])))
				{
					this._databaseContext.HR_YearWorkDays.Remove(day);
				}
			}
			this.Save();
		}
		public void SaveCalendarRowNH(CalendarRow cal, DateTime CurrentDate)
		{
			var lstWorkDays = this._databaseContext.HR_YearWorkDays.Where(a => a.Date.Year == CurrentDate.Year && a.Date.Month == CurrentDate.Month).ToList();

			for (int i = 1; i < DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month) + 1; i++)
			{
				DateTime CD = new DateTime(CurrentDate.Year, CurrentDate.Month, i);
				var day = lstWorkDays.Find(wd => wd.Date == CD);

				if(day != null && cal[i] == true)
				{
					day.IsNationalHoliday = true;
				}
				else if(day != null && cal[i] == false)
				{
					day.IsNationalHoliday = false;
				}
				else
				{
					if(cal[i] == true)
					{
						day = new HR_YearWorkDays();
						day.Date = new DateTime(CurrentDate.Year, CurrentDate.Month, i);
						day.IsWorkDay = false;
						day.IsNationalHoliday = true;

						this._databaseContext.HR_YearWorkDays.Add(day);
					}
				}
			}
			this.Save();
		}
		public bool MoveTreeNodeUp(int id_department)
		{
			var oSource = this.UN_Departments.GetById(id_department);
			UN_Departments oDest;

			if (oSource.id_departmentParent != oSource.id_department)
			{
				oDest = this._databaseContext.UN_Departments
				.Where(a => a.id_departmentParent == oSource.id_departmentParent && a.TreeOrder < oSource.TreeOrder)
				.OrderByDescending(a => a.TreeOrder).FirstOrDefault();
			}
			else
			{
				oDest = this._databaseContext.UN_Departments
				.Where(a => a.id_department == a.id_departmentParent && a.TreeOrder < oSource.TreeOrder)
				.OrderByDescending(a => a.TreeOrder).FirstOrDefault();
			}

			if (oSource != null && oDest != null)
			{
				int tmp = oSource.TreeOrder;
				oSource.TreeOrder = oDest.TreeOrder;
				oDest.TreeOrder = tmp;
				this.Save();
				return true;
			}
			return false;
		}
		public bool MoveTreeNodeDown(int id_department)
		{
			var oSource = this.UN_Departments.GetById(id_department);
			UN_Departments oDest;

			if (oSource.id_departmentParent != oSource.id_department)
			{
				oDest = this._databaseContext.UN_Departments
				.Where(a => a.id_departmentParent == oSource.id_departmentParent && a.TreeOrder > oSource.TreeOrder)
				.OrderBy(a => a.TreeOrder).FirstOrDefault();
			}
			else
			{
				oDest = this._databaseContext.UN_Departments
				.Where(a => a.id_department == a.id_departmentParent && a.TreeOrder > oSource.TreeOrder)
				.OrderBy(a => a.TreeOrder).FirstOrDefault();
			}

			if (oSource != null && oDest != null)
			{
				int tmp = oSource.TreeOrder;
				oSource.TreeOrder = oDest.TreeOrder;
				oDest.TreeOrder = tmp;
				this.Save();
				return true;
			}
			return false;
		}
		public HR_StructurePositions FindStructurePositionByName(string gstr, int id_currentDepartment)
		{
			var result = this._databaseContext.HR_StructurePositions.SingleOrDefault(a => a.HR_GlobalPositions.Name == gstr && a.id_department == id_currentDepartment);
			return result;
		}
		public HR_GlobalPositions GetGlobalPositionByName(string gstr)
		{
			var result = this._databaseContext.HR_GlobalPositions.Where(a => a.Name == gstr).Single();
			return result;
			//ДИРЕКТОР, ЛЕЧЕБНО ЗАВЕД.
			//ДИРЕКТОР,  ЛЕЧЕБНО ЗАВЕД.
		}
		public bool MoveStructurePositionDown(int id_structurePosition)
		{
			var oSource = this.HR_StructurePositions.GetById(id_structurePosition);

			var oDest = this._databaseContext.HR_StructurePositions
				.Where(a => a.id_department == oSource.id_department && a.Order > oSource.Order)
				.OrderBy(a => a.Order).FirstOrDefault();
			if (oSource != null && oDest != null)
			{
				int tmp = oSource.Order;
				oSource.Order = oDest.Order;
				oDest.Order = tmp;
				this.Save();
				return true;
			}
			return false;
		}
		public void DeleteDepartment(int id_department)
		{

			var department = this.UN_Departments.GetById(id_department);

			this.UN_Departments.Delete(department);

			this.Save();
		}
		public List<StructurePositionViewModel> GetStructurePositions(int id_department, bool IsActiveOnly)
		{
			using (var data = new AmbulanceEntities())
			{
				var query = data.HR_StructurePositions.Where(a => a.id_department == id_department);

				if (IsActiveOnly == true)
				{
					query = query.Where(a => a.IsActive == true);
				}

				var result = query.OrderBy(a => a.Order).ToList();
				var result2 = result
					.Select(a => new StructurePositionViewModel
					{
						Code = a.Code,
						GlobalPositionName = a.HR_GlobalPositions.Name,
						id_department = a.id_department,
						id_structurePosition = a.id_structurePosition,
						PositionType = a.HR_GlobalPositions.NM_PositionTypes.Name,
						StaffCount = a.StaffCount.ToString(),
						Order = a.Order
					}).ToList();
				return result2;
			}
		}

		//public UN_DepartmentTree GetTreeNode(int id_department)
		//{
		//	var result = this._databaseContext.UN_DepartmentTree.FirstOrDefault(a => a.id_department == id_department);

		//	return result;
		//}

		public int GetTreeLevel(int id_department)
		{
			int level;
			var result = this._databaseContext.UN_Departments.FirstOrDefault(a => a.id_department == id_department);
			level = 0;
			if (result == null)
			{
				level = 0;
			}
			else if (result.id_departmentParent == result.id_department)
			{
				level = 1;
			}
			else if (result.UN_Departments2.id_departmentParent == result.UN_Departments2.id_department)
			{
				level = 2;
			}
			else if (result.UN_Departments2.UN_Departments2.id_departmentParent == result.UN_Departments2.UN_Departments2.id_department)
			{
				level = 3;
			}
			else if (result.UN_Departments2.UN_Departments2.UN_Departments2.id_departmentParent == result.UN_Departments2.UN_Departments2.UN_Departments2.id_department)
			{
				level = 4;
			}
			return level;
		}
		public void RecalculateLevels()
		{
			var lstDepartments = this.UN_Departments.GetAll();

			foreach (var dep in lstDepartments)
			{
				dep.Level = this.GetTreeLevel(dep.id_department);
			}

			this.Save();
		}
		public void SetWorkTime()
		{
			var lstDepartments = this._databaseContext.UN_Departments.Where(a => a.Level == 2).ToList();

			lstDepartments = lstDepartments.Where(a => a.Name.ToLower().Contains("смяна")).ToList();

			foreach (var department in lstDepartments)
			{
				var lstContracts = this._databaseContext.HR_Assignments
					.Where(a => a.IsActive == true
							&& (a.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int) PositionTypes.Doctor
								|| a.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int) PositionTypes.Driver
								|| a.HR_StructurePositions.HR_GlobalPositions.id_positionType == (int) PositionTypes.MedicalStaff)).ToList();
				foreach (var contract in lstContracts)
				{
					contract.id_workTime = 2;
				}

				var lstPositions =
					this._databaseContext.HR_StructurePositions.Where(a => a.id_department == department.id_department).ToList();

				var lstGlobals = new List<int> {6, 18, 21, 46, 50};
				foreach (var i in lstGlobals)
				{
					var pos = lstPositions.FirstOrDefault(a => a.id_globalPosition == i);
					if (pos == null)
					{
						pos = FillNewPosition(i, department.id_department);
						this._databaseContext.HR_StructurePositions.Add(pos);
					}
				}
			}
			this.Save();
			var lstPositionsOrder = this._databaseContext.HR_StructurePositions.Where(a => a.Order == 0).ToList();
			foreach (var pos in lstPositionsOrder)
			{
				pos.Order = pos.id_structurePosition;
			}
			this.Save();
		}
		private HR_StructurePositions FillNewPosition(int id_gl, int id_dep)
		{
			HR_StructurePositions pos;
			pos = new HR_StructurePositions();
			pos.ActiveFrom = DateTime.Now;
			pos.Code = "";
			pos.IsActive = true;
			pos.Order = 0;
			pos.StaffCount = 1;
			pos.id_department = id_dep;
			pos.id_globalPosition = id_gl;
			return pos;
		}

		#endregion
		public new void Save()
		{
			try
			{
				_databaseContext.SaveChanges();
			}
			catch (EntityException)
			{
				ThrowZoraException(ErrorCodes.NoDb);
			}
			catch (DbUpdateException ex)
			{
				Exception exp = ex.InnerException;
				while (exp.InnerException != null)
				{
					exp = exp.InnerException;
				}

				if (exp is SqlException)
				{
					var sqexp = exp as SqlException;
					if (sqexp.Number == 547)
					{
						ThrowZoraException(ErrorCodes.DeleteRecordAlreadyReferred);
					}
					else if (sqexp.Number == 2627)
					{
						ThrowZoraException(ErrorCodes.DuplicateName);
					}
					else
					{
						throw exp;
					}
				}
				ThrowZoraException(ErrorCodes.NoDb);
			}
			catch (System.Data.Entity.Validation.DbEntityValidationException ex)
			{
				var lstErrors = ex.EntityValidationErrors.ToList();
				List<string> lstMissingFields = new List<string>();
				foreach (var Error in lstErrors)
				{
					foreach (var Err in Error.ValidationErrors)
					{
						lstMissingFields.Add(Err.PropertyName);
					}
				}
				string message = "Има непопълени задължителни полета: ";
				foreach (var s in lstMissingFields)
				{
					message += " " + s;
				}
				ThrowZoraException(ErrorCodes.FieldNotFilled, true, message, true);
			}
			catch (Exception ex)
			{
				ThrowZoraException(ErrorCodes.NoDb);
			}
		}
	}
}
