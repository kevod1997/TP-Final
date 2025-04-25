using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Commands;
using ProductService.Application.DTOs;
using ProductService.Application.Queries;
using TrabajoFinal.Common.SharedKernel.Models;
using FluentValidation;

namespace ProductService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<ProductDto>>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<IEnumerable<ProductDto>>>> GetAll()
        {
            try
            {
                var query = new GetAllProductsQuery();
                var products = await _mediator.Send(query);

                var response = ApiResponse<IEnumerable<ProductDto>>.SuccessResponse(
                    products,
                    "Productos obtenidos exitosamente"
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<IEnumerable<ProductDto>>.FailureResponse(
                    "Error al obtener productos",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }

        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> GetById(int id)
        {
            try
            {
                var query = new GetProductByIdQuery { Id = id };
                var product = await _mediator.Send(query);

                if (product == null)
                {
                    var notFoundResponse = ApiResponse<ProductDto>.FailureResponse(
                        $"No se encontró el producto con ID {id}",
                        new List<string> { $"El producto con ID {id} no existe" }
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<ProductDto>.SuccessResponse(
                    product,
                    "Producto obtenido exitosamente"
                );

                return Ok(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error al obtener el producto",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Create([FromBody] CreateProductDto productDto)
        {
            try
            {
                var command = new CreateProductCommand { ProductDto = productDto };
                var result = await _mediator.Send(command);

                var response = ApiResponse<ProductDto>.SuccessResponse(
                    result,
                    "Producto creado exitosamente"
                );

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error de validación",
                    errors
                );
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error al crear el producto",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }

        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> Update(int id, [FromBody] UpdateProductDto productDto)
        {
            try
            {
                if (id != productDto.Id)
                {
                    var badRequestResponse = ApiResponse<ProductDto>.FailureResponse(
                        "ID no coincidente",
                        new List<string> { "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" }
                    );
                    return BadRequest(badRequestResponse);
                }

                var command = new UpdateProductCommand { ProductDto = productDto };
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    var notFoundResponse = ApiResponse<ProductDto>.FailureResponse(
                        $"No se encontró el producto con ID {id}",
                        new List<string> { $"El producto con ID {id} no existe" }
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<ProductDto>.SuccessResponse(
                    result,
                    "Producto actualizado exitosamente"
                );

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error de validación",
                    errors
                );
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error al actualizar el producto",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }

        [HttpPatch("{id}/stock")]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponse<ProductDto>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProductDto>>> UpdateStock(int id, [FromBody] UpdateStockDto stockDto)
        {
            try
            {
                if (id != stockDto.Id)
                {
                    var badRequestResponse = ApiResponse<ProductDto>.FailureResponse(
                        "ID no coincidente",
                        new List<string> { "El ID en la URL no coincide con el ID en el cuerpo de la solicitud" }
                    );
                    return BadRequest(badRequestResponse);
                }

                var command = new UpdateStockCommand { StockDto = stockDto };
                var result = await _mediator.Send(command);

                if (result == null)
                {
                    var notFoundResponse = ApiResponse<ProductDto>.FailureResponse(
                        $"No se encontró el producto con ID {id}",
                        new List<string> { $"El producto con ID {id} no existe" }
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<ProductDto>.SuccessResponse(
                    result,
                    "Stock actualizado exitosamente"
                );

                return Ok(response);
            }
            catch (ValidationException ex)
            {
                var errors = ex.Errors.Select(e => e.ErrorMessage).ToList();
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error de validación",
                    errors
                );
                return BadRequest(response);
            }
            catch (Exception ex)
            {
                var response = ApiResponse<ProductDto>.FailureResponse(
                    "Error al actualizar el stock",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse<bool>), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<bool>>> Delete(int id)
        {
            try
            {
                var command = new DeleteProductCommand { Id = id };
                var result = await _mediator.Send(command);

                if (!result)
                {
                    var notFoundResponse = ApiResponse<bool>.FailureResponse(
                        $"No se encontró el producto con ID {id}",
                        new List<string> { $"El producto con ID {id} no existe" }
                    );
                    return NotFound(notFoundResponse);
                }

                var response = ApiResponse<bool>.SuccessResponse(
                    true,
                    "Producto eliminado exitosamente"
                );

                // Para respetar el estándar HTTP, devolvemos NoContent sin contenido,
                // aunque podríamos devolver Ok(response) si queremos mantener el mensaje
                return NoContent();
            }
            catch (Exception ex)
            {
                var response = ApiResponse<bool>.FailureResponse(
                    "Error al eliminar el producto",
                    new List<string> { ex.Message }
                );
                return StatusCode(500, response);
            }
        }
    }
}