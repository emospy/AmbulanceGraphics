﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
	public enum ErrorCodes
	{
		Succes = 0,

		[Description("Няма връзка с база данни!")]
		NoDb = 1,

		[Description("Грешна парола!")]
		WrongPassword = 1000,

		[Description("Този потребител не е открит!")]
		UserNotFound,

		[Description("Въведената парола в двете полета не съвпада!")]
		PasswordsDoNotMatch,

		[Description("Въведеното име вече съществува!")]
		DuplicateName,

		[Description("Избраният запис вече е рефериран в други таблици с данни и не може да бъде изтрит!")]
		DeleteRecordAlreadyReferred,

		[Description("Има непопълено задължително поле!")]
		FieldNotFilled,

		#region Templates

		[Description("Няма такъв шаблон!")]
		NoSuchTemplate,

		[Description("Грешка при печат!")]
		PrintError,

		#endregion
	}
}
