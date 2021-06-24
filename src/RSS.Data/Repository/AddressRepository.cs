using Microsoft.EntityFrameworkCore;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.Data.Context;
using System;
using System.Threading.Tasks;

namespace RSS.Data.Repository
{
    public class AddressRepository : Repository<Address>, IAddressRepository
    {
        public AddressRepository(CompleteAppDbContext context) : base(context)
        {
        }

        public async Task<Address> GetAddressBySupplier(Guid supplierId)
        {
            return await _db.Addresses.AsNoTracking().FirstOrDefaultAsync(a => a.SupplierId == supplierId);
        }
    }
}
