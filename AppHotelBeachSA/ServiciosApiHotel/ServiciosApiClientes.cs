using AppHotelBeachSA.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Text.Json;

namespace AppHotelBeachSA.ServiciosApiHotel
{
    public class ServiciosApiClientes
    {
        private static ConexionApi conexion;
        public ServiciosApiClientes()
        {
            conexion = new ConexionApi();
        }

        public async Task<List<Cliente>> ListaClientes()
        {
            List<Cliente> lista = new List<Cliente>();

            HttpClient client = conexion.Inicial();


            var response = await client.GetAsync("/api/Clientes/ListaClientes");

            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<List<Cliente>>(jsonRespuesta);
                lista = resultado.ToList();
            }
            return lista;
        }

        public async Task<Cliente> BuscarCliente(int id)
        {
            Cliente temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Clientes/Buscar/" + id);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Cliente>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }


        public async Task<Cliente> BuscarCedula(string cedula)
        {
            Cliente temp = null;

            HttpClient client = conexion.Inicial();
            var response = await client.GetAsync("/api/Clientes/BuscarCedula/" + cedula);
            if (response.IsSuccessStatusCode)
            {
                var jsonRespuesta = await response.Content.ReadAsStringAsync();
                var resultado = JsonConvert.DeserializeObject<Cliente>(jsonRespuesta);
                temp = resultado;
            }

            return temp;
        }



        public async Task<bool> Agregar(Cliente pCliente)
        {
            bool agregado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pCliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Clientes/agregar", content);

            if (response.IsSuccessStatusCode)
            {
                agregado = true;
            }

            return agregado;
        }


        public async Task<bool> Editar(Cliente pCliente, int idCliente)
        {
            bool editado = false;

            HttpClient client = conexion.Inicial();

            var content = new StringContent(JsonConvert.SerializeObject(pCliente), Encoding.UTF8, "application/json");

            var response = await client.PutAsync("/api/Clientes/Editar/" + idCliente, content);

            if (response.IsSuccessStatusCode)
            {
                editado = true;
            }

            return editado;
        }


        public async Task<bool> Eliminar(int? idCliente)
        {
            bool eliminado = false;
            HttpClient client = conexion.Inicial();

            var response = await client.DeleteAsync("/api/Clientes/Eliminar/" + idCliente);
            if (response.IsSuccessStatusCode)
            {
                eliminado = true;
            }

            return eliminado;
        }

    }//fin del controller
}//fin del namespace
