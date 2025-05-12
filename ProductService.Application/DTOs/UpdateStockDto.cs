using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Application.DTOs
{
    public class UpdateStockDto
    {
        public int Id { get; set; }
        public int Quantity { get; set; }
    }
}