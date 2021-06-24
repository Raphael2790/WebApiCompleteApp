using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace RSS.WebApi.DTOs
{
    public class SupplierDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Name { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(14, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 11)]
        public string IdentificationDocument { get; set; }

        public int SupplierType { get; set; }

        public bool Active { get; set; }

        /*EF Relations*/
        public AddressDTO Address { get; set; }
        public IEnumerable<ProductDTO> Products { get; set; }
    }
}
