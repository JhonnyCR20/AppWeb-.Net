
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;




namespace AppHotelBeachSA.Models
{
    public class Cliente
    {

        [Key]
        public int IdCliente { get; set; }

        [Required]
        public string? TipoCedula { get; set; } = null!;

        public string Cedula { get; set; }

        [Required]
        [DisplayName("Nombre Cliente")]
        public string? NombreCompleto { get; set; } = null!;
        public int Telefono { get; set; }


        public string? Direccion { get; set; }

        [DisplayName("Correo")]
        [DataType(DataType.EmailAddress)]
        public string? CorreoElectronico { get; set; }

    }
}
