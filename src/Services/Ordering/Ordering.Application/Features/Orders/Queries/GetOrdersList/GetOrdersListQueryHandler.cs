using AutoMapper;
using MediatR;
using Ordering.Application.Contracts.Persistent;
using Ordering.Domain.Entities;

namespace Ordering.Application.Features.Orders.Queries.GetOrdersList;

public class GetOrdersListQueryHandler : IRequestHandler<GetOrdersListQuery, List<OrderVm>>
{
    private readonly IOrderRepository _orderRepository;

    private readonly IMapper _mapper;

    public GetOrdersListQueryHandler(IOrderRepository orderRepository, IMapper mapper)
    {
        _orderRepository = orderRepository;
        _mapper = mapper;
    }

    public async Task<List<OrderVm>> Handle(GetOrdersListQuery request, CancellationToken cancellationToken)
    {
        var orderEntities =  await _orderRepository.GetOrdersByUserName(request.UserName);
        return _mapper.Map<IEnumerable<Order>, List<OrderVm>>(orderEntities);
    }
}