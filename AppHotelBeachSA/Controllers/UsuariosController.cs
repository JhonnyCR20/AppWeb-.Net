using Microsoft.AspNetCore.Mvc;
using AppHotelBeachSA.ServiciosApiHotel;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using AppHotelBeachSA.Models;
using NuGet.Common;

namespace AppHotelBeachSA.Controllers
{
    public class UsuariosController : Controller
    {

        //variable para manejar la instacia del objeto json
        private readonly ServiciosApiUsuarios apiUsuarios;
        public static string EmailRestablecer = "";
        public static string CorreoToken = "";
        public static string Token = "";

        public UsuariosController()
        {
            apiUsuarios = new ServiciosApiUsuarios();
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Login()
        {
            return View();
        }//cierre get login

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([Bind] Usuario usuarios)
        {
            var temp = await this.ValidarUsuario(usuarios);

            if (temp != null)
            {
                bool restablecer = false;

                restablecer = await this.VerificarRestablecer(usuarios);

                if (restablecer)
                {
                    return RedirectToAction("Restablecer", "Usuarios", new { Email = temp.Email });
                }
                else
                {
                    var userClaims = new List<Claim>() { 
                     new Claim(ClaimTypes.Email, temp.Email),
                     new Claim(ClaimTypes.Name, temp.NombreCompleto),
                     new Claim(ClaimTypes.Role, temp.RolAcceso)};
                    var grandmaIdentity = new ClaimsIdentity(userClaims, "User identity");
                    var userPrincipal = new ClaimsPrincipal(new[] { grandmaIdentity });
                   await HttpContext.SignInAsync(userPrincipal);
                    return RedirectToAction("Index", "Home");
                }

            }
            TempData["Mensaje"] = "Error el usuario o contraseña son incorectos ..";
            return View(usuarios);
        }//cierre post login


        private async Task<Usuario> ValidarUsuario(Usuario usuarios)
        {
            Usuario autorizado = null;
            //se busca el usuario en la base de datos con el email autenticado
            var user = await apiUsuarios.BuscarUsuarios(usuarios.Email);

            //se pregunta si existen datos del usuario autenticado
            if (user != null)
            {//se verifica su password
                if (user.Email.Equals(usuarios.Email))
                {
                    autorizado = user;
                }
            }
            return autorizado;
        }//cierre validar usario

        private async Task<bool> VerificarRestablecer(Usuario usuarios)
        {
            bool verificado = false;
            //consultar los datos del usuario 
            var user = await apiUsuarios.BuscarUsuarios(usuarios.Email);
            if (user != null)
            {//si restablecer esta en cero quiere decir que es la primera vez que inicia sesion y cambiar la clave
                if (user.Restablecer == 0)
                {
                    verificado = true;
                }
            }
            return verificado;
        }//cierre verificar restablecer



        [HttpGet]
        public async Task<IActionResult> CrearUsuario()
        {
            return View();
        }//cierre get crear usuario

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario([Bind] Usuario usuarios)
        {
            string correo = usuarios.Email;
            if (usuarios != null)
            {
                var user = await apiUsuarios.BuscarUsuarios(correo);
                if (user == null)
                {
                    bool respuesta;
                    usuarios.FechaRegistro = DateTime.Now;
                    usuarios.Estado = 'A';
                    usuarios.RolAcceso = "Particular";

                    usuarios.Password = this.GenerarClave();

                    respuesta = await apiUsuarios.Agregar(usuarios);

                    try
                    {
                        if (await EnviarEmail(usuarios) && respuesta)
                        {
                            TempData["MensajeCreado"] = "Usuario creado correactamente. Su contraseña fue enviada por email.";
                            return RedirectToAction("Login", "Usuarios");

                        }
                        else
                        {
                            TempData["MensajeCreado"] = "Usaurio creado pero no se envio el email. Comuniquese con el administrador";
                        }
                    }//ciere try
                    catch (Exception ex)
                    {
                        TempData["MensajeCreado"] = "No se logro crear la cuenta.." + ex.Message;
                    }//cierre catch
                    return View();
                }//cierre if temp == null
                else
                {
                    TempData["MensajeCreado"] = "Ya existe un Usuario con este correo..";
                    return View();
                }//cierre else temp == null

            }//cierre IF usuarios != null
            else
            {
                TempData["MensajeCreado"] = "No se logro crear la cuenta..";
                return View();

            }//cierre else usuarios != null




        }//cierre post Crear usuario


        private string GenerarClave()
        {
            Random random = new Random();
            string clave = string.Empty;
            clave = "ABCDEFGHIJKLMNOPQRSTUVWXYZ123456789";

            //SE GENERA UNA CLAVE TEMPORAL
            return new string(Enumerable.Repeat(clave, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }//CIERRE GENERAR CLAVE


        private async Task<bool> EnviarEmail(Usuario usuarios)
        {
            try
            {
                //variable control 
                bool enviado = false;
                var temp = await apiUsuarios.BuscarUsuarios(usuarios.Email);

                //ser instancia el objeto 
                Email email = new Email();

                //se utiliza el metodo para enviar el email
                email.EnviarClave(temp);

                //se indica que se envio 
                enviado = true;

                //enviamos el valor 
                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//cierre enviar email


        [HttpGet]
        public async Task<IActionResult> Restablecer(string? Email)
        {
            var temp = await apiUsuarios.BuscarUsuarios(Email);

            SeguridadRestablecer seguridadRestablecer = new SeguridadRestablecer();
            seguridadRestablecer.Email = temp.Email;
            seguridadRestablecer.Password = temp.Password;
            EmailRestablecer = temp.Email;
            return View(seguridadRestablecer);

        }//cierre get restablecer


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Restablecer([Bind] SeguridadRestablecer pRestablecer)
        {
            bool respuesta = false;
            if (pRestablecer != null)
            {
                var temp = await apiUsuarios.BuscarUsuarios(EmailRestablecer);


                if (temp.Password.Equals(pRestablecer.Password))
                {
                    if (pRestablecer.NuevoPassword.Equals(pRestablecer.Confirmar))
                    {
                        temp.Password = pRestablecer.Confirmar;
                        temp.Restablecer = 1;

                        respuesta = await apiUsuarios.Editar(temp, temp.Email);

                        if (respuesta)
                        {
                            return RedirectToAction("Login", "Usuarios");

                        }//cierre if respuesta
                        else
                        {
                            TempData["MensajeError"] = "Ocurrio un error...";
                            return View(pRestablecer);
                        }//cierre else respuesta


                    }//Cierre if la confirmacion del nuevo password
                    else
                    {
                        TempData["MensajeError"] = "La confirmacion de la contraseña no es correcta...";
                        return View(pRestablecer);
                    }//cierre else confirfacion nuevo password

                }//cierre if verificar contraseña actual
                else
                {
                    TempData["MensajeError"] = "La contraseña actual es incorrecta";
                    return View(pRestablecer);
                }//cierre else verificar contraseña actual

            }//cierre if pRestablecer != null
            else
            {
                TempData["MensajeError"] = "Datos incorrectos...";
                return View(pRestablecer);
            }//cierre else pRestablecer != null
        }//cierre post restablecer


        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }//cierrre Logout
         ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        private string GenerarToken()
        {
            Random random = new Random();
            string clave = string.Empty;
            clave = "1234567890";

            return new string(Enumerable.Repeat(clave, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }//Cierre generar token

        private async Task<bool> EnviarToken(SeguridadToken temp)
        {
            try
            {
                //variable control 
                bool enviado = false;

                //ser instancia el objeto 
                Email email = new Email();

                //se utiliza el metodo para enviar el email
                email.EnviarToken(temp);

                //se indica que se envio 
                enviado = true;

                //enviamos el valor 
                return enviado;
            }
            catch (Exception ex)
            {
                return false;
            }
        }//cierre enviar token
        [HttpGet]
        public  IActionResult EmailToken()
        {
            return View();
        }//cierre get EmailToken

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmailToken([Bind] SeguridadToken pSeguridad)
        {
            var temp = await apiUsuarios.BuscarUsuarios(pSeguridad.Email);

            if (temp != null)
            {
                SeguridadToken seguridad = new SeguridadToken();
                seguridad.Email = temp.Email;
                seguridad.Token = this.GenerarToken();
                CorreoToken = temp.Email;
                Token = seguridad.Token;

                try
                {
                    if (await EnviarToken(seguridad))
                    {
                        return RedirectToAction("RestablecerClave", "Usuarios");
                    }
                    else
                    {
                        TempData["MensajeCreado"] = "No se envio el email.Comuniquese con el administrador.";
                        return View();
                    }
                }//cierre try
                catch (Exception ex)
                {
                    TempData["MensajeCreado"] = "No se logro enviar.. " + ex.Message;

                    return View();
                }//cierre catch
            }//cierre if temp != null
            else
            {
                TempData["MensajeCreado"] = "No existe el correo digitado";
                return RedirectToAction("EmailToken", "Usuarios");
            }//cierre else temp != null
        }//cierre post EmailToken

        [HttpGet]
        public async Task<IActionResult> RestablecerClave()
        {
            return View();
        }//cierre get RestablecerClave

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestablecerClave([Bind] SeguridadToken pSeguridad)
        {
            bool respuesta = false;
            if (pSeguridad != null)
            {
                var temp = await apiUsuarios.BuscarUsuarios(CorreoToken);

                if (pSeguridad.Token.Equals(Token))
                {
                    if (pSeguridad.NuevoPassword.Equals(pSeguridad.Confirmar))
                    {
                        temp.Password = pSeguridad.Confirmar;
                        respuesta = await apiUsuarios.Editar(temp, temp.Email);

                        if (respuesta)
                        {
                            return RedirectToAction("Login", "Usuarios");
                        }//cierre if respuesta
                        else
                        {
                            TempData["MensajeError"] = "Ocurrio un error...";
                            return View(pSeguridad);
                        }//cierre else respuesta

                    }//cierre if confirmacion contraseñasa
                    else
                    {
                        TempData["MensajeError"] = "La confirmacion de la contraseña no es correcta...";
                        return View(pSeguridad);
                    }//cierre else confirmacion contraseña

                }//cierre if confirmacion token
                else
                {
                    TempData["MensajeError"] = "Numero de Token Incorrecto";
                    return View(pSeguridad);
                }//cierre else confirmacion token

            }//cierre if pSeguridad != null
            else
            {
                TempData["MensajeError"] = "Datos incorrectos...";
                return View(pSeguridad);
            }//cierre else pSeguridad != null
        }//cierre post restablecer clave




    }//Cierre clase
}//Cierre nameespace
