using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_List.Areas.UserArea.Models
{
	public class MainCategory
	{
		[Key]
		[Required(ErrorMessage ="Id is required")]
		public int Id { get; set; }
		[Required(ErrorMessage ="Name is required")]
		[Length(5,20,ErrorMessage ="Length must between 5 to 20")]
		public string Name { get; set; }
		public DateTime CreatTime { get; set; } = DateTime.Now;
		public List<SubCategory>? SubCategory { get; set; }=new List<SubCategory>();
		[ForeignKey(nameof(User))]
		public string? UserId { get; set; }
		public IdentityUser? User { get; set; } 
	}
}
