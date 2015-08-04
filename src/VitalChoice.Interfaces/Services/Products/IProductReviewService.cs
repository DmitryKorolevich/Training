using System.Collections.Generic;
using System.Threading.Tasks;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Domain.Transfer.Products;
using VitalChoice.DynamicData.Entities;
using VitalChoice.DynamicData.Interfaces;

namespace VitalChoice.Interfaces.Services.Products
{
	public interface IProductReviewService 
	{
	    Task<PagedList<VProductsWithReview>> GetVProductsWithReviewsAsync(VProductsWithReviewFilter filter);

        Task<PagedList<ProductReview>> GetProductReviewsAsync(ProductReviewFilter filter);

        Task<ProductReview> GetProductReviewAsync(int id);

        Task<ProductReview> UpdateProductReviewAsync(ProductReview model);

        Task<bool> DeleteProductReviewAsync(int id);
    }
}
