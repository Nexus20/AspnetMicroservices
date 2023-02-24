using System.Collections.Generic;
using System.Threading.Tasks;
using AspnetRunBasics.Models;

namespace AspnetRunBasics.Services
{
    public interface ICatalogService
    {
        Task<IEnumerable<CatalogModel>> GetCatalogAsync();
        Task<IEnumerable<CatalogModel>> GetCatalogByCategoryAsync(string category);
        Task<CatalogModel> GetCatalogByIdAsync(string id);
        Task<CatalogModel> CreateCatalogAsync(CatalogModel model);
    }
}