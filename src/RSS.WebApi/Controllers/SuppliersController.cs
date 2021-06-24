using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSS.Business.Interfaces;
using RSS.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class SuppliersController : MainController
    {
        private readonly ISupplierRepository _supplierRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly IMapper _mapper;

        public SuppliersController(ISupplierRepository supplierRepository, IAddressRepository addressRepository, IMapper mapper)
        {
            _supplierRepository = supplierRepository;
            _addressRepository = addressRepository;
            _mapper = mapper;
        }

        [ProducesResponseType(typeof(IEnumerable<SupplierDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SupplierDTO>>> GetAllSuppliers()
        {
            var suppliers = _mapper.Map<IEnumerable<SupplierDTO>>(await _supplierRepository.GetAll());

            return Ok(suppliers);
        }
    }
}
