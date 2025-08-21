using To_Do_List.Areas.UserArea.Models;
using static To_Do_List.Areas.UserArea.Models.Tasks;

namespace To_Do_List.Areas.UserArea.Repository
{
	public interface ISubCategoryRepository:IRepository<SubCategory>
	{
		public Task<IEnumerable<SubCategory>> GetAllAsync(int maincategoryid);
		 public Task<SubCategory> GetByIdAsync(int id);
		public string GetSubcategoryName(int id);
		public Task<SubCategory> GetByNameAsync(int maincategoryid, string Name);
		public Task<IEnumerable<Tasks>> GetTasks(int maincategoryid, int Subcategoryid);
		public Task<IEnumerable<Tasks>> GetCompletedTasks(int maincategoryid, int Subcategoryid);
		public Task<IEnumerable<Tasks>> GetByPriorityLevel(int maincategoryid, int Subcategoryid, PriorityLevel priority);

	}
}
