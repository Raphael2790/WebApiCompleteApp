using RSS.Business.Models.Enums;
using System.Collections.Generic;

namespace RSS.Business.Models
{
    public class Supplier : Entity
    {
        public string Name { get; set; }
        public string IdentificationDocument { get; set; }
        public SupplierType SupplierType { get; set; }
        public bool Active { get; set; }

        /*EF Relations*/
        public Address Adress { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
