using KissLog;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.Business.Models.Validations;
using System;
using System.Threading.Tasks;

namespace RSS.Business.Services
{
    public class ProductService : BaseService, IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository,
                                INotifiable notifiable,
                                ILogger logger) : base(notifiable, logger)
        {
            _productRepository = productRepository;
        }

        public async Task AddProduct(Product product)
        {
            try
            {
                if (!ExecuteValidation(new ProductValidation(), product)) return;

                await _productRepository.Add(product);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(ProductService));
            }
        }

        public async Task RemoveProduct(Guid id)
        {
            try
            {
                await _productRepository.Remove(id);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(ProductService));
            }
        }

        public async Task UpdateProduct(Product product)
        {
            try
            {
                if (!ExecuteValidation(new ProductValidation(), product)) return;

                await _productRepository.Update(product);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(ProductService));
            }
        }

        public void Dispose()
        {
            _productRepository?.Dispose();
        }
    }
}
