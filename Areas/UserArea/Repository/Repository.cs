using Microsoft.EntityFrameworkCore;
using To_Do_List.Areas.UserArea.Data;

namespace To_Do_List.Areas.UserArea.Repository
{
	public class Repository<T> : IRepository<T> where T : class
	{
		public readonly AppDbContext _Context;
		public readonly DbSet<T> _Entities;
		public Repository(AppDbContext Context)
		{
			_Context = Context;
			_Entities = Context.Set<T>();
			
		}
		public async Task<bool> CreateAsync(T entity)
		{
			try
			{
			    await _Entities.AddAsync(entity);
				return await SaveAsync();

			}
			catch (Exception)
			{
				return false;
			}
		}

		public async Task<bool> DeleteAsync(int Id)
		{
			T?obj= await _Entities.FindAsync(Id);
			if (obj is not null) {
				try
				{
					_Entities.Remove(obj);
					return await SaveAsync();

				}
				catch (Exception)
				{ return false;

					throw;
				}
			}
			return false;
		}

		public async Task<bool> SaveAsync()
		{
			try
			{
		      return await _Context.SaveChangesAsync()>0 ?true:false;

			}
			catch (Exception e)
			{

				Console.WriteLine(e.Message);
				return false;
			}
		}

	
	}
}
