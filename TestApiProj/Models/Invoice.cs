namespace TestApiProj.Models
{
    public class Invoice
    {
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public string ItemName { get; set; }
        public decimal ItemPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount { get; set; }

        public decimal Itemtax { get; set; }
    }
}
