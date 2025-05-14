using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs.External
{
    // DTO para actualizar el stock de un producto
    public class UpdateStockDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}