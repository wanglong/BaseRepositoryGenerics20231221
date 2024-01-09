namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.Models.Order
{
    public class Product
    {
        public Product(int id, string name, decimal cost)
        {
            ProductId = id;
            ProductName = name;
            Cost = cost;
        }

        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        public string ProductCategory { get; set; }

        public int ProductCategoryId { get; set; }

        public decimal Cost { get; set; }
    }
}
