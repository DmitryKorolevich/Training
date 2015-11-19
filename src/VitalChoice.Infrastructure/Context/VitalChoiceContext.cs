using Microsoft.Data.Entity;
using System.Data.SqlClient;
using Microsoft.Extensions.OptionsModel;
using VitalChoice.Data.Context;
using VitalChoice.Ecommerce.Domain.Entities.Addresses;
using VitalChoice.Ecommerce.Domain.Options;
using VitalChoice.Infrastructure.Domain.Content;
using VitalChoice.Infrastructure.Domain.Content.Articles;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Content.ContentPages;
using VitalChoice.Infrastructure.Domain.Content.Faq;
using VitalChoice.Infrastructure.Domain.Content.Products;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Entities.Help;
using VitalChoice.Infrastructure.Domain.Entities.Localization;
using VitalChoice.Infrastructure.Domain.Entities.Settings;
using VitalChoice.Infrastructure.Domain.Entities.Users;
using VitalChoice.Infrastructure.Domain.Entities.VitalGreen;

namespace VitalChoice.Infrastructure.Context
{
    public class VitalChoiceContext : IdentityDataContext
    {
        private readonly IOptions<AppOptionsBase> _options;

        public VitalChoiceContext(IOptions<AppOptionsBase> options)
        {
            _options = options;
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

            builder.Entity<LocalizationItem>(entity =>
            {
                entity.HasKey(p => new {p.GroupId, p.ItemId});
            });

            builder.Entity<LocalizationItemData>(entity =>
            {
                entity.HasKey(p => new {p.GroupId, p.ItemId, p.CultureId});
            });

            builder.Entity<LocalizationItem>(entity =>
            {
                entity
                    .HasMany(p => p.LocalizationItemDatas)
                    .WithOne(p => p.LocalizationItem)
                    .HasForeignKey(p => new {p.GroupId, p.ItemId})
                    .HasPrincipalKey(p => new {p.GroupId, p.ItemId});
                entity.Ignore(x => x.Id);
            });

            builder.Entity<LocalizationItemData>(entity =>
            {
                entity.Ignore(x => x.Id);
            });


            #endregion

            #region Contents

            builder.Entity<ContentTypeEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentTypes");
            });


