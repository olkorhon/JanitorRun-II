using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShowWeather : MonoBehaviour {

    public Image weatherImage;

    // Weather sprites
    public Sprite day;
    public Sprite night;
    public Sprite rain;
    public Sprite snow;
    public Sprite fog;

    private void Awake()
    {
        if (FindObjectsOfType<ShowWeather>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void Show(WeatherType type)
    {
        switch (type)
        {
            case WeatherType.CLEAR:
                weatherImage.sprite = day;
                break;
            case WeatherType.RAIN:
                weatherImage.sprite = rain;
                break;
            case WeatherType.SNOW:
                weatherImage.sprite = snow;
                break;
            case WeatherType.FOG:
                weatherImage.sprite = fog;
                break;
            default:
                break;
        }
    }
}
