namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.Models.Order
{
    public class Receipt
    {
        public Receipt(int id, decimal cost)
        {
            ReceiptId = id;
            Cost = cost;
        }

        public int ReceiptId { get; set; }

        public string Name { get; set; }

        public decimal Cost { get; set; }
    }
}