            builder.Entity<ContentItemToContentProcessor>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentItemsToContentProcessors");
            });

            builder.Entity<MasterContentItemToContentProcessor>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("MasterContentItemsToContentProcessors");
            });

            builder.Entity<ContentProcessorEntity>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentProcessors");
            });


            builder.Entity<MasterContentItem>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("MasterContentItems");
                entity
                    .HasOne(p => p.Type)
                    .WithMany()
                    .HasForeignKey(p => p.TypeId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.MasterContentItemToContentProcessors)
                    .WithOne(p => p.MasterContentItem)
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
            });

            builder.Entity<ContentProcessorEntity>(entity =>
            {
                entity
                    .HasMany(p => p.MasterContentItemsToContentProcessors)
                    .WithOne(p => p.ContentProcessor)
                    .HasForeignKey(p => p.ContentProcessorId)
                    .HasPrincipalKey(p => p.Id);
            });

            builder.Entity<MasterContentItem>(entity =>
            {
                entity
                    .HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.UserId)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<ContentItem>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentItems");
                entity
                    .HasMany(p => p.ContentItemToContentProcessors)
                    .WithOne(p => p.ContentItem)
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
            });

            builder.Entity<ContentProcessorEntity>(entity =>
            {
                entity
                    .HasMany(p => p.ContentItemsToContentProcessors)
                    .WithOne(p => p.ContentProcessor)
                    .HasForeignKey(p => p.ContentItemProcessorId)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<ContentCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentCategories");
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<RecipeToProduct>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("RecipesToProducts");
                entity.Ignore(p => p.ShortProductInfo);
            });


            builder.Entity<RelatedRecipe>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("RelatedRecipes");
            });


            builder.Entity<RecipeCrossSell>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("RecipeCrossSells");
            });


            builder.Entity<Recipe>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Recipes");
            });

            builder.Entity<RecipeToContentCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("RecipesToContentCategories");
            });

            builder.Entity<Recipe>(entity =>
            {
                entity
                    .HasMany(p => p.RecipesToContentCategories)
                    .WithOne(p => p.Recipe)
                    .HasForeignKey(p => p.RecipeId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.RelatedRecipes)
                    .WithOne()
                    .HasForeignKey(p => p.IdRecipe)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.CrossSells)
                    .WithOne()
                    .HasForeignKey(p => p.IdRecipe)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.RecipesToProducts)
                    .WithOne()
                    .HasForeignKey(t => t.IdRecipe)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });


            builder.Entity<RecipeDefaultSetting>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("RecipeDefaultSettings");
            });


            builder.Entity<FAQ>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("FAQs");
            });

            builder.Entity<FAQToContentCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("FAQsToContentCategories");
            });

            builder.Entity<FAQ>(entity =>
            {
                entity
                    .HasMany(p => p.FAQsToContentCategories)
                    .WithOne(p => p.FAQ)
                    .HasForeignKey(p => p.FAQId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
            });


            builder.Entity<ArticleToProduct>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ArticlesToProducts");
                entity.Ignore(p => p.ShortProductInfo);
            });


            builder.Entity<Article>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Articles");
            });

            builder.Entity<ArticleToContentCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ArticlesToContentCategories");
            });

            builder.Entity<Article>(entity =>
            {
                entity
                    .HasMany(p => p.ArticlesToContentCategories)
                    .WithOne(p => p.Article)
                    .HasForeignKey(p => p.ArticleId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
                entity
                    .HasMany(p => p.ArticlesToProducts)
                    .WithOne()
                    .HasForeignKey(t => t.IdArticle)
                    .HasPrincipalKey(p => p.Id)
                    .IsRequired();
            });


            builder.Entity<ContentPage>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentPages");
            });

            builder.Entity<ContentPageToContentCategory>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentPagesToContentCategories");
            });

            builder.Entity<ContentPage>(entity =>
            {
                entity
                    .HasMany(p => p.ContentPagesToContentCategories)
                    .WithOne(p => p.ContentPage)
                    .HasForeignKey(p => p.ContentPageId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId).HasPrincipalKey(p => p.Id);
            });


            builder.Entity<ContentArea>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ContentAreas");
                entity
                    .HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<CustomPublicStyle>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("CustomPublicStyles");
                entity
                    .HasOne(p => p.User)
                    .WithMany()
                    .HasForeignKey(p => p.IdEditedBy)
                    .HasPrincipalKey(p => p.Id);
            });


            #endregion

            #region Products

            builder.Entity<ProductCategoryContent>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("ProductCategories");
                entity.Ignore(p => p.Name);
                entity.Ignore(p => p.UserId);
                entity.Ignore(p => p.User);
                entity.Ignore(p => p.ProductCategory);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
            });


            #endregion

            #region Users

            builder.Entity<ProductContent>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Products");
                entity.Ignore(p => p.Name);
                entity.Ignore(p => p.UserId);
                entity.Ignore(p => p.User);
                entity
                    .HasOne(p => p.MasterContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.MasterContentItemId)
                    .HasPrincipalKey(p => p.Id);
                entity
                    .HasOne(p => p.ContentItem)
                    .WithMany()
                    .HasForeignKey(p => p.ContentItemId)
                    .HasPrincipalKey(p => p.Id);
            });


            builder.Entity<AdminProfile>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.ToTable("AdminProfiles");
                entity
                    .HasOne(x => x.User)
                    .WithOne(x => x.Profile)
                    .HasForeignKey<AdminProfile>(a => a.Id)
                    .HasPrincipalKey<ApplicationUser>(x => x.Id);
                entity.HasKey(x => x.Id);
                entity.ToTable("AdminProfiles");
                entity
                    .HasOne(x => x.User)
                    .WithOne(x => x.Profile)
                    .HasForeignKey<AdminProfile>(a => a.Id)
                    .HasPrincipalKey<ApplicationUser>(x => x.Id);
            });


            #endregion

            #region Settings

            builder.Entity<Country>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("Countries");
                entity.Ignore(p => p.States);
            });


            builder.Entity<State>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("States");
            });


            builder.Entity<AppSettingItem>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.ToTable("AppSettings");
            });


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