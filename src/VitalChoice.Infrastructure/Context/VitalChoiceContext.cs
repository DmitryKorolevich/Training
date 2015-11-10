using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using System.Data.SqlClient;
using System.IO;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Extensions.OptionsModel;
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
            //if (!created)
            //{
            //	//Database.AsRelational().AsSqlServer();//.EnsureCreated();//ApplyMigration()//.AsMigrationsEnabled()
            //             created = true;
            //}
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
            #region Identity

            base.OnModelCreating(builder);

            #endregion

            #region LocalizationItems

            builder.Entity<LocalizationItem>().HasKey(p => new {p.GroupId, p.ItemId});
            builder.Entity<LocalizationItemData>().HasKey(p => new {p.GroupId, p.ItemId, p.CultureId});
            builder.Entity<LocalizationItem>()
                .HasMany(p => p.LocalizationItemDatas)
                .WithOne(p => p.LocalizationItem)
                .HasForeignKey(p => new {p.GroupId, p.ItemId})
                .HasPrincipalKey(p => new {p.GroupId, p.ItemId});
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
            builder.Entity<ContentProcessorEntity>().HasKey(p => p.Id);
            builder.Entity<ContentProcessorEntity>().ToTable("ContentProcessors");

            builder.Entity<MasterContentItem>().HasKey(p => p.Id);
            builder.Entity<MasterContentItem>().ToTable("MasterContentItems");
            builder.Entity<MasterContentItem>()
                .HasOne(p => p.Type)
                .WithMany()
                .HasForeignKey(p => p.TypeId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<MasterContentItem>()
                .HasMany(p => p.MasterContentItemToContentProcessors)
                .WithOne(p => p.MasterContentItem)
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentProcessorEntity>()
                .HasMany(p => p.MasterContentItemsToContentProcessors)
                .WithOne(p => p.ContentProcessor)
                .HasForeignKey(p => p.ContentProcessorId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<MasterContentItem>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .HasPrincipalKey(p => p.Id);

            builder.Entity<ContentItem>().HasKey(p => p.Id);
            builder.Entity<ContentItem>().ToTable("ContentItems");
            builder.Entity<ContentItem>()
                .HasMany(p => p.ContentItemToContentProcessors)
                .WithOne(p => p.ContentItem)
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentProcessorEntity>()
                .HasMany(p => p.ContentItemsToContentProcessors)
                .WithOne(p => p.ContentProcessor)
                .HasForeignKey(p => p.ContentItemProcessorId)
                .HasPrincipalKey(p => p.Id);

            builder.Entity<ContentCategory>().HasKey(p => p.Id);
            builder.Entity<ContentCategory>().ToTable("ContentCategories");
            builder.Entity<ContentCategory>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentCategory>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);

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
                .HasForeignKey(p => p.RecipeId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>()
                .HasMany(p => p.RelatedRecipes)
                .WithOne()
                .HasForeignKey(p => p.IdRecipe)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>()
                .HasMany(p => p.CrossSells)
                .WithOne()
                .HasForeignKey(p => p.IdRecipe)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Recipe>()
                .HasMany(p => p.RecipesToProducts)
                .WithOne()
                .HasForeignKey(t => t.IdRecipe)
                .HasPrincipalKey(p => p.Id)
                .IsRequired();

            builder.Entity<RecipeDefaultSetting>().HasKey(p => p.Id);
            builder.Entity<RecipeDefaultSetting>().ToTable("RecipeDefaultSettings");

            builder.Entity<FAQ>().HasKey(p => p.Id);
            builder.Entity<FAQ>().ToTable("FAQs");
            builder.Entity<FAQToContentCategory>().HasKey(p => p.Id);
            builder.Entity<FAQToContentCategory>().ToTable("FAQsToContentCategories");
            builder.Entity<FAQ>()
                .HasMany(p => p.FAQsToContentCategories)
                .WithOne(p => p.FAQ)
                .HasForeignKey(p => p.FAQId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<FAQ>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<FAQ>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<FAQ>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);

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
                .HasForeignKey(p => p.ArticleId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Article>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Article>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<Article>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
            builder.Entity<Article>()
                .HasMany(p => p.ArticlesToProducts)
                .WithOne()
                .HasForeignKey(t => t.IdArticle)
                .HasPrincipalKey(p => p.Id)
                .IsRequired();

            builder.Entity<ContentPage>().HasKey(p => p.Id);
            builder.Entity<ContentPage>().ToTable("ContentPages");
            builder.Entity<ContentPageToContentCategory>().HasKey(p => p.Id);
            builder.Entity<ContentPageToContentCategory>().ToTable("ContentPagesToContentCategories");
            builder.Entity<ContentPage>()
                .HasMany(p => p.ContentPagesToContentCategories)
                .WithOne(p => p.ContentPage)
                .HasForeignKey(p => p.ContentPageId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentPage>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentPage>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ContentPage>().HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);

            builder.Entity<ContentArea>().HasKey(p => p.Id);
            builder.Entity<ContentArea>().ToTable("ContentAreas");
            builder.Entity<ContentArea>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.IdEditedBy)
                .HasPrincipalKey(p => p.Id);

            builder.Entity<CustomPublicStyle>().HasKey(p => p.Id);
            builder.Entity<CustomPublicStyle>().ToTable("CustomPublicStyles");
            builder.Entity<CustomPublicStyle>()
                .HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.IdEditedBy)
                .HasPrincipalKey(p => p.Id);

            #endregion

            #region Products

            builder.Entity<ProductCategoryContent>().HasKey(p => p.Id);
            builder.Entity<ProductCategoryContent>().ToTable("ProductCategories");
            builder.Entity<ProductCategoryContent>().Ignore(p => p.Name);
            builder.Entity<ProductCategoryContent>().Ignore(p => p.UserId);
            builder.Entity<ProductCategoryContent>().Ignore(p => p.User);
            builder.Entity<ProductCategoryContent>().Ignore(p => p.ProductCategory);
            builder.Entity<ProductCategoryContent>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ProductCategoryContent>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);

            #endregion

	        #region Users
            builder.Entity<ProductContent>().HasKey(p => p.Id);
            builder.Entity<ProductContent>().ToTable("Products");
            builder.Entity<ProductContent>().Ignore(p => p.Name);
            builder.Entity<ProductContent>().Ignore(p => p.UserId);
            builder.Entity<ProductContent>().Ignore(p => p.User);
            builder.Entity<ProductContent>()
                .HasOne(p => p.MasterContentItem)
                .WithMany()
                .HasForeignKey(p => p.MasterContentItemId)
                .HasPrincipalKey(p => p.Id);
            builder.Entity<ProductContent>()
                .HasOne(p => p.ContentItem)
                .WithMany()
                .HasForeignKey(p => p.ContentItemId)
                .HasPrincipalKey(p => p.Id);

            builder.Entity<AdminProfile>().HasKey(x => x.Id);
            builder.Entity<AdminProfile>().ToTable("AdminProfiles");
            builder.Entity<AdminProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.Profile)
                .HasForeignKey<AdminProfile>(a => a.Id)
                .HasPrincipalKey<ApplicationUser>(x => x.Id);

            builder.Entity<AdminProfile>().HasKey(x => x.Id);
            builder.Entity<AdminProfile>().ToTable("AdminProfiles");
            builder.Entity<AdminProfile>()
                .HasOne(x => x.User)
                .WithOne(x => x.Profile)
                .HasForeignKey<AdminProfile>(a => a.Id)
                .HasPrincipalKey<ApplicationUser>(x => x.Id);

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
                    .HasForeignKey(p => p.IdBugTicket)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
                entity.HasMany(p => p.Files)
                    .WithOne()
                    .HasForeignKey(p => p.IdBugTicket)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
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
                    .HasForeignKey(p => p.IdBugTicketComment)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
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
                    .HasForeignKey(p => p.ZoneId)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired(false);
            });
        }

        #endregion
    }
}
