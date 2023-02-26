using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RehabRally.Web.Core.Models;

namespace RehabRally.Web.Data
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
			: base(options)
		{

		}
		public DbSet<Category> Categories { get; set; }
		public DbSet<Exercise> Exercises { get; set; }
		public DbSet<PatientExercise> PatientExercises { get; set; }
		public DbSet<RegisterdMashine> RegisterdMashines { get; set; }
		public DbSet<PatientConclusion> PatientConclusions{ get; set; } 

	}
}