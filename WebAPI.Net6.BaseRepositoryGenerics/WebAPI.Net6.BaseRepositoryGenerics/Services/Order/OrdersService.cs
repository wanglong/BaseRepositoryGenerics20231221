using OneOf;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Order;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models.Order;

namespace WebAPI.Net6.BaseRepositoryGenerics.Services.Order
{
    public class OrdersService : IOrdersService
    {
        private List<Product> _products;
        private List<Receipt> _receipts;
        private int _receiptId;
        public OrdersService()
        {
            _products = new List<Product>
        {
            new Product(1, "Keyboard", 80),
            new Product(2, "Mouse", 50),
            new Product(3, "Monitor", 500)
        };
            _receipts = new List<Receipt>();
        }

        public Receipt PlaceOrder(Domain.Models.Order.Order order)
        {
            var product = _products.SingleOrDefault(p => p.ProductId == order.ProductId);

            if (product is null)
            {
                throw new Exception("Product doesn't exist");
            }

            if (product.Cost > order.Payment)
            {
                throw new Exception("Insufficient funds");
            }

            var receipt = new Receipt(++_receiptId, order.Payment);
            _receipts.Add(receipt);
            return receipt;
        }

        public OneOf<Receipt, PlaceOrderError> PlaceOrderDiscriminatedUnions(Domain.Models.Order.Order order)
        {
            var product = _products.SingleOrDefault(p => p.ProductId == order.ProductId);

            if (product is null)
            {
                return PlaceOrderError.DoesntExist;
            }

            if (product.Cost > order.Payment)
            {
                return PlaceOrderError.InsufficientFunds;
            }

            var receipt = new Receipt(++_receiptId, order.Payment);
            _receipts.Add(receipt);
            return receipt;
        }

        public (Receipt? receipt, PlaceOrderError error) PlaceOrderTuples(Domain.Models.Order.Order order)
        {
            var product = _products.SingleOrDefault(p => p.ProductId == order.ProductId);

            if (product is null)
            {
                return (default(Receipt), PlaceOrderError.DoesntExist);
            }

            if (product.Cost > order.Payment)
            {
                return (default(Receipt), PlaceOrderError.InsufficientFunds);
            }

            var receipt = new Receipt(++_receiptId, order.Payment);
            _receipts.Add(receipt);
            return (receipt, PlaceOrderError.Success);
        }
    }
}
