﻿using BuildingBlocks.Core.ErrorHandling;
using Microsoft.EntityFrameworkCore;
using MusicStore.Modules.Catalog.Data;
using MusicStore.Modules.Catalog.Errors;

namespace MusicStore.Modules.Catalog.Contracts;

internal sealed class CatalogService(CatalogDbContext dbContext) : ICatalogService
{
    public async Task<Result<ProductDetails>> GetProductForCartAsync(Guid productId, CancellationToken ct = default)
    {
        var product = await dbContext.Products
            .AsNoTracking()
            .Where(p => p.Id == productId && p.IsAvailable)
            .Select(p => new ProductDetails
            (
                p.Name, 
                p.GetMainImage(),
                p.Price
            ))
            .FirstOrDefaultAsync(ct);
        
        return product ?? Result.Failure<ProductDetails>(ProductErrors.NotFound(productId));
    }
}   
