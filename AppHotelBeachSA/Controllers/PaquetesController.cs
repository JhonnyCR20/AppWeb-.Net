using AppHotelBeachSA.Data;
using AppHotelBeachSA.Models;
using AppHotelBeachSA.ServiciosApiHotel;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;


namespace AppHotelBeachSA.Controllers
{
    public class PaquetesController : Controller
    {
        private readonly ServiciosApiPaquetes apiPaquetes;

        public List<Paquete> paquetes;

        //variable para mejorar la conexion con la api
        private TipoCambioApi api;
        //variable para menejar la instancia del objeto json
        public static TipoCambio varTipoCambio;
        public PaquetesController()
        {
            apiPaquetes = new ServiciosApiPaquetes();

            this.extraerTipoCambio();
        }
        public async Task<IActionResult> Index()
        {

            paquetes = await apiPaquetes.ListaPaquetes();
            return View(paquetes);

        }


        [HttpGet]
        public IActionResult CrearPaquetes()
        {

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                return View();
            }

            return RedirectToAction("Login", "Usuarios");


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearPaquetes([Bind] Paquete pPaquete)
        {
            bool respuesta;

            var temp = await apiPaquetes.BuscarPaqueteNombre(pPaquete.NombrePaquete);

            if (temp == null)
            {
                pPaquete.ProcentajePrima = pPaquete.ProcentajePrima / 100;


                respuesta = await apiPaquetes.Agregar(pPaquete);

                if (respuesta)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return View();
                }
            }
            else
            {

                TempData["MensajeError"] = "Ya existe un paquete que tiene ese nombre. Escoge otro";
                return View();
            }




        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {

            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Paquete paquete;
                paquete = await apiPaquetes.BuscarPaquete(id);
                if (paquete == null)
                {
                    return NotFound();
                }
                return View(paquete);
            }

            return RedirectToAction("Login", "Usuarios");



        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([Bind] Paquete pPaquete, int id)
        {
            bool respuesta;
            pPaquete.ProcentajePrima = pPaquete.ProcentajePrima/100;
            respuesta = await apiPaquetes.Editar(pPaquete, id);

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
        public async Task<IActionResult> Eliminar(int id)
        {
            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Paquete paquete;
                paquete = await apiPaquetes.BuscarPaquete(id);
                if (paquete == null)
                {
                    return NotFound();
                }
                return View(paquete);
            }

            return RedirectToAction("Login", "Usuarios");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int? id)
        {
            bool respuesta;


            respuesta = await apiPaquetes.Eliminar(id);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
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


    }
}
