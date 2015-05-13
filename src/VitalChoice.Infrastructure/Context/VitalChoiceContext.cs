using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Metadata;
using System.Data.SqlClient;
using System.IO;
using Microsoft.Framework.OptionsModel;
using VitalChoice.Data.DataContext;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Entities.Localization;
using VitalChoice.Domain.Entities.Options;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Entities.Roles;
using VitalChoice.Domain.Entities.Users;
using VitalChoice.Domain.Entities.Settings;

namespace VitalChoice.Infrastructure.Context
{
	public class VitalChoiceContext : IdentityDataContext
    {
	    private readonly IOptions<AppOptions> _options;
	    private static bool created;

		public VitalChoiceContext(IOptions<AppOptions> options)
		{
		    _options = options;
		    // Create the database and schema if it doesn't exist
			// This is a temporary workaround to create database until Entity Framework database migrations 
			// are supported in ASP.NET 5
			if (!created)
			{
				//Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
                created = true;
			}
		}

	    public VitalChoiceContext(IOptions<AppOptions> options, bool uofScoped = false) : this(options)
	    {
	        
	    }       

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
		{
            var connectionString = (new SqlConnectionStringBuilder
            {
                DataSource = _options.Options.Connection.Server,
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Infrastructure",
                UserID = _options.Options.Connection.UserName,
                Password = _options.Options.Connection.Password,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
            #region LocalizationItems

            builder.Entity<LocalizationItem>().Key(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItemData>().Key(p => new { p.GroupId, p.ItemId, p.CultureId });
            builder.Entity<LocalizationItem>().Collection(p => p.LocalizationItemDatas).InverseReference(p => p.LocalizationItem).ForeignKey(p => new { p.GroupId, p.ItemId }).
                PrincipalKey(p => new { p.GroupId, p.ItemId });
            builder.Entity<LocalizationItem>().Ignore(x => x.Id);
            builder.Entity<LocalizationItemData>().Ignore(x => x.Id);

            #endregion

            #region Contents

		    builder.Entity<ContentTypeEntity>().Key(p => p.Id);
            builder.Entity<ContentTypeEntity>().ForSqlServer().Table("ContentTypes");

            builder.Entity<ContentItemToContentProcessor>().Key(p => p.Id);
            builder.Entity<ContentItemToContentProcessor>().ForSqlServer().Table("ContentItemsToContentProcessors");
            builder.Entity<MasterContentItemToContentProcessor>().Key(p => p.Id);
            builder.Entity<MasterContentItemToContentProcessor>().ForSqlServer().Table("MasterContentItemsToContentProcessors");
            builder.Entity<ContentProcessor>().Key(p => p.Id);
            builder.Entity<ContentProcessor>().ForSqlServer().Table("ContentProcessors");

            builder.Entity<MasterContentItem>().Key(p => p.Id);
            builder.Entity<MasterContentItem>().ForSqlServer().Table("MasterContentItems");
            builder.Entity<MasterContentItem>().Reference(p => p.Type).InverseCollection().ForeignKey(p => p.TypeId).PrincipalKey(p => p.Id);
            builder.Entity<MasterContentItem>().Collection(p => p.MasterContentItemToContentProcessors).InverseReference(p => p.MasterContentItem).ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentProcessor>().Collection(p => p.MasterContentItemsToContentProcessors).InverseReference(p => p.ContentProcessor).ForeignKey(p => p.ContentProcessorId).PrincipalKey(p => p.Id);
            builder.Entity<MasterContentItem>().Reference(p => p.User).InverseCollection().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);

            builder.Entity<ContentItem>().Key(p => p.Id);
            builder.Entity<ContentItem>().ForSqlServer().Table("ContentItems");
            builder.Entity<ContentItem>().Collection(p => p.ContentItemToContentProcessors).InverseReference(p => p.ContentItem).ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentProcessor>().Collection(p => p.ContentItemsToContentProcessors).InverseReference(p => p.ContentProcessor).ForeignKey(p => p.ContentProcessorId).PrincipalKey(p => p.Id);

            builder.Entity<ContentCategory>().Key(p => p.Id);
            builder.Entity<ContentCategory>().ForSqlServer().Table("ContentCategories");
            builder.Entity<ContentCategory>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).
                PrincipalKey(p => p.Id);
            builder.Entity<ContentCategory>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).
                PrincipalKey(p => p.Id);

            builder.Entity<Recipe>().Key(p => p.Id);
            builder.Entity<Recipe>().ForSqlServer().Table("Recipes");
            builder.Entity<RecipeToContentCategory>().Key(p => p.Id);
            builder.Entity<RecipeToContentCategory>().ForSqlServer().Table("RecipesToContentCategories");
            builder.Entity<Recipe>().Collection(p => p.RecipesToContentCategories).InverseReference(p => p.Recipe).ForeignKey(p => p.RecipeId).PrincipalKey(p => p.Id);
            builder.Entity<Recipe>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Recipe>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Recipe>().Reference(p => p.User).InverseCollection().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);

