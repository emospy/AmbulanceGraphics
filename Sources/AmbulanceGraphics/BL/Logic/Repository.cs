using BL.DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zora.Core.Logic
{
	public interface IRepository<T> where T : class
	{
		List<T> GetAll();
		T GetById(int id);
		void Add(T entity);
		void Update(T entity);
		void Delete(T entity);
		void Delete(int id);
	}

	public class Repository<T> : CoreLogic, IRepository<T> where T : class
	{
		protected AmbulanceEntities DbContext { get; set; }
		protected DbSet<T> DbSet { get; set; }

		public Repository()
		{
		}

		public Repository(AmbulanceEntities dbContext)
		{
			if (dbContext == null)
			{
				throw new NullReferenceException("dbContext");
			}
			this.DbContext = dbContext;
			this.DbSet = dbContext.Set<T>();
		}

		#region Implementation of IRepository<T>

		public virtual IEnumerable<T> Get(
			Expression<Func<T, bool>> filter = null,
			Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
			string includeProperties = "")
		{
			IQueryable<T> query = DbSet;

			if (filter != null)
			{
				query = query.Where(filter);
			}

			foreach (var includeProperty in includeProperties.Split
				(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
			{
				query = query.Include(includeProperty);
			}

			if (orderBy != null)
			{
				return orderBy(query).ToList();
			}
			else
			{
				return query.ToList();
			}
		}

		public List<T> GetAll()
		{
			return DbSet.ToList();
		}

		public T GetById(int id)
		{
			return DbSet.Find(id);
		}

		public void Add(T entity)
		{
			DbSet.Add(entity);
		}

		public void Update(T entity)
		{
			var entry = DbContext.Entry(entity);
			if (entry.State == EntityState.Detached)
			{
				DbSet.Attach(entity);
			}
			entry.State = EntityState.Modified;
			
        }

		public void Delete(T entity)
		{
			//var entry = DbContext.Entry(entity);
			//DbSet.Attach(entity);
			//entry.State = EntityState.Deleted;
			if (DbContext.Entry(entity).State == EntityState.Detached)
			{
				DbSet.Attach(entity);
			}
			DbSet.Remove(entity);
		}

		public void Delete(int id)
		{
			var entity = GetById(id);
			if (entity == null) return;
			Delete(entity);
		}

		#endregion
	}
}
