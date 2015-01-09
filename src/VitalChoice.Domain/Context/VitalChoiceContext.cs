using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Domain.Context
{
	public class VitalChoiceContext : DataContext
	{
		private static bool created;

		public VitalChoiceContext()
		{
			// Create the database and schema if it doesn't exist
			// This is a temporary workaround to create database until Entity Framework database migrations 
			// are supported in ASP.NET 5
			if (!created)
			{
				Database.AsRelational().AsSqlServer().ApplyMigrations();
				created = true;
			}
		}

		protected override void OnConfiguring(DbContextOptions builder)
		{
			builder.UseSqlServer("Server=localhost;Database=VitalChoice;Integrated security=True;");

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			/*builder.Entity<User>();*/
			/*OneToMany(b => b.Posts, p => p.Blog)
				.ForeignKey(p => p.BlogId)*/

			base.OnModelCreating(builder);
		}
	}
}
