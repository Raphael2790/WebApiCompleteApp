using KissLog;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.Business.Models.Validations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.Business.Services
{
    public class SupplierService : BaseService, ISupplierService
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ILogger _logger;

        public SupplierService(ISupplierRepository supplierRepository, 
                                IAddressRepository addressRepository,
                                INotifiable notifiable,
                                ILogger logger) : base(notifiable, logger)
        {
            _supplierRepository = supplierRepository;
            _addressRepository = addressRepository;
        }

        public async Task AddSupplier(Supplier supplier)
        {
            try
            {
                if (!ExecuteValidation(new SupplierValidation(), supplier)
                        || !ExecuteValidation(new AddressValidation(), supplier.Adress)) return;

                if (_supplierRepository.Find(s => s.IdentificationDocument == supplier.IdentificationDocument).Result.Any())
                {
                    Notify("Já existe um fornecedor com este documento informado");
                    return;
                }

                await _supplierRepository.Add(supplier);
            }
            catch (Exception ex)
            {

               ExecuteLoggingError(ex.Message, nameof(SupplierService));
            }
        }

        public async Task RemoveSupplier(Guid id)
        {
            try
            {
                if (_supplierRepository.GetSupplierProductsAddress(id).Result.Products.Any(p => p.Active))
                {
                    Notify("O fornecedor possui produtos cadastrados ativos!");
                    return;
                }

                await _supplierRepository.Remove(id);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(SupplierService));
            }
        }

        public async Task UpdateSupplier(Supplier supplier)
        {
            try
            {
                if (!ExecuteValidation(new SupplierValidation(), supplier)) return;

                if (_supplierRepository.Find(s => s.IdentificationDocument == supplier.IdentificationDocument && s.Id != supplier.Id).Result.Any())
                {
                    Notify("Já existe um fornecedor com documento informado");
                    return;
                }

                await _supplierRepository.Update(supplier);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(SupplierService));
            }
        }

        public async Task UpdateSupplierAddress(Address address)
        {
            try
            {
                if (!ExecuteValidation(new AddressValidation(), address)) return;

                await _addressRepository.Update(address);
            }
            catch (Exception ex)
            {

                ExecuteLoggingError(ex.Message, nameof(SupplierService));
            }
        }

        public void Dispose()
        {
            _addressRepository?.Dispose();
            _supplierRepository?.Dispose();
        }
    }
}
