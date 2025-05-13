using MediatR;
using ProductService.Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrabajoFinal.Common.Shared.Constants;
using TrabajoFinal.Common.Shared.Logging;
using TrabajoFinal.Common.Shared.Results;

namespace ProductService.Application.Commands
{
    public class DeleteProductCommand : IRequest<Result<bool>>
    {
        public int Id { get; set; }
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILoggerService _logger;

        public DeleteProductCommandHandler(
            IProductRepository productRepository,
            ILoggerService logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation($"Procesando la eliminación del producto con ID: {request.Id}");

                if (!await _productRepository.ExistsAsync(request.Id))
                {
                    _logger.LogWarning($"Producto con ID {request.Id} no encontrado para eliminar");
                    return Result<bool>.NotFound(ErrorMessages.Product.NotFound);
                }

                await _productRepository.DeleteAsync(request.Id);

                _logger.LogInformation($"Producto con ID {request.Id} eliminado exitosamente");

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar el producto con ID: {request.Id}");
                return Result<bool>.Failure($"Error al eliminar el producto: {ex.Message}");
            }
        }
    }
}