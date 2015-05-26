using System.Threading.Tasks;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Domain.Transfer.ContentManagement;

namespace VitalChoice.Interfaces.Services.Content
{
	public interface ICategoryService
    {
        Task<ContentCategory> GetCategoriesTreeAsync(CategoryTreeFilter filter);
        /// <summary>
        /// Allow only update ordering categories in a tree, without ability to add or delete items. Not active status is ignored for this method, but not active categories willn't 
        /// render on the public part.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        Task<bool> UpdateCategoriesTreeAsync(ContentCategory category);
        Task<ContentCategory> GetCategoryAsync(int id);
        Task<ContentCategory> UpdateCategoryAsync(ContentCategory category);
        Task<bool> DeleteCategoryAsync(int id);
    }
}
