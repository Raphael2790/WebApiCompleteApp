using Microsoft.EntityFrameworkCore;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.Data.Context;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RSS.Data.Repository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        public SupplierRepository(CompleteAppDbContext context) : base(context){}

        public async Task<Supplier> GetSupplierAddress(Guid id)
        {
            return await _db.Suppliers.AsNoTracking().Include(s => s.Adress).FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<Supplier> GetSupplierProductsAddress(Guid id)
        {
            return await _db.Suppliers
                .Include(s => s.Products)
                .Include(s => s.Adress)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
    }
}
