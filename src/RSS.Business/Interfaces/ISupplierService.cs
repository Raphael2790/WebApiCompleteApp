using RSS.Business.Models;
using System;
using System.Threading.Tasks;

namespace RSS.Business.Interfaces
{
    public interface ISupplierService : IDisposable
    {
        Task AddSupplier(Supplier supplier);
        Task UpdateSupplier(Supplier supplier);
        Task RemoveSupplier(Guid id);
        Task UpdateSupplierAddress(Address address);
    }
}
