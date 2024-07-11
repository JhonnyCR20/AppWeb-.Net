using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AppHotelBeachSA.Models
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [DisplayName("Correo")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DisplayName("Nombre Completo")]
        public string NombreCompleto { get; set; }
        [Required]
        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DisplayName("Fecha de Registro")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:d}")]
        public DateTime FechaRegistro { get; set; }
        public char Estado { get; set; }
        public int Restablecer { get; set; }
        public string? RolAcceso { get; set; }

    }
}
