using Microsoft.EntityFrameworkCore;
using To_Do_List.Areas.UserArea.Data;
using To_Do_List.Areas.UserArea.Models;

namespace To_Do_List.Areas.UserArea.Repository
{
	public class SubCategoryRepository : Repository<SubCategory>, ISubCategoryRepository
	{
		private AppDbContext _context;
		public SubCategoryRepository(AppDbContext Context) : base(Context)
		{
			_context = Context;
		}


		public string GetSubcategoryName(int id)
		{
			var subcatogry = _Context.Set<SubCategory>().Find(id);
			if (subcatogry != null)
			{
				return subcatogry.Name;
			}
			else return string.Empty;
		

		}

		public async Task<IEnumerable<SubCategory>> GetAllAsync(int maincategoryid)
		{
		  return await	_context.SubCategories.Include(sc=>sc.Tasks).Where(sc=>sc.MainCategoryId == maincategoryid).ToListAsync()?? Enumerable.Empty<SubCategory>();
		}

		public async Task<SubCategory> GetByIdAsync(int id)
		{
			return await _context.SubCategories.Include(sc=>sc.main_Category).FirstAsync(sc=>sc.Id==id);
		}

		public async Task<SubCategory> GetByNameAsync(int maincategoryid, string Name)
		{
			return  await _context.SubCategories.FirstOrDefaultAsync(sc => sc.MainCategoryId == maincategoryid && sc.Name == Name); 
		}

		public async Task<IEnumerable<Tasks>> GetByPriorityLevel(int maincategoryid, int Subcategoryid, Tasks.PriorityLevel priority)
		{
			return await  _context.Tasks.Where(t=>t.SubCategoryId==Subcategoryid && t.SubCategory.MainCategoryId==maincategoryid&&t.Priority==priority).ToListAsync() ??Enumerable.Empty<Tasks>();
		}

		public async Task<IEnumerable<Tasks>> GetCompletedTasks(int maincategoryid, int Subcategoryid)
		{
			return await _context.Tasks.Where(t => t.SubCategoryId == Subcategoryid && t.SubCategory.MainCategoryId == maincategoryid && t.IsComplete ==true ).ToListAsync() ?? Enumerable.Empty<Tasks>();
		}

		public async Task<IEnumerable<Tasks>> GetTasks(int maincategoryid, int Subcategoryid)
		{
			return await _context.Tasks.Where(t => t.SubCategoryId == Subcategoryid && t.SubCategory.MainCategoryId == maincategoryid).ToListAsync() ?? Enumerable.Empty<Tasks>();
		}
	}
}
