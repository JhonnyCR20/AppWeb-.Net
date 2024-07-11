namespace AppHotelBeachSA.Data
{
    public class TipoCambioApi
    {
        public HttpClient Inicial()
        {
            //Se instancia un objeto HttpClient
            var client = new HttpClient();
            //aqui le indicamos la direccion web donde esta la api
            client.BaseAddress = new Uri("http://apis.gometa.org");

            return client;
        }
    }
}
