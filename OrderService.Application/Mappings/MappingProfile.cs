using AutoMapper;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // Mapeo de entidades a DTOs
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();

            // Mapeo especial para la creación de órdenes
            // Ignoramos Items porque se manejan manualmente en el handler
            CreateMap<CreateOrderDto, Order>()
                .ConstructUsing(dto => new Order(dto.CustomerId, dto.CustomerName))
                .ForMember(dest => dest.Items, opt => opt.Ignore());

            // Mapeo para los ítems de orden
            CreateMap<CreateOrderItemDto, OrderItem>()
                .ConstructUsing((src, ctx) => new OrderItem(0, src.ProductId, src.ProductName, src.UnitPrice, src.Quantity));
        }
    }
}