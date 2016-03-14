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
		public object GetPrimaryKeyValue(DbEntityEntry entry)
		{
			var objectStateEntry = ((IObjectContextAdapter)this).ObjectContext.ObjectStateManager.GetObjectStateEntry(entry.Entity);
			return objectStateEntry.EntityKey.EntityKeyValues[0].Value;
		}

		public override int SaveChanges()
		{
			ObjectContext context = ((IObjectContextAdapter)this).ObjectContext;

			var now = DateTime.Now;

			var modifiedEntities = ChangeTracker.Entries()
				.Where(p => p.State == EntityState.Modified).ToList();

			foreach (var change in modifiedEntities)
			{
				var entityName = change.Entity.GetType().BaseType.Name;
				
				var primaryKey = GetPrimaryKeyValue(change);

				foreach (var prop in change.OriginalValues.PropertyNames)
				{
					string originalValue;
                    if (change.OriginalValues[prop] != null)
					{
						originalValue = change.OriginalValues[prop].ToString();
					}
					else
					{
						originalValue = "";
					}
					string currentValue;
					if(change.CurrentValues[prop] != null)
					{
						currentValue = change.CurrentValues[prop].ToString();
					}
					else
					{
						currentValue = "";
					}
					if (originalValue != currentValue)
					{
						UN_AuditLog log = new UN_AuditLog()
						{
							TableName = entityName,
							PrimaryKeyID = (int)primaryKey,
							ColumnName = prop,
							OldValue = originalValue,
							NewValue = currentValue,
							DateChanged = now,
							id_userLogin = Settings.id_userLogin,
							Operation = "Modify"
						};
						this.UN_AuditLog.Add(log);
					}
				}
			}

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
				dv.Timestamp = now;
				dv.id_userLogin = Settings.id_userLogin;
			}

			return base.SaveChanges();
		}
	}
}
