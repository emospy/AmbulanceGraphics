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

namespace BL.Logic
{

    public enum AbsenceExportTypes
    {
        Sickness = 1,
        Holidays = 2
    }
    public class AbsenceExportLogic : ExportLogic
    {
        public void GenerateAbsenceReport(string fileName, DateTime dateFrom, DateTime dateTo, int sickTreshold, AbsenceExportTypes typeExport)
        {
            FileInfo templateFile;
            switch (typeExport)
            {
                case AbsenceExportTypes.Sickness:
                    templateFile = new FileInfo("Absence.xlsx");
                    break;
                case AbsenceExportTypes.Holidays:
                    templateFile = new FileInfo("Holiday.xlsx");
                    break;
                default:
                    return;
            }
            
            FileInfo newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete(); // ensures we create a new workbook
                newFile = new FileInfo(fileName);
            }
            this.package = new ExcelPackage(newFile, templateFile);

            var baseDepartments = this._databaseContext.UN_Departments.Where(a => a.NumberShifts > 0).ToList();

            int CurrentRow = 2;

            ExcelWorksheet worksheet = package.Workbook.Worksheets[1];


            foreach (var bdep in baseDepartments)
            {
                worksheet.Cells[CurrentRow, 1].Value = bdep.Name;
                var subDeps = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent != a.id_department
                                                                              &&
                                                                              a.id_departmentParent ==
                                                                              bdep.id_department)
                    .ToList();
                CurrentRow++;

                foreach (var sdep in subDeps)
                {
                    this.ExportDepartmentPeriodAbsence(worksheet, dateFrom, dateTo, sdep, sickTreshold, ref CurrentRow, typeExport);
                }

                CurrentRow++;
            }

