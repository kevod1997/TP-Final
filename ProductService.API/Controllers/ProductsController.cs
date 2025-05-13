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

        // GET: api/Products
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            _logger.LogInformation("Solicitando listado de todos los productos");
            var query = new GetAllProductsQuery();
            var result = await _mediator.Send(query);

            return result.ToActionResult(this);
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            _logger.LogInformation($"Solicitando producto con ID: {id}");
            var query = new GetProductByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return result.ToActionResult(this);
        }

        // POST: api/Products
        [HttpPost]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto productDto)
        {
            _logger.LogInformation($"Creando nuevo producto: {productDto.Name}");
            var command = new CreateProductCommand { ProductDto = productDto };
            var result = await _mediator.Send(command);

            if (result.IsSuccess)
            {
                // Para creación, usamos CreatedAtAction para devolver 201 Created
                return CreatedAtAction(nameof(GetById), new { id = result.Value.Id }, result.Value);
            }

            // Para los errores, usamos el ToActionResult estándar
            return result.ToActionResult(this);
        }

        // PUT: api/Products/5
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto productDto)
        {
            _logger.LogInformation($"Actualizando producto con ID: {id}");

            if (id != productDto.Id)
            {
                _logger.LogWarning($"El ID en la ruta ({id}) no coincide con el ID en el cuerpo ({productDto.Id})");
                return BadRequest(ErrorMessages.IdMismatch);
            }

            var command = new UpdateProductCommand { ProductDto = productDto };
            var result = await _mediator.Send(command);

            return result.ToActionResult(this);
        }

        // PATCH: api/Products/5/stock
        [HttpPatch("{id}/stock")]
        [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductDto>> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            _logger.LogInformation($"Actualizando stock del producto con ID: {id}, cantidad: {stockDto.Quantity}");

            if (id != stockDto.Id)
            {
                _logger.LogWarning($"El ID en la ruta ({id}) no coincide con el ID en el cuerpo ({stockDto.Id})");
                return BadRequest(ErrorMessages.IdMismatch);
            }

            var command = new UpdateStockCommand { StockDto = stockDto };
            var result = await _mediator.Send(command);

            return result.ToActionResult(this);
        }

        // DELETE: api/Products/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogInformation($"Eliminando producto con ID: {id}");

            var command = new DeleteProductCommand { Id = id };
            var result = await _mediator.Send(command);

            // Usamos el método específico para DELETE que devuelve NoContent para éxito
            return result.ToActionResultWithNoContent(this);
        }
    }
}