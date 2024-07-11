using System.Net.Mail;
using System.Net.Mime;
using System.Net;
using System.Text;

namespace AppHotelBeachSA.Models
{
    public class Email
    {

        public void EnviarClave(Usuario user)
        {
            //se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Datos de registro en plataforma web Hotel Beach";

            //Destinatarios
            email.To.Add(new MailAddress("supporhotelbeachljha123@outlook.com"));
            email.To.Add(new MailAddress(user.Email));

            //emisor del correo 
            email.From = new MailAddress("supporhotelbeachljha123@outlook.com");
           
            //se consutruye la vista html para el body del email
            string html = "Bienvenidos a nuestra plataforma Hotel Beach ";
            html += "<br> A continuacion detallamos los datos registrados en nuetra plataforma web";
            html += "<br><b>ID Login: </b>" + user.Id;
            html += "<br><b>Email: </b>" + user.Email;
            html += "<br><b>Su contraseña temporal: </b>" + user.Password;
            html += "<br><b>No responda este correo porque fue generado de forma automatica ";
            html += "por la plataforma web Hotel Beach </b>";

            //se indica el contenido esen html
            email.IsBodyHtml = true;

            //se indica la prioridad recomendacion debe ser prioridad normal 
            email.Priority = MailPriority.Normal;

            //se instancia la vista del html para el campo del body del email
            AlternateView view = AlternateView.CreateAlternateViewFromString(html,
                Encoding.UTF8, MediaTypeNames.Text.Html);

            //se agrega la vista html al cuerpo del email 
            email.AlternateViews.Add(view);

            //configuracion del protocolo de comunicacion smtp 
            SmtpClient smtp = new SmtpClient();

            //servidor de correo a implementar 
            smtp.Host = "smtp-mail.outlook.com";

            //puerto de comunicacion 
            smtp.Port = 587;

            //se indica si el buton utiliza seguridad tipo SSL 
            smtp.EnableSsl = true;

            //se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //se asignan los datos para los credenciales 
            smtp.Credentials = new NetworkCredential("supporhotelbeachljha123@outlook.com", "LuisJoshHarryAndre123");

            //metodo para enviar el email
            smtp.Send(email);

            email.Dispose();
            smtp.Dispose();
        }


        public void EnviarToken(SeguridadToken user)
        {
            //se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Datos para restablecer la contraseña en plataforma web Hotel Beach";

            //Destinatarios
            email.To.Add(new MailAddress("supporhotelbeachljha123@outlook.com"));
            email.To.Add(new MailAddress(user.Email));

            //emisor del correo 
            email.From = new MailAddress("supporhotelbeachljha123@outlook.com");

            //se consutruye la vista html para el body del email
            string html = "Bienvenidos a nuestra plataforma Hotel Beach ";
            html += "<br> A continuacion detallamos su numero de Token";
            html += "<br><b>Email: </b>" + user.Email;
            html += "<br><b>Numero de Token: </b>" + user.Token;
            html += "<br><b>No responda este correo porque fue generado de forma automatica ";
            html += "por la plataforma web Hotel Beach </b>";

            //se indica el contenido esen html
            email.IsBodyHtml = true;

            //se indica la prioridad recomendacion debe ser prioridad normal 
            email.Priority = MailPriority.Normal;

            //se instancia la vista del html para el campo del body del email
            AlternateView view = AlternateView.CreateAlternateViewFromString(html,
                Encoding.UTF8, MediaTypeNames.Text.Html);

            //se agrega la vista html al cuerpo del email 
            email.AlternateViews.Add(view);

            //configuracion del protocolo de comunicacion smtp 
            SmtpClient smtp = new SmtpClient();

            //servidor de correo a implementar 
            smtp.Host = "smtp-mail.outlook.com";

            //puerto de comunicacion 
            smtp.Port = 587;

            //se indica si el buton utiliza seguridad tipo SSL 
            smtp.EnableSsl = true;

            //se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //se asignan los datos para los credenciales 
            smtp.Credentials = new NetworkCredential("supporhotelbeachljha123@outlook.com", "LuisJoshHarryAndre123");

            //metodo para enviar el email
            smtp.Send(email);

            email.Dispose();
            smtp.Dispose();
        }

