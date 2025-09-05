using MediatR;
using System.ComponentModel.DataAnnotations;

namespace ScrummersSalesApi.Backend.Products.Domain.Commands.Command
{
    public class CreateProductCommand : IRequest<CreateProductCommandResponse>
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string? Description { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
