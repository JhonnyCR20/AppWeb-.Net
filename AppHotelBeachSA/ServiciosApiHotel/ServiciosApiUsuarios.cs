using AppHotelBeachSA.Models;
using Newtonsoft.Json;
using System.Text;

namespace AppHotelBeachSA.ServiciosApiHotel
{
    public class ServiciosApiUsuarios
    {
        public static ConexionApi conexion;

        public ServiciosApiUsuarios()
        {
            conexion = new ConexionApi();
        }



        public async Task<Usuario> BuscarUsuarios(string email)
        {
            Usuario temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Usuarios/Buscar/" + email);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Usuario>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }

        public async Task<bool> Agregar(Usuario pUsuario)
        {
            bool agregado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pUsuario), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Usuarios/agregar", content);

            if (response.IsSuccessStatusCode)
            {
                agregado = true;
            }

            return agregado;
        }//cierre agregar usuario


        public async Task<bool> Editar(Usuario usuarios, string email)
        {
            bool editado = false;
            var temp = await BuscarUsuarios(email);

            HttpClient client = conexion.Inicial();



            var content = new StringContent(JsonConvert.SerializeObject(usuarios), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Usuarios/EditarPassword/" + email, content);

            if (response.IsSuccessStatusCode)
            {
                editado = true;
            }

            return editado;
        }//cierre editar usuario



    }
}
