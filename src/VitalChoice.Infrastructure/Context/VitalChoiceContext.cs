﻿using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System.Data.SqlClient;
using System.IO;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
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
                Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
                created = true;
			}
		}

	    public VitalChoiceContext(bool uofScoped = false) : this()
	    {
	        
	    }       

        protected override void OnConfiguring(DbContextOptions builder)
		{
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = @"localhost",
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Infrastructure",
                IntegratedSecurity = true,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            ////builder.Entity<ApplicationUser>().Ignore(x => x.ObjectState);

            #region LocalizationItems

            builder.Entity<LocalizationItem>().Key(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().Key(p => new { p.GroupId, p.ItemId, p.CultureId });
            builder.Entity<LocalizationItem>().HasMany(p => p.LocalizationItemDatas).WithOne(p => p.LocalizationItem).ForeignKey(p => new { p.GroupId, p.ItemId })
                .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItem>().Ignore(x => x.Id);
            //builder.Entity<LocalizationItemData>().HasOne(p => p.LocalizationItem).WithMany(p => p.LocalizationItemDatas).ForeignKey(p => new { p.GroupId, p.ItemId })
            //    .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().Ignore(x => x.Id);

            #endregion

            #region Contents

            builder.Entity<MasterContentItem>().ToTable("MasterContentItems").Key(p => p.Id);
            builder.Entity<ContentCategory>().ToTable("ContentCategories").Key(p => p.Id);
            builder.Entity<ContentCategory>().HasOne(p => p.MasterContentItem).WithMany(p=>p.ContentCategories).ForeignKey(p=>p.MasterContentItemId).ReferencedKey(p=>p.Id);

            builder.Entity<ContentItemToContentItemProcessor>().ToTable("ContentItemsToContentProcessors").Key(p => p.Id);
            builder.Entity<ContentItem>().ToTable("ContentItems").Key(p => p.Id);
            builder.Entity<ContentItemProcessor>().ToTable("ContentItemProcessors").Key(p => p.Id);
            builder.Entity<ContentItem>().HasMany(p => p.ContentItemsToContentItemProcessors).WithOne(p => p.ContentItem).ForeignKey(p=>p.ContentItemId).ReferencedKey(p=>p.Id);
            builder.Entity<ContentItemProcessor>().HasMany(p => p.ContentItemsToContentItemProcessors).WithOne(p => p.ContentItemProcessor).ForeignKey(p => p.ContentItemProcessorId).ReferencedKey(p => p.Id);

            builder.Entity<Recipe>().ToTable("Recipes").Key(p => p.Id);
            builder.Entity<Recipe>().HasMany(p => p.RecipesToContentCategories).WithOne(p => p.Recipe).ForeignKey(p => p.RecipeId).ReferencedKey(p => p.Id);
            builder.Entity<Recipe>().HasOne(p => p.MasterContentItem).WithMany(p => p.Recipes).ForeignKey(p => p.MasterContentItemId).ReferencedKey(p => p.Id);
            builder.Entity<RecipeToContentCategory>().ToTable("RecipesToContentCategories").Key(p => p.Id);


            #endregion

            builder.Entity<Comment>().HasOne(x => x.Author).WithMany(y => y.Comments).ForeignKey(x => x.AuthorId).ReferencedKey(y => y.Id);

            base.OnModelCreating(builder);
		}
	}
}
