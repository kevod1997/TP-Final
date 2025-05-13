using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;
using FluentValidation;
using ProductService.Domain.Exceptions;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Results;


namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILoggerService _logger;

        public ProductsController(IMediator mediator, ILoggerService logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            _logger.LogInformation("Recibida solicitud para obtener todos los productos");

            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);

            return result.ToActionResult(this);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var query = new GetProductByIdQuery { Id = id };
            var product = await _mediator.Send(query);

            if (product == null)
                return NotFound();

            return Ok(product);
        }

        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto productDto)
        {
            var command = new CreateProductCommand { ProductDto = productDto };
            var result = await _mediator.Send(command);

            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto productDto)
        {
            if (id != productDto.Id)
                return BadRequest();

            var command = new UpdateProductCommand { ProductDto = productDto };
            var result = await _mediator.Send(command);

            if (result == null)
                return NotFound();

            return Ok(result);
        }

        [HttpPatch("{id}/stock")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            _logger.LogInformation($"Recibida solicitud para actualizar stock del producto {id}");

            // Validación básica de coherencia de datos
            if (id != stockDto.Id)
            {
                _logger.LogWarning($"Discrepancia de IDs en la solicitud: ruta={id}, cuerpo={stockDto.Id}");
                return BadRequest(new { message = ErrorMessages.IdMismatch });
            }

            var command = new UpdateStockCommand { StockDto = stockDto };
            var result = await _mediator.Send(command);

            _logger.LogInformation($"Procesada solicitud para actualizar stock del producto {id}, éxito: {result.IsSuccess}");

            return result.ToActionResult(this);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}