using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Localization;

namespace VitalChoice.Infrastructure.Context
{
	public class VitalChoiceContext : DataContext
	{
		private static bool created;

		public DbSet<Comment> Comments { get; set; }

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

	    public VitalChoiceContext(bool uofScoped = false) : this()
	    {
	        
	    }


        protected override void OnConfiguring(DbContextOptions builder)
		{
			builder.UseSqlServer("Server=localhost;Database=VitalChoice;Integrated security=True;");
			//builder.UseSqlServer();

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            //builder.Entity<ApplicationUser>().Ignore(x => x.ObjectState);

            builder.Entity<LocalizationItem>().HasMany(p => p.LocalizationItemDatas).WithOne(p => p.LocalizationItem).ForeignKey(p => new { p.GroupId, p.ItemId })
                .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().HasOne(p => p.LocalizationItem).WithMany(p => p.LocalizationItemDatas).ForeignKey(p => new { p.GroupId, p.ItemId })
    .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<Comment>().HasOne(x => x.Author).WithMany(y => y.Comments).ForeignKey(x => x.AuthorId).ReferencedKey(y => y.Id);
            //builder.Entity<Comment>().Ignore(x => x.ObjectState);

            base.OnModelCreating(builder);
		}
	}
}
