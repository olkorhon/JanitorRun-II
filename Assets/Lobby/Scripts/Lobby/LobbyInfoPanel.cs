using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace Prototype.NetworkLobby 
{

    /// <summary>
    /// 3rd party class for network lobby functionality. Lobby info panel functionality
    /// </summary>

    public class LobbyInfoPanel : MonoBehaviour
    {
        public Text infoText;
        public Text buttonText;
        public Button singleButton;

        public void Display(string info, string buttonInfo, UnityEngine.Events.UnityAction buttonClbk)
        {
            infoText.text = info;

            buttonText.text = buttonInfo;

            singleButton.onClick.RemoveAllListeners();

            if (buttonClbk != null)
            {
                singleButton.onClick.AddListener(buttonClbk);
            }

            singleButton.onClick.AddListener(() => { gameObject.SetActive(false); });

            gameObject.SetActive(true);
        }
    }
}