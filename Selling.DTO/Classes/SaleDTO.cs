using System;

namespace NAlex.Selling.DTO.Classes
{
    public class SaleDTO: IEquatable<SaleDTO>
    {
        public CustomerDTO Customer { get; set; }        
        public int Id { get; set; }        
        public ManagerDTO Manager { get; set; }        
        public ProductDTO Product { get; set; }
        public DateTime SaleDate { get; set; }
        public double Total { get; set; }

        public bool Equals(SaleDTO other)
        {
            if (other == null)
                return false;

            bool res = Customer != null ? Customer.Equals(other.Customer) : other.Customer == null;
            res &= Manager != null ? Manager.Equals(other.Manager) : other.Manager == null;
            res &= Product != null ? Product.Equals(other.Product) : other.Product == null;
            res &= SaleDate.Equals(other.SaleDate) && Total.Equals(other.Total) && Id.Equals(other.Id);

            return res;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as SaleDTO);
        }

        public override int GetHashCode()
        {
            int code = 0;

            if (Customer != null)
                code ^= Customer.GetHashCode();
            if (Manager != null)
                code ^= Manager.GetHashCode();
            if (Product != null)
                code ^= Product.GetHashCode();
            code ^= SaleDate.GetHashCode() ^ Total.GetHashCode() ^ Id.GetHashCode();

            return code;
        }
    }
}
