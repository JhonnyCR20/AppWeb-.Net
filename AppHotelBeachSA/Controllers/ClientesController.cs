using Microsoft.AspNetCore.Mvc;
using AppHotelBeachSA.Models;

//Librerias 
using AppHotelBeachSA.ServiciosApiHotel;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;


namespace AppHotelBeachSA.Controllers
{
    public class ClientesController : Controller
    {



        //variable para manejar instancia del objeto Json

        private readonly ServiciosApiClientes apiClientes;
        public readonly Cliente cliente;

        public static List<Cliente> listaClientes;


        List<Cliente> clientes;
        public ClientesController()
        {
            apiClientes = new ServiciosApiClientes();
        }
        public async Task<IActionResult> Index()
        {

            if (User.Identity.IsAuthenticated&& User.IsInRole("Admin"))
            {
                listaClientes = await apiClientes.ListaClientes();

                return View(listaClientes);
            }

            return RedirectToAction("Login", "Usuarios");

        }



        [HttpGet]
        public async Task<ActionResult> CrearCliente()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CrearCliente([Bind] Cliente pCliente)
        {
            bool respuesta;

            if (pCliente.TipoCedula == "vacio")
            {
                TempData["Mensaje"] = "Debes escoger un tipo de cedula";
                return RedirectToAction("CrearCliente", "Clientes");
            }
            if (ModelState.IsValid)
            {
                var temp = await apiClientes.BuscarCedula(pCliente.Cedula);
                if (temp == null)
                {

                    respuesta = await apiClientes.Agregar(pCliente);
                    if (respuesta)
                    {
                        if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
                        {
                            return RedirectToAction("Index");
                        }
                        else
                        {
                            return RedirectToAction("ValidarCliente", "Reservacion");
                        }
                        
                    }
                    else
                    {
                        return View();
                    }
                }
                else
                {
                    TempData["Mensaje"] = "Ya existe un cliente con ese numero de cedula o pasaporte";
                    return RedirectToAction("CrearCliente", "Clientes");
                }

            }
            else
            {
                return View(pCliente); // Vuelve a mostrar la vista con el modelo actualizado
            }
        }

        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {


            if (User.Identity.IsAuthenticated && User.IsInRole("Admin"))
            {
                Cliente cliente;
                cliente = await apiClientes.BuscarCliente(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }

            return RedirectToAction("Login", "Usuarios");




        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar([Bind] Cliente pCliente, int idCliente)
        {
            bool respuesta;

            respuesta = await apiClientes.Editar(pCliente, idCliente);

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
                Cliente cliente;
                cliente = await apiClientes.BuscarCliente(id);
                if (cliente == null)
                {
                    return NotFound();
                }
                return View(cliente);
            }

            return RedirectToAction("Login", "Usuarios");


        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int? id)
        {
            bool respuesta;


            respuesta = await apiClientes.Eliminar(id);

            if (respuesta)
            {
                return RedirectToAction("Index");
            }
            else
            {
                return View();
            }
        }
    }//fin del controller
}//fin del namespace
