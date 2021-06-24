using RSS.Business.Models;
using System;
using System.Threading.Tasks;

namespace RSS.Business.Interfaces
{
    public interface IProductService : IDisposable
    {
        Task AddProduct(Product product);
        Task UpdateProduct(Product product);
        Task RemoveProduct(Guid id);
    }
}
