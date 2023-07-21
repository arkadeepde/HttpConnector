namespace HttpConnection.Example
{
    /// <summary>
    /// Sample Http class where you can modify the client for any login service or others
    /// </summary>
    public class WeatherHttpService : IHttpConnectorModule
    {
        public async Task<HttpClient> ModifyClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            return client;
        }
    }
}

