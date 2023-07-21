// Example of HttpConnector using WebForcast API


using HttpConnection;
using HttpConnection.Example;

Console.WriteLine("Hello, World! Example of HttpConnector.");

var city = "New York"; // set the city

// initialize the instance
HttpConnector<WeatherResponse, WeatherRequest> httpWeatherConnector
    = new HttpConnector<WeatherResponse, WeatherRequest>(new Uri("http://api.weatherstack.com/"), null, typeof(WeatherHttpService));

// consume post call
var result = await httpWeatherConnector.Post($"current?access_key=<api key>&query={city}", null); // pass your API key

// print result
if (result != null)
{
    Console.WriteLine($"{city}'s temparature: {result?.current?.temperature}");
}
else
{
    Console.WriteLine("was not able to fetch!");
}
