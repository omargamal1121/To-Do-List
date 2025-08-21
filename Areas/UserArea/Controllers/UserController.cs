using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using To_Do_List.Areas.UserArea.Models;
using To_Do_List.Areas.UserArea.Repository;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace To_Do_List.Areas.UserArea.Controllers
{
	[Area("UserArea")]
	[Authorize]
	public class UserController : Controller
	{
		private readonly UserManager<IdentityUser> _userManager;
		private readonly IMainCategoryRepository _categoryRepository;
		private readonly ITasksRepository _tasksRepository;
		private readonly ISubCategoryRepository _subCategoryRepository;

		public UserController(IMainCategoryRepository categoryRepository, ISubCategoryRepository subCategoryRepository,
							  ITasksRepository tasksRepository, UserManager<IdentityUser> userManager)
		{
			_tasksRepository = tasksRepository;
			_categoryRepository = categoryRepository;
			_userManager = userManager;
			_subCategoryRepository = subCategoryRepository;
		}

		public async Task< IActionResult >CreateTask(int subCategoryId,int taskid=-1)
		{
			ViewBag.SubCategoryId = subCategoryId;
			if (taskid !=-1)
			{
				var task=await _tasksRepository.GetByIdAsync(taskid);
				SetViewBag();

				return View(task);

			}
			SetViewBag();
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> CreateTask(Tasks task, int subCategoryId)
		{
			ViewBag.SubCategoryId = subCategoryId;
			if (!ModelState.IsValid)
			{
				
				SetViewBag();
				return View(task);
			}
			var oldtask = (await _tasksRepository.GetByIdAsync(task.Id));
			if (oldtask is not null)
			{
				oldtask.Name= task.Name;
				oldtask.CreatTime= task.CreatTime;
				oldtask.EndTime= task.EndTime;
				oldtask.StartTime= task.StartTime;
				oldtask.Priority= task.Priority;
				oldtask.Description= task.Description;
				
				if (await _tasksRepository.SaveAsync()){
					TempData["mess"] = "Updated";
					return RedirectToAction("Tasks", new { SubCategoryId = subCategoryId });
				}
				else
				{
					ModelState.AddModelError("", "Error");
					return View(task);
				}
			}

			try
			{
				task.SubCategoryId = subCategoryId;
				await _tasksRepository.CreateAsync(task);
				return RedirectToAction("Tasks", new { SubCategoryId = subCategoryId });
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
				ViewBag.SubCategoryId = subCategoryId;
				SetViewBag();
				return View(task);
			}
		}



		public async Task<IActionResult> DeleteList(int id)
		{
			var subCategory = await _subCategoryRepository.GetByIdAsync(id);
			if (subCategory == null)
				return NotFound();

			if (await _subCategoryRepository.DeleteAsync(id))
				TempData["mess"] = "Deleted :)";
			else
				TempData["mess"] = "Error :)";

			return RedirectToAction("Lists", new { categoryId = subCategory.MainCategoryId });
		}

		public async Task<IActionResult> DeleteTask(int id)
		{
			var task = await _tasksRepository.GetByIdAsync(id);
			if (task == null)
				return NotFound();

			if (await _tasksRepository.DeleteAsync(id))
				TempData["Mess"] = "Task Deleted";
			else
				TempData["Mess"] = "Error";

			return RedirectToAction("Tasks", new { SubCategoryId = task.SubCategoryId });
		}

		public async Task<IActionResult> CompleteTask(int taskId)
		{
			var task = await _tasksRepository.GetByIdAsync(taskId);
			if (task == null)
				return NotFound();

			task.IsComplete = !task.IsComplete;
			TempData["mess"] = await _tasksRepository.SaveAsync() ? "Updated" : "Error";

			return RedirectToAction("Tasks", new { SubCategoryId = task.SubCategoryId });
		}

		private void SetViewBag()
		{
			ViewData["items"] = new SelectList(new List<SelectListItem>
			{
				new SelectListItem { Value = "1", Text = "Low" },
				new SelectListItem { Value = "2", Text = "Medium" },
				new SelectListItem { Value = "3", Text = "High" }
			}, "Value", "Text");
		}

		public async Task<IActionResult> Tasks(int subCategoryId)
		{
			ViewBag.SubCategoryId = subCategoryId;
			var subCategory = await _subCategoryRepository.GetByIdAsync(subCategoryId);
			if (subCategory == null)
				return NotFound();

			ViewBag.CategoryId = subCategory.MainCategoryId;
			return View(await _tasksRepository.GetAllAsync(subCategoryId));
		}

		public async Task<IActionResult> Index()
		{
			var userId = _userManager.GetUserId(User);
			var categories = await _categoryRepository.GetAllAsync(userId);
			var subcategories = categories.SelectMany(c => c.SubCategory).ToList();
			ViewBag.CompletedTasks = subcategories.SelectMany(sc => sc.Tasks).Count(t => t.IsComplete);
			ViewBag.Tasks = subcategories.SelectMany(sc => sc.Tasks).Count();

			return View(categories);
		}

		[HttpPost]
		public async Task<IActionResult> CreateList(string name, int categoryId)
		{
			var subCategory = new SubCategory { Name = name, MainCategoryId = categoryId, CreatTime = DateTime.Now };
			if (!TryValidateModel(subCategory))
			{
				TempData["mess"] = "Must form 5 to 20";
				TempData["name"] = name;
			}
			else if (await _subCategoryRepository.CreateAsync(subCategory))
				TempData["mess"] = "Added";

			return RedirectToAction("Lists", new { categoryId = categoryId });
		}

		public async Task<IActionResult> Lists(int categoryId)
		{
			ViewBag.CategoryId = categoryId;
			return View(await _subCategoryRepository.GetAllAsync(categoryId));
		}

		[HttpPost]
		public async Task<IActionResult> Create(string categoryName)
		{
			var category = new MainCategory { Name = categoryName, UserId = _userManager.GetUserId(User) };
			if (!TryValidateModel(category))
			{
				TempData["mess"] = "Invalid category name";
				return RedirectToAction("Index");
			}

			TempData["mess"] = await _categoryRepository.CreateAsync(category) ? "Created" : "Failed";
			return RedirectToAction("Index");
		}

		public async Task< IActionResult >ChangeName(int CategoryId, string CategoryName)
		{
			MainCategory category = await _categoryRepository.GetByIdAsync(CategoryId);
			ViewBag.categoryid = CategoryId;
			if (category is not null) 
			{
				if (CategoryName.Length >= 5)
				{
					category.Name = CategoryName;
					try
					{
						if (await _categoryRepository.SaveAsync())
						{
							TempData["mess"] = "Updated";

						}


					}
					catch (Exception e)
					{

						Console.WriteLine(e.Message);
						TempData["mess"] = "Error";
					}
				}
				else
					TempData["mess"] = "Must Name length between 5 to 20";
			}
			else
				TempData["mess"] = "Error";

			return RedirectToAction("Lists",new { categoryId = CategoryId });

		}
		public async Task<IActionResult> ChangeNameOfSubCategry(int SubCategoryId, string SubCategoryName)
		{

			SubCategory subCategory = await _subCategoryRepository.GetByIdAsync(SubCategoryId);
			ViewBag.SubCategoryId = SubCategoryId;
			if (subCategory is not null)
			{
				if (SubCategoryName.Length >= 5)
				{
					subCategory.Name = SubCategoryName;
					try
					{
						if (await _categoryRepository.SaveAsync())
						{
							TempData["mess"] = "Updated";

						}


					}
					catch (Exception e)
					{

						Console.WriteLine(e.Message);
						TempData["mess"] = "Error";
					}
				}
				else
					TempData["mess"] = "must name length > 5";

			}
			else
				TempData["mess"] = "Error";

			return RedirectToAction("Tasks", new { subCategoryId = SubCategoryId });

		}
		public async Task<IActionResult> Delete(int id)
		{
			TempData["message"] = await _categoryRepository.DeleteAsync(id) ? "Deleted Successfully" : "Deletion Failed";
			return RedirectToAction("Index");
		}
	}
}
