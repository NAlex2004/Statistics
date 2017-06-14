using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NAlex.Selling.DTO.Classes
{
    public class ProductDTO: IEquatable<ProductDTO>
    {
        public int Id { get; set; }        
        public double Price { get; set; }
        public string ProductName { get; set; }

        public bool Equals(ProductDTO other)
        {
            return other != null
                ? ProductName == other.ProductName // && Price == other.Price // индекс unique на ProductName
                : false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as ProductDTO);
        }

        public override int GetHashCode()
        {
            return string.IsNullOrEmpty(ProductName) ? 0 : ProductName.GetHashCode();
        }
    }
}
