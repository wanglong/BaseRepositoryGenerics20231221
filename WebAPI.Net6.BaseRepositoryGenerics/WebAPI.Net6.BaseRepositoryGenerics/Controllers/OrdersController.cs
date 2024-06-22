using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.IServices.Order;
using WebAPI.Net6.BaseRepositoryGenerics.Domain.Models.Order;

namespace WebAPI.Net6.BaseRepositoryGenerics.Controllers
{
    [ApiController]
    [Route("api/v1/")]
    [EnableCors("any")] // 设置跨域处理的 代理
    public class OrdersController : ControllerBase
    {
        private readonly IOrdersService _ordersService;
        public OrdersController(IOrdersService ordersService)
        {
            _ordersService = ordersService;
        }

        [HttpPost("Order")]
        public IActionResult Post(Domain.Models.Order.Order order)
        {
            var receipt = _ordersService.PlaceOrder(order);
            return Ok(receipt);
        }

        [HttpPost("OrderDiscriminatedUnions")]
        public IActionResult PostDiscriminatedUnions(Domain.Models.Order.Order order)
        {
            var placeOrderResult = _ordersService.PlaceOrderDiscriminatedUnions(order);
            return placeOrderResult.Match<IActionResult>(
                receipt => Ok(receipt),
                error => StatusCode(500, new
                {
                    error = error.ToString()
                }));
        }

        [HttpPost("OrderTuples")]
        public IActionResult PostTuples(Domain.Models.Order.Order order)
        {
            var placeOrderResult = _ordersService.PlaceOrderTuples(order);
            return Ok(placeOrderResult);
        }
    }
}
