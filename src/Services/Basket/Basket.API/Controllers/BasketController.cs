using AutoMapper;
using Basket.API.Entities;
using Basket.API.GrpcServices;
using Basket.API.Repositories;
using EventBus.Messages.Events;
using MassTransit;
using Microsoft.AspNetCore.Mvc;

namespace Basket.API.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class BasketController : ControllerBase
{
    private readonly IBasketRepository _repository;
    private readonly DiscountGrpcService _discountGrpcService;
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly IMapper _mapper;

    public BasketController(IBasketRepository repository, DiscountGrpcService discountGrpcService, IMapper mapper, IPublishEndpoint publishEndpoint)
    {
        _repository = repository;
        _discountGrpcService = discountGrpcService;
        _mapper = mapper;
        _publishEndpoint = publishEndpoint;
    }
    
    [HttpGet("{userName}", Name = "GetBasket")]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> GetBasket(string userName)
    {
        var basket = await _repository.GetBasketAsync(userName);
        return Ok(basket ?? new ShoppingCart(userName));
    }
    
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingCart), StatusCodes.Status200OK)]
    public async Task<ActionResult<ShoppingCart>> UpdateBasket([FromBody] ShoppingCart basket)
    {
        // TODO : Communicate with Discount.Grpc
        // and Calculate latest prices of product into shopping cart
        // consume Discount Grpc
        foreach (var item in basket.Items)
        {
            var coupon = await _discountGrpcService.GetDiscountAsync(item.ProductName);
            item.Price -= coupon.Amount;
        }

        return Ok(await _repository.UpdateBasketAsync(basket));
    }
    
    [HttpDelete("{userName}", Name = "DeleteBasket")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteBasket(string userName)
    {
        await _repository.DeleteBasketAsync(userName);
        return Ok();
    }

    [Route("[action]")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status202Accepted)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
    {
        // Get existing basket with total price from Redis by username
        // Create basketCheckout event
        // Set TotalPrice no basketCheckout eventMessage
        // Send checkout event to rabbitMQ
        // Remove the basket
        
        // Get existing basket with total price from Redis by username
        var basket = await _repository.GetBasketAsync(basketCheckout.UserName);

        if (basket == null)
            return BadRequest();
        
        // Send checkout event to rabbitMQ
        var eventMessage = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
        eventMessage.TotalPrice = basket.TotalPrice;
        await _publishEndpoint.Publish(eventMessage);
        
        // Remove the basket
        await _repository.DeleteBasketAsync(basket.UserName);

        return Accepted();
    }
}