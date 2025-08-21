using To_Do_List.Areas.UserArea.Models;
using static To_Do_List.Areas.UserArea.Models.Tasks;

namespace To_Do_List.Areas.UserArea.Repository
{
	public interface IMainCategoryRepository:IRepository<MainCategory>
	{

		public string GetName(int id);

		public Task<bool> DeleteAll(string userid);
		public Task<MainCategory> GetByIdAsync(int id);
		public  Task<IEnumerable<MainCategory>> GetAllAsync(string UserId);
		public Task<MainCategory> GetByNameAsync(string UserId, string Name);
		public Task<IEnumerable<SubCategory>> GetSubCategory(string UserId, int MainCategoryid);
		public Task<IEnumerable<Tasks>> GetTasks(string UserId, int MainCategoryid, int Subcategoryid);
		public Task<IEnumerable<Tasks>> GetCompletedTasks(string UserId, int MainCategoryid, int Subcategoryid);
		public Task<IEnumerable<Tasks>> GetByPriorityLevel(string UserId, int MainCategoryid, int Subcategoryid, PriorityLevel priority);
	}
}
