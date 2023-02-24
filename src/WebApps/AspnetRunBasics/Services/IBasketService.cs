using System.Threading.Tasks;
using AspnetRunBasics.Models;

namespace AspnetRunBasics.Services
{
    public interface IBasketService
    {
        Task<BasketModel> GetBasketByUserNameAsync(string userName);
        Task<BasketModel> UpdateBasketAsync(BasketModel model);
        Task CheckoutBasketAsync(BasketCheckoutModel model);
    }
}