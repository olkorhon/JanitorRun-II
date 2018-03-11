
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;
using System;

public enum WeatherType
{
    CLEAR,
    RAIN,
    SNOW,
    FOG
}

/// <summary>
/// Class that contains code for weather data polling and visual effects
/// </summary>

/*
    Weather data is request from OpenWeather.org only by server. 
    Weather effects are then runned at each client.    

    @Author: Mikael Martinviita
*/
public class Weather : NetworkBehaviour
{
	public Light keyLight;	   // Direcitonal key light
	public Light[] fillLights; // Directional balancing lights

	private float dayKeyIntensity;
	private float[] dayFillIntensity;

	public float nightKeyIntensity = 0.0f;
	public float[] nightFillIntensity = new float[] {0.0f, 0.0f, 0.0f};

	public Material daySkybox;
    public Material nightSkybox;

	public WeatherType weather;

    public GameObject rain;
    public GameObject snow;

    private Camera pcamera;

    private System.DateTime timeNow; // Time when game is played
    private bool sunUp = false; // Bool to check if sun is up for other weather effects

    private ShowWeather showWeatherWidget; // Weather UI widget.

    // Runs when object is loaded
    void Start()
    {
        //city_id = 643493;
        showWeatherWidget = FindObjectOfType<ShowWeather>();
        if (showWeatherWidget == null)
        {
            Debug.LogError("Cannot find the UI widget. Make sure GUICanvas is present in the scene.");
            return;
        }

        // Request weather data only if server
        if (isServer)
        {
            string url = "http://api.openweathermap.org/data/2.5/weather?id=643493&units=metric&lang=fi&appid=487d8a5d17a089de9eeb152037686111";
			WWW www= new WWW(url); // Make a request
			StartCoroutine(WaitForRequest(www)); // Wait for answer
        }

		// Store day lighting settings that should be on by default
		dayKeyIntensity = keyLight.intensity;
		dayFillIntensity = new float[fillLights.GetLength(0)];
		for (int i = 0; i < fillLights.GetLength (0); i++)
			dayFillIntensity [i] = fillLights [i].intensity;
    }

