using Catalog.API.Data;
using Catalog.API.Entities;
using MongoDB.Driver;

namespace Catalog.API.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly ICatalogContext _context;

    public ProductRepository(ICatalogContext context)
    {
        _context = context;
    }

    public Task<List<Product>> GetProducts()
    {
        return _context.Products.Find(x => true).ToListAsync();
    }

    public Task<Product> GetProduct(string id)
    {
        return _context.Products.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public Task<List<Product>> GetProductByName(string name)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Name, name);

        return _context.Products.Find(filter).ToListAsync();
    }

    public Task<List<Product>> GetProductByCategory(string categoryName)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Category, categoryName);

        return _context.Products.Find(filter).ToListAsync();
    }

    public Task CreateProduct(Product product)
    {
        return _context.Products.InsertOneAsync(product);
    }

    public async Task<bool> UpdateProduct(Product product)
    {
        var updateResult =
            await _context.Products.ReplaceOneAsync(filter: p => p.Id == product.Id, replacement: product);

        return updateResult.IsAcknowledged && updateResult.ModifiedCount > 0;
    }

    public async Task<bool> DeleteProduct(string id)
    {
        var filter = Builders<Product>.Filter.Eq(p => p.Id, id);

        var deleteResult = await _context
            .Products
            .DeleteOneAsync(filter);

        return deleteResult.IsAcknowledged
               && deleteResult.DeletedCount > 0;
    }
}