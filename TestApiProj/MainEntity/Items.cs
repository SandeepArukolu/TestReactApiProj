using System.ComponentModel.DataAnnotations;

namespace TestApiProj.MainEntity
{
    public class Items
    {
        [Key]
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string? ItemDesc { get; set; }
        public int? ItemQty { get; set; }
        public int? ItemStock { get; set; }
        public double ItemPrice { get; set; }     
        public double? ItemDiscount { get; set; }
        public double? ItemTax { get; set; }
    }
}
