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
        const long REQUEST_SIZE_LIMIT = 40000000;
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

        //Recebe o Upload Image em base64 convertendo novamente para arquivo, porém é limitado pelo tamnho do corpo da requisição
        [HttpPost]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> AddProduct([FromBody]ProductDTO productDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            var imgName = Guid.NewGuid() + "_" + productDTO.Image;

            if (!FileUploaded(productDTO.UploadImage, imgName)) return CustomResponse(productDTO);

            productDTO.Image = imgName;

            await _productService.AddProduct(_mapper.Map<Product>(productDTO));
            
            return CustomResponse(productDTO);
        }

        //Aumenta o tamanho máximo do request body
        //Envio via form data usando chave e valor, inclusive para o arquivo
        [RequestSizeLimit(REQUEST_SIZE_LIMIT)]
        [HttpPost]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductFormFileDTO>> AddProductWithFile([FromForm] ProductFormFileDTO productDTO)
        {
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if(productDTO.UploadImage.Length > Request.Body.Length)
            {
                NotifyError("A imagem fornecida é maior do que a permitida");
                return CustomResponse(productDTO);
            }

            var imgPrefix = Guid.NewGuid() + "_";

            if (!await FormFileUploaded(productDTO.UploadImage, imgPrefix)) return CustomResponse(productDTO);

            productDTO.Image = imgPrefix + productDTO.UploadImage.FileName;

            await _productService.AddProduct(_mapper.Map<Product>(productDTO));

            return CustomResponse(productDTO);
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDTO>> UpdateProduct(Guid id, ProductDTO productDTO)
        {
            if(id != productDTO.Id)
            {
                NotifyError("Os ids de atualização e produto atual são diferentes");
                return CustomResponse();
            }

            var saveProduct = await GetProduct(id);
            productDTO.Image = saveProduct.Image;
            if (!ModelState.IsValid) return CustomResponse(ModelState);

            if(productDTO.UploadImage != null)
            {
                var imgName = Guid.NewGuid() + "_" + productDTO.Image;
                if (!FileUploaded(productDTO.UploadImage, imgName)) return CustomResponse(ModelState);
                saveProduct.Image = imgName;
            }

            saveProduct.Name = productDTO.Name;
            saveProduct.Description = productDTO.Description;
            saveProduct.Price = productDTO.Price;
            saveProduct.Active = productDTO.Active;

            await _productService.UpdateProduct(_mapper.Map<Product>(saveProduct));

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

        private async Task<bool> FormFileUploaded(IFormFile file, string imgPrefix)
        {
            if(file.Length == 0 || file == null)
            {
                NotifyError("Forneça uma imagem para este produto!");
                return false;
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", imgPrefix + file.FileName);

            if (System.IO.File.Exists(path))
            {
                NotifyError("Já existe um arquivo com este nome!");
                return false;
            }

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return true;
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
