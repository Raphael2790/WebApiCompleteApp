using System;
using System.ComponentModel.DataAnnotations;

namespace RSS.WebApi.DTOs
{
    public class AddressDTO
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(200, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Street { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Number { get; set; }

        public string Complement { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(8, ErrorMessage = "o campo {0} precisa ter {1} caracteres", MinimumLength = 8)]
        public string ZipCode { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string Neighborhood { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(100, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string City { get; set; }

        [Required(ErrorMessage = "O campo {0} é obrigatório")]
        [StringLength(50, ErrorMessage = "o campo {0} precisa ter entre {2} e {1}", MinimumLength = 2)]
        public string State { get; set; }
    }
}