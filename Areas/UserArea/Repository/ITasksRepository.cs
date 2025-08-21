using To_Do_List.Areas.UserArea.Models;
using static To_Do_List.Areas.UserArea.Models.Tasks;

namespace To_Do_List.Areas.UserArea.Repository
{
	public interface ITasksRepository:IRepository<Tasks>
	{
		Task<IEnumerable<Tasks>> GetAllAsync(int SubCategoryId);
		Task<Tasks> GetByName(string Name); 
		Task<bool> UpdateAsync(Tasks task); 

		Task<Tasks> GetByIdAsync(int Id);
		public Task<IEnumerable<Tasks>> GetCompletedTasks(int Subcategoryid);
		public Task<IEnumerable<Tasks>> GetByPriorityLevel(int Subcategoryid, PriorityLevel priority);
	}
}
