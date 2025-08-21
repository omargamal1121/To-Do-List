using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.General;
using System.Reflection.Emit;
using To_Do_List.Areas.UserArea.Models;

namespace To_Do_List.Areas.UserArea.Data
{
	public class Context : IdentityDbContext<IdentityUser>
	{
		public  DbSet<MainCategory> MainCategory { get; set; }
		public  DbSet<SubCategory> SubCategories { get; set; }
		public  DbSet<Tasks> Tasks { get; set; }
		public Context(DbContextOptions contextOptions):base(contextOptions) { }

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);
			builder.Entity<IdentityUser>()
		   .HasIndex(u => u.Email)
		   .IsUnique();

			builder.Entity<MainCategory>().HasOne(x => x.User).
				WithMany().
				HasForeignKey(x => x.UserId);

			builder.Entity<MainCategory>()
				.HasMany(mc => mc.SubCategory) 
				.WithOne(sc => sc.main_Category) 
				.HasForeignKey(sc => sc.MainCategoryId)
				.OnDelete(DeleteBehavior.Cascade);


			builder.Entity<SubCategory>()
				.HasMany(sc => sc.Tasks)
				.WithOne(t => t.SubCategory)
				.HasForeignKey(t => t.SubCategoryId)
				.OnDelete(DeleteBehavior.Cascade);

			builder.Entity<IdentityUser>()
	.Property<string>("ConfirmationCode")
	.HasColumnName("Code")
	.HasColumnType("char(5)")
	.HasMaxLength(5);
			
		}

	}
}
