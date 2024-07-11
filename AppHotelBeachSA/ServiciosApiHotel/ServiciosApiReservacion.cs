using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using AppHotelBeachSA.Models;


namespace AppHotelBeachSA.ServiciosApiHotel
{
    public class ServiciosApiReservacion
    {
        private static ConexionApi conexion;

        public ServiciosApiReservacion()
        {
            conexion = new ConexionApi();
        }

        public async Task<List<Reservacion>> ListaReservaciones()
        {
            List<Reservacion> listaReservacion = new List<Reservacion>();

            HttpClient client = conexion.Inicial();


            var response = await client.GetAsync("/api/Reservacion/ListaReservacion");

            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<Reservacion>>(jsonRespuesta);
                listaReservacion = resultado.ToList();
            }

            return listaReservacion;
        }


        public async Task<Reservacion> Buscar(int id)
        {
            Reservacion temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Reservacion/Buscar/" + id);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Reservacion>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }


        public async Task<bool> Agregar(Reservacion pReservacion)
        {
            bool agregado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pReservacion), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Reservacion/agregar", content);

            if (response.IsSuccessStatusCode)
            {
                agregado = true;
            }

            return agregado;
        }


        public async Task<bool> Eliminar(int? idReservacion)
        {
            bool eliminado = false;
            HttpClient client = conexion.Inicial();

            var response = await client.DeleteAsync("/api/Reservacion/Eliminar/" + idReservacion);
            if (response.IsSuccessStatusCode)
            {
                eliminado = true;
            }

            return eliminado;
        }
    }
}
