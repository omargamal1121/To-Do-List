using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_List.Areas.UserArea.Models
{
	public class SubCategory
	{
		[Key]
		[Required(ErrorMessage = "Id is required")]
		public int Id { get; set; }
		[Required(ErrorMessage = "Name is required")]
		[Length(5, 20, ErrorMessage = "Length must between 5 to 20")]
		public string Name { get; set; }
		public DateTime CreatTime { get; set; } = DateTime.Now;
	    
		
		[Required(ErrorMessage = "Id of Main Category is required")]
		[BindNever]
		[ForeignKey(nameof(main_Category))]
		public int MainCategoryId { get; set; }
		[BindNever]
		public MainCategory? main_Category { get; set; }
		[BindNever]
		public List<Tasks>? Tasks { get; set; }=new List<Tasks>();	
	}

}
