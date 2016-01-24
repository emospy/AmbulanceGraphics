using System;
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

		#region BookIn

		[Description("Грешка! Заприхождаването не е открито!")]
		BookInNotFound,

		[Description("Грешка! Подобен артикул съсществува повече от един път в склада или не съществува! Моля свържете се с поддръжката!")]
		StockQuantityFoundMoreThanOnce,

		[Description("Трябва да бъде избран артикул!")]
		StockArticleNotSelected,

		[Description("Количеството трябва да е положително число!")]
		QuantityMustBePositive,

		[Description("Трябва да се избере състояние на артикула!")]
		ArticleStateNotSelected,

		[Description("За артикули от тип отпадък трябва да се избере код на отпадъка!")]
		ArticleScrapCodeNotSelected,

		[Description("Няма достатъчна наличност в склада!")]
		InsufficientQuantity,

		[Description("Проблем при създаването на приемо- предавателния протокол!")]
		ReportingFailed,

		[Description("Цената трябва да е положително число!")]
		IncorrectPrice,

		[Description("Количестовото на артикули, които имат мерна единица брой, трябва да е цяло число!")]
		ArticlesWithCountMesurmentUnit,

		[Description("Количестовото на разбивките е по-голямо от количеството на заприхождаването!")]
		DetailsQuantitiesGreaterThanBaseBookIn,
		#endregion

		#region BookOut
		[Description("Няма такъв артикул в склада!")]
		NoSuchArticle,

		[Description("Грешка, не е намерено количесто в склада за този атикул!")]
		StockQuantityNotFound,

		[Description("Грешка, не е намерено изписването!")]
		BookOutNotFound,
		#endregion

		#region Templates

		[Description("Няма такъв шаблон!")]
		NoSuchTemplate,

		[Description("Грешка при печат!")]
		PrintError,

		#endregion
	}
}
