using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace To_Do_List.Areas.UserArea.Models
{
	public class Tasks : IValidatableObject
	{
		[Key]
		[Required(ErrorMessage = "Id is required")]
		public int Id { get; set; }

		[Required(ErrorMessage = "Name is required")]
		[Length(5, 20, ErrorMessage = "Name length must be between 5 to 20")]
		public string Name { get; set; }
		public DateTime CreatTime { get; set; } = DateTime.Now;

		[Required(ErrorMessage = "Start time is required")]
		public DateTime StartTime { get; set; }

		[Required(ErrorMessage = "End time is required")]
		public DateTime EndTime { get; set; }

		[Range(1, 3)]
		public PriorityLevel Priority { get; set; } = PriorityLevel.Low;

		public bool IsComplete { get; set; } = false;

		public string Description { get; set; }

		[Required(ErrorMessage = "SubCategoryId is required")]
		[BindNever]
		[ForeignKey(nameof(SubCategory))]

		public int SubCategoryId { get; set; }
		[BindNever]


		public SubCategory? SubCategory { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if (StartTime < DateTime.Now)
			{
				yield return new ValidationResult(
					$"Start date must be after: {DateTime.Now.ToString("MM/dd/yyyy hh:mm tt")}",
					new[] { nameof(StartTime) });
			}

			if (EndTime < StartTime)
			{
				yield return new ValidationResult(
					$"End date must be after: {StartTime.ToString("MM/dd/yyyy hh:mm tt")}",
					new[] { nameof(EndTime) });
			}
		}
		public enum PriorityLevel
		{
			Low = 1,
			Medium = 2,
			High = 3
		}
	}
}