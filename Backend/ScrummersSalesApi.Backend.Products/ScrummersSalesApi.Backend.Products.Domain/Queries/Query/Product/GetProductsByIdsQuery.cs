using MediatR;
using ScrummersSalesApi.Backend.Products.Domain.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScrummersSalesApi.Backend.Products.Domain.Queries.Query.Product
{
    /// <summary>
    /// Query to retrieve multiple products by their Ids.
    /// </summary>
    public sealed class GetProductsByIdsQuery : IRequest<IEnumerable<ProductDto>>
    {
        public IReadOnlyList<Guid> Ids { get; }

        public GetProductsByIdsQuery(IReadOnlyList<Guid> ids)
            => Ids = ids ?? Array.Empty<Guid>();
    }
}
