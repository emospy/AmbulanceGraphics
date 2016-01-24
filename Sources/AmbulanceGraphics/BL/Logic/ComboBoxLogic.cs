using BL.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Logic
{
	public class ComboBoxLogic : BaseLogic, IDisposable
	{
		public enum ArticleGroups
		{
			Stock = 1,
			Scrap = 2,
			Materials = 3,
			ScrapMaterials = 4,
			ScrapReceivers = 5,
		}

		public List<ComboBoxModel> ReadPositionTypes(int id_positionType = 0)
		{
			List<ComboBoxModel> result = new List<ComboBoxModel>();
			result.Add(new ComboBoxModel() { id = 0, Name = "", IsActive = true });
			result = this._databaseContext.NM_PositionTypes.Where(c => c.IsActive == true).Select(c => new ComboBoxModel { id = c.id_positionType, IsActive = c.IsActive, Name = c.Name }).ToList();
			if (id_positionType != 0)
			{
				if (result.Any(c => c.id == id_positionType) == false)
				{
					var positionType = this._databaseContext.NM_PositionTypes.FirstOrDefault(c => c.id_positionType == id_positionType);
					if (positionType != null)
					{
						result.Add(new ComboBoxModel { id = positionType.id_positionType, Name = positionType.Name, IsActive = positionType.IsActive });
					}
				}
			}
			return result;
		}

		public void Dispose()
		{
			//throw new NotImplementedException();
		}
	}
}