            builder.Entity<FAQ>().Key(p => p.Id);
            builder.Entity<FAQ>().ForSqlServer().Table("FAQs");
            builder.Entity<FAQToContentCategory>().Key(p => p.Id);
            builder.Entity<FAQToContentCategory>().ForSqlServer().Table("FAQsToContentCategories");
            builder.Entity<FAQ>().Collection(p => p.FAQsToContentCategories).InverseReference(p => p.FAQ).ForeignKey(p => p.FAQId).PrincipalKey(p => p.Id);
            builder.Entity<FAQ>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<FAQ>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<FAQ>().Reference(p => p.User).InverseCollection().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);


            builder.Entity<Article>().Key(p => p.Id);
            builder.Entity<Article>().ForSqlServer().Table("Articles");
            builder.Entity<ArticleToContentCategory>().Key(p => p.Id);
            builder.Entity<ArticleToContentCategory>().ForSqlServer().Table("ArticlesToContentCategories");
            builder.Entity<Article>().Collection(p => p.ArticlesToContentCategories).InverseReference(p => p.Article).ForeignKey(p => p.ArticleId).PrincipalKey(p => p.Id);
            builder.Entity<Article>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Article>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<Article>().Reference(p => p.User).InverseCollection().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);


            builder.Entity<ContentPage>().Key(p => p.Id);
            builder.Entity<ContentPage>().ForSqlServer().Table("ContentPages");
            builder.Entity<ContentPageToContentCategory>().Key(p => p.Id);
            builder.Entity<ContentPageToContentCategory>().ForSqlServer().Table("ContentPagesToContentCategories");
            builder.Entity<ContentPage>().Collection(p => p.ContentPagesToContentCategories).InverseReference(p => p.ContentPage).ForeignKey(p => p.ContentPageId).PrincipalKey(p => p.Id);
            builder.Entity<ContentPage>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentPage>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).PrincipalKey(p => p.Id);
            builder.Entity<ContentPage>().Reference(p => p.User).InverseCollection().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);
            
            #endregion

            #region Products

            builder.Entity<ProductCategoryContent>().Key(p => p.Id);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.Name);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.Url);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.ParentId);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.StatusCode);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.Assigned);
            builder.Entity<ProductCategoryContent>().Ignore(x => x.Order);
            builder.Entity<ProductCategoryContent>().ForRelational().Table("ProductCategories");
            builder.Entity<ProductCategoryContent>().Reference(p => p.MasterContentItem).InverseCollection().ForeignKey(p => p.MasterContentItemId).
                PrincipalKey(p => p.Id);
            builder.Entity<ProductCategoryContent>().Reference(p => p.ContentItem).InverseCollection().ForeignKey(p => p.ContentItemId).
                PrincipalKey(p => p.Id);

            #endregion

            #region Users

            builder.Entity<AdminProfile>().Key(x => x.Id);
			builder.Entity<AdminProfile>().ForSqlServer().Table("AdminProfiles");
			builder.Entity<AdminProfile>().Reference(x => x.User).InverseReference(x => x.Profile).PrincipalKey<ApplicationUser>(x=>x.Id).Required();

            #endregion

            #region Settings

            builder.Entity<Country>().Key(p => p.Id);
            builder.Entity<Country>().ForSqlServer().Table("Countries");
            builder.Entity<Country>().Ignore(p => p.States);

            builder.Entity<State>().Key(p => p.Id);
            builder.Entity<State>().ForSqlServer().Table("States");

            builder.Entity<AppSettingItem>().Key(p => p.Id);
            builder.Entity<AppSettingItem>().ForSqlServer().Table("AppSettings");

            #endregion

            base.OnModelCreating(builder);
		}
	}
}
