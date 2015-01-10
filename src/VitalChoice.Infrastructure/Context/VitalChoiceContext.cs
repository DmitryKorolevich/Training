using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;

namespace VitalChoice.Infrastructure.Context
{
	public class VitalChoiceContext : DataContext
	{
		private static bool created;

		public DbSet<Comment> Blogs { get; set; }

		public VitalChoiceContext()
		{
			// Create the database and schema if it doesn't exist
			// This is a temporary workaround to create database until Entity Framework database migrations 
			// are supported in ASP.NET 5
			if (!created)
			{
				Database.AsRelational().AsSqlServer().EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
				created = true;
			}
		}

		protected override void OnConfiguring(DbContextOptions builder)
		{
			//builder.UseSqlServer("Server=localhost;Database=VitalChoice;Integrated security=True;");
			builder.UseSqlServer();

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            builder.Entity<ApplicationUser>().Ignore(x => x.ObjectState);

			builder.Entity<Comment>().ManyToOne(x => x.Author, y => y.Comments).ForeignKey(x=>x.AuthorId).ReferencedKey(y => y.Id);
			builder.Entity<Comment>().Ignore(x => x.ObjectState);

			base.OnModelCreating(builder);
		}
	}
}
