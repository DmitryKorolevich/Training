using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
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
using VitalChoice.Domain.Entities.Help;
using VitalChoice.Domain.Entities.VitalGreen;

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
                DataSource = _options.Value.Connection.Server,
                // TODO: Currently nested queries are run while processing the results of outer queries
                // This either requires MARS or creation of a new connection for each query. Currently using
                // MARS since cloning connections is known to be problematic.
                MultipleActiveResultSets = true,
                InitialCatalog = "VitalChoice.Infrastructure",
                UserID = _options.Value.Connection.UserName,
                Password = _options.Value.Connection.Password,
                ConnectTimeout = 60
            }).ConnectionString;
            builder.UseSqlServer(connectionString);

			base.OnConfiguring(builder);
		}

	    protected override void OnModelCreating(ModelBuilder builder)
	    {
	        #region LocalizationItems

	        builder.Entity<LocalizationItem>().HasKey(p => new {p.GroupId, p.ItemId});
	        builder.Entity<LocalizationItemData>().HasKey(p => new {p.GroupId, p.ItemId, p.CultureId});
	        builder.Entity<LocalizationItem>()
	            .HasMany(p => p.LocalizationItemDatas)
	            .WithOne(p => p.LocalizationItem)
	            .ForeignKey(p => new {p.GroupId, p.ItemId})
	            .
	            PrincipalKey(p => new {p.GroupId, p.ItemId});
	        builder.Entity<LocalizationItem>().Ignore(x => x.Id);
	        builder.Entity<LocalizationItemData>().Ignore(x => x.Id);

	        #endregion

	        #region Contents

	        builder.Entity<ContentTypeEntity>().HasKey(p => p.Id);
	        builder.Entity<ContentTypeEntity>().ToTable("ContentTypes");

	        builder.Entity<ContentItemToContentProcessor>().HasKey(p => p.Id);
	        builder.Entity<ContentItemToContentProcessor>().ToTable("ContentItemsToContentProcessors");
	        builder.Entity<MasterContentItemToContentProcessor>().HasKey(p => p.Id);
	        builder.Entity<MasterContentItemToContentProcessor>().ToTable("MasterContentItemsToContentProcessors");
	        builder.Entity<ContentProcessor>().HasKey(p => p.Id);
	        builder.Entity<ContentProcessor>().ToTable("ContentProcessors");

	        builder.Entity<MasterContentItem>().HasKey(p => p.Id);
	        builder.Entity<MasterContentItem>().ToTable("MasterContentItems");
	        builder.Entity<MasterContentItem>()
	            .HasOne(p => p.Type)
	            .WithMany()
	            .ForeignKey(p => p.TypeId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<MasterContentItem>()
	            .HasMany(p => p.MasterContentItemToContentProcessors)
	            .WithOne(p => p.MasterContentItem)
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ContentProcessor>()
	            .HasMany(p => p.MasterContentItemsToContentProcessors)
	            .WithOne(p => p.ContentProcessor)
	            .ForeignKey(p => p.ContentProcessorId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<MasterContentItem>()
	            .HasOne(p => p.User)
	            .WithMany()
	            .ForeignKey(p => p.UserId)
	            .PrincipalKey(p => p.Id);

	        builder.Entity<ContentItem>().HasKey(p => p.Id);
	        builder.Entity<ContentItem>().ToTable("ContentItems");
	        builder.Entity<ContentItem>()
	            .HasMany(p => p.ContentItemToContentProcessors)
	            .WithOne(p => p.ContentItem)
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ContentProcessor>()
	            .HasMany(p => p.ContentItemsToContentProcessors)
	            .WithOne(p => p.ContentProcessor)
	            .ForeignKey(p => p.ContentItemProcessorId)
	            .PrincipalKey(p => p.Id);

	        builder.Entity<ContentCategory>().HasKey(p => p.Id);
	        builder.Entity<ContentCategory>().ToTable("ContentCategories");
	        builder.Entity<ContentCategory>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .
	            PrincipalKey(p => p.Id);
	        builder.Entity<ContentCategory>().HasOne(p => p.ContentItem).WithMany().ForeignKey(p => p.ContentItemId).
	            PrincipalKey(p => p.Id);

	        builder.Entity<RecipeToProduct>().HasKey(p => p.Id);
	        builder.Entity<RecipeToProduct>().ToTable("RecipesToProducts");
	        builder.Entity<RecipeToProduct>().Ignore(p => p.ShortProductInfo);

	        builder.Entity<RelatedRecipe>().HasKey(p => p.Id);
	        builder.Entity<RelatedRecipe>().ToTable("RelatedRecipes");

	        builder.Entity<RecipeCrossSell>().HasKey(p => p.Id);
	        builder.Entity<RecipeCrossSell>().ToTable("RecipeCrossSells");

	        builder.Entity<Recipe>().HasKey(p => p.Id);
	        builder.Entity<Recipe>().ToTable("Recipes");
	        builder.Entity<RecipeToContentCategory>().HasKey(p => p.Id);
	        builder.Entity<RecipeToContentCategory>().ToTable("RecipesToContentCategories");
	        builder.Entity<Recipe>()
	            .HasMany(p => p.RecipesToContentCategories)
	            .WithOne(p => p.Recipe)
	            .ForeignKey(p => p.RecipeId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>()
	            .HasOne(p => p.ContentItem)
	            .WithMany()
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>().HasOne(p => p.User).WithMany().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>()
	            .HasMany(p => p.RelatedRecipes)
	            .WithOne()
	            .ForeignKey(p => p.IdRecipe)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>()
	            .HasMany(p => p.CrossSells)
	            .WithOne()
	            .ForeignKey(p => p.IdRecipe)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Recipe>()
	            .HasMany(p => p.RecipesToProducts)
	            .WithOne()
	            .ForeignKey(t => t.IdRecipe)
	            .PrincipalKey(p => p.Id)
	            .Required();

	        builder.Entity<RecipeDefaultSetting>().HasKey(p => p.Id);
	        builder.Entity<RecipeDefaultSetting>().ToTable("RecipeDefaultSettings");

	        builder.Entity<FAQ>().HasKey(p => p.Id);
	        builder.Entity<FAQ>().ToTable("FAQs");
	        builder.Entity<FAQToContentCategory>().HasKey(p => p.Id);
	        builder.Entity<FAQToContentCategory>().ToTable("FAQsToContentCategories");
	        builder.Entity<FAQ>()
	            .HasMany(p => p.FAQsToContentCategories)
	            .WithOne(p => p.FAQ)
	            .ForeignKey(p => p.FAQId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<FAQ>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<FAQ>()
	            .HasOne(p => p.ContentItem)
	            .WithMany()
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<FAQ>().HasOne(p => p.User).WithMany().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);

	        builder.Entity<ArticleToProduct>().HasKey(p => p.Id);
	        builder.Entity<ArticleToProduct>().ToTable("ArticlesToProducts");
	        builder.Entity<ArticleToProduct>().Ignore(p => p.ShortProductInfo);

	        builder.Entity<Article>().HasKey(p => p.Id);
	        builder.Entity<Article>().ToTable("Articles");
	        builder.Entity<ArticleToContentCategory>().HasKey(p => p.Id);
	        builder.Entity<ArticleToContentCategory>().ToTable("ArticlesToContentCategories");
	        builder.Entity<Article>()
	            .HasMany(p => p.ArticlesToContentCategories)
	            .WithOne(p => p.Article)
	            .ForeignKey(p => p.ArticleId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Article>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Article>()
	            .HasOne(p => p.ContentItem)
	            .WithMany()
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<Article>().HasOne(p => p.User).WithMany().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);
	        builder.Entity<Article>()
	            .HasMany(p => p.ArticlesToProducts)
	            .WithOne()
	            .ForeignKey(t => t.IdArticle)
	            .PrincipalKey(p => p.Id)
	            .Required();

	        builder.Entity<ContentPage>().HasKey(p => p.Id);
	        builder.Entity<ContentPage>().ToTable("ContentPages");
	        builder.Entity<ContentPageToContentCategory>().HasKey(p => p.Id);
	        builder.Entity<ContentPageToContentCategory>().ToTable("ContentPagesToContentCategories");
	        builder.Entity<ContentPage>()
	            .HasMany(p => p.ContentPagesToContentCategories)
	            .WithOne(p => p.ContentPage)
	            .ForeignKey(p => p.ContentPageId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ContentPage>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ContentPage>()
	            .HasOne(p => p.ContentItem)
	            .WithMany()
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ContentPage>().HasOne(p => p.User).WithMany().ForeignKey(p => p.UserId).PrincipalKey(p => p.Id);

	        builder.Entity<ContentArea>().HasKey(p => p.Id);
	        builder.Entity<ContentArea>().ToTable("ContentAreas");
	        builder.Entity<ContentArea>()
	            .HasOne(p => p.User)
	            .WithMany()
	            .ForeignKey(p => p.IdEditedBy)
	            .PrincipalKey(p => p.Id);

	        builder.Entity<CustomPublicStyle>().HasKey(p => p.Id);
	        builder.Entity<CustomPublicStyle>().ToTable("CustomPublicStyles");
	        builder.Entity<CustomPublicStyle>()
	            .HasOne(p => p.User)
	            .WithMany()
	            .ForeignKey(p => p.IdEditedBy)
	            .PrincipalKey(p => p.Id);

	        #endregion

	        #region Products

	        builder.Entity<ProductCategoryContent>().HasKey(p => p.Id);
	        builder.Entity<ProductCategoryContent>().ToTable("ProductCategories");
	        builder.Entity<ProductCategoryContent>()
	            .HasOne(p => p.MasterContentItem)
	            .WithMany()
	            .ForeignKey(p => p.MasterContentItemId)
	            .PrincipalKey(p => p.Id);
	        builder.Entity<ProductCategoryContent>()
	            .HasOne(p => p.ContentItem)
	            .WithMany()
	            .ForeignKey(p => p.ContentItemId)
	            .PrincipalKey(p => p.Id);

	        #endregion

	        #region Users

	        builder.Entity<AdminProfile>().HasKey(x => x.Id);
	        builder.Entity<AdminProfile>().ToTable("AdminProfiles");
	        builder.Entity<AdminProfile>()
	            .HasOne(x => x.User)
	            .WithOne(x => x.Profile)
	            .ForeignKey<AdminProfile>(a => a.Id)
	            .PrincipalKey<ApplicationUser>(x => x.Id);

	        #endregion

	        #region Settings

	        builder.Entity<Country>().HasKey(p => p.Id);
	        builder.Entity<Country>().ToTable("Countries");
	        builder.Entity<Country>().Ignore(p => p.States);

	        builder.Entity<State>().HasKey(p => p.Id);
	        builder.Entity<State>().ToTable("States");

	        builder.Entity<AppSettingItem>().HasKey(p => p.Id);
	        builder.Entity<AppSettingItem>().ToTable("AppSettings");

	        #endregion

	        #region Help

	        builder.Entity<BugTicket>(entity =>
	        {
	            entity.HasKey(t => t.Id);
	            entity.ToTable("BugTickets");
	            entity.HasMany(p => p.Comments)
	                .WithOne(p => p.BugTicket)
	                .ForeignKey(p => p.IdBugTicket)
	                .PrincipalKey(p => p.Id)
	                .Required();
	            entity.HasMany(p => p.Files)
	                .WithOne()
	                .ForeignKey(p => p.IdBugTicket)
	                .PrincipalKey(p => p.Id)
	                .Required(false);
	            entity.Ignore(p => p.AddedBy);
	            entity.Ignore(p => p.AddedByEmail);
	            entity.Ignore(p => p.AddedByAgent);
	        });

	        builder.Entity<BugTicketComment>(entity =>
	        {
	            entity.HasKey(t => t.Id);
	            entity.ToTable("BugTicketComments");
	            entity.HasMany(p => p.Files)
	                .WithOne()
	                .ForeignKey(p => p.IdBugTicketComment)
	                .PrincipalKey(p => p.Id)
	                .Required(false);
	            entity.Ignore(p => p.EditedBy);
	            entity.Ignore(p => p.EditedByAgent);
	        });

	        builder.Entity<BugFile>(entity =>
	        {
	            entity.HasKey(t => t.Id);
	            entity.ToTable("BugFiles");
	        });

	        #endregion

	        #region VitalGreen

	        builder.Entity<FedExZone>(entity =>
	        {
	            entity.HasKey(t => t.Id);
	            entity.ToTable("FedExZones");
	        });

	        builder.Entity<VitalGreenRequest>(entity =>
	        {
	            entity.HasKey(t => t.Id);
	            entity.ToTable("VitalGreenRequests");
	            entity.HasOne(p => p.Zone)
	                .WithMany()
	                .ForeignKey(p => p.ZoneId)
	                .PrincipalKey(p => p.Id)
	                .Required(false);
	        });

	        #endregion

	        base.OnModelCreating(builder);
	    }
    }
}
