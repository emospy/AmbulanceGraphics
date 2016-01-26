using BL.DB;
using BL.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zora.Core.Logic;

namespace BL.Logic
{
	public class NomenclaturesLogic : BaseLogic
	{
		//internal readonly AmbulanceEntities _databaseContext;

		public NomenclaturesLogic()
		{
			//_databaseContext = new AmbulanceEntities();
			//_databaseContext.Database.Connection.ConnectionString = cs;
		}

		#region Service Methods
		public List<GR_Ambulances> GetAmbulances(bool IsActiveOnly)
		{
			using (var data = new AmbulanceEntities())
			{
				var query = data.GR_Ambulances.Select(a => a);

				if (IsActiveOnly == true)
				{
					query = query.Where(a => a.IsActive == true);
				}
				return query.OrderBy(a => a.RegNumber).ToList();
			}
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

		public List<StructureTreeViewModel> GetTreeNodes(bool IsRoot, int id_departmentParent)
		{
			if (IsRoot)
			{
				var result = this._databaseContext.UN_DepartmentTree.Where(a => a.id_departmentTree == a.id_departmentParent)
					.OrderBy(a => a.TreeOrder)
					.Select(a => new StructureTreeViewModel
					{
						DepartmentName = a.UN_Departments.Name,
						id_departmentParent = (int)a.id_departmentParent,
						id_departmentTree = a.id_departmentTree,
						id_departmnet = a.id_department,
						TreeOrder = a.TreeOrder
					}).OrderBy(a => a.TreeOrder).ToList();
				return result;
			}
			else
			{
				var result = this._databaseContext.UN_DepartmentTree.Where(a => a.id_departmentParent == id_departmentParent && a.id_departmentTree != a.id_departmentParent)
					.OrderBy(a => a.TreeOrder)
					.Select(a => new StructureTreeViewModel
					{
						DepartmentName = a.UN_Departments.Name,
						id_departmentParent = (int)a.id_departmentParent,
						id_departmentTree = a.id_departmentTree,
						id_departmnet = a.id_department,
						TreeOrder = a.TreeOrder
					}).OrderBy(a => a.TreeOrder).ToList();
				return result;
			}
		}

		public List<StructurePositionViewModel> GetStructurePositions(int id_department, bool IsActiveOnly)
		{
			using (var data = new AmbulanceEntities())
			{
				var query = data.HR_StructurePositions.Where(a => a.id_department == id_department );

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
						StaffCount = a.StaffCount.ToString()
					}).ToList();
				return result2;
			}
		}

		public UN_DepartmentTree GetTreeNode(int id_department)
		{
			var result = this._databaseContext.UN_DepartmentTree.FirstOrDefault(a => a.id_department == id_department);
			return result;
		}

		#endregion
		public new void Save()
		{
			try
			{
				_databaseContext.SaveChanges();
			}
			catch(DbUpdateException ex)
			{
				if(ex.InnerException != null)
				{
					if(ex.InnerException.InnerException != null)
					{
						var exc = ex.InnerException.InnerException as SqlException;
						if(exc.Number == 2627)
						{
							ThrowZoraException(ErrorCodes.DuplicateName);
						}
					}
				}
				ThrowZoraException(ErrorCodes.NoDb);
			}
			catch(Exception ex)
			{
				ThrowZoraException(ErrorCodes.NoDb);
			}
		}

		
	}
}