        public void EnviarPdf(Reservacion reservacion)
        {
            // Ruta del archivo PDF creado
            string filePath = "reservacion.pdf";

            //se crea una instancia del objeto email
            MailMessage email = new MailMessage();

            //Asunto
            email.Subject = "Datos de la reserva en plataforma web Hotel Beach";

            //Destinatarios
            email.To.Add(new MailAddress("supporhotelbeachljha123@outlook.com"));
            email.To.Add(new MailAddress(reservacion.CorreoElectronico));

            //emisor del correo 
            email.From = new MailAddress("supporhotelbeachljha123@outlook.com");

            // Adjuntar el archivo PDF
            Attachment attachment = new Attachment(filePath, MediaTypeNames.Application.Pdf);
            email.Attachments.Add(attachment);

            // Construir el cuerpo del correo en HTML
            string htmlBody = "<html>";
            htmlBody += "<head>";
            htmlBody += "<style>";
            htmlBody += "body { font-family: Arial, sans-serif; }";
            htmlBody += "h1 { color: #336699; }";
            htmlBody += "p { margin-bottom: 10px; }";
            htmlBody += "</style>";
            htmlBody += "</head>";
            htmlBody += "<body>";
            htmlBody += "<h1>Bienvenido(a) a Hotel Beach S.A.</h1>";
            htmlBody += "<p>Estimado(a) " + reservacion.NombreCliente + ",</p>";
            htmlBody += "<p>¡Nos complace informarle que se ha realizado una reserva a su nombre en nuestro hotel!</p>";
            htmlBody += "<p>Agradecemos su elección y esperamos brindarle una experiencia inolvidable en nuestro hotel.</p>";
            htmlBody += "<p>Si tiene alguna pregunta o requerimiento adicional, no dude en contactarnos. Nuestro equipo estará encantado de asistirle durante su estadía.</p>";
            htmlBody += "<p>¡Esperamos su llegada y que disfrute de una estancia cómoda y placentera en Hotel Beach S.A.!</p>";
            htmlBody += "<p>Atentamente,</p>";
            htmlBody += "<p>El equipo de Hotel Beach S.A</p>";
            htmlBody += "</body>";
            htmlBody += "</html>";

            //se indica el contenido esen html
            email.IsBodyHtml = true;

            //se indica la prioridad recomendacion debe ser prioridad normal 
            email.Priority = MailPriority.Normal;

            //se instancia la vista del html para el campo del body del email
            AlternateView view = AlternateView.CreateAlternateViewFromString(htmlBody,
                Encoding.UTF8, MediaTypeNames.Text.Html);

            //se agrega la vista html al cuerpo del email 
            email.AlternateViews.Add(view);

            //configuracion del protocolo de comunicacion smtp 
            SmtpClient smtp = new SmtpClient();

            //servidor de correo a implementar 
            smtp.Host = "smtp-mail.outlook.com";

            //puerto de comunicacion 
            smtp.Port = 587;

            //se indica si el buton utiliza seguridad tipo SSL 
            smtp.EnableSsl = true;

            //se indica si el buzon utiliza credenciales por default
            smtp.UseDefaultCredentials = false;

            //se asignan los datos para los credenciales 
            smtp.Credentials = new NetworkCredential("supporhotelbeachljha123@outlook.com", "LuisJoshHarryAndre123");

            try
            {
                // Enviar el correo electrónico
                smtp.Send(email);
                Console.WriteLine("Correo electrónico enviado correctamente.");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error al enviar el correo electrónico: " + ex.Message);
            }
            finally
            {
                // Dispose y limpieza de recursos
                email.Dispose();
                smtp.Dispose();
            }
        }
    }
}
