using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Data.Services;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;

namespace VitalChoice.Business.Services.Contracts.Content
{
	public interface ICategoryService
    {
        Task<ContentCategory> GetCategoriesTreeAsync(ContentType type, RecordStatusCode? status = null);
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
