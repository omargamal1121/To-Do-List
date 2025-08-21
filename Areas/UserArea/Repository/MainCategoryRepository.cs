using Microsoft.EntityFrameworkCore;
using To_Do_List.Areas.UserArea.Data;
using To_Do_List.Areas.UserArea.Models;

namespace To_Do_List.Areas.UserArea.Repository
{
	public class MainCategoryRepository : Repository<MainCategory> , IMainCategoryRepository 
	{

		public readonly AppDbContext _context;
		public MainCategoryRepository(AppDbContext context):base(context)
		{ 
			_context = context;
		}

		public async Task<bool> DeleteAll(string userid)
		{

		return  await _context.MainCategory.Where(M=>M.UserId==userid).ExecuteDeleteAsync()>0;
		}

		public async Task<IEnumerable<MainCategory>> GetAllAsync(string UserId)
		{

			
			return await _context.MainCategory.
				Include(c => c.SubCategory).
				ThenInclude(c => c.Tasks).
				Where(c => c.UserId == UserId).ToListAsync()?? Enumerable.Empty<MainCategory>();

		}

		public async Task<MainCategory> GetByIdAsync(int id)
		{
			return await _context.MainCategory.SingleOrDefaultAsync(m=>m.Id==id);
		}

		public async Task<MainCategory> GetByNameAsync(string UserId, string Name)
		{

		 return await	_context.MainCategory.Where(c => c.UserId == UserId).FirstOrDefaultAsync(x=>x.Name==Name);
			//var list = await GetAllAsync(UserId);
		 // return list.FirstOrDefault(x => x.Name == Name);
		}

		public async Task<IEnumerable<Tasks>> GetByPriorityLevel(string UserId, int MainCategoryid, int Subcategoryid, Tasks.PriorityLevel priority)
		{
			return await _context.Tasks.Where(t => t.SubCategoryId == Subcategoryid && t.SubCategory.MainCategoryId == MainCategoryid && t.SubCategory.main_Category.UserId == UserId&&t.Priority==priority).ToListAsync()?? Enumerable.Empty<Tasks>();
		 //var tasks= await GetTasks(UserId, MainCategoryid, Subcategoryid);
		 //return tasks.Where(t => t.Priority == priority).ToList()??Enumerable.Empty<Tasks>();

		}

		public async Task<IEnumerable<Tasks>> GetCompletedTasks(string UserId, int MainCategoryid, int Subcategoryid)
		{
			return await _context.Tasks.Where( t => t.SubCategoryId == Subcategoryid && t.SubCategory.MainCategoryId == MainCategoryid && t.SubCategory.main_Category.UserId == UserId && t.IsComplete==true).ToListAsync()?? Enumerable.Empty<Tasks>();
			//MainCategory? category= await _maincategories.Include(s=>s.SubCategory).ThenInclude(sc=>sc.Tasks).Where(c=>c.UserId==UserId).FirstOrDefaultAsync(x=>x.Id== MainCategoryid);
			//if(category is null)
			//	return Enumerable.Empty<Tasks>();
			//SubCategory? subCategory= category.SubCategory.FirstOrDefault(x=>x.Id==Subcategoryid);
			//return subCategory?.Tasks.Where(x => x.IsComplete == true) ?? Enumerable.Empty<Tasks>();
		}

		public string GetName(int id)
		{
			var category= _context.MainCategory.SingleOrDefault(c => c.Id == id);
			if(category is not null)
			{
				return category.Name;
			}
			else
				return string.Empty;
		 
		}

		public async Task<IEnumerable<SubCategory>>? GetSubCategory(string UserId, int Maincategoryid)
		{


		return await	_context.SubCategories.Where(s => s.MainCategoryId == Maincategoryid && s.main_Category.UserId == UserId).ToListAsync()?? Enumerable.Empty<SubCategory>();
			//MainCategory? category = await _maincategories.Include(x => x.SubCategory).FirstOrDefaultAsync(x => x.Id==Maincategoryid);

			//if(category is null)
			//	return Enumerable.Empty<SubCategory>();
			//return category?.SubCategory.ToList() ?? Enumerable.Empty<SubCategory>();
		}

		public async Task<IEnumerable<Tasks>> GetTasks(string UserId, int MainCategoryid, int Subcategoryid)
		{

		return await	_context.Tasks.Where(t => t.SubCategoryId == Subcategoryid && t.SubCategory.MainCategoryId == MainCategoryid && t.SubCategory.main_Category.UserId==UserId).ToListAsync()?? Enumerable.Empty<Tasks>();

			//MainCategory? category = await _maincategories.Include(s => s.SubCategory).ThenInclude(sc => sc.Tasks).FirstOrDefaultAsync();
			//if (category is null)
			//	return Enumerable.Empty<Tasks>();
			//SubCategory? subCategory = category.SubCategory.FirstOrDefault(x => x.Id == Subcategoryid);
			//return subCategory?.Tasks.OrderBy(x=>x.Priority).ToList() ?? Enumerable.Empty<Tasks>();

		}

		
	}
}
