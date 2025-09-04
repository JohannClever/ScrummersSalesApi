using MediatR;
using Microsoft.AspNetCore.Mvc;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Products;
using ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Products;

namespace ScrummersSalesApi.Backend.Products.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly ILogger<ProductsController> _logger;
        private readonly IMediator _mediator;

        public ProductsController(ILogger<ProductsController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        /// <summary>
        /// Lista paginada de productos (Dapper).
        /// GET /Products?page=1&pageSize=20
        /// </summary>
        [HttpGet]
        public Task<PagedResult<ProductDto>> Get([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            return _mediator.Send(new GetProductsQuery(page, pageSize));
        }

        /// <summary>
        /// Obtiene un producto por Id (Dapper).
        /// GET /Products/5
        /// </summary>
        [HttpGet("{id:Guid}")]
        public Task<ProductDto?> GetById(Guid id)
        {
            return _mediator.Send(new GetProductByIdQuery(id));
        }

        /// <summary>
        /// Crea un producto (EF - Repository/UoW) via Command.
        /// POST /Products
        /// </summary>
        [HttpPost]
        // [Authorize] // <- Descomenta si tu endpoint debe requerir autenticación
        public async Task<ActionResult<Product>> Create([FromBody] CreateProductCommand input, CancellationToken ct)
        {
            CreateProductCommandResponse created = await _mediator.Send(input, ct);
            // Devuelve 201 con Location header: /Products/{id}
            return CreatedAtAction(nameof(GetById), new { id = created.ProductId }, created);
        }
    }
}