    // Coroutine for waiting answer for request
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // Check for errors
		if (www.error != null) {
			Debug.LogWarning ("WeatherWWW Error: " + www.error);
		} else {
			// Parse the JSON response
			JSONNode jsonBody = JSON.Parse (www.text);

			// Get the main weather data
			weather = parseWeatherString(jsonBody["weather"][0]["main"]);
			Debug.Log ("Current weather: " + weather);

			// Get UTC time now
			timeNow = System.DateTime.UtcNow;

			// Set lighting based on real life sun
			this.sunUp = isSunUp(jsonBody["sys"]["sunrise"], jsonBody["sys"]["sunset"]);

			if (this.sunUp)
				setupDayLighting();
			else
				setupNightLighting();

			// Inform clients what weather to use
			yield return applyWeather(this.weather);
			Debug.Log("WeatherWWW Ok");
		}
    }

	// Returns a matching WeatherType enum for provided weather text, defaults to CLEAR
	private WeatherType parseWeatherString(string weather)
	{
		switch (weather) {
		case "Snow":
			return WeatherType.SNOW;
		case "Rain":
		case "Drizzle":
			return WeatherType.RAIN;
		case "Clouds":
		case "Atmosphere":
			return WeatherType.FOG;
		case "Clear":
			return WeatherType.CLEAR;
		default:
			Debug.LogWarning("Could not parse weather: " + weather);
			return WeatherType.CLEAR;
		}
	}

	private bool isSunUp(string sunrise, string sunset)
	{
		try {
			bool beforeSunrise = timeNow < UnixTimeStampToDateTime(System.Convert.ToDouble(sunrise));
			bool afterSundown = timeNow > UnixTimeStampToDateTime(System.Convert.ToDouble(sunset));
			return !(beforeSunrise || afterSundown);
		}
		catch (FormatException exception) {
			Debug.LogWarning("Could not convert sunrise or sunset to double: " + sunrise + ", " + sunset);
			Debug.LogWarning(exception.ToString());
			return true;
		}
	}

	public void setupDayLighting()
	{
		RenderSettings.skybox = daySkybox;

		// Set bright lighting
		keyLight.intensity = dayKeyIntensity; 
		for (int i = 0; i < fillLights.GetLength(0); i++) {
			fillLights [i].intensity = dayFillIntensity[i];
		}
	}

	public void setupNightLighting()
	{
		RenderSettings.skybox = nightSkybox;

		// Set dim lighting
		keyLight.intensity = nightKeyIntensity; 
		for (int i = 0; i < fillLights.GetLength(0); i++) {
			fillLights [i].intensity = nightFillIntensity[i];
		}
	}

	public IEnumerator applyWeather(WeatherType currentWeather) 
	{
		// Make decision based on main weather data
		switch (currentWeather) {
		case WeatherType.SNOW:
			yield return new WaitForSeconds(1); // Wait for clients to be ready
			RpcSnow(); // Called on server, runned on clients
			break;
		case WeatherType.RAIN:
			yield return new WaitForSeconds(1); // Wait for clients to be ready
			RpcRain(); // Called on server, runned on clients
			break;
		case WeatherType.FOG:
			yield return new WaitForSeconds(1); // Wait for clients to be ready
			RpcClouds(); // Called on server, runned on clients
			break;
		case WeatherType.CLEAR:
			// Increases brithness so do only if sun is up
			if (sunUp) {
				yield return new WaitForSeconds (1); // Wait for clients to be ready
				RpcClear (); // Called on server, runned on clients
			}
			break;
		default:
			Debug.LogWarning ("No weather implementation for case: " + weather + ", sunup-" + sunUp);
			yield return new WaitForSeconds (1); // Wait for clients to be ready
			RpcClear(); // Called on server, runned on clients
			break;
		}
	}

    // Helper function to convert unix timestamp to .net DateTime format
    public static System.DateTime UnixTimeStampToDateTime(double unixTimeStamp)
    {
        // Unix timestamp is seconds past epoch
        System.DateTime dtDateTime = new System.DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
        dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
        return dtDateTime;
    }

	// Called on server, executed on clients
    // Makes snow and frost visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcSnow()
    {
        rain.SetActive(false);
        snow.SetActive(true);
        //Camera.main.GetComponent<FrostEffect>().enabled = true;

        // Show snow icon:
        showWeatherWidget.Show(WeatherType.SNOW);
    }

	// Called on server, executed on clients
    // Makes rain visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcRain()
    {
        snow.SetActive(false);
        rain.SetActive(true);

        // Show rain icon:
		showWeatherWidget.Show(this.weather);

    }

	// Called on server, executed on clients
    // Makes clouds visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcClouds()
    {
        Debug.Log("Clouds called in client");
        snow.SetActive(false);
        rain.SetActive(false);
   
        RenderSettings.fogDensity = 0.06f; // Set global fog, works as clouds
        RenderSettings.fog = true;

        // Show fog icon:
		showWeatherWidget.Show(this.weather);
    }

	// Called on server, executed on clients
    // Makes snow and frost visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcClear()
    {
        // Set lighting little brighter
		keyLight.intensity *= 1.1f; 
		for (int i = 0; i < fillLights.GetLength(0); i++) {
			fillLights [i].intensity *= 1.1f;
		}

        // Show sun icon:
		showWeatherWidget.Show(this.weather);
    }

    // Only for testing how visual effects works
    /*
    public void Update()
    {
        if (isLocalPlayer)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                snow.SetActive(false);
                rain.SetActive(true);
                pcamera.GetComponent<FrostEffect>().enabled = false;
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                rain.SetActive(false);
                snow.SetActive(true);
                pcamera.GetComponent<FrostEffect>().enabled = true;
            }
            if (Input.GetKeyDown(KeyCode.C))
            {
                snow.SetActive(false);
                rain.SetActive(false);
                pcamera.GetComponent<FrostEffect>().enabled = false;
            }
        }
    }
    */
}
