using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CustomerService.Domain.Entities
{
    public class Customer
    {
        // Propiedades con acceso privado para setter para mantener la encapsulación
        public int Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public DateTime RegistrationDate { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        // Constructor privado para EF Core
        private Customer() { }

        // Constructor para crear nuevos clientes
        public Customer(string name, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del cliente no puede estar vacío", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email del cliente no puede estar vacío", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("El formato del email no es válido", nameof(email));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("La dirección del cliente no puede estar vacía", nameof(address));

            Name = name;
            Email = email;
            Address = address;
            RegistrationDate = DateTime.UtcNow;
        }

        // Métodos para modificar la entidad
        public void UpdateDetails(string name, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("El nombre del cliente no puede estar vacío", nameof(name));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("El email del cliente no puede estar vacío", nameof(email));

            if (!IsValidEmail(email))
                throw new ArgumentException("El formato del email no es válido", nameof(email));

            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("La dirección del cliente no puede estar vacía", nameof(address));

            Name = name;
            Email = email;
            Address = address;
            UpdatedAt = DateTime.UtcNow;
        }

        // Método privado para validar el formato del email
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            // Usando una expresión regular simple para validar el formato del email
            var regex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return regex.IsMatch(email);
        }
    }
}
