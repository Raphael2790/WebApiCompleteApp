using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace RSS.WebApi.DTOs
{
    public class ProductDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public Guid SupplierId { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(1000, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Description { get; set; }

        public string UploadImage { get; set; }

        public string Image { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        public decimal Price { get; set; }

        //evite que ao gerar o model na view o atributo seja usado no scaffold
        [ScaffoldColumn(false)]
        public DateTime RegisterDate { get; set; }

        public bool Active { get; set; }

        [ScaffoldColumn(false)]
        public string SupplierName { get; set; }
    }
}