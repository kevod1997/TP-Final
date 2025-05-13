using AutoMapper;
using CustomerService.Application.DTOs;
using CustomerService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomerService.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // De entidad a DTO
            CreateMap<Customer, CustomerDto>();

            // De DTO a entidad
            CreateMap<CreateCustomerDto, Customer>()
                .ConstructUsing(dto => new Customer(dto.Name, dto.Email, dto.Address));
        }
    }
}
