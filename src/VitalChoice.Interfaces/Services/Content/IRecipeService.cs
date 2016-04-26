using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Content.Recipes;
using VitalChoice.Infrastructure.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IRecipeService
    {
        Task<PagedList<Recipe>> GetRecipesAsync(RecipeListFilter filter);
        Task<Recipe> GetRecipeAsync(int id);
	    Task<Recipe> GetRecipeByIdOldAsync(int id);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        Task<bool> AttachRecipeToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteRecipeAsync(int id);
		Task<IList<RecipeDefaultSetting>> GetRecipeSettingsAsync();
    }
}
