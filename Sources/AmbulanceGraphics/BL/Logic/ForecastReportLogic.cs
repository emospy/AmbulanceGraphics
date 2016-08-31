using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;
using BL.DB;
using BL.Models;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace BL.Logic
{
	public class ForecastReportLogic : ExportLogic
	{
		private void PrintRecapitulation(ExcelWorksheet worksheet, CrewRecapitulation recModel, int crow, out int currentRow)
		{
			currentRow = crow;

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalMedicalCrewsW8, currentRow);

			////////////////////////

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи от други смени ";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи от други смени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи от други смени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExternalMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи от други смени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExternalMedicalCrewsW8, currentRow);

			//брой ЛЕКАРСКИ екипи смесени
			//брой ЛЕКАРСКИ екипи смесени
			//брой ФЕЛШЕРСКИ екипи смесени
			//брой ФЕЛШЕРСКИ екипи смесени

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи смесени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.MeshDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи смесени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.MeshDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи смесени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.MeshMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРСКИ екипи смесени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.MeshMedicalCrewsW8, currentRow);

			//брой рез шофьори от смяната
			//брой рез шофьори от смяната
			//брой рез шофьори от други смени
			//брой рез шофьори от други смени

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeDriversW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори от други смени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExternalFreeDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори от други смени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExternalFreeDriversW8, currentRow);

			//брой свободни лекари от смяната
			//брой свободни лекари от смяната
			//брой свободни лекари от други смени
			//брой свободни лекари от други смени

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни лекари от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeDoctorsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни лекари от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeDoctorsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни лекари от други смени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalFreeDoctorsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни лекари от други смени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalFreeDoctorsW8, currentRow);

			//брой свободни фелдшери и сестри от смяната
			//брой свободни фелдшери и сестри от смяната
			//брой свободни фелдшери и сестри от дриги смени
			//брой свободни фелдшери и сестри от други смени

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни фелдшери и сестри от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeMedicsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни фелдшери и сестри от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.InternalFreeMedicsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни фелдшери и сестри от други смени";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalFreeMedicsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободни фелдшери и сестри от други смени";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.ExtertnalFreeMedicsW8, currentRow);



			int[] ShiftCrews719 = new int[31];
			int[] ShiftCrews820 = new int[31];
			int[] TotalCrews719 = new int[31];
			int[] TotalCrews820 = new int[31];
			int[] TotalCrewsTotal = new int[31];
			for (int i = 0; i < 31; i++)
			{
				ShiftCrews719[i] = recModel.InternalDoctorCrewsW7[i] + recModel.InternalMedicalCrewsW7[i]
								   + recModel.ExtertnalDoctorCrewsW7[i] + recModel.ExternalMedicalCrewsW7[i]
								   + recModel.MeshDoctorCrewsW7[i] + recModel.MeshMedicalCrewsW7[i];

				TotalCrews719[i] = ShiftCrews719[i] + recModel.InternalFreeDoctorsW7[i] + recModel.InternalFreeMedicsW7[i]
									+ recModel.ExtertnalFreeDoctorsW7[i] + recModel.ExtertnalFreeMedicsW7[i];

				ShiftCrews820[i] = recModel.InternalDoctorCrewsW8[i] + recModel.InternalMedicalCrewsW8[i]
								   + recModel.ExtertnalDoctorCrewsW8[i] + recModel.ExternalMedicalCrewsW8[i]
								   + recModel.MeshDoctorCrewsW8[i] + recModel.MeshMedicalCrewsW8[i];

				TotalCrews820[i] = ShiftCrews820[i] + recModel.InternalFreeDoctorsW8[i] + recModel.InternalFreeMedicsW8[i]
									+ recModel.ExtertnalFreeDoctorsW8[i] + recModel.ExtertnalFreeMedicsW8[i];

				TotalCrewsTotal[i] = TotalCrews719[i] + TotalCrews820[i];
			}

			//общ брой екипи
			//общ брой екипи
			//общ брой екипи + свободни лекари и фелдшери и екипни сестри
			//общ брой екипи + свободни лекари и фелдшери и екипни сестри
			//общ брой екипи всичко В НАЛИЧНОСТ ЕКИПИ ПО БРОЯ НА ЛЕК.И ФЕЛДШ. ЗА ДЕЖУРСТВОТО

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, ShiftCrews719, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, ShiftCrews820, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи + свободни лекари и фелдшери и екипни сестри";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, TotalCrews719, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи + свободни лекари и фелдшери и екипни сестри";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, TotalCrews820, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи всичко В НАЛИЧНОСТ ЕКИПИ ПО БРОЯ НА ЛЕК.И ФЕЛДШ. ЗА ДЕЖУРСТВОТО";
			worksheet.Cells[currentRow, 5].Value = "всички";
			this.PrintRecapitulationDayCount(worksheet, TotalCrewsTotal, currentRow);

			currentRow++;
		}

		private void PrintBranchRecapitulation(ExcelWorksheet worksheet, List<CrewRecapitulation> lstRecModels, List<UN_Departments> lstDeps, int currentRow, DateTime currentDate)
		{
			int[] NightDoctors = new int[31];
			int[] NightMedics = new int[31];
			int[] NightDrivers = new int[31];
			int[] NightSupport = new int[31];
			int[] DayDoctors = new int[31];
			int[] DayMedics = new int[31];
			int[] DayDrivers = new int[31];
			int[] DaySupport = new int[31];

			for (int i = 0; i < DateTime.DaysInMonth(currentDate.Year, currentDate.Month); i++)
			{
				for (int j = 0; j < lstDeps.Count; j++)
				{
					bool? IsDayShift = this.GetShiftTypeByDate(this.startDate, this.numberShifts,
						new DateTime(currentDate.Year, currentDate.Month, i + 1), this.startShift, j);
					if (IsDayShift == null)
					{
						continue;
					}
					var recModel = lstRecModels[j];

					int docs = recModel.InternalDoctorCrewsW7[i] + recModel.InternalFreeDoctorsW7[i] +
							   recModel.InternalDoctorCrewsW8[i] +
							   recModel.InternalFreeDoctorsW8[i]
							   + recModel.ExtertnalFreeDoctorsW7[i] + recModel.ExtertnalFreeDoctorsW8[i] +
							   recModel.ExtertnalDoctorCrewsW7[i] +
							   recModel.ExtertnalDoctorCrewsW8[i]
							   + recModel.MeshDoctorCrewsW7[i] + recModel.MeshDoctorCrewsW8[i];

					var meds = recModel.InternalMedicalCrewsW7[i] + recModel.InternalFreeMedicsW7[i] +
								recModel.InternalMedicalCrewsW8[i] +
								recModel.InternalFreeMedicsW8[i]
								+ recModel.ExtertnalFreeMedicsW7[i] + recModel.ExtertnalFreeMedicsW8[i] +
								recModel.ExternalMedicalCrewsW7[i] +
								recModel.ExternalMedicalCrewsW8[i]
								+ recModel.MeshMedicalCrewsW7[i] + recModel.MeshMedicalCrewsW8[i];

					var drvs = recModel.InternalDoctorCrewsW7[i] + recModel.InternalDoctorCrewsW8[i]
								 + recModel.ExtertnalDoctorCrewsW7[i] + recModel.ExtertnalDoctorCrewsW8[i]
								 + recModel.MeshDoctorCrewsW7[i] + recModel.MeshDoctorCrewsW8[i]
								 + recModel.InternalMedicalCrewsW7[i] + recModel.InternalMedicalCrewsW8[i]
								 + recModel.ExternalMedicalCrewsW7[i] + recModel.ExternalMedicalCrewsW8[i]
								 + recModel.MeshMedicalCrewsW7[i] + recModel.MeshMedicalCrewsW8[i]
								 + recModel.InternalFreeDriversW7[i] + recModel.InternalFreeDriversW8[i]
								 + recModel.ExternalFreeDriversW7[i] + recModel.ExternalFreeDriversW8[i];

					var sups = recModel.FreeSupportPersonnel[i];

					if (IsDayShift == true)
					{
						DayDoctors[i] = docs;
						DayMedics[i] = meds;
						DayDrivers[i] = drvs;
						DaySupport[i] = sups;
					}
					else
					{
						NightDoctors[i] = docs;
						NightMedics[i] = meds;
						NightDrivers[i] = drvs;
						NightSupport[i] = sups;
					}
				}
			}

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРИ";
			worksheet.Cells[currentRow, 5].Value = "дневна";
			this.PrintRecapitulationDayCount(worksheet, DayDoctors, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРИ";
			worksheet.Cells[currentRow, 5].Value = "нощна";
			this.PrintRecapitulationDayCount(worksheet, DayDoctors, currentRow);


			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРИ и ЕКИПНИ СЕСТРИ";
			worksheet.Cells[currentRow, 5].Value = "дневна";
			this.PrintRecapitulationDayCount(worksheet, DayMedics, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛДШЕРИ и ЕКИПНИ СЕСТРИ";
			worksheet.Cells[currentRow, 5].Value = "нощна";
			this.PrintRecapitulationDayCount(worksheet, NightMedics, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой шофьори";
			worksheet.Cells[currentRow, 5].Value = "дневна";
			this.PrintRecapitulationDayCount(worksheet, DayDrivers, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой шофьори";
			worksheet.Cells[currentRow, 5].Value = "нощна";
			this.PrintRecapitulationDayCount(worksheet, NightDrivers, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободен помощен персонал";
			worksheet.Cells[currentRow, 5].Value = "дневна";
			this.PrintRecapitulationDayCount(worksheet, DaySupport, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой свободен помощен персонал";
			worksheet.Cells[currentRow, 5].Value = "нощна";
			this.PrintRecapitulationDayCount(worksheet, NightSupport, currentRow);

			currentRow++;
		}

		private void PrintDispatchRecapitulation(ExcelWorksheet worksheet, List<CrewRecapitulation> lstRecModels, List<UN_Departments> lstDeps, int currentRow, DateTime currentDate)
		{
			int[] NightDoctors = new int[31];
			int[] NightMedics = new int[31];
			int[] NightDrivers = new int[31];
			int[] NightSupport = new int[31];
			int[] DayDoctors = new int[31];
			int[] DayMedics = new int[31];
			int[] DayDrivers = new int[31];
			int[] DaySupport = new int[31];

			for (int i = 0; i < DateTime.DaysInMonth(currentDate.Year, currentDate.Month); i++)
			{
				for (int j = 0; j < lstDeps.Count; j++)
				{
					bool? IsDayShift = this.GetShiftTypeByDate(this.startDate, this.numberShifts,
						new DateTime(currentDate.Year, currentDate.Month, i + 1), this.startShift, j);
					if (IsDayShift == null)
					{
						continue;
					}
					var recModel = lstRecModels[j];

					int docs = recModel.InternalDoctorCrewsW7[i] + recModel.InternalFreeDoctorsW7[i] +
							   recModel.InternalDoctorCrewsW8[i] +
							   recModel.InternalFreeDoctorsW8[i]
							   + recModel.ExtertnalFreeDoctorsW7[i] + recModel.ExtertnalFreeDoctorsW8[i] +
							   recModel.ExtertnalDoctorCrewsW7[i] +
							   recModel.ExtertnalDoctorCrewsW8[i]
							   + recModel.MeshDoctorCrewsW7[i] + recModel.MeshDoctorCrewsW8[i];

					var meds = recModel.InternalMedicalCrewsW7[i] + recModel.InternalFreeMedicsW7[i] +
								recModel.InternalMedicalCrewsW8[i] +
								recModel.InternalFreeMedicsW8[i]
								+ recModel.ExtertnalFreeMedicsW7[i] + recModel.ExtertnalFreeMedicsW8[i] +
								recModel.ExternalMedicalCrewsW7[i] +
								recModel.ExternalMedicalCrewsW8[i]
								+ recModel.MeshMedicalCrewsW7[i] + recModel.MeshMedicalCrewsW8[i];

					var drvs = recModel.InternalDoctorCrewsW7[i] + recModel.InternalDoctorCrewsW8[i]
								 + recModel.ExtertnalDoctorCrewsW7[i] + recModel.ExtertnalDoctorCrewsW8[i]
								 + recModel.MeshDoctorCrewsW7[i] + recModel.MeshDoctorCrewsW8[i]
								 + recModel.InternalMedicalCrewsW7[i] + recModel.InternalMedicalCrewsW8[i]
								 + recModel.ExternalMedicalCrewsW7[i] + recModel.ExternalMedicalCrewsW8[i]
								 + recModel.MeshMedicalCrewsW7[i] + recModel.MeshMedicalCrewsW8[i]
								 + recModel.InternalFreeDriversW7[i] + recModel.InternalFreeDriversW8[i]
								 + recModel.ExternalFreeDriversW7[i] + recModel.ExternalFreeDriversW8[i];

					var sups = recModel.FreeSupportPersonnel[i];

					if (IsDayShift == true)
					{
						DaySupport[i] = sups + drvs + docs + meds;
					}
					else
					{
						NightSupport[i] = sups + drvs + docs + meds;
					}
				}
			}

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой оператори";
			worksheet.Cells[currentRow, 5].Value = "дневна";
			this.PrintRecapitulationDayCount(worksheet, DaySupport, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой оператори";
			worksheet.Cells[currentRow, 5].Value = "нощна";
			this.PrintRecapitulationDayCount(worksheet, NightSupport, currentRow);

			currentRow++;
		}
		private void PrintRecapitulationDayCount(ExcelWorksheet worksheet, int[] recRow, int currentRow)
		{
			for (int col = 0; col < 31; col++)
			{
				worksheet.Cells[currentRow, col + 7].Value = recRow[col];
			}
		}

		private void InsertInRecapitulation(CrewScheduleListViewModel crew, int[] rec, DateTime month)
		{
			for (int i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
			{
				var date = new DateTime(month.Year, month.Month, i);
				if (crew[i] != 0)
				{
					if (IsCrewFull(date, crew, true) || IsCrewFull(date, crew, false))
					{
						rec[i - 1]++;
					}
				}
			}
		}

		private void InsertInDriverRecapitulation(CrewScheduleListViewModel crew, int[] rec, DateTime month)
		{
			for (int i = 1; i <= DateTime.DaysInMonth(month.Year, month.Month); i++)
			{
				var date = new DateTime(month.Year, month.Month, i);
				if (crew[i] == (int)PresenceTypes.DayShift
					|| crew[i] == (int)PresenceTypes.NightShift
					|| crew[i] == (int)PresenceTypes.RegularShift)
				{
					rec[i - 1]++;
				}
			}
		}

		private bool? CheckCrewAssembly(CrewScheduleListViewModel crew, int id_department)
		{
			int id_dep;
			id_dep = crew.id_department;
			bool IsFromShift = false;
			if (id_dep != id_department)
			{
				for (int i = 0; i < crew.LstCrewMembers.Count; i++)
				{
					if (crew.LstCrewMembers[i].id_department == id_department)
					{
						return null;
					}
				}
				return false;
			}
			else
			{
				for (int i = 0; i < crew.LstCrewMembers.Count; i++)
				{
					if (crew.LstCrewMembers[i].id_person != 0)
					{
						if (crew.LstCrewMembers[i].id_department != id_department)
						{
							return null;
						}
					}
				}
				return true;
			}
		}

		private int ExportCurrentDepartment(DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, ScheduleTypes scheduleType, ref int currentRow, ExcelWorksheet worksheet,
			UN_Departments dep, out CrewRecapitulation recModel)
		{
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = dep.Name;
			currentRow++;

			recModel = new CrewRecapitulation();
			var lstCrewSchedules = this.GetDepartmentCrewsAndSchedules(dep.id_department, dateBegin, dateCurrent, dateEnd, (int)scheduleType);
			//.OrderBy(c => c.CrewName).ThenBy(c => c.RowPosition);
			foreach (var crew in lstCrewSchedules)
			{
				if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
				{
					currentRow++;
				}

				this.PrintScheduleRow(worksheet, crew, currentRow, dateCurrent);
				currentRow++;

				InsertInRecapitulation(dateCurrent, dep, crew, recModel);
			}

			return currentRow;
		}

		private void InsertInRecapitulation(DateTime dateCurrent, UN_Departments dep, CrewScheduleListViewModel crew,
			CrewRecapitulation recModel)
		{
			bool wt7 = false;
			bool? IsFromShift = false;
			if (crew.WorkTime == "7-19")
			{
				wt7 = true;
			}
			else
			{
				wt7 = false;
			}

			if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
			{
				IsFromShift = this.CheckCrewAssembly(crew, dep.id_department);

				switch ((CrewTypes)crew.id_crewType)
				{
					case CrewTypes.Reanimation:
					case CrewTypes.Doctor:
						if (wt7 == true)
						{
							if (IsFromShift == true)
							{
								this.InsertInRecapitulation(crew, recModel.InternalDoctorCrewsW7, dateCurrent); //1
							}
							else if (IsFromShift == false)
							{
								this.InsertInRecapitulation(crew, recModel.ExtertnalDoctorCrewsW7, dateCurrent); //
							}
							else
							{
								this.InsertInRecapitulation(crew, recModel.MeshDoctorCrewsW7, dateCurrent);
							}
						}
						else
						{
							if (IsFromShift == true)
							{
								this.InsertInRecapitulation(crew, recModel.InternalDoctorCrewsW8, dateCurrent);
							}
							else if (IsFromShift == false)
							{
								this.InsertInRecapitulation(crew, recModel.ExtertnalDoctorCrewsW8, dateCurrent);
							}
							else
							{
								this.InsertInRecapitulation(crew, recModel.MeshDoctorCrewsW8, dateCurrent);
							}
						}
						break;
					case CrewTypes.Paramedic:
						if (wt7)
						{
							if (IsFromShift == true)
							{
								this.InsertInRecapitulation(crew, recModel.InternalMedicalCrewsW7, dateCurrent);
							}
							else if (IsFromShift == false)
							{
								this.InsertInRecapitulation(crew, recModel.ExternalMedicalCrewsW7, dateCurrent);
							}
							else
							{
								this.InsertInRecapitulation(crew, recModel.MeshMedicalCrewsW7, dateCurrent);
							}
						}
						else
						{
							if (IsFromShift == true)
							{
								this.InsertInRecapitulation(crew, recModel.InternalMedicalCrewsW8, dateCurrent);
							}
							else if (IsFromShift == false)
							{
								this.InsertInRecapitulation(crew, recModel.ExternalMedicalCrewsW8, dateCurrent);
							}
							else
							{
								this.InsertInRecapitulation(crew, recModel.MeshMedicalCrewsW8, dateCurrent);
							}
						}
						break;

				}
			}
			else if (crew.id_crew == 0 || crew.IsIncomplete == true)
			{
				if (dep.id_department != crew.id_department)
				{
					IsFromShift = false;
				}
				else
				{
					IsFromShift = true;
				}
				if (crew.id_positionType == (int)PositionTypes.Driver)
				{
					if (wt7)
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeDriversW7, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExternalFreeDriversW7, dateCurrent);
						}
					}
					else
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeDriversW8, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExternalFreeDriversW8, dateCurrent);
						}
					}
				}
				else if (crew.id_positionType == (int)PositionTypes.Doctor)
				{
					if (wt7)
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeDoctorsW7, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExtertnalFreeDoctorsW7, dateCurrent);
						}
					}
					else
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeDoctorsW8, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExtertnalFreeDoctorsW8, dateCurrent);
						}
					}
				}
				else if (crew.id_positionType == (int)PositionTypes.MedicalStaff)
				{
					if (wt7)
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeMedicsW7, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExtertnalFreeMedicsW7, dateCurrent);
						}
					}
					else
					{
						if (IsFromShift == true)
						{
							this.InsertInDriverRecapitulation(crew, recModel.InternalFreeMedicsW8, dateCurrent);
						}
						else
						{
							this.InsertInDriverRecapitulation(crew, recModel.ExtertnalFreeMedicsW8, dateCurrent);
						}
					}
				}
				else
				{
					this.InsertInDriverRecapitulation(crew, recModel.FreeSupportPersonnel, dateCurrent);
				}
			}
		}

		public void ExportMonthlySchedule(string fileName, string header, DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, ScheduleTypes scheduleType)
		{
			//if (scheduleType != ScheduleTypes.FinalMonthSchedule && scheduleType != ScheduleTypes.ForecastMonthSchedule)
			//{
			//	return;
			//}

			//FileInfo newFile = new FileInfo(fileName);
			//if (newFile.Exists)
			//{
			//	newFile.Delete();  // ensures we create a new workbook
			//	newFile = new FileInfo(fileName);
			//}

			//using (ExcelPackage package = new ExcelPackage(newFile))
			//{
			//	// add a new worksheet to the empty workbook
			//	using (var logic = new SchedulesLogic())
			//	{
			//		var lstAllDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive).ToList();
			//		var lstDepartments = lstAllDepartments.Where(d => d.IsActive == true
			//															&& d.NumberShifts > 1).ToList();

			//		foreach (var department in lstDepartments)
			//		{
			//			int currentRow = 2;
			//			ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
			//			//Add the headers
			//			this.PrintColumnHeaders(dateCurrent, worksheet);
			//			List<UN_Departments> subDeps = lstAllDepartments.Where(d => d.id_departmentParent == department.id_department
			//																&& d.NumberShifts < 2)
			//					.OrderBy(a => a.TreeOrder).ToList();

			//			var lstRecModels = new List<CrewRecapitulation>();

			//			foreach (var dep in subDeps)
			//			{
			//				var recModel = new CrewRecapitulation();
			//				currentRow = ExportCurrentDepartment(dateBegin, dateCurrent, dateEnd, scheduleType, ref currentRow, worksheet, department, out recModel);
			//				lstRecModels.Add(recModel);
			//			}
			//		}
			//	}
			//	package.Save();
			//}
		}

		private void PrintScheduleRow(ExcelWorksheet worksheet, CrewScheduleListViewModel crewMember, int currentRow, DateTime date)
		{
			worksheet.Cells[currentRow, 1].Value = crewMember.CrewName;
			worksheet.Cells[currentRow, 2].Value = crewMember.RegNumber;
			worksheet.Cells[currentRow, 3].Value = crewMember.BaseDepartment;
			worksheet.Cells[currentRow, 4].Value = crewMember.Name;
			worksheet.Cells[currentRow, 5].Value = crewMember.WorkTime;
			worksheet.Cells[currentRow, 6].Value = crewMember.Position;
			if (crewMember.PF == null)
			{
				return;
			}
			int col;

			for (col = crewMember.DateStart.Day; col <= crewMember.DateEnd.Day; col++)
			{
				var shift = crewMember.lstShiftTypes.FirstOrDefault(a => a.id_shiftType == crewMember[col]);
				if (shift != null)
				{
					worksheet.Cells[currentRow, col + 6].Value = shift.Name;
				}
			}

			col = DateTime.DaysInMonth(date.Year, date.Month) + 1;

			if (crewMember.IsTemporary == false)
			{
				//Д	Н	изр.ч.	Ч.+	Ч.-
				worksheet.Cells[currentRow, 6 + col].Value = crewMember.CountDayShifts;
				worksheet.Cells[currentRow, 6 + col + 1].Value = crewMember.CountNightShifts;
				worksheet.Cells[currentRow, 6 + col + 2].Value = crewMember.CountRegularShifts;
				worksheet.Cells[currentRow, 6 + col + 3].Value = crewMember.Shifts;
				if (crewMember.Difference > 0)
				{
					worksheet.Cells[currentRow, 6 + col + 4].Value = crewMember.Difference;
				}
				else if (crewMember.Difference < 0)
				{
					worksheet.Cells[currentRow, 6 + col + 5].Value = crewMember.Difference;
				}

				worksheet.Cells[currentRow, 6 + col + 6].Value = crewMember.Norm;
				worksheet.Cells[currentRow, 6 + col + 7].Value = crewMember.Shifts;
				worksheet.Cells[currentRow, 6 + col + 8].Value = crewMember.WorkTimeAbsences;
				worksheet.Cells[currentRow, 6 + col + 9].Value = crewMember.TotalWorkedOut;
				worksheet.Cells[currentRow, 6 + col + 10].Value = crewMember.Difference;
				worksheet.Cells[currentRow, 6 + col + 11].Value = crewMember.Month1Difference;
				worksheet.Cells[currentRow, 6 + col + 12].Value = crewMember.Month2Difference;
				worksheet.Cells[currentRow, 6 + col + 13].Value = crewMember.Month3Difference;
				worksheet.Cells[currentRow, 6 + col + 14].Value = crewMember.Month4Difference;
				worksheet.Cells[currentRow, 6 + col + 15].Value = crewMember.Month5Difference;
				worksheet.Cells[currentRow, 6 + col + 16].Value = crewMember.Month6Difference;
				worksheet.Cells[currentRow, 6 + col + 17].Value = crewMember.PeriodTotalDifference;
				worksheet.Cells[currentRow, 6 + col + 18].Value = crewMember.Month1OverTime;
				worksheet.Cells[currentRow, 6 + col + 19].Value = crewMember.Month2OverTime;
				worksheet.Cells[currentRow, 6 + col + 20].Value = crewMember.Month3OverTime;
				worksheet.Cells[currentRow, 6 + col + 21].Value = crewMember.Month4OverTime;
				worksheet.Cells[currentRow, 6 + col + 22].Value = crewMember.Month5OverTime;
				worksheet.Cells[currentRow, 6 + col + 23].Value = crewMember.Month6OverTime;
				worksheet.Cells[currentRow, 6 + col + 24].Value = crewMember.PeriodTotalOverTime;
			}
		}

		public void ExportSingleForecastMonthlyShedule(string fileName, DateTime dateBegin, DateTime dateCurrent,
			DateTime dateEnd,
			ScheduleTypes scheduleType, int id_department)
		{
			var department = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);
			if (department == null)
			{
				return;
			}

			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}

			using (ExcelPackage package = new ExcelPackage(newFile))
			{

				if (department.UN_Departments2.id_departmentType == (int)DepartmentTypes.Branch)
				{
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.UN_Departments2.Name);
					ExportMultyDepartmentMonthlySchedule(worksheet, dateBegin, dateCurrent, dateEnd, scheduleType, id_department, true);

					worksheet.Cells.AutoFitColumns(0);
					worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
					worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
					worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");
					worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
				}
				else if (department.UN_Departments2.id_departmentType == (int)DepartmentTypes.Central)
				{
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
					ExportSingleDepartmentMonthlySchedule(worksheet, dateBegin, dateCurrent, dateEnd, scheduleType, id_department);

					worksheet.Cells.AutoFitColumns(0);
					worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
					worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
					worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");
					worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
				}
				else if (department.UN_Departments2.id_departmentType == (int)DepartmentTypes.Dispatchers
					|| department.UN_Departments2.id_departmentType == (int)DepartmentTypes.Support)
				{
					ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.UN_Departments2.Name);
					ExportMultyDepartmentMonthlySchedule(worksheet, dateBegin, dateCurrent, dateEnd, scheduleType, id_department, false);

					worksheet.Cells.AutoFitColumns(0);
					worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
					worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
					worksheet.PrinterSettings.RepeatRows = new ExcelAddress("$1:$1");
					worksheet.Cells.Style.Border.Top.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Top.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Bottom.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Left.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Left.Color.SetColor(System.Drawing.Color.Black);
					worksheet.Cells.Style.Border.Right.Style = ExcelBorderStyle.Thin;
					worksheet.Cells.Style.Border.Right.Color.SetColor(System.Drawing.Color.Black);
				}
				if (package.Workbook.Worksheets.Count > 0)
				{
					package.Save();
				}
			}
		}
		private void ExportSingleDepartmentMonthlySchedule(ExcelWorksheet worksheet, DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd,
			ScheduleTypes scheduleType, int id_department)
		{
			if (scheduleType != ScheduleTypes.FinalMonthSchedule && scheduleType != ScheduleTypes.ForecastMonthSchedule)
			{
				return;
			}

			var department = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);
			if (department == null)
			{
				return;
			}

			// add a new worksheet to the empty workbook
			int currentRow = 2;
			//Add the headers
			this.PrintColumnHeaders(dateCurrent, worksheet);
			var recModel = new CrewRecapitulation();

			this.ExportCurrentDepartment(dateBegin, dateCurrent, dateEnd, scheduleType, ref currentRow, worksheet, department, out recModel);
			this.PrintRecapitulation(worksheet, recModel, currentRow, out currentRow);

		}

		public void ExportMultyDepartmentMonthlySchedule(ExcelWorksheet worksheet, DateTime dateBegin, DateTime dateCurrent, DateTime dateEnd, ScheduleTypes scheduleType, int id_department, bool IsBranch)
		{
			if (scheduleType != ScheduleTypes.ForecastMonthSchedule)
			{
				return;
			}

			var department = this._databaseContext.UN_Departments.FirstOrDefault(d => d.id_department == id_department);
			if (department == null)
			{
				return;
			}

			// add a new worksheet to the empty workbook
			int currentRow = 2;
			//Add the headers
			this.PrintColumnHeaders(dateCurrent, worksheet);


			var lstDeps =
				this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == department.id_departmentParent && a.id_departmentParent != a.id_department)
				.OrderBy(a => a.TreeOrder).ToList();

			var lstRecModels = new List<CrewRecapitulation>();

			foreach (var dep in lstDeps)
			{
				var recModel = new CrewRecapitulation();

				//currentRow++;
				//worksheet.Cells[currentRow, 4].Value = dep.Name;
				//currentRow++;
				this.ExportCurrentDepartment(dateBegin, dateCurrent, dateEnd, scheduleType, ref currentRow, worksheet, dep, out recModel);
				lstRecModels.Add(recModel);
			}
			if (IsBranch)
			{
				this.PrintBranchRecapitulation(worksheet, lstRecModels, lstDeps, currentRow, dateCurrent);
			}
			else
			{
				this.PrintDispatchRecapitulation(worksheet, lstRecModels, lstDeps, currentRow, dateCurrent);
			}
		}

		private void PrintColumnHeaders(DateTime date, ExcelWorksheet worksheet)
		{
			worksheet.Cells[1, 1].Value = "№";
			worksheet.Cells[1, 2].Value = "Линейка";
			worksheet.Cells[1, 3].Value = "Data";
			worksheet.Cells[1, 4].Value = "Име";
			worksheet.Cells[1, 5].Value = "Рв";
			worksheet.Cells[1, 6].Value = "Длъжност";
			int col;
			for (col = 1; col <= DateTime.DaysInMonth(date.Year, date.Month); col++)
			{
				worksheet.Cells[1, col + 6].Value = col.ToString();
			}
			//Д	Н	изр.ч.	Ч.+	Ч.-
			worksheet.Cells[1, 6 + col + 0].Value = "Д";
			worksheet.Cells[1, 6 + col + 1].Value = "Н";
			worksheet.Cells[1, 6 + col + 2].Value = "р";
			worksheet.Cells[1, 6 + col + 3].Value = "изр.ч.";
			worksheet.Cells[1, 6 + col + 4].Value = "Ч+";
			worksheet.Cells[1, 6 + col + 5].Value = "Ч-";

			worksheet.Cells[1, 6 + col + 6].Value = "Норматив";
			worksheet.Cells[1, 6 + col + 7].Value = "Смени";
			worksheet.Cells[1, 6 + col + 8].Value = "Извънредни";
			worksheet.Cells[1, 6 + col + 9].Value = "Отработени";
			worksheet.Cells[1, 6 + col + 10].Value = "Разлика";
			worksheet.Cells[1, 6 + col + 11].Value = "Р-ка M1";
			worksheet.Cells[1, 6 + col + 12].Value = "Р-ка M2";
			worksheet.Cells[1, 6 + col + 13].Value = "Р-ка M3";
			worksheet.Cells[1, 6 + col + 14].Value = "Р-ка M4";
			worksheet.Cells[1, 6 + col + 15].Value = "Р-ка M5";
			worksheet.Cells[1, 6 + col + 16].Value = "Р-ка M6";
			worksheet.Cells[1, 6 + col + 17].Value = "Общо разлика";
			worksheet.Cells[1, 6 + col + 18].Value = "Извънредни M1";
			worksheet.Cells[1, 6 + col + 19].Value = "Извънредни M2";
			worksheet.Cells[1, 6 + col + 20].Value = "Извънредни M3";
			worksheet.Cells[1, 6 + col + 21].Value = "Извънредни M4";
			worksheet.Cells[1, 6 + col + 22].Value = "Извънредни M5";
			worksheet.Cells[1, 6 + col + 23].Value = "Извънредни M6";
			worksheet.Cells[1, 6 + col + 24].Value = "Общо извънредни";
		}
	}
}
