using CustomerService.Application.Commands;
using CustomerService.Application.DTOs;
using CustomerService.Application.Queries;
using TrabajoFinal.Common.Shared.Results;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CustomerService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CustomersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CustomersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/Customers
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CustomerDto>>> GetCustomers()
        {
            var result = await _mediator.Send(new GetAllCustomersQuery());
            return result.ToActionResult(this);
        }

        // GET: api/Customers/5
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetCustomer(int id)
        {
            var result = await _mediator.Send(new GetCustomerByIdQuery { Id = id });
            return result.ToActionResult(this);
        }

        // GET: api/Customers/email/user@example.com
        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> GetCustomerByEmail(string email)
        {
            var result = await _mediator.Send(new GetCustomerByEmailQuery { Email = email });
            return result.ToActionResult(this);
        }

        // POST: api/Customers
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CustomerDto>> CreateCustomer([FromBody] CreateCustomerDto customerDto)
        {
            var result = await _mediator.Send(new CreateCustomerCommand { CustomerDto = customerDto });

            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetCustomer), new { id = result.Value.Id }, result.Value);

            return result.ToActionResult(this);
        }

        // PUT: api/Customers/5
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CustomerDto>> UpdateCustomer(int id, [FromBody] UpdateCustomerDto customerDto)
        {
            if (id != customerDto.Id)
                return BadRequest("El ID en la ruta no coincide con el ID en el cuerpo de la solicitud");

            var result = await _mediator.Send(new UpdateCustomerCommand { CustomerDto = customerDto });
            return result.ToActionResult(this);
        }

        // DELETE: api/Customers/5
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var result = await _mediator.Send(new DeleteCustomerCommand { Id = id });
            return result.ToActionResultWithNoContent(this);
        }
    }
}