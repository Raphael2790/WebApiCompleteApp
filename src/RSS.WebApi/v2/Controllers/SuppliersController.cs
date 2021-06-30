using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.WebApi.Controllers;
using RSS.WebApi.DTOs;
using RSS.WebApi.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.WebApi.v2.Controllers
{
    [Authorize]
    [ApiVersion("2.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class SuppliersController : MainController
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ISupplierService _supplierService;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierRepository supplierRepository, 
                                    IMapper mapper,
                                    IAddressRepository addressRepository,
                                    INotifiable notifiable,
                                    IUser appUser) : base(notifiable, appUser)
        {
            _supplierRepository = supplierRepository;
            _mapper = mapper;
            _addressRepository = addressRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(typeof(IEnumerable<SupplierDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetAllSuppliers()
        {
            var suppliers = _mapper.Map<IEnumerable<SupplierDTO>>(await _supplierRepository.GetAll());

            return Ok(suppliers);
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(SupplierDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<SupplierDTO>> GetSupplierById(Guid id)
        {
            var supplier = await GetSupplierAddressProducts(id);

            if (supplier == null) return NotFound();

            return Ok(supplier);
        }

        [HttpPost]
        [ClaimsAuthorize("Supplier", "Add")]
        [ProducesResponseType(typeof(Supplier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddSupplier(SupplierDTO supplierDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _supplierService.AddSupplier(_mapper.Map<Supplier>(supplierDTO));

            return CustomResponse(supplierDTO);
        }

        [HttpPut("{id:guid}")]
        [ClaimsAuthorize("Supplier", "Update")]
        [ProducesResponseType(typeof(Supplier), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> UpdateSupplier(Guid id, SupplierDTO supplierDTO)
        {
            if (id != supplierDTO.Id)
            {
                NotifyError("O id informado é diferente do id do fornecedor");
                return CustomResponse(supplierDTO);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _supplierService.UpdateSupplier(_mapper.Map<Supplier>(supplierDTO));

            return CustomResponse(supplierDTO);
        }

        [HttpDelete("id:guid")]
        [ClaimsAuthorize("Supplier", "Delete")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> DeleteSupplier(Guid id)
        {
            var supplierDTO = await GetSupplierAddress(id);

            if (supplierDTO == null) return NotFound();

            await _supplierService.RemoveSupplier(id);

            return CustomResponse(supplierDTO);
        }

        [HttpGet("get-address/{id:guid}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<AddressDTO>> GetAddressById(Guid id)
        {
            return Ok(_mapper.Map<AddressDTO>(await _addressRepository.FindById(id)));
        }

        [HttpPut("update-address/{id:guid}")]
        [ClaimsAuthorize("Supplier", "Update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AddressDTO>> UpdateAddress(Guid id, AddressDTO addressDTO)
        {
            if(id != addressDTO.Id)
            {
                NotifyError("O id informado é diferente do endereço");
                CustomResponse(addressDTO);
            }

            if (!ModelState.IsValid) return CustomResponse(ModelState);

            await _supplierService.UpdateSupplierAddress(_mapper.Map<Address>(addressDTO));

            return CustomResponse(addressDTO);
        }

        private async Task<SupplierDTO> GetSupplierAddress(Guid id)
        {
            return _mapper.Map<SupplierDTO>(await _supplierRepository.GetSupplierAddress(id));
        }

        private async Task<SupplierDTO> GetSupplierAddressProducts(Guid id)
        {
            return _mapper.Map<SupplierDTO>(await _supplierRepository.GetSupplierProductsAddress(id));
        }
    }
}
