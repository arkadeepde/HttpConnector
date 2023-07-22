# HttpConnector
Custom HttpClient connector where you can consume external or internal APIs with custom model and plugin module.

## How to use
1. Add the HttpConnector package
2. Create the models for request & response 
3. Create the instance of HttpConnector with request & response model 
4. Call the method (either Get, Post, Put, Delete or Patch)

## Usage/Examples
```C#
using HttpConnection;

// initialize the instance
HttpConnector<WeatherResponse, WeatherRequest> httpWeatherConnector
    = new HttpConnector<WeatherResponse, WeatherRequest>(new Uri("http://api.weatherstack.com/"), null, null);

// consume post call
var result = await httpWeatherConnector.Post($"current?access_key=<api key>&query={city}", null); // pass your API key    
```

## Custom overridden method
To use any custom method to update the HttpClient use the below method. 
1. Create a class and inherited from IHttpConnectorModule
2. Write down your custom code to modify the HttpClient
3. Add the class in HttpConnector while initializing

```C#
public class WeatherHttpService : IHttpConnectorModule
{
    public async Task<HttpClient> ModifyClient(HttpClient client)
    {
        // your code
        return client;
    }
}
```
```C#
// initialize the instance
HttpConnector<WeatherResponse, WeatherRequest> httpWeatherConnector
    = new HttpConnector<WeatherResponse, WeatherRequest>(new Uri("http://api.weatherstack.com/")
    , null, typeof(**WeatherHttpService**));
```
   
   
