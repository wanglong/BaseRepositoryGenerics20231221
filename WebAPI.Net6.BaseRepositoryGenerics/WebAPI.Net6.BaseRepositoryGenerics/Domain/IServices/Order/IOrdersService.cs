using OneOf;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models.Order;

namespace WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Order
{
    public interface IOrdersService
    {
        Receipt PlaceOrder(Domain.Models.Order.Order order);

        OneOf<Receipt, PlaceOrderError> PlaceOrderDiscriminatedUnions(Domain.Models.Order.Order order);

        (Receipt? receipt, PlaceOrderError error) PlaceOrderTuples(Domain.Models.Order.Order order);
    }
}
