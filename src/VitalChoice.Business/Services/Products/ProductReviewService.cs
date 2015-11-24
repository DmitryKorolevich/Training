using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using VitalChoice.Business.Queries.Product;
using VitalChoice.Data.Repositories.Specifics;
using VitalChoice.Interfaces.Services;
using VitalChoice.Interfaces.Services.Products;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Ecommerce.Domain.Entities.Products;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Transfer.Products;

namespace VitalChoice.Business.Services.Products
{
    public class ProductReviewService : IProductReviewService
    {
        private readonly IEcommerceRepositoryAsync<VProductsWithReview> _vProductsWithReviewRepository;
        private readonly IEcommerceRepositoryAsync<ProductReview> _productReviewRepository;
        private readonly IEcommerceRepositoryAsync<Product> _productRepository;
        private readonly ILogger logger;

        public ProductReviewService(IEcommerceRepositoryAsync<VProductsWithReview> vProductsWithReviewRepository,
            IEcommerceRepositoryAsync<ProductReview> productReviewRepository,
            IEcommerceRepositoryAsync<Product> productRepository,
            ILoggerProviderExtended loggerProvider)
        {
            this._vProductsWithReviewRepository = vProductsWithReviewRepository;
            this._productReviewRepository = productReviewRepository;
            this._productRepository = productRepository;
            logger = loggerProvider.CreateLoggerDefault();
        }


        public async Task<PagedList<VProductsWithReview>> GetVProductsWithReviewsAsync(VProductsWithReviewFilter filter)
        {
            var conditions = new VProductsWithReviewQuery().WithStatus(filter.StatusCode).WithName(filter.SearchText);
            var query = _vProductsWithReviewRepository.Query(conditions);

            Func<IQueryable<VProductsWithReview>, IOrderedQueryable<VProductsWithReview>> sortable = x => x.OrderBy(y => y.ProductName);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case VProductsWithReviewSortPath.ProductName:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.ProductName)
                                : x.OrderByDescending(y => y.ProductName);
                    break;
                case VProductsWithReviewSortPath.Count:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Count)
                                : x.OrderByDescending(y => y.Count);
                    break;
                case VProductsWithReviewSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case VProductsWithReviewSortPath.Rating:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Rating)
                                : x.OrderByDescending(y => y.Rating);
                    break;
            }

            PagedList<VProductsWithReview> toReturn = await query.OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

        public async Task<PagedList<ProductReview>> GetProductReviewsAsync(ProductReviewFilter filter)
        {
            var conditions = new ProductReviewQuery().WithStatus(filter.StatusCode).WithIdProduct(filter.IdProduct);
            var query = _productReviewRepository.Query(conditions);

            Func<IQueryable<ProductReview>, IOrderedQueryable<ProductReview>> sortable = x => x.OrderByDescending(y => y.DateCreated);
            var sortOrder = filter.Sorting.SortOrder;
            switch (filter.Sorting.Path)
            {
                case ProductReviewSortPath.CustomerName:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.CustomerName)
                                : x.OrderByDescending(y => y.CustomerName);
                    break;
                case ProductReviewSortPath.Title:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Title)
                                : x.OrderByDescending(y => y.Title);
                    break;
                case ProductReviewSortPath.ProductName:
                    //_productReviewRepository.EarlyRead = true; //added temporarly till ef 7 becomes stable

                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Product.Name)
                                : x.OrderByDescending(y => y.Product.Name);
                    break;
                case ProductReviewSortPath.DateCreated:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.DateCreated)
                                : x.OrderByDescending(y => y.DateCreated);
                    break;
                case ProductReviewSortPath.Rating:
                    sortable =
                        (x) =>
                            sortOrder == SortOrder.Asc
                                ? x.OrderBy(y => y.Rating)
                                : x.OrderByDescending(y => y.Rating);
                    break;
            }

            PagedList<ProductReview> toReturn = await query.Include(p => p.Product).OrderBy(sortable).SelectPageAsync(filter.Paging.PageIndex, filter.Paging.PageItemCount);
            return toReturn;
        }

	    public async Task<int> GetApprovedCountAsync(int productId)
	    {
			var conditions = new ProductReviewQuery().WithStatus(RecordStatusCode.Active).WithIdProduct(productId);

			return await _productReviewRepository.Query(conditions).SelectCountAsync();
		}

	    public async Task<int> GetApprovedAverageRatingsAsync(int productId)
	    {
			var conditions = new ProductReviewQuery().WithStatus(RecordStatusCode.Active).WithIdProduct(productId);

			return await Task.FromResult((int)Math.Round(_productReviewRepository.Query(conditions).Select(x=>x.Rating,false).Average()));
		}

	    public async Task<ProductReview> GetProductReviewAsync(int id)
        {
            var conditions = new ProductReviewQuery().WithId(id).NotDeleted();
            var query = _productReviewRepository.Query(conditions).Include(p => p.Product);

            return (await query.SelectAsync(false)).FirstOrDefault();
        }

        public async Task<ProductReview> UpdateProductReviewAsync(ProductReview model)
        {
            ProductReview dbItem = null;
            if (model.Id == 0)
            {
                dbItem = new ProductReview() { IdProduct = model.IdProduct };
                var productExist = await _productRepository.Query(p => p.Id == model.IdProduct && p.StatusCode != (int)RecordStatusCode.Deleted)
                    .SelectAnyAsync();
                if (!productExist)
                {
                    throw new AppValidationException("The product with the given id doesn't exist.");
                }

                dbItem.StatusCode = RecordStatusCode.NotActive;
                dbItem.DateCreated = DateTime.Now;
                dbItem.DateEdited = dbItem.DateCreated;
            }
            else
            {
                dbItem = await GetProductReviewAsync(model.Id);
            }

            if (dbItem != null && dbItem.StatusCode != RecordStatusCode.Deleted)
            {
                dbItem.CustomerName = model.CustomerName;
                dbItem.Email = model.Email;
                dbItem.Title = model.Title;
                dbItem.Description = model.Description;
                dbItem.Rating = model.Rating;
                dbItem.Product = null;
                if(model.StatusCode==RecordStatusCode.Active || model.StatusCode==RecordStatusCode.NotActive)
                {
                    dbItem.StatusCode = model.StatusCode;
                }

                if (model.Id == 0)
                {
                    await _productReviewRepository.InsertAsync(dbItem);
                }
                else
                {
                    dbItem.DateEdited = DateTime.Now;
                    await _productReviewRepository.UpdateAsync(dbItem);
                }
            }

            return dbItem;
        }

        public async Task<bool> DeleteProductReviewAsync(int id)
        {
            bool toReturn = false;
            var dbItem = (await _productReviewRepository.Query(p => p.Id == id).SelectAsync(false)).FirstOrDefault();
            if (dbItem != null)
            {
                dbItem.StatusCode = RecordStatusCode.Deleted;
                await _productReviewRepository.UpdateAsync(dbItem);

                toReturn = true;
            }
            return toReturn;
        }
    }
}