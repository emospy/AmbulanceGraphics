using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
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
    public class SchemeExportLogic : SchedulesLogic
    {
        const int offset = 4;
        internal ExcelPackage package;
        
        internal int currentCrewOrder;

        public void ExportYearScheme(string fileName, DateTime date)
        {
            var sdfn = fileName;

            FileInfo templateFile = new FileInfo("YearScheme.xlsx");
            FileInfo newFile = new FileInfo(fileName);
            if (newFile.Exists)
            {
                newFile.Delete(); // ensures we create a new workbook
                newFile = new FileInfo(fileName);
            }
            this.package = new ExcelPackage(newFile, templateFile);

            for (int i = 1; i <= 12; i++)
            {
                date = new DateTime(date.Year, i, 1);
                this.ExportMonthShifts(date, package.Workbook.Worksheets[i]);
            }
            this.package.Save();
        }
        
        private void ExportMonthShifts(DateTime date, ExcelWorksheet sheet)
        {
            //generate month scheme for 8 and 7 hour shift type departments
            var lstSchemes = this._databaseContext.GR_StartShifts.Select(a => a).ToList();

            var s5 = lstSchemes.FirstOrDefault(a => a.ShiftsNumber == 5);
            var s4 = lstSchemes.FirstOrDefault(a => a.ShiftsNumber == 4);

            //Find a depart for 4 and 5 shifts -- to activate the cascade

            var d5 = this._databaseContext.UN_Departments.FirstOrDefault(a => a.NumberShifts == 5);
            var d4 = this._databaseContext.UN_Departments.FirstOrDefault(a => a.NumberShifts == 4);

            //get d5 lead shift
            var lead5 = GetLeadShiftNumber(date, d5.id_department);
            //get d4 lead shifit
            var lead4 = GetLeadShiftNumber(date, d4.id_department);

            //place year on the worksheet 
            this.PlaceYear(sheet, date.Year);

            //generate calendar row for the month with work and non work days
            CalendarRow calRow = new CalendarRow(date);

            this.ColorColumns(sheet, calRow);
            //place shift number on the worksheet
            //this.PlaceLeadShiftNumber(date, sheet, d5, d4);

            this.PlaceDepartmentSchedules(sheet, d5, d4, date);
            //place workdays on 3 places
            this.PlaceWorkdays(sheet, calRow);
        }

        private void PlaceWorkdays(ExcelWorksheet sheet, CalendarRow row)
        {
            sheet.Cells[3, 14].Value = row.WorkDays;
            sheet.Cells[20, 14].Value = row.WorkDays;
            sheet.Cells[38, 14].Value = row.WorkDays;
        }

        private void PlaceDepartmentSchedules(ExcelWorksheet sheet, UN_Departments d5, UN_Departments d4, DateTime date)
        {
            var deps4 = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == d4.id_department && a.id_departmentParent != a.id_department).OrderBy(a => a.TreeOrder).ToList();
            var deps5 = this._databaseContext.UN_Departments.Where(a => a.id_departmentParent == d5.id_department && a.id_departmentParent != a.id_department).OrderBy(a => a.TreeOrder).ToList();

            this.PlaceLeadShiftNumber(date, sheet, deps5[0], deps4[0]);
            for (int d = 0; d < 5; d++)
            {
                var sched5 = GetDepartmentSchedules(deps5[d].id_department, date).FirstOrDefault();
                for (int i = 1 + offset; i < 31 + offset; i++)
                {
                    sheet.Cells[7 + 2 * d, i].Value = sched5[i - offset];
                    sheet.Cells[24 + 2 * d, i].Value = sched5[i - offset];
                }
                sheet.Cells[7 + 2 * d, 36].Value = sched5.CountDayShifts;
                sheet.Cells[7 + 2 * d, 37].Value = sched5.CountNightShifts;
                sheet.Cells[24 + 2 * d, 36].Value = sched5.CountDayShifts;
                sheet.Cells[24 + 2 * d, 37].Value = sched5.CountNightShifts;
            }

            for (int d = 0; d < 4; d++)
            {
                var sched4 = GetDepartmentSchedules(deps4[d].id_department, date).FirstOrDefault();
                for (int i = 1 + offset; i < 31 + offset; i++)
                {
                    sheet.Cells[42 + 2 * d, i].Value = sched4[i - offset];
                    if (sched4[i - offset] != "")
                    {
                        sched4[i - offset] = "Д";
                    }
                    sheet.Cells[52 + 2 * d, i ].Value = sched4[i - offset];
                }
                sheet.Cells[42 + 2 * d, 36].Value = sched4.CountDayShifts;
                sheet.Cells[42 + 2 * d, 37].Value = sched4.CountNightShifts;

                sheet.Cells[52 + 2 * d, 36].Value = sched4.CountDayShifts + sched4.CountNightShifts;
            }
        }

        private void PlaceLeadShiftNumber(DateTime date, ExcelWorksheet sheet, UN_Departments d5, UN_Departments d4)
        {
            var ls = this.GetLeadShiftNumber(date, d5.id_department);
            for (int i = 1; i <= 31; i ++)
            {
                sheet.Cells[4, i + offset].Value = ls;
                sheet.Cells[21, i + offset].Value = ls;
                ls++;
                if (ls > 5)
                {
                    ls = 1;
                }
            }
            ls = this.GetLeadShiftNumber(date, d4.id_department);
            for (int i = 1; i <= 31; i++)
            {
                sheet.Cells[39, i + offset].Value = ls;
                sheet.Cells[50, i + offset].Value = ls;
                ls++;
                if (ls > 4)
                {
                    ls = 1;
                }
            }
        }

        private void PlaceYear(ExcelWorksheet sheet, int year)
        {
            sheet.Cells[1, 21].Value = year;
            sheet.Cells[18, 21].Value = year;
            sheet.Cells[36, 21].Value = year;
        }

        private void ColorColumns(ExcelWorksheet sheet, CalendarRow calRow)
        {
            
            //colour the columns
            for (int i = 1 + offset; i <= 31 + offset; i ++)
            {
                if (calRow[i - offset] == false)
                {
                    for (int j = 5; j <= 16; j ++)
                    {
                        sheet.Cells[j, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                    for (int j = 22; j <= 33; j++)
                    {
                        sheet.Cells[j, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                    for (int j = 40; j <= 49; j++)
                    {
                        sheet.Cells[j, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                    for (int j = 51; j <= 59; j++)
                    {
                        sheet.Cells[j, i].Style.Fill.PatternType = ExcelFillStyle.Solid;
                        sheet.Cells[j, i].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
                    }
                }
            }
        }
    }
}