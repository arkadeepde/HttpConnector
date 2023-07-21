using System.Net.Http.Headers;
using System.Text.Json;

namespace HttpConnection
{

    /// <summary>
    /// HttpConnector class to connect with external or internal APIs
    /// Creat a new() instance or inject in Program.cs file to be used anywhere
    /// </summary>
    /// <typeparam name="TResponse">Expected model which will be returning from API</typeparam>
    /// <typeparam name="TContent">Expected model to pass the content to API</typeparam>
    public class HttpConnector<TResponse, TContent> where TResponse : class where TContent : class
    {
        #region Fields
        /// <summary>
        /// Base API URL
        /// </summary>
        private Uri _baseAddress { get; set; }

        /// <summary>
        /// Http Client generated from Http Client Factory or some other source
        /// </summary>
        private HttpClient? _client { get; set; }

        /// <summary>
        /// Overloaded class name for custom modification of HttpClient
        /// </summary>
        private Type? _overloadedClassName { get; set; }
        #endregion

        /// <summary>
        /// Constructor to initiatlize the HttpConnector
        /// </summary>
        /// <param name="baseURI">Base URI of the API</param>
        /// <param name="client">HttpClient to connect the API. If no HttpClient is present, then pass null. If null is passed, then new HttpClient will be dispose automatically</param>
        /// <param name="overloadedClassName">To modify the client, add custom class. Inherit the class form IHttpConnectorModule</param>
        public HttpConnector(Uri baseURI, HttpClient? client, Type? overloadedClassName = null)
        {
            _baseAddress = baseURI;
            _client = client;
            _overloadedClassName = overloadedClassName;
        }

        #region Public methods
        /// <summary>
        /// Perform a post operation with API relative path and class object
        /// </summary>
        /// <param name="apiPath">API relative path</param>
        /// <param name="content">Class object content to be passed to API</param>
        /// <returns></returns>
        public async Task<TResponse> Post(string apiPath, TContent content)
        {
            try
            {
                return await GetResponse(HttpMethod.Post, apiPath, content);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Perform a get operation with API relative path
        /// </summary>
        /// <param name="apiPath"></param>
        /// <returns></returns>
        public async Task<TResponse> Get(string apiPath)
        {
            try
            {
                return await GetResponse(HttpMethod.Get, apiPath);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Perform a post operation with API relative path and class object
        /// </summary>
        /// <param name="apiPath">API relative path</param>
        /// <param name="content">Class object content to be passed to API</param>
        /// <returns></returns>
        public async Task<TResponse> Put(string apiPath, TContent content)
        {
            try
            {
                return await GetResponse(HttpMethod.Put, apiPath, content);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Perform a delete operation with API relative path and class object
        /// </summary>
        /// <param name="apiPath">API relative path</param>
        /// <param name="content">Class object content to be passed to API</param>
        /// <returns></returns>
        public async Task<TResponse> Delete(string apiPath, TContent content)
        {
            try
            {
                return await GetResponse(HttpMethod.Delete, apiPath, content);
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Perform a patch operation with API relative path and class object
        /// </summary>
        /// <param name="apiPath">API relative path</param>
        /// <param name="content">Class object content to be passed to API</param>
        /// <returns></returns>
        public async Task<TResponse> Patch(string apiPath, TContent content)
        {
            try
            {
                return await GetResponse(HttpMethod.Patch, apiPath, content);
            }
            catch
            {
                throw;
            }
        }
        #endregion


        #region Private methods
        /// <summary>
        /// Actual
        /// </summary>
        /// <param name="operationType"></param>
        /// <param name="apiPath"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        private async Task<TResponse> GetResponse(HttpMethod operationType, string apiPath, TContent? content = default)
        {
            var isHttpClientNull = false;
            // in case the HttpClient is null
            if (_client == null)
            {
                _client = new();
                isHttpClientNull = true;
            }

            // output of the method
            HttpResponseMessage? response = null;

            // resposne result variable initilize
            TResponse responseResult = default(TResponse);

            // setting the base address if null
            _client.BaseAddress = _client.BaseAddress != null ? _client.BaseAddress : _baseAddress;

            // add custom Http Modifier
            if (_overloadedClassName != null)
            {
                var instance = CreateInstance<IHttpConnectorModule>(_overloadedClassName);
                if (instance != null)
                {
                    var result = instance.ModifyClient(_client);
                    _client = result.Result;
                }
            }

            switch (operationType)
            {
                case HttpMethod m when m == HttpMethod.Get:
                    {
                        response = await _client.GetAsync(apiPath);
                        break;
                    };
                case HttpMethod m when m == HttpMethod.Post:
                    {
                        var httpContent = GetHttpContent(content);
                        response = await _client.PostAsync(apiPath, httpContent);
                        break;
                    }
                case HttpMethod m when m == HttpMethod.Put:
                    {
                        var httpContent = GetHttpContent(content);
                        response = await _client.PutAsync(apiPath, httpContent);
                        break;
                    }
                case HttpMethod m when m == HttpMethod.Delete:
                    {
                        response = await _client.DeleteAsync(apiPath);
                        break;
                    }
                case HttpMethod m when m == HttpMethod.Patch:
                    {
                        var httpContent = GetHttpContent(content);
                        response = await _client.PatchAsync(apiPath, httpContent);
                        break;
                    }
                default:
                    return responseResult;
            }

            string responseBody = await response.Content.ReadAsStringAsync();

            // if the status code is true, convert the data to resposne type or return default
            if (response.IsSuccessStatusCode) //response.IsSuccessStatusCode
            {
                try
                {
                    if (!string.IsNullOrEmpty(responseBody))
                    {
                        responseResult = JsonSerializer.Deserialize<TResponse>(responseBody);
                    }
                }
                catch
                {
                    // error in converting premitive data typ to json
                    responseResult = (TResponse)Convert.ChangeType(responseBody, typeof(TResponse));
                }
            }

            // dispose HttpClient if created by this module
            if (isHttpClientNull)
            {
                _client.Dispose();
            }

            return responseResult;
        }

        private ITService CreateInstance<ITService>(Type type) where ITService : class
        {
            //var type = (from _assembly in AppDomain.CurrentDomain.GetAssemblies()
            //            from t in _assembly.GetTypes()
            //            where t.Name == className
            //            select t).FirstOrDefault();

            return Activator.CreateInstance(type) as ITService;
        }

        /// <summary>
        /// Create the HttpContent from class object
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private ByteArrayContent GetHttpContent(TContent? content)
        {
            if (content != null)
            {
                string serializeContent = JsonSerializer.Serialize(content);
                if (!typeof(TContent).IsPrimitive)
                {
                    var myContent = JsonSerializer.Serialize(content);
                }
                else
                {
                    serializeContent = Convert.ToString(content);
                }
                var buffer = System.Text.Encoding.UTF8.GetBytes(serializeContent ?? string.Empty);
                var byteContent = new ByteArrayContent(buffer);
                byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
                return byteContent;
            }
            else
            {
                return null;
            }
        }
        #endregion
    }
}