using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface IRecipeService
    {
        Task<PagedList<Recipe>> GetRecipesAsync(RecipeListFilter filter);
        Task<Recipe> GetRecipeAsync(int id);
        Task<Recipe> UpdateRecipeAsync(Recipe recipe);
        Task<bool> AttachRecipeToCategoriesAsync(int id, IEnumerable<int> categoryIds);
        Task<bool> DeleteRecipeAsync(int id);
    }
}
