using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.DB
{
	public class EntityBase
	{
		public DateTime Timestamp { get; set; }
		public int id_userLogin { get; set; }
	}

	public partial class AmbulanceEntities : DbContext
	{
		public override int SaveChanges()
		{
			ObjectContext context = ((IObjectContextAdapter)this).ObjectContext;

			//Find all Entities that are Added/Modified that inherit from my EntityBase
			//IEnumerable<ObjectStateEntry> objectStateEntries =
			//	from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
			//	where
			//		e.IsRelationship == false &&
			//		e.Entity != null &&
			//		typeof(EntityBase).IsAssignableFrom(e.Entity.GetType())
			//	select e;
			IEnumerable<ObjectStateEntry> objectStateEntries =
				from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
				where
					e.IsRelationship == false &&
					e.Entity != null 
				select e;

			var currentTime = DateTime.Now;

			foreach (var entry in objectStateEntries)
			{
				dynamic dv = entry.Entity;
				dv.Timestamp = DateTime.Now;
				dv.id_userLogin = Settings.id_userLogin;
			}

			return base.SaveChanges();
		}
	}
}
