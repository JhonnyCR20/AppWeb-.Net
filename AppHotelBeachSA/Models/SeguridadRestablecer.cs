using System.ComponentModel.DataAnnotations;

namespace AppHotelBeachSA.Models
{
    public class SeguridadRestablecer
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar el password enviado por email")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nuevo password")]
        [DataType(DataType.Password)]
        public string NuevoPassword { get; set; }

        [Required(ErrorMessage = "Es importante la confirmacion del nuevo password")]
        [DataType(DataType.Password)]
        public string Confirmar { get; set; }
    }
}
