using Microsoft.EntityFrameworkCore;
using To_Do_List.Areas.UserArea.Data;
using To_Do_List.Areas.UserArea.Models;

namespace To_Do_List.Areas.UserArea.Repository
{
	public class TasksRepository :Repository<Tasks>, ITasksRepository
	{
		private readonly AppDbContext _context;
		public readonly DbSet<Tasks> _tasks;
		public TasksRepository(AppDbContext Context) : base(Context)
		{
			_context = Context;
			_tasks=_context.Tasks; 
		}

		public async Task<bool> UpdateAsync(Tasks task)
		{
			_tasks.Update(task);
			return await SaveAsync();

		}
		public async Task<IEnumerable<Tasks>> GetAllAsync(int SubCategoryId)
		{
			return  await _tasks.Include(t=>t.SubCategory).ThenInclude(sc=>sc.main_Category).Where(t=>t.SubCategoryId == SubCategoryId).ToListAsync()??Enumerable.Empty<Tasks>(); 
		}

		public async Task<Tasks> GetByName(string Name)
		{
			return await _tasks.FirstOrDefaultAsync(t=>t.Name == Name);
		}

		public async Task<Tasks> GetByIdAsync(int Id)
		{
			return await _tasks.Include(t=>t.SubCategory).FirstOrDefaultAsync(t => t.Id == Id);
		}

		public async Task<IEnumerable<Tasks>> GetCompletedTasks(int Subcategoryid)
		{
			return await _tasks.Where(t => t.IsComplete).ToListAsync();
		}

		public async Task<IEnumerable<Tasks>> GetByPriorityLevel(int Subcategoryid, Tasks.PriorityLevel priority)
		{
			return await _tasks.Where(t=>t.SubCategoryId == Subcategoryid && t.Priority==priority).ToListAsync();
		}
	}
}
