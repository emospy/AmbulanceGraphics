using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.DoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.MedicalCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой рез шофьори по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDriversW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ЛЕКАРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalDoctorCrewsW8, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW7, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "брой изв. ФЕЛШЕРСКИ екипи по дежурства";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, recModel.AdditionalMedicalCrewsW8, currentRow);

			int[] ShiftCrews719 = new int[31];
			int[] ShiftCrews820 = new int[31];
			int[] TotalCrews719 = new int[31];
			int[] TotalCrews820 = new int[31];
			for (int i = 0; i < 31; i ++)
			{
				ShiftCrews719[i] = recModel.DoctorCrewsW7[i] + recModel.MedicalCrewsW7[i];
				TotalCrews719[i] = ShiftCrews719[i] + recModel.AdditionalDoctorCrewsW7[i] + recModel.AdditionalMedicalCrewsW7[i];

				ShiftCrews820[i] = recModel.DoctorCrewsW8[i] + recModel.MedicalCrewsW8[i];
				TotalCrews820[i] = ShiftCrews820[i] + recModel.AdditionalDoctorCrewsW8[i] + recModel.AdditionalMedicalCrewsW8[i];
			}

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, ShiftCrews719, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи от смяната";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, ShiftCrews820, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи всичко";
			worksheet.Cells[currentRow, 5].Value = "7-19";
			this.PrintRecapitulationDayCount(worksheet, TotalCrews719, currentRow);

			currentRow++;
			worksheet.Cells[currentRow, 4].Value = "общ брой екипи всичко";
			worksheet.Cells[currentRow, 5].Value = "8-20";
			this.PrintRecapitulationDayCount(worksheet, TotalCrews820, currentRow);

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
			for (int i = 1; i < DateTime.DaysInMonth(month.Year, month.Month); i++)
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

		private int ExportCurrentDepartment(DateTime date, ScheduleTypes scheduleType, int currentRow, ExcelWorksheet worksheet,
			UN_Departments dep)
		{
			currentRow++;
			worksheet.Cells[currentRow, 3].Value = dep.Name;
			currentRow++;

			var recModel = new CrewRecapitulation();
			var lstCrewSchedules = this.GetDepartmentCrewsAndSchedules(dep.id_department, date, (int)scheduleType);
			//.OrderBy(c => c.CrewName).ThenBy(c => c.RowPosition);
			foreach (var crew in lstCrewSchedules)
			{
				if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
				{
					currentRow++;
				}

				this.PrintScheduleRow(worksheet, crew, currentRow, date);
				currentRow++;

				if (crew.LstCrewMembers != null && crew.LstCrewMembers.Count > 0)
				{
					if (crew.IsTemporary == true)
					{
						switch ((CrewTypes)crew.id_crewType)
						{
							case CrewTypes.Reanimation: case CrewTypes.Doctor:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW7, date);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalDoctorCrewsW8, date);
								}
								break;
							case CrewTypes.Paramedic:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalMedicalCrewsW7, date);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.AdditionalMedicalCrewsW8, date);
								}
								break;
							default:
								break;
						}
					}
					else
					{
						switch ((CrewTypes)crew.id_crewType)
						{
							case CrewTypes.Reanimation: case CrewTypes.Doctor:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW7, date);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.DoctorCrewsW8, date);
								}
								break;
							
							case CrewTypes.Paramedic:
								if (crew.WorkTime == "7-19")
								{
									this.InsertInRecapitulation(crew, recModel.MedicalCrewsW7, date);
								}
								else
								{
									this.InsertInRecapitulation(crew, recModel.MedicalCrewsW8, date);
								}
								break;
							default:
								break;
						}
					}
				}
				else
				{
					if (crew.id_positionType == (int)PositionTypes.Driver)
					{
						if (crew.WorkTime == "7-19")
						{
							this.InsertInRecapitulation(crew, recModel.AdditionalDriversW7, date);
						}
						else
						{
							this.InsertInRecapitulation(crew, recModel.AdditionalDriversW8, date);
						}
					}
				}
			}
			this.PrintRecapitulation(worksheet, recModel, currentRow, out currentRow);
			return currentRow;
		}

		public void ExportMonthlySchedule(string fileName, string header, DateTime date, ScheduleTypes scheduleType)
		{
			if (scheduleType != ScheduleTypes.FinalMonthSchedule && scheduleType != ScheduleTypes.ForecastMonthSchedule)
			{
				return;
			}

			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete();  // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}

			using (ExcelPackage package = new ExcelPackage(newFile))
			{
				// add a new worksheet to the empty workbook
				using (var logic = new SchedulesLogic())
				{
					var lstAllDepartments = this._databaseContext.UN_Departments.Where(d => d.IsActive).ToList();
					var lstDepartments = lstAllDepartments.Where(d => d.IsActive == true
																		&& d.NumberShifts > 1).ToList();

					foreach (var department in lstDepartments)
					{
						int currentRow = 2;
						ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
						//Add the headers
						this.PrintColumnHeaders(date, worksheet);
						List<UN_Departments> subDeps = lstAllDepartments.Where(d => d.id_departmentParent == department.id_department
																			&& d.NumberShifts < 2)
								.OrderBy(a => a.TreeOrder).ToList();
						foreach (var dep in subDeps)
						{
							currentRow = ExportCurrentDepartment(date, scheduleType, currentRow, worksheet, dep);
						}
						worksheet.Cells.AutoFitColumns(0);
						worksheet.PrinterSettings.PaperSize = ePaperSize.A3;
						worksheet.PrinterSettings.Orientation = eOrientation.Landscape;
						//worksheet.PrinterSettings.RepeatRows = new ExcelAddress(1,1,1,42);
					}
				}
				package.Save();
			}
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
				worksheet.Cells[currentRow, 6 + col + 1].Value = crewMember.CountDayShifts;
				worksheet.Cells[currentRow, 6 + col + 2].Value = crewMember.CountNightShifts;
				worksheet.Cells[currentRow, 6 + col + 3].Value = crewMember.Shifts;
				if (crewMember.Difference > 0)
				{
					worksheet.Cells[currentRow, 6 + col + 4].Value = crewMember.Difference;
				}
				else if (crewMember.Difference < 0)
				{
					worksheet.Cells[currentRow, 6 + col + 5].Value = crewMember.Difference;
				}
			}
		}

		public void ExportSingleDepartmentMonthlySchedule(string fileName, DateTime date,
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

			FileInfo newFile = new FileInfo(fileName);
			if (newFile.Exists)
			{
				newFile.Delete(); // ensures we create a new workbook
				newFile = new FileInfo(fileName);
			}

			using (ExcelPackage package = new ExcelPackage(newFile))
			{
				// add a new worksheet to the empty workbook
				int currentRow = 2;
				ExcelWorksheet worksheet = package.Workbook.Worksheets.Add(department.Name);
				//Add the headers
				this.PrintColumnHeaders(date, worksheet);

				this.ExportCurrentDepartment(date, scheduleType, currentRow, worksheet, department);

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

				package.Save();
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
			worksheet.Cells[1, 6 + col + 1].Value = "Д";
			worksheet.Cells[1, 6 + col + 2].Value = "Н";
			worksheet.Cells[1, 6 + col + 3].Value = "изр.ч.";
			worksheet.Cells[1, 6 + col + 4].Value = "Ч+";
			worksheet.Cells[1, 6 + col + 5].Value = "Ч-";
		}
	}
}
