using System;

namespace NAlex.Selling.DTO.Classes
{
    public class TempSaleDTO: IEquatable<TempSaleDTO>
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string ManagerName { get; set; }
        public string ProductName { get; set; }
        public DateTime SaleDate { get; set; }
        public Guid SessionId { get; set; }
        public double Total { get; set; }

        public bool Equals(TempSaleDTO other)
        {
            return other != null
                ? CustomerName.Equals(other.CustomerName)
                    && ManagerName.Equals(other.ManagerName)
                    && ProductName.Equals(other.ProductName)
                    && SaleDate.Equals(other.SaleDate)
                    && SessionId.Equals(other.SessionId)
                    && Total.Equals(other.Total)
                    && Id.Equals(other.Id)
                : false;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TempSaleDTO);
        }

        public override int GetHashCode()
        {
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}_{6}", CustomerName, ManagerName, ProductName, SaleDate, SessionId, Total, Id).GetHashCode();
        }
    }
}
