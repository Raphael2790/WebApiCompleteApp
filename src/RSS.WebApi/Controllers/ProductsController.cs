using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RSS.Business.Interfaces;
using RSS.Business.Models;
using RSS.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace RSS.WebApi.Controllers
{
    [Route("api/[controller]")]
    public class ProductsController : MainController
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductService _productService;
        private readonly IMapper _mapper;

        public ProductsController(IProductRepository productRepository,
                                    IProductService productService,
                                    IMapper mapper,
                                    INotifiable notifiable) : base(notifiable)
        {
            _productRepository = productRepository;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDTO>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAllProducts()
        {
            return Ok(_mapper.Map<IEnumerable<ProductDTO>>(await _productRepository.GetAll()));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> GetProductById(Guid id)
        {
            var product = await GetProduct(id);

            if (product == null) return NotFound();

            return Ok(product);
        }

        [HttpPost]
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductDTO productDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgName = Guid.NewGuid() + "_" + productDTO.Image;

            if (!FileUploaded(productDTO.UploadImage, imgName)) return CustomResponse(productDTO);

            productDTO.Image = imgName;

            await _productService.AddProduct(_mapper.Map<Product>(productDTO));
            
            return CustomResponse(productDTO);
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDTO>> DeleteProduct(Guid id)
        {
            var product = await GetProduct(id);

            if (product == null) return NotFound();

            await _productRepository.Remove(id);

            return CustomResponse(product);
        }

        private async Task<ProductDTO> GetProduct(Guid id)
        {
            return _mapper.Map<ProductDTO>(await _productRepository.FindById(id));
        }

        private bool FileUploaded(string file, string imgName)
        {
            if (string.IsNullOrEmpty(file))
            {
                NotifyError("Forneça uma imagem para este produto!");
                return false;
            }

            var imageDataByteArray = Convert.FromBase64String(file);

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgName);

            if (System.IO.File.Exists(filePath))
            {
                NotifyError("Já existe um arquivo com este nome!");
                return false;
            }

            System.IO.File.WriteAllBytes(filePath, imageDataByteArray);

            return true;
        }
    }
}
