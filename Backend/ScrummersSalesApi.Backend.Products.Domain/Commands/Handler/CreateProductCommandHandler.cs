using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Entities.Products;
using ScrummersSalesApi.Backend.Products.Domain.Commands.Command;
using ScrummersSalesApi.Backend.Products.Domain.Services.Interfaces;

namespace ScrummersSalesApi.Backend.Products.Domain.Commands.Handler
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, CreateProductCommandResponse>
    {
        private readonly IProductService _productService;

        public CreateProductCommandHandler(IProductService productService)
        {
            _productService = productService;
        }
        public async Task<CreateProductCommandResponse> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product()
            {
                Name = request.Name,
                Price = request.Price,
                Description = request.Description,
                Stock = request.Stock
            };
            product = await _productService.CreateProductAsync(product);
            return new CreateProductCommandResponse() { ProductId  = product.Id };
        }
    }
}
