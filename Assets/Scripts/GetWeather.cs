
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using SimpleJSON;

/// <summary>
/// Class that contains code for weather data polling and visual effects
/// </summary>

/*
    Weather data is request from OpenWeather.org only by server. 
    Weather effects are then runned at each client.    

    @Author: Mikael Martinviita
*/


public class GetWeather : NetworkBehaviour {

    public Light lightDown;     // Directional light downwards, Main light
    public Light lightUp;       // Directional light upwards, balancing light for ceiling
    public Material nightsky;     // Nightsky skybox

    public string weather; //string that contains main weather information

    private PlayerControl pControl;
    public GameObject rain;
    public GameObject snow;

    private Camera pcamera;

    private string sunrise; //Sunrise time
    private string sunset; //Sunset time
    private System.DateTime timeNow; //Time when game is played
    private bool sunUp = false; // Bool to check if sun is up for other weather effects

    // Runs when object is loaded
    void Start()
    {
        //city_id = 643493;

        //Get player and particle objects
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        
        // Request weather data only if server
        if (isServer)
        {
            string url = "api.openweathermap.org/data/2.5/weather?id=643493&units=metric&lang=fi&APPID=487d8a5d17a089de9eeb152037686111 ";
            // Make a request
            WWW www= new WWW(url);
            // Wait for answer
            StartCoroutine(WaitForRequest(www));

        }
        for(int i = 0; i < players.Length; i++)
        {
            Debug.Log("players.Length : " + players.Length);
            pControl = players[i].GetComponent<PlayerControl>();
            Debug.Log("GetWeather.cs is calling GetWeatherObject() and caller is : " + players[i].name);
            pControl.GetWeatherObject(); //Tells to PlayerControl that GetWeather gameobject is ready
        }

        
    }

    // Coroutine for waiting answer for request
    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // Check for errors
        if (www.error == null)
        {
            // Parse the JSON style answer
            var N = JSON.Parse(www.text);
            // Get the main weather data
            weather = N["weather"][0]["main"];
            sunrise = N["sys"]["sunrise"];
            sunset = N["sys"]["sunset"];

            Debug.Log(weather);
            //weather = "Snow";
            //weather = "Rain";

            // Get UTC time now
            timeNow = System.DateTime.UtcNow;

            // Set lighting based on real life sun
            if(timeNow < UnixTimeStampToDateTime(System.Convert.ToDouble(sunrise)))
            {
                //Sun hasn't rised yet
                // Darkness
                sunUp = false;
                RenderSettings.skybox = nightsky;
                lightDown.intensity = 0;
                lightUp.intensity = 0;
                
            }
            else
            {
                // Sun has rised
                // Bright
                sunUp = true;
                // Sun is default bright so no need to set intensity
                if (timeNow < UnixTimeStampToDateTime(System.Convert.ToDouble(sunset)))
                {
                    // Sun hasn't set yet
                    // Bright
                    sunUp = true;
                    // Sun is default bright so no need to set intensity
                }
                else
                {
                    // Sun has set
                    // Darkness
                    sunUp = false;
                    RenderSettings.skybox = nightsky;
                    lightDown.intensity = 0;
                    lightUp.intensity = 0;
                }
            }
            

            //Make decision based on main weather data

            if (weather == "Snow")
            {
                yield return new WaitForSeconds(1); //Wait for clients to be ready
                RpcSnow(); //Called on server, runned on clients
            }
            else if ((weather == "Rain" ) || (weather == "Drizzle"))
            {
                yield return new WaitForSeconds(1); //Wait for clients to be ready
                RpcRain(); //Called on server, runned on clients
            }
            else if ((weather == "Clouds") || (weather == "Atmosphere"))
            {
                yield return new WaitForSeconds(1); //Wait for clients to be ready
                RpcClouds(); //Called on server, runned on clients
            }
            else if (weather == "Clear" && sunUp) // Increases brithness so do only if sun is up
            {
                yield return new WaitForSeconds(1); //Wait for clients to be ready
                RpcClear(); //Called on server, runned on clients
            }
            else
            {
                Debug.Log("else called");
            }

            Debug.Log("WWW Ok!: ");
        }
        else {
            Debug.Log("WWW Error: " + www.error);
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


    //Called on server, runned on clients
    // Makes snow and frost visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcSnow()
    {
        rain.SetActive(false);
        snow.SetActive(true);
        Camera.main.GetComponent<FrostEffect>().enabled = true;
    }

    //Called on server, runned on clients
    // Makes rain visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcRain()
    {
        snow.SetActive(false);
        rain.SetActive(true);
        if(Camera.main.GetComponent<FrostEffect>() != null)
        {
            Camera.main.GetComponent<FrostEffect>().enabled = false;
        }
        else
        {
            Debug.Log("Frosteffect null");
        }

    }

    //Called on server, runned on clients
    // Makes clouds visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcClouds()
    {
        Debug.Log("Clouds called in client");
        snow.SetActive(false);
        rain.SetActive(false);
        if (Camera.main.GetComponent<FrostEffect>() != null)
        {
            Camera.main.GetComponent<FrostEffect>().enabled = false;
        }
        else
        {
            Debug.Log("Frosteffect null");
        }
        RenderSettings.fogDensity = 0.06f; // Set global fog, works as clouds
        RenderSettings.fog = true;
    }

    //Called on server, runned on clients
    // Makes snow and frost visual effects enabled and other effects disabled
    [ClientCallback]
    [ClientRpc]
    public void RpcClear()
    {
        // Set lighting little brighter
        lightDown.intensity = 1.0f; 
        lightUp.intensity = 0.5f;
    }

    // Only for testing how visual effects works
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
}
