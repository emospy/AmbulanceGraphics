using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BL.DB;
using BL.Models;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using OfficeOpenXml.Style;

namespace BL.Logic
{
    public class TwelveExportLogic : ExportLogic
    {
        public void GenerateTwelveReport(string fileName, DateTime dateFrom, DateTime dateTo)
        {
            FileInfo templateFile;
            
            templateFile = new FileInfo("Twelve.xlsx");
                    
            
            FileInfo newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete(); // ensures we create a new workbook
                newFile = new FileInfo(fileName);
            }
            this.package = new ExcelPackage(newFile, templateFile);

            var baseDepartments = this._databaseContext.UN_Departments.Where(a => a.NumberShifts > 0).ToList();

            int CurrentRow = 4;

            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];

            int TotalDuties = 0;
            int TotalDutyEmployees = 0;


            foreach (var bdep in baseDepartments)
            {
                worksheet.Cells[CurrentRow, 1].Value = bdep.Name;
                worksheet.Cells[CurrentRow,1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                worksheet.Cells[CurrentRow, 1].Style.Font.Size = 18;
                worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;

                var subDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent != a.id_department
                                                                              &&
                                                                              a.id_departmentParent ==
                                                                              bdep.id_department)
                    .ToList();
                CurrentRow++;

                foreach (var sdep in subDeps)
                {
                    worksheet.Cells[CurrentRow, 1].Value = sdep.Name;
                    worksheet.Cells[CurrentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
                    worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;
                    CurrentRow++;
                    this.ExportDepartmentTwelve(worksheet, dateFrom, dateTo, sdep, ref CurrentRow, ref TotalDuties, ref TotalDutyEmployees);
                }

                //CurrentRow++;
            }

            CurrentRow++;
            worksheet.Cells[CurrentRow, 1].Value = string.Format("{0}.{1} - {2}.{3}", dateFrom.Month,dateFrom.Year, dateTo.Month,dateTo.Year);
            worksheet.Cells[CurrentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 1].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;
            CurrentRow++;
            worksheet.Cells[CurrentRow, 1].Value = string.Format("Общ брой 12 ч.смени");
            worksheet.Cells[CurrentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 1].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[CurrentRow, 2].Value = TotalDuties;
            worksheet.Cells[CurrentRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 2].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 2].Style.Font.Bold = true;
            CurrentRow++;
            worksheet.Cells[CurrentRow, 1].Value = string.Format("Общ брой служители с 12 ч.смени");
            worksheet.Cells[CurrentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 1].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[CurrentRow, 2].Value = TotalDutyEmployees;
            worksheet.Cells[CurrentRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 2].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 2].Style.Font.Bold = true;
            CurrentRow++;
            worksheet.Cells[CurrentRow, 1].Value = string.Format("СРЕДНО АРИТМЕТИЧНО");
            worksheet.Cells[CurrentRow, 1].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 1].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 1].Style.Font.Bold = true;
            worksheet.Cells[CurrentRow, 2].Value = (float)((float)TotalDuties / (float)TotalDutyEmployees);
            worksheet.Cells[CurrentRow, 2].Style.Border.BorderAround(ExcelBorderStyle.Thick);
            worksheet.Cells[CurrentRow, 2].Style.Font.Size = 18;
            worksheet.Cells[CurrentRow, 2].Style.Font.Bold = true;

            this.package.Save();
        }

        public void ExportDepartmentTwelve(ExcelWorksheet worksheet, DateTime dateFrom, DateTime dateTo,
            UN_Departments dep, ref int CurrentRow, ref int TotalDuties, ref int TotalDutyEmployees)
        {
            List<PFRow> lstEmployees = new List<PFRow>();

            DateTime startMonth = new DateTime(dateTo.Year, dateTo.Month, 1);
            DateTime endMonth = new DateTime(dateTo.Year, dateTo.Month, DateTime.DaysInMonth(dateTo.Year, dateTo.Month));
            var lstAssignments =
                this._databaseContext.HR_Assignments.Where(
                    a => a.HR_StructurePositions.id_department == dep.id_department
                         && startMonth <= a.ValidTo && a.AssignmentDate < endMonth).ToList();

            for (DateTime CurrentDate = dateTo; CurrentDate >= dateFrom; CurrentDate = CurrentDate.AddMonths(-1))
            {
                CalendarRow cRow;
                CalendarRow cRowNH;
                CalendarRow cRowNH1;
                using (NomenclaturesLogic logic = new NomenclaturesLogic())
                {
                    cRow = logic.FillCalendarRow(CurrentDate);
                    cRowNH = logic.FillCalendarRowNH(CurrentDate);
                    cRowNH1 = logic.FillCalendarRowNH(CurrentDate.AddMonths(1));
                }

                foreach (var ass in lstAssignments)
                {
                    var DS = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract
                                                                                        && a.Date.Month == CurrentDate.Month
                                                                                        && a.Date.Year == CurrentDate.Year
                                                                                        && a.id_scheduleType == (int) ScheduleTypes.DailySchedule);

                    if (DS == null)
                    {
                        continue;
                    }
                    //Gode[ - svoge Людмил Стойнев Спасов м 20.09

                    //Евелина Александрова 29.08

                    //PFRow PresenceFrom = new PFRow();
                    PFRow Result = new PFRow();
                    //PresenceFrom.PF = PF;
                    Result.PF = DS;

                    Result.id_person = DS.HR_Contracts.id_person;
                    Result.Name = DS.HR_Contracts.UN_Persons.Name;

                    Result.RealDate = CurrentDate;
                    if (ass.HR_WorkTime == null)
                    {
                        continue;
                    }
                    Result.WorkHours = ass.HR_WorkTime.WorkHours;
                    Result.lstShiftTypes = this._databaseContext.GR_ShiftTypes.ToList();
                    Result.cRow = cRow;

                    var workDaysNorm = cRow.WorkDays;
                    var fad = ass.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false).AssignmentDate;
                    if (fad.Year == CurrentDate.Year && fad.Month == CurrentDate.Month 
                        && ass.HR_Contracts.IsFired == false)
                    {
                        // assigned in the current month
                        workDaysNorm = this.CalculateWorkDays(fad,
                            new DateTime(CurrentDate.Year, CurrentDate.Month,
                                DateTime.DaysInMonth(CurrentDate.Year, CurrentDate.Month)), this.lstCalendarRows);
                    }
                    else if ((fad.Year != CurrentDate.Year || fad.Month != CurrentDate.Month) && ass.HR_Contracts.IsFired &&
                             ass.HR_Contracts.DateFired.Value.Year == CurrentDate.Year &&
                             ass.HR_Contracts.DateFired.Value.Month == CurrentDate.Month)
                    {
                        //fired in the current month
                        workDaysNorm = this.CalculateWorkDays(new DateTime(CurrentDate.Year, CurrentDate.Month, 1),
                            ass.HR_Contracts.DateFired.Value, this.lstCalendarRows);
                    }
                    else if (fad.Year == CurrentDate.Year && fad.Month == CurrentDate.Month &&
                             ass.HR_Contracts.IsFired && ass.HR_Contracts.DateFired.Value.Year == CurrentDate.Year &&
                             ass.HR_Contracts.DateFired.Value.Month == CurrentDate.Month)
                    {
                        //assigned and fired in the same month
                        workDaysNorm = this.CalculateWorkDays(fad, ass.HR_Contracts.DateFired.Value,
                            this.lstCalendarRows);
                    }

                    Result.Norm = workDaysNorm * ass.HR_WorkTime.WorkHours;
                    if (ass.GR_WorkHours != null)
                    {
                        Result.IsSumWorkTime = ass.GR_WorkHours.IsSumWorkTime;
                    }

                    Result.CalculateHours();
                    lstEmployees.Add(Result);
                }
            }

            var EmployeeGroups = lstEmployees.GroupBy(a => a.id_person);

            foreach (var Employee in EmployeeGroups)
            {
                List<PFRow> emplList;
                emplList = Employee.ToList();

                worksheet.Cells[CurrentRow, 1].Value = Employee.First().Name;
                worksheet.Cells[CurrentRow, 2].Value = Employee.First().PF
                    .HR_Contracts.HR_Assignments.FirstOrDefault(a=>a.IsActive == true)?.HR_StructurePositions.HR_GlobalPositions.Name;

                int days = 0, nights = 0;
                for (int i = 0; i < emplList.Count; i ++)
                {
                    days += emplList[i].CountDayShifts;
                    nights += emplList[i].CountNightShifts;
                }
                worksheet.Cells[CurrentRow, 3].Value = days;
                worksheet.Cells[CurrentRow, 4].Value = nights;
                TotalDuties += days + nights;
                if (days + nights > 0)
                {
                    TotalDutyEmployees ++;
                }
                CurrentRow++;
            }
        }
    }
}

