using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppHotelBeachSA.Models
{
    public class Paquete
    {
        [Key]
        public int IdPaquete { get; set; }

        [Required(ErrorMessage = "Debe digitar el nombre del paquete")]

        [DisplayName("Nombre Paquete")]
        public string NombrePaquete { get; set; } = null!;

        public double Precio { get; set; }



        [DisplayName("Porcentaje Prima")]
        public double ProcentajePrima { get; set; }

        [DisplayName("Numero de mensualidades")]
        public int NumMensualidades { get; set; }




    }
}
