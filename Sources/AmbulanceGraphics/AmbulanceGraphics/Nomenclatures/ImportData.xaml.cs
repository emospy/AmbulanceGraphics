﻿using BL.DB;
using BL.Logic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Excel = Microsoft.Office.Interop.Excel;

namespace AmbulanceGraphics.Nomenclatures
{
	/// <summary>
	/// Interaction logic for ImportData.xaml
	/// </summary>
	public partial class ImportData : Window
	{
		public ImportData()
		{
			InitializeComponent();
		}

		private void btnImportGlobalPositions_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog opf = new OpenFileDialog();

			if (opf.ShowDialog().Value == true)
			{
				Excel.Worksheet xlsheet;
				Excel.Workbook xlwkbook;

				xlwkbook = (Excel.Workbook)System.Runtime.InteropServices.Marshal.BindToMoniker(opf.FileName);
				xlsheet = (Excel.Worksheet)xlwkbook.ActiveSheet;

				Excel.Range oRng;

				for (int i = 2; i < 48; i++)
				{
					HR_GlobalPositions pos = new HR_GlobalPositions();
					//name
					string gstr;
					oRng = (Excel.Range)xlsheet.Cells[i, 1];
					try
					{
						gstr = oRng.get_Value(Missing.Value).ToString();
					}
					catch (System.NullReferenceException)
					{
						continue;
					}
					if (gstr == "")
					{
						continue;
					}
					pos.Name = gstr;


					oRng = (Excel.Range)xlsheet.Cells[i, 6];
					try
					{
						gstr = oRng.get_Value(Missing.Value).ToString();
					}
					catch (System.NullReferenceException)
					{
						continue;
					}
					if (gstr == "")
					{
						continue;
					}
					pos.id_positionType = int.Parse(gstr);
					pos.IsActive = true;
					pos.ActiveFrom = DateTime.Now;

					using (var logic = new NomenclaturesLogic())
					{
						logic.HR_GlobalPositions.Add(pos);
						logic.Save();
					}
				}
			}
		}

		private string GetRangeValue(int i, int cell, Excel.Worksheet xlsheet)
		{
			var oRng = (Excel.Range)xlsheet.Cells[i, cell];
			string gstr;
			try
			{
				gstr = oRng.get_Value(Missing.Value).ToString().Trim();
			}
			catch (System.NullReferenceException)
			{
				return null;
			}
			return gstr;
		}

		private void btnImportPersonsAndPositions_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog opf = new OpenFileDialog();

