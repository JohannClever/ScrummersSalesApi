// ScrummersSalesApi.Backend.Orders.Api/Controllers/OrdersController.cs
using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Commands.Command.ScrummersSalesApi.Backend.Orders.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Orders.Domain.Dto;
using ScrummersSalesApi.Backend.Orders.Domain.Dto.Order;
using ScrummersSalesApi.Backend.Orders.Domain.Enums;
using ScrummersSalesApi.Backend.Orders.Domain.Queries.Query.Products;

namespace ScrummersSalesApi.Backend.Orders.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<OrdersController> _logger;

        public OrdersController(IMediator mediator, ILogger<OrdersController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Returns a paged list of orders.
        /// GET /api/orders?page=1&pageSize=20
        /// </summary>
        [HttpGet]
        public Task<PagedResult<OrderDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
            => _mediator.Send(new GetOrdersQuery(page, pageSize));

        /// <summary>
        /// Returns an order by Id.
        /// GET /api/orders/{id}
        /// </summary>
        [HttpGet("{id:guid}")]
        public Task<OrderDto?> GetById(Guid id)
            => _mediator.Send(new GetOrderByIdQuery(id));

        /// <summary>
        /// Creates a new order.
        /// POST /api/orders
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CreateOrderResponse>> Create([FromBody] CreateOrderCommand command, CancellationToken ct)
        {
            var created = await _mediator.Send(command, ct);
            // 201 with Location header pointing to GET by Id
            return CreatedAtAction(nameof(GetById), new { id = created.OrderId }, created);
        }

        /// <summary>
        /// Updates the status of an existing order.
        /// PUT /api/orders/{id}/status
        /// </summary>
        [HttpPut("{id:guid}/status")]
        public async Task<ActionResult<UpdateOrderResponse>> UpdateStatus(
            Guid id,
            [FromBody] UpdateOrderStatusRequest input,
            CancellationToken ct)
        {
            // Build the command from route + body
            var result = await _mediator.Send(new UpdateOrderStatusCommand
            {
                Id = id,
                Status = input.Status
            }, ct);

            // If your handler returns Updated=false on errors, map to 400/404 as you prefer.
            if (!result.Updated)
                return BadRequest(result);

            return Ok(result);
        }
    }

}
