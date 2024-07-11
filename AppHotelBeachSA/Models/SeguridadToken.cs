using System.ComponentModel.DataAnnotations;
namespace AppHotelBeachSA.Models
{
    public class SeguridadToken
    {
        public string Email { get; set; }

        [Required(ErrorMessage = "Debe ingresar el Token enviado por email")]
        public string Token { get; set; }

        [Required(ErrorMessage = "Debe ingresar el nuevo password")]
        [DataType(DataType.Password)]
        public string NuevoPassword { get; set; }

        [Required(ErrorMessage = "Es importante la confirmacion del nuevo password")]
        [DataType(DataType.Password)]
        public string Confirmar { get; set; }
    }
}
