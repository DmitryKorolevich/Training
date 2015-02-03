using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System.IO;
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

            #region LocalizationItems

            builder.Entity<LocalizationItem>().Key(p => new {p.GroupId, p.ItemId});
            builder.Entity<LocalizationItemData>().Key(p => new { p.GroupId, p.ItemId,p.CultureId });
            builder.Entity<LocalizationItem>().HasMany(p => p.LocalizationItemDatas).WithOne(p => p.LocalizationItem).ForeignKey(p => new { p.GroupId, p.ItemId })
                .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItem>().Ignore(x => x.Id);
            builder.Entity<LocalizationItem>().Property(x => x.GroupName).MaxLength(250).Required();
            builder.Entity<LocalizationItem>().Property(x => x.ItemName).MaxLength(250).Required();
            builder.Entity<LocalizationItem>().Property(x => x.Comment).MaxLength(250).Required();
            builder.Entity<LocalizationItemData>().HasOne(p => p.LocalizationItem).WithMany(p => p.LocalizationItemDatas).ForeignKey(p => new { p.GroupId, p.ItemId })
                .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().Ignore(x => x.Id);
            builder.Entity<LocalizationItemData>().Property(x => x.CultureId).MaxLength(5).Required();
            builder.Entity<LocalizationItemData>().Property(x => x.Value).MaxLength(1000).Required();

            #endregion

            builder.Entity<Comment>().HasOne(x => x.Author).WithMany(y => y.Comments).ForeignKey(x => x.AuthorId).ReferencedKey(y => y.Id);
            //builder.Entity<Comment>().Ignore(x => x.ObjectState);

            base.OnModelCreating(builder);
		}
	}
}
