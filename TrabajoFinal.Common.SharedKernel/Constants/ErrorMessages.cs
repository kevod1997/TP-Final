using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrabajoFinal.Common.Shared.Constants
{
    /// <summary>
    /// Mensajes de error estándar para toda la aplicación
    /// </summary>
    public static class ErrorMessages
    {
        // Mensajes generales
        public const string ResourceNotFound = "El recurso solicitado no fue encontrado.";
        public const string InvalidRequest = "La solicitud es inválida.";
        public const string ServerError = "Se produjo un error en el servidor.";
        public const string ValidationError = "Error de validación en los datos de entrada.";
        public const string IdMismatch = "El ID en la ruta no coincide con el ID en el cuerpo de la solicitud.";
        public const string Unauthorized = "No está autorizado para realizar esta acción.";
        public const string Forbidden = "No tiene permisos para acceder a este recurso.";

        // Mensajes específicos de productos
        public static class Product
        {
            public const string NotFound = "No se encontró el producto con el ID especificado.";
            public const string InsufficientStock = "Stock insuficiente para realizar la operación.";
            public const string InvalidStock = "La cantidad de stock no puede ser negativa.";
            public const string InvalidPrice = "El precio no puede ser negativo.";
            public const string NameRequired = "El nombre del producto es obligatorio.";
            public const string IdRequired = "El ID del producto es obligatorio.";
            public const string DescriptionTooLong = "La descripción del producto es demasiado larga.";
            public const string QuantityZero = "La cantidad a actualizar no puede ser cero.";
        }

        // Mensajes específicos de clientes
        public static class Customer
        {
            public const string NotFound = "No se encontró el cliente con el ID especificado.";
            public const string EmailInvalid = "El correo electrónico no tiene un formato válido.";
            public const string NameRequired = "El nombre del cliente es obligatorio.";
            public const string AddressRequired = "La dirección del cliente es obligatoria.";
            public const string EmailRequired = "El correo electrónico del cliente es obligatorio.";
            public const string EmailAlreadyExists = "Ya existe un cliente con este correo electrónico.";
        }

        // Mensajes específicos de órdenes
        public static class Order
        {
            public const string NotFound = "No se encontró la orden con el ID especificado.";
            public const string CustomerNotFound = "No se encontró el cliente para esta orden.";
            public const string ProductNotFound = "Uno o más productos de la orden no fueron encontrados.";
            public const string EmptyOrder = "La orden debe contener al menos un producto.";
            public const string InsufficientStock = "No hay suficiente stock para uno o más productos de la orden.";
            public const string InvalidQuantity = "La cantidad debe ser mayor que cero.";
        }
    }
}