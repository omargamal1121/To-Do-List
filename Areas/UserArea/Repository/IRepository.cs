namespace To_Do_List.Areas.UserArea.Repository
{
	public interface IRepository<T>where T : class
	{
		public  Task<bool> CreateAsync(T entity);

		public Task<bool> DeleteAsync(int Id);
		public Task<bool> SaveAsync();
	}
}
