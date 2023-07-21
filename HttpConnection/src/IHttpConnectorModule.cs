namespace HttpConnection
{
    public interface IHttpConnectorModule
    {
        Task<HttpClient> ModifyClient(HttpClient client);
    }
}