			if (opf.ShowDialog().Value == true)
			{
				Excel.Worksheet xlsheet;
				Excel.Workbook xlwkbook;

				xlwkbook = (Excel.Workbook)System.Runtime.InteropServices.Marshal.BindToMoniker(opf.FileName);
				xlsheet = (Excel.Worksheet)xlwkbook.ActiveSheet;

				Excel.Range oRng;

				int id_currentDepartment = 0;
				int id_parentDepartment = 0;


				for (int i = 1; i < 12; i++)
				{
					using (var logic = new NomenclaturesLogic())
					{
						string gstr;
						if (id_currentDepartment == 0)
						{
							gstr = this.GetRangeValue(i, 1, xlsheet);
							if (gstr == "")
							{
								continue;
							}
							if (gstr == "1000")
							{

								gstr = this.GetRangeValue(i, 2, xlsheet);

								id_currentDepartment = logic.GetDepartmentByName(gstr).id_department;
								id_parentDepartment = id_currentDepartment;
								continue;
							}
							else if (gstr == "999")
							{
								gstr = this.GetRangeValue(i, 2, xlsheet);
								id_currentDepartment = logic.GetDepartmentShiftByName(gstr, id_currentDepartment).id_department;
								continue;
							}
						}
						else
						{

							gstr = this.GetRangeValue(i, 1, xlsheet);
							if (gstr == "")
							{
								continue;
							}
							#region parse department
							if (gstr == "1000")
							{
								gstr = this.GetRangeValue(i, 2, xlsheet);
								id_currentDepartment = logic.GetDepartmentByName(gstr).id_department;
								id_parentDepartment = id_currentDepartment;
								continue;
							}
							else if (gstr == "999")
							{
								gstr = this.GetRangeValue(i, 2, xlsheet);
								id_currentDepartment = logic.GetDepartmentShiftByName(gstr, id_parentDepartment).id_department;
								continue;
							}
							#endregion
							else
							{
								gstr = this.GetRangeValue(i, 2, xlsheet);


								HR_StructurePositions spos = null;

								UN_Persons per = new UN_Persons();
								HR_Contracts con = new HR_Contracts();
								HR_Assignments ass = new HR_Assignments();

								spos = logic.FindStructurePositionByName(gstr, id_currentDepartment);
								if (spos == null)
								{
									spos = new HR_StructurePositions();
									spos.id_globalPosition = logic.GetGlobalPositionByName(gstr).id_globalPosition;
									spos.id_department = id_currentDepartment;
									spos.IsActive = true;
									spos.ActiveFrom = DateTime.Now;
									spos.StaffCount = 1;
									logic.HR_StructurePositions.Add(spos);
								}

								gstr = this.GetRangeValue(i, 3, xlsheet);
								per.Name = gstr;

								con.UN_Persons = per;

								ass.HR_StructurePositions = spos;
								ass.AdditionalHolidays = 0;
								ass.NumberHolidays = 20;
								ass.HR_Contracts = con;
								ass.IsActive = true;
								ass.IsAdditionalAssignment = false;

								logic.UN_Persons.Add(per);

								logic.HR_Contracts.Add(con);
								logic.HR_Assignments.Add(ass);

								logic.Save();

								spos.Order = spos.id_structurePosition;
								logic.Save();

							}
						}
					}
				}
			}
		}

		private void btnImportAmbulances_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog opf = new OpenFileDialog();

			if (opf.ShowDialog().Value == true)
			{
				Excel.Worksheet xlsheet;
				Excel.Workbook xlwkbook;

				xlwkbook = (Excel.Workbook)System.Runtime.InteropServices.Marshal.BindToMoniker(opf.FileName);
				xlsheet = (Excel.Worksheet)xlwkbook.ActiveSheet;

				Excel.Range oRng;

				for (int i = 1; i < 112; i++)
				{
					GR_Ambulances amb = new GR_Ambulances();
					//name
					string gstr;
					oRng = (Excel.Range)xlsheet.Cells[i, 2];
					try
					{
						gstr = oRng.get_Value(Missing.Value).ToString();
					}
					catch (System.NullReferenceException)
					{
						continue;
					}
					if (gstr == "")
					{
						continue;
					}
					amb.Name = gstr;


					oRng = (Excel.Range)xlsheet.Cells[i, 3];
					try
					{
						gstr = oRng.get_Value(Missing.Value).ToString();
					}
					catch (System.NullReferenceException)
					{
						continue;
					}
					if (gstr == "")
					{
						continue;
					}
					amb.Description = gstr;

					oRng = (Excel.Range)xlsheet.Cells[i, 4];
					try
					{
						gstr = oRng.get_Value(Missing.Value).ToString();
					}
					catch (System.NullReferenceException)
					{
						continue;
					}
					if (gstr == "")
					{
						continue;
					}
					amb.id_ambulanceType = int.Parse(gstr);

					using (var logic = new NomenclaturesLogic())
					{
						logic.GR_Ambulances.Add(amb);
						logic.Save();
					}
				}
			}
		}

		private void btnImportPersonalData_Click(object sender, RoutedEventArgs e)
		{
			OpenFileDialog opf = new OpenFileDialog();

			if (opf.ShowDialog().Value == true)
			{
				Excel.Worksheet xlsheet;
				Excel.Workbook xlwkbook;

				xlwkbook = (Excel.Workbook)System.Runtime.InteropServices.Marshal.BindToMoniker(opf.FileName);
				xlsheet = (Excel.Worksheet)xlwkbook.ActiveSheet;

				Excel.Range oRng;

				List<UN_Persons> lstPersons = new List<UN_Persons>();
				List<HR_WorkTime> lstWorktimes = new List<HR_WorkTime>();
				List<string> lstMissing = new List<string>();
				using (var logic = new PersonalLogic())
				{
					for (int i = 2; i < 136; i++)
					{
						if (i == 2)
						{
							lstPersons = logic.UN_Persons.GetAll();
							lstWorktimes = logic.HR_WorkTime.GetAll();
						}
						//name
						string gstr;
						oRng = (Excel.Range)xlsheet.Cells[i, 1];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
						}
						catch
						{
							continue;
						}
						if (gstr == "")
						{
							continue;
						}

						var person = lstPersons.Find(p => p.Name == gstr);
						if (person == null)
						{
							lstMissing.Add(gstr);
							continue;
						}

						oRng = (Excel.Range)xlsheet.Cells[i, 2];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
						}
						catch
						{
							continue;
						}

						person.EGN = gstr;

						var con = person.HR_Contracts.FirstOrDefault();
						if (con == null)
						{
							continue;
						}
						var ass = con.HR_Assignments.FirstOrDefault();
						if (ass == null)
						{
							continue;
						}

						oRng = (Excel.Range)xlsheet.Cells[i, 3];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
						}
						catch
						{
							continue;
						}
						DateTime date;
						if (DateTime.TryParse(gstr, out date) == true)
						{
							ass.AssignmentDate = date;
						}

						oRng = (Excel.Range)xlsheet.Cells[i, 4];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
						}
						catch
						{
							continue;
						}
						double wt;
						if (double.TryParse(gstr, out wt) == true)
						{
							var W = lstWorktimes.FirstOrDefault(f => f.WorkHours == wt);
							ass.id_workTime = W.id_worktime;
						}

						oRng = (Excel.Range)xlsheet.Cells[i, 5];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
							con.ContractNumber = gstr;
							ass.ContractNumber = gstr;
						}
						catch
						{
							//continue;
						}


						oRng = (Excel.Range)xlsheet.Cells[i, 6];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
						}
						catch
						{
							continue;
						}
						if (DateTime.TryParse(gstr, out date) == true)
						{
							ass.ContractDate = date;
							con.ContractDate = date;
						}

						oRng = (Excel.Range)xlsheet.Cells[i, 7];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
							person.GSM = gstr;
						}
						catch
						{
						}



						oRng = (Excel.Range)xlsheet.Cells[i, 8];
						try
						{
							gstr = oRng.get_Value(Missing.Value).ToString();
							person.Address = gstr;
						}
						catch
						{
						}


						logic.Save();

					}
				}
			}
		}
	}
}
