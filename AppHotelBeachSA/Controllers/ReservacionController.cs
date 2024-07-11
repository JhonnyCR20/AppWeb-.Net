using AppHotelBeachSA.Data;
using AppHotelBeachSA.Models;
using AppHotelBeachSA.ServiciosApiHotel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppHotelBeachSA.Controllers
{
    public class ReservacionController : Controller
    {
        private readonly ServiciosApiReservacion apiReservacion;
        private readonly ServiciosApiPaquetes apiPaquetes;
        private readonly ServiciosApiClientes apiClientes;
        private readonly IWebHostEnvironment webHostEnvironment;

        public List<Reservacion> reservaciones;
        public static List<Paquete> listaPaquetes;
        private double montoTotalPrima;
        public static Reservacion reservaMostrar = null;
        public static Reservacion informe = null;
        //variable para mejorar la conexion con la api
        private TipoCambioApi api;
        //variable para menejar la instancia del objeto json
        public static TipoCambio varTipoCambio;

        public ReservacionController(IWebHostEnvironment webHost)
        {
            apiReservacion = new ServiciosApiReservacion();
            apiPaquetes = new ServiciosApiPaquetes();
            apiClientes = new ServiciosApiClientes();
            this.extraerTipoCambio();
            webHostEnvironment = webHost;
        }
        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                reservaciones = await apiReservacion.ListaReservaciones();
                return View(reservaciones);
            }

            return RedirectToAction("Login", "Usuarios");


        }

        [HttpGet]
        public async Task<IActionResult> CrearReserva()
        {


            listaPaquetes = await apiPaquetes.ListaPaquetes();

            // Lógica para configurar los modelos

            ViewBag.listaPaquetes = listaPaquetes;
            reservaMostrar.FechaReserva = DateTime.Now;

            return View(reservaMostrar);

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearReserva([Bind] Reservacion pReservacion, string? chequeBanco, string? numeroCheque)
        {

            if (pReservacion.CantidadPersonas <= 0 || pReservacion.CantidadNoches <= 0)
            {
                TempData["Mensaje"] = "No puedes selecionar valores negativos";
                return RedirectToAction("CrearReserva", "Reservacion");
            }


            if (pReservacion.PaqueteEscogido == "vacio")
            {
                TempData["Mensaje"] = "Debes escoger una paquete";
                return RedirectToAction("CrearReserva", "Reservacion");
            }


            if (pReservacion.FormaPago == "vacio")
            {
                TempData["Mensaje"] = "Debes escoger una forma de pago";
                return RedirectToAction("CrearReserva", "Reservacion");
            }
            else
            {
                if (pReservacion.FormaPago == "Cheque")
                {
                    pReservacion.NombreBanco = chequeBanco;
                    pReservacion.NumeroCheque = numeroCheque;
                }

            }



            var paquete = await apiPaquetes.BuscarPaqueteNombre(pReservacion.PaqueteEscogido);
            pReservacion.Precio = paquete.Precio;
            pReservacion.Subtotal = CalcularSubtotal(pReservacion.Precio, pReservacion.CantidadNoches, pReservacion.CantidadPersonas);
            pReservacion.Descuento = CalcularDescuento(pReservacion.FormaPago, pReservacion.CantidadNoches);
            pReservacion.MontoTotal = CalcularMontoTotal(pReservacion.Subtotal, pReservacion.Descuento, pReservacion.Impuesto);
            pReservacion.Prima = CalcularPrima(pReservacion.MontoTotal, paquete.ProcentajePrima);
            pReservacion.PagoMensual = CalcularMensualidad(paquete.NumMensualidades);
            pReservacion.PrecioColones = pReservacion.MontoTotal * varTipoCambio.venta;

            informe = pReservacion;

            return RedirectToAction("InformePrevio", "Reservacion");

        }

        [HttpGet]
        public async Task<IActionResult> Eliminar(int id)
        {

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Reservacion reserva;
                reserva = await apiReservacion.Buscar(id);

                if (reserva == null)
                {
                    return NotFound();
                }
                return View(reserva);
            }

            return RedirectToAction("Login", "Usuarios");



        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int? id)
        {
            bool respuesta;


            respuesta = await apiReservacion.Eliminar(id);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }



        }


        [HttpGet]
        public IActionResult ValidarCliente()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }

            return RedirectToAction("Login", "Usuarios");


        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ValidarCliente([Bind] Cliente pCliente)
        {
            Reservacion clienteReserva = new Reservacion();
            var clienteTemp = await apiClientes.BuscarCedula(pCliente.Cedula);
            if (clienteTemp != null)
            {

                clienteReserva.NombreCliente = clienteTemp.NombreCompleto;
                clienteReserva.CorreoElectronico = clienteTemp.CorreoElectronico;
                reservaMostrar = clienteReserva;
                return RedirectToAction("CrearReserva", "Reservacion");
            }
            else
            {
                return RedirectToAction("CrearCliente", "Clientes");
            }


        }



        [HttpGet]
        public IActionResult InformePrevio()
        {
            return View(informe);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> InformePrevio([Bind] Reservacion pReservacion)
        {
            pReservacion.FormaPago = informe.FormaPago;
            if(pReservacion.FormaPago == "Cheque")
            {
                pReservacion.NombreBanco=informe.NombreBanco;
            }
            bool respuesta;
            respuesta = await apiReservacion.Agregar(pReservacion);

            if (respuesta)
            {
                try
                {
                    // Crear el documento
                    Document document = new Document();

                    // Crear el escritor PDF
                    PdfWriter writer = PdfWriter.GetInstance(document, new FileStream("reservacion.pdf", FileMode.Create));

                    // Abrir el documento
                    document.Open();

                    // Configurar las propiedades del documento
                    document.SetPageSize(PageSize.A4);
                    document.SetMargins(30, 30, 30, 30);
                    document.AddCreationDate();

                    // Agregar el encabezado
                    PdfPTable headerTable = new PdfPTable(3);
                    headerTable.WidthPercentage = 100;
                    headerTable.DefaultCell.BackgroundColor = new BaseColor(250, 250, 210);
                    headerTable.DefaultCell.Padding = 10f;
                    headerTable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER;

                    // Agregar el logotipo o cualquier otro contenido del encabezado
                    // Obtener la ruta de la imagen
                    string rutaImagen = Path.Combine(webHostEnvironment.WebRootPath, "css/img/logo.png");

                    // Crear la imagen y establecer su tamaño
                    iTextSharp.text.Image logo = iTextSharp.text.Image.GetInstance(rutaImagen);
                    logo.ScaleToFit(50, 50); // Ancho, alto

                    // Agregar la imagen a la celda
                    headerTable.AddCell(logo);

                    // Crear un objeto BaseColor con el valor hexadecimal
                    BaseColor color = new BaseColor(0xDE, 0xB8, 0x87);

                    headerTable.AddCell(new Phrase("Hotel Beach S.A", new Font(Font.FontFamily.HELVETICA, 20, Font.BOLD, color)));
                    headerTable.AddCell(new Phrase("123 Avenida Ocean, Hotel Beach, PB123\n+506 2696 4884 / 2696 8448\nhotelbeachsa_@outlook.com", new Font(Font.FontFamily.HELVETICA, 10, Font.NORMAL, color)));

                    document.Add(headerTable);

                    // Agregar el contenido del PDF
                    document.Add(Chunk.NEWLINE);

                    // Agregar los datos del cliente
                    Paragraph clienteParagraph = new Paragraph("Datos del cliente", new Font(Font.FontFamily.HELVETICA, 11, Font.UNDERLINE | Font.BOLD));
                    document.Add(clienteParagraph);

                    // Agregar los datos del cliente
                    document.Add(new Chunk("Nombre completo: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                    document.Add(new Chunk(pReservacion.NombreCliente, new Font(Font.FontFamily.HELVETICA, 10)));
                    document.Add(Chunk.NEWLINE); // Agregar un salto de línea
                    document.Add(new Chunk("Correo electrónico: ", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)));
                    document.Add(new Chunk(pReservacion.CorreoElectronico, new Font(Font.FontFamily.HELVETICA, 10)));

                    // Crear la tabla
                    PdfPTable reservaTable = new PdfPTable(2); // Número de columnas: 2
                    reservaTable.WidthPercentage = 100;

                    // Agregar las celdas a la tabla
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Fecha de reserva", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.FechaReserva.ToString("dd/MM/yyyy"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Cantidad de Personas", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.CantidadPersonas.ToString(), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Cantidad de noches reservadas", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.CantidadNoches.ToString(), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Forma de Pago", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.FormaPago, new Font(Font.FontFamily.HELVETICA, 10)))));
                    if (pReservacion.FormaPago == "Cheque")
                    {
                        reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Nombre de banco", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                        reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.NombreBanco, new Font(Font.FontFamily.HELVETICA, 10)))));
                        reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Numero de cheque", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                        reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.NumeroCheque, new Font(Font.FontFamily.HELVETICA, 10)))));
                    }
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Paquete reservado", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.PaqueteEscogido, new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Subtotal", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.Subtotal.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Descuento", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.Descuento.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Impuesto", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.Impuesto.ToString(), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Monto total", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.MontoTotal.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Precio en Colones", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.PrecioColones.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Prima", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.Prima.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk("Pagos mensuales", new Font(Font.FontFamily.HELVETICA, 10, Font.BOLD)))));
                    reservaTable.AddCell(new PdfPCell(new Phrase(new Chunk(pReservacion.PagoMensual.ToString("F"), new Font(Font.FontFamily.HELVETICA, 10)))));

                    document.Add(reservaTable);

                    // Agregar el texto adicional
                    document.Add(Chunk.NEWLINE);

                    // Primer párrafo
                    Paragraph paragraph1 = new Paragraph();
                    paragraph1.Alignment = Element.ALIGN_JUSTIFIED;
                    paragraph1.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    paragraph1.Add("Estimado huésped,\r\n\r\nEn nombre de todo el equipo de Hotel Beach S.A., nos complace agradecerle sinceramente por haber elegido nuestro hotel para su estadía. Valoramos su confianza y nos comprometemos a brindarle una experiencia excepcional llena de comodidad y hospitalidad.\r\n\r\nQueremos asegurarnos de que su estancia sea lo más placentera posible, por lo que nos gustaría compartir algunos detalles importantes con usted:\r\n\r\nPolíticas de Cancelación:\r\nEntendemos que los planes pueden cambiar y comprendemos la necesidad de flexibilidad. Si necesita realizar cambios en su reserva o cancelarla, le recomendamos que nos informe con anticipación según nuestras políticas de cancelación. De esta manera, podemos ofrecerle opciones y evitar posibles cargos por cancelaciones tardías.\r\n\r\nCheck-in y Check-out:\r\nPara garantizar una experiencia fluida, nuestro horario de check-in es a partir de las 14:00 horas, y el check-out es a más tardar a las 12:00 horas del día de su partida. Si planea llegar antes o salir después de estos horarios, por favor, avísenos con anticipación para poder hacer los arreglos necesarios y asegurarnos de que su estadía sea cómoda.");

                    // Segundo párrafo
                    Paragraph paragraph2 = new Paragraph();
                    paragraph2.Alignment = Element.ALIGN_JUSTIFIED;
                    paragraph2.Font = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 11);
                    paragraph2.Add("Servicios del Hotel:\r\nEn Hotel Beach S.A., nos esforzamos por brindarle servicios de calidad que superen sus expectativas.\r\n\r\nPodrá disfrutar de nuestras modernas instalaciones, que incluyen una piscina al aire libre, un gimnasio bien equipado y un spa rejuvenecedor.\r\nAdemás, ofrecemos servicios de habitación las 24 horas, servicio de lavandería y limpieza diaria para asegurar su comodidad y satisfacción.\r\n\r\nNuestro amable personal está siempre a su disposición para ayudarlo con cualquier consulta o requerimiento adicional que pueda tener durante su estancia. No dude en comunicarse con nosotros en cualquier momento, ya sea personalmente en la recepción o mediante nuestro servicio de atención al cliente.\r\n\r\nUna vez más, le agradecemos por elegir Hotel Beach S.A. Esperamos que su estancia con nosotros sea inolvidable y que disfrute de todas las maravillas que nuestro hotel y su entorno tienen para ofrecer.\r\n\r\n¡Bienvenido y que tenga una estancia extraordinaria!\r\n\r\nAtentamente,\r\n\r\nEl equipo de Hotel Beach S.A");

                    document.Add(paragraph1);
                    document.Add(paragraph2);
                    // Crear el párrafo adicional
                    Paragraph additionalTextParagraph = new Paragraph();
                    additionalTextParagraph.Add(paragraph1);
                    additionalTextParagraph.Add(paragraph2);

                    // Cerrar el documento
                    document.Close();
                    // Cerrar el escritor PDF
                    writer.Close();
                }
                catch
                {

                }
                Email email = new Email();
                email.EnviarPdf(pReservacion);
                return RedirectToAction("Index", "Home");

            }
            else
            {
                return View();
            }






        }





        //-------------------------------------------------------------------------------------------------------------------



        private double CalcularDescuento(string formaPago, int cantidadNoches)
        {
            double descuento = 0.0;

            if (formaPago.Equals("Efectivo"))
            {
                if (cantidadNoches >= 3 && cantidadNoches <= 6)
                {
                    descuento = 0.10;// 10% 
                    return descuento;
                }
                else if (cantidadNoches >= 7 && cantidadNoches <= 9)
                {
                    descuento = 0.15;// 15% 
                    return descuento;
                }
                else if (cantidadNoches >= 10 && cantidadNoches <= 12)
                {
                    descuento = 0.20;// 20% 
                    return descuento;
                }
                else if (cantidadNoches >= 13)
                {
                    descuento = 0.25; // 25%
                    return descuento;
                }
            }

            return 0.0;
        }
        private double CalcularMontoTotal(double subTotal, double descuento, double impuesto)
        {
            double porcentajeDescuento = 0;
            double porcentejeImpuesto = 0;
            double montoTotal = 0;

            porcentajeDescuento = (subTotal * descuento);
            montoTotal = subTotal - porcentajeDescuento;
            porcentejeImpuesto = montoTotal * impuesto;
            montoTotal = montoTotal + porcentejeImpuesto;


            return montoTotal;

        }


        private double CalcularSubtotal(double precio, int cantidadNoches, int cantidadPersona)
        {
            double subtotal = 0;
            subtotal = precio * cantidadPersona;
            subtotal = subtotal * cantidadNoches;


            return subtotal;

        }

        private double CalcularPrima(double montoTotal, double porcentajePrima)
        {
            double primaTotal = 0;

            primaTotal = montoTotal * porcentajePrima;

            montoTotalPrima = montoTotal - primaTotal;
            return primaTotal;

        }

        private double CalcularMensualidad(int numMensualidades)
        {
            double mensualidad = 0;

            mensualidad = montoTotalPrima / numMensualidades;


            return mensualidad;

        }

        private async void extraerTipoCambio()
        {
            //reglas de negocio
            try
            {//intento de conexion con la api
             //se crea una instancia de la api
                this.api = new TipoCambioApi();
                //se obtiene el objeto client para consumir la api
                HttpClient client = this.api.Inicial();

                //aqui se utiliza el metodo de la api de gometa
                HttpResponseMessage response = await client.GetAsync("tdc/tdc.json");

                if (response.IsSuccessStatusCode)
                {
                    //aqui se lee los datos obtenidos del objeto json
                    var result = response.Content.ReadAsStringAsync().Result;

                    //se convierte el  objeto json al objeto tipoCambio del modelo
                    varTipoCambio = JsonConvert.DeserializeObject<TipoCambio>(result);
                }
            }//en caso de error capturamos el error con la ex
            catch (Exception ex)
            {

            }
        }//cierre extraer TipoCambio



    }//cierre class
}//cierre namespace 
