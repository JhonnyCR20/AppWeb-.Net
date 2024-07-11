using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using AppHotelBeachSA.Models;

namespace AppHotelBeachSA.ServiciosApiHotel
{
    public class ServiciosApiPaquetes
    {
        private static ConexionApi conexion;
        public ServiciosApiPaquetes()
        {
            conexion = new ConexionApi();
        }


        public async Task<List<Paquete>> ListaPaquetes()
        {
            List<Paquete> listaPaquetes = new List<Paquete>();

            HttpClient client = conexion.Inicial();


            var response = await client.GetAsync("/api/Paquetes/ListaPaquetes");

            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<Paquete>>(jsonRespuesta);
                listaPaquetes = resultado.ToList();
            }

            return listaPaquetes;
        }

        public async Task<Paquete> BuscarPaquete(int id)
        {
            Paquete temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Paquetes/Buscar/"+id);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Paquete>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }

        public async Task<Paquete> BuscarPaqueteNombre(string nombre)
        {
            Paquete temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Paquetes/BuscarNombre/" + nombre);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Paquete>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }

        public async Task<bool> Agregar(Paquete pPaquete)
        {
            bool agregado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pPaquete), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Paquetes/agregar", content);

            if (response.IsSuccessStatusCode)
            {
                agregado = true;
            }

            return agregado;
        }


        public async Task<bool> Editar(Paquete pPaquete, int idPaquete)
        {
            bool editado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pPaquete), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Paquetes/Editar/" + idPaquete, content);

            if (response.IsSuccessStatusCode)
            {
                editado = true;
            }

            return editado;
        }


        public async Task<bool> Eliminar(int? idPaquete)
        {
            bool eliminado = false;
            HttpClient client = conexion.Inicial();

            var response = await client.DeleteAsync("/api/Paquetes/Eliminar/" + idPaquete);
            if (response.IsSuccessStatusCode)
            {
                eliminado = true;
            }

            return eliminado;
        }
    }
}
