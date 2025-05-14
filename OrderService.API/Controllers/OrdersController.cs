using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands;
using OrderService.Application.DTOs;
using OrderService.Application.Queries;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Results;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Orders
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders()
        {
            var result = await _mediator.Send(new GetAllOrdersQuery());
            return result.ToActionResult(this);
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> GetOrder(int id)
        {
            var result = await _mediator.Send(new GetOrderByIdQuery { Id = id });
            return result.ToActionResult(this);
        }

        // GET: api/Orders/customer/5
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrdersByCustomer(int customerId)
        {
            var result = await _mediator.Send(new GetOrdersByCustomerIdQuery { CustomerId = customerId });
            return result.ToActionResult(this);
        }

        // POST: api/Orders
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            var result = await _mediator.Send(new CreateOrderCommand { OrderDto = orderDto });

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetOrder), new { id = result.Value.Id }, result.Value);

            return result.ToActionResult(this);
        }

        // PUT: api/Orders/5/items
        [HttpPut("{orderId}/items")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<OrderDto>> UpdateOrderItem(int orderId, [FromBody] UpdateOrderItemDto orderItemDto)
        {
            var result = await _mediator.Send(new UpdateOrderItemCommand
            {
                OrderId = orderId,
                OrderItemDto = orderItemDto
            });

            return result.ToActionResult(this);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            var result = await _mediator.Send(new DeleteOrderCommand { Id = id });
            return result.ToActionResultWithNoContent(this);
        }
    }
}
