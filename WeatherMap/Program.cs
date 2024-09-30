using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

class Program
{
    private static readonly string apiKey = "20614bb692955825817276cd03188f19";
    private static readonly string geoAPIKey = "11488c45a5874803bc3c8123d467d9fc ";

    static async Task Main(string[] args)
    {
        Console.Write("Enter the city name: ");
        string city = Console.ReadLine().ToLower();

        Console.Write("Enter the state (or country if outside the US): ");
        string state = Console.ReadLine().ToLower();

        (double? lat, double? lon) = await GetCoordinates(city, state);

        if (lat.HasValue && lon.HasValue)
        {
            Console.WriteLine($"The coordinates for {city.ToUpper()}, {state.ToUpper()} are Latitude: {lat.Value}, Longitude: {lon.Value}");
        }
        else
        {
            Console.WriteLine($"Could not find the coordinates for {city}, {state}.");
        }

        string weatherData = await GetWeatherData(lat.Value, lon.Value);

        if (weatherData != null)
        {
            ParseAndDisplayWeather(weatherData);
        }
        else
        {
            Console.WriteLine("Failed to retrieve weather data.");
        }
    }

    private static async Task<string> GetWeatherData(double lat, double lon)
    {
        string url = $"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={apiKey}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    return result;
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
            }
        }
        return null;
    }

    private static void ParseAndDisplayWeather(string jsonData)
    {
        JObject weatherJson = JObject.Parse(jsonData);

        // Extract information from the JSON
        string cityName = weatherJson["name"]?.ToString();
        double temperature = weatherJson["main"]["temp"].ToObject<double>();
        double feelsLike = weatherJson["main"]["feels_like"].ToObject<double>();
        string weatherDescription = weatherJson["weather"][0]["description"]?.ToString();

        // Convert temperature from Kelvin to Celsius
        temperature = (temperature - 273.15) * 9 / 5 + 32;
        feelsLike = (feelsLike - 273.15) * 9 / 5 + 32;

        // Display the data
        Console.WriteLine($"Current weather for {cityName}:");
        Console.WriteLine($"Temperature: {temperature:F2}°F");
        Console.WriteLine($"Feels like: {feelsLike:F2}°F");
        Console.WriteLine($"Description: {weatherDescription}");
    }

    private static async Task<(double?, double?)> GetCoordinates(string city, string state)
    {
        string url = $"https://api.opencagedata.com/geocode/v1/json?q={Uri.EscapeDataString(city)}&key={geoAPIKey}";

        using (HttpClient client = new HttpClient())
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    string result = await response.Content.ReadAsStringAsync();
                    JObject json = JObject.Parse(result);

                    // Check if there are results
                    if (json["results"] != null && json["results"].HasValues)
                    {
                        foreach (var resultItem in json["results"])
                        {
                            string foundCity = resultItem["components"]["city"]?.ToString();
                            string foundState = resultItem["components"]["state"]?.ToString();
                            string foundCountry = resultItem["components"]["country"]?.ToString();

                            // Check if city and state/country match the user input
                            if (string.Equals(foundCity, city, StringComparison.OrdinalIgnoreCase) &&
                               (string.Equals(foundState, state, StringComparison.OrdinalIgnoreCase) ||
                                string.Equals(foundCountry, state, StringComparison.OrdinalIgnoreCase)))
                            {
                                double latitude = resultItem["geometry"]["lat"].ToObject<double>();
                                double longitude = resultItem["geometry"]["lng"].ToObject<double>();

                                return (latitude, longitude);
                            }
                        }
                    }
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Request exception: {e.Message}");
            }
        }

        return (null, null); // Return null if not found
    }
    
}

