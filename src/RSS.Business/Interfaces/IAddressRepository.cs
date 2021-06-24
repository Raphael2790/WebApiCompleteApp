using RSS.Business.Models;
using System;
using System.Threading.Tasks;

namespace RSS.Business.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<Address> GetAddressBySupplier(Guid supplierId);
    }
}
