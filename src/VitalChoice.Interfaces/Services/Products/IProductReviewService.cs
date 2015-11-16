using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

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
