using System.Net.Http;

namespace AppHotelBeachSA.ServiciosApiHotel
{
    public class ConexionApi
    {
        public HttpClient Inicial()
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://www.apihotelbeachljha.somee.com/");

            return client;
        }
    }//fin del class
}//fin del namespace