            this.package.Save();
        }

        public void ExportDepartmentPeriodAbsence(ExcelWorksheet worksheet, DateTime dateFrom, DateTime dateTo,
            UN_Departments dep, int sickTreshold, ref int CurrentRow, AbsenceExportTypes typeExport)
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
                    //var PF = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract
                    //																	&& a.Date.Month == date.Month
                    //																	&& a.Date.Year == date.Year
                    //																	&& a.id_scheduleType == (int)ScheduleTypes.PresenceForm);
                    //if (PF == null)
                    //{
                    //	continue;
                    //}

                    var DS = this._databaseContext.GR_PresenceForms.FirstOrDefault(a => a.id_contract == ass.id_contract
                                                                                        &&
                                                                                        a.Date.Month ==
                                                                                        CurrentDate.Month
                                                                                        &&
                                                                                        a.Date.Year == CurrentDate.Year
                                                                                        &&
                                                                                        a.id_scheduleType ==
                                                                                        (int)
                                                                                            ScheduleTypes.DailySchedule);

                    if (DS == null)
                    {
                        continue;
                    }

                    //PFRow PresenceFrom = new PFRow();
                    PFRow Result = new PFRow();
                    //PFRow Result = new PFRow();
                    //PresenceFrom.PF = PF;
                    Result.PF = DS;
                    //Result.PF = new GR_PresenceForms();


                    //for (i = 1; i <= date.Day; i++)
                    //{
                    //	Result[i] = PresenceFrom[i];
                    //}
                    //int dm = DateTime.DaysInMonth(date.Year, date.Month);
                    //for (; i <= dm; i++)
                    //{
                    //	Result[i] = DailySchedule[i];
                    //}

                    Result.id_person = DS.HR_Contracts.id_person;
                    Result.Name = DS.HR_Contracts.UN_Persons.Name;

                    Result.LstWorktimeAbsences =
                        this._databaseContext.GR_WorkTimeAbsence.Where(a => a.id_contract == ass.id_contract
                                                                            && a.Date.Month == CurrentDate.Month
                                                                            && a.Date.Year == CurrentDate.Year)
                            .ToList();

                    Result.RealDate = CurrentDate;
                    if (ass.HR_WorkTime == null)
                    {
                        continue;
                    }
                    Result.WorkHours = ass.HR_WorkTime.WorkHours;
                    Result.lstShiftTypes = this._databaseContext.GR_ShiftTypes.ToList();
                    Result.cRow = cRow;

                    var workDaysNorm = cRow.WorkDays;
                    var fad =
                        ass.HR_Contracts.HR_Assignments.FirstOrDefault(b => b.IsAdditionalAssignment == false)
                            .AssignmentDate;
                    if (fad.Year == CurrentDate.Year && fad.Month == CurrentDate.Month &&
                        ass.HR_Contracts.IsFired == false)
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

            var countMonths = ((dateTo.Year - dateFrom.Year) * 12) + dateTo.Month - dateFrom.Month;

            
            foreach (var Employee in EmployeeGroups)
            {
                //var emplCount = Employee.Count();
                List<PFRow> emplList;
                emplList = Employee.ToList();
                //var corr = countMonths - emplCount;
                //var sortpf = Employee.OrderByDescending(a => a.RealDate).ToList();

                worksheet.Cells[CurrentRow, 1].Value = Employee.First().Name;
                worksheet.Cells[CurrentRow, 2].Value = Employee.First().WorkHours.ToString();

                switch (typeExport)
                {
                    case AbsenceExportTypes.Sickness:
                        this.PrintExportSickness(worksheet, countMonths, emplList, sickTreshold, ref CurrentRow);
                        break;
                    case AbsenceExportTypes.Holidays:
                        this.PrintExportHolidays(worksheet, countMonths, emplList, sickTreshold, ref CurrentRow);
                        break;
                }
            }
        }

        public void PrintExportSickness(ExcelWorksheet worksheet, int countMonths, List<PFRow> emplList, int sickTreshold, ref int CurrentRow)
        {
            const int start = 3;
            bool toAdd = false;
            for (int i = countMonths, pos = 0; i >= 0; i--, pos++)
            {
                if (emplList[pos].CountSickness > sickTreshold)
                {
                    toAdd = true;
                }
                else
                {
                    continue;
                }
                worksheet.Cells[CurrentRow, start + i * 4].Value = emplList[pos].Norm;
                worksheet.Cells[CurrentRow, start + i * 4 + 1].Value = emplList[pos].TotalWorkedOut;
                worksheet.Cells[CurrentRow, start + i * 4 + 2].Value = emplList[pos].Difference;
                worksheet.Cells[CurrentRow, start + i * 4 + 3].Value = emplList[pos].CountSickness;

            }
            if (toAdd)
            {
                CurrentRow++;
            }
            else
            {
                for (int i = 1; i <= 3 + (countMonths + 1) * 4; i++)
                {
                    worksheet.Cells[CurrentRow, i].Value = "";
                }
            }
        }

        public void PrintExportHolidays(ExcelWorksheet worksheet, int countMonths, List<PFRow> emplList, int sickTreshold, ref int CurrentRow)
        {
            const int start = 3;
            bool toAdd = false;
            for (int i = countMonths, pos = 0; i >= 0; i--, pos++)
            {
                if (emplList[pos].CountHoliday + emplList[pos].CountUnpaid > sickTreshold)
                {
                    toAdd = true;
                }
                else
                {
                    continue;
                }
                worksheet.Cells[CurrentRow, start + i * 6].Value = emplList[pos].Norm;
                worksheet.Cells[CurrentRow, start + i * 6 + 1].Value = emplList[pos].TotalWorkedOut;
                worksheet.Cells[CurrentRow, start + i * 6 + 2].Value = emplList[pos].Difference;
                worksheet.Cells[CurrentRow, start + i * 6 + 3].Value = emplList[pos].CountHoliday + emplList[pos].CountUnpaid;
                worksheet.Cells[CurrentRow, start + i * 6 + 4].Value = emplList[pos].CountUnpaid;
                worksheet.Cells[CurrentRow, start + i * 6 + 5].Value = emplList[pos].CountHoliday;
            }
            if (toAdd)
            {
                CurrentRow++;
            }
            else
            {
                for (int i = 1; i <= 3 + (countMonths + 1) * 6; i++)
                {
                    worksheet.Cells[CurrentRow, i].Value = "";
                }
            }
        }
    }
}

