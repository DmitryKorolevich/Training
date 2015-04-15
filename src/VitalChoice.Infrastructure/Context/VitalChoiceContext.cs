using Microsoft.Data.Entity;
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
	public class VitalChoiceContext : IdentityDataContext
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

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
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
            builder.Entity<LocalizationItem>().Collection(p => p.LocalizationItemDatas).InverseReference(p => p.LocalizationItem).ForeignKey(p => new { p.GroupId, p.ItemId }).
                PrincipalKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItem>().Ignore(x => x.Id);
            //builder.Entity<LocalizationItemData>().HasOne(p => p.LocalizationItem).WithMany(p => p.LocalizationItemDatas).ForeignKey(p => new { p.GroupId, p.ItemId })
            //    .ReferencedKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().Ignore(x => x.Id);

            #endregion

            #region Contents
            
            builder.Entity<ContentTypeEntity>().ToTable("ContentTypes").Key(p => p.Id);

            builder.Entity<ContentItemToContentProcessor>().ToTable("ContentItemsToContentProcessors").Key(p => p.Id);
            builder.Entity<MasterContentItemToContentProcessor>().ToTable("MasterContentItemsToContentProcessors").Key(p => p.Id);
            builder.Entity<ContentProcessor>().ToTable("ContentProcessors").Key(p => p.Id);

            builder.Entity<MasterContentItem>().ToTable("MasterContentItems").Key(p => p.Id);
            //builder.Entity<MasterContentItem>().Property(p => p.Created).StoreGeneratedPattern(StoreGeneratedPattern.Computed);
            //builder.Entity<MasterContentItem>().Property(p => p.Updated).StoreGeneratedPattern(StoreGeneratedPattern.Computed);
            builder.Entity<MasterContentItem>().Reference(p => p.Type).InverseCollection().ForeignKey(p => p.TypeId).PrincipalKey(p => p.Id);
            builder.Entity<MasterContentItem>().Collection(p => p.MasterContentItemToContentProcessors).InverseReference(p => p.MasterContentItem).ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentProcessor>().Collection(p => p.MasterContentItemsToContentProcessors).InverseReference(p => p.ContentProcessor).ForeignKey(p => p.ContentProcessorId).PrincipalKey(p => p.Id);

            builder.Entity<ContentItem>().ToTable("ContentItems").Key(p => p.Id);
            //builder.Entity<ContentItem>().Property(p => p.Created).StoreGeneratedPattern(StoreGeneratedPattern.Computed);
            //builder.Entity<ContentItem>().Property(p => p.Updated).StoreGeneratedPattern(StoreGeneratedPattern.Computed);
            builder.Entity<ContentItem>().Collection(p => p.ContentItemToContentProcessors).InverseReference(p => p.ContentItem).ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentProcessor>().Collection(p => p.ContentItemsToContentProcessors).InverseReference(p => p.ContentProcessor).ForeignKey(p => p.ContentProcessorId).PrincipalKey(p => p.Id);

            builder.Entity<ContentCategory>().ToTable("ContentCategories").Key(p => p.Id);
            builder.Entity<ContentCategory>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p=>p.MasterContentItemId).
                PrincipalKey(p=>p.Id);
            builder.Entity<ContentCategory>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).
                PrincipalKey(p => p.Id);

            builder.Entity<Recipe>().ToTable("Recipes").Key(p => p.Id);
            builder.Entity<RecipeToContentCategory>().ToTable("RecipesToContentCategories").Key(p => p.Id);
            builder.Entity<Recipe>().Collection(p => p.RecipesToContentCategories).InverseReference(p => p.Recipe).ForeignKey(p => p.RecipeId).PrincipalKey(p => p.Id);
            builder.Entity<Recipe>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Recipe>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);

            builder.Entity<FAQ>().ToTable("FAQs").Key(p => p.Id);
            builder.Entity<FAQToContentCategory>().ToTable("FAQsToContentCategories").Key(p => p.Id);
            builder.Entity<FAQ>().Collection(p => p.FAQsToContentCategories).InverseReference(p => p.FAQ).ForeignKey(p => p.FAQId).PrincipalKey(p => p.Id);
            builder.Entity<FAQ>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<FAQ>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);

            builder.Entity<Article>().ToTable("Articles").Key(p => p.Id);
            builder.Entity<ArticleToContentCategory>().ToTable("ArticlesToContentCategories").Key(p => p.Id);
            builder.Entity<Article>().Collection(p => p.ArticlesToContentCategories).InverseReference(p => p.Article).ForeignKey(p => p.ArticleId).PrincipalKey(p => p.Id);
            builder.Entity<Article>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Article>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);

            #endregion

            base.OnModelCreating(builder);
		}
	}
}
