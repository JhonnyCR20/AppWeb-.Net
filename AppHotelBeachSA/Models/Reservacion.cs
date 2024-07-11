using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AppHotelBeachSA.Models
{
    public class Reservacion
    {
        public Reservacion()
        {
            Impuesto = 0.13;
        }

        [Key]
        public int idReservacion { get; set; }

        [Required]
        [DisplayName("Nombre Cliente")]
        public string? NombreCliente { get; set; }

        [DisplayName("Correo")]
        [DataType(DataType.EmailAddress)]
        public string CorreoElectronico { get; set; }
        [DisplayName("Fecha reserva")]
        public DateTime FechaReserva { get; set; }
        [DisplayName("Cantidad Noches")]
        public int CantidadNoches { get; set; }

        [DisplayName("Cantidad de Personas")]
        public int CantidadPersonas { get; set; }
        [DisplayName("Forma Pago")]
        public string? FormaPago { get; set; }

        [DisplayName("Nombre Banco")]
        public string? NombreBanco { get; set; }
        [DisplayName("Numero de Cheque")]
        public string? NumeroCheque { get; set; }
        public double Precio { get; set; }
        [DisplayName("Paquete escogido")]
        public string? PaqueteEscogido { get; set; }
        public double Subtotal { get; set; }
        public double Descuento { get; set; }
        public double Impuesto { get; }
        [DisplayName("Monto Total")]
        public double MontoTotal { get; set; }

        [DisplayName("Precio en Colones")]
        public double PrecioColones{ get; set; }
        public double Prima { get; set; }
        [DisplayName("Pago Mensual")]
        public double PagoMensual { get; set; }
    }
}
