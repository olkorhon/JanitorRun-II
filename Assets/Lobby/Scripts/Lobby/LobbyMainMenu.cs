using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Prototype.NetworkLobby
{

    /// <summary>
    /// 3rd party class for network lobby functionality. Mainmenu functionality 
    /// </summary>

    //Main menu, mainly only a bunch of callback called by the UI (setup throught the Inspector)
    public class LobbyMainMenu : MonoBehaviour 
    {
        public LobbyManager lobbyManager;

        public RectTransform lobbyMainMenu;
        public RectTransform lobbyServerList;
        public RectTransform lobbyPanel;
        public RectTransform lobbyAvatar;
        public RectTransform lobbyLeaderboard;
        public RectTransform failurePanel;

        public InputField avatarName;
        public Dropdown avatarColour;

        public LobbyPlayer lobbyPlayer;
        public PlayerObject playerObject;

        public InputField matchNameInput;

        static Color[] Colors = new Color[] {
            new Color(0, 0.52f, 0.74f),
            new Color(0.77f, 0, 0.2f),
            new Color(0.1f, 0.1f, 0.1f),
            new Color(0.38f, 0.28f, 0.48f),
            new Color(1.0f, 0.82f, 0f),
            new Color(0, 0.62f, 0.42f) };

        public void OnEnable()
        {
            //lobbyManager.topPanel.ToggleVisibility(true);

            //ipInput.onEndEdit.RemoveAllListeners();
            //ipInput.onEndEdit.AddListener(onEndEditIP);

            //If server call OnClickCreateMatchMaking x times

            matchNameInput.onEndEdit.RemoveAllListeners();
            matchNameInput.onEndEdit.AddListener(onEndEditGameName);
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                onExit();
            }
        }

        public void OnClickCreateMatchmakingGame()
        {
            if (lobbyManager.matches != null)
            {
                failurePanel.gameObject.SetActive(true);
            }
            else
            {
                lobbyManager.StartMatchMaker();
                lobbyManager.matchMaker.CreateMatch(
                    matchNameInput.text,
                    (uint)lobbyManager.maxPlayers,
                    true,
                    "", "", "", 0, 0,
                    lobbyManager.OnMatchCreate);

                lobbyManager.backDelegate = lobbyManager.StopHost;
                lobbyManager._isMatchmaking = true;
                lobbyManager.DisplayIsConnecting();

                lobbyManager.SetServerInfo("Matchmaker Host", lobbyManager.matchHost);
            }
        }

        public void OnClickOpenServerList()
        {
            lobbyManager.StartMatchMaker();
            lobbyManager.backDelegate = lobbyManager.SimpleBackClbk;
            lobbyManager.ChangeTo(lobbyServerList);
        }

        void onEndEditGameName(string text)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                OnClickCreateMatchmakingGame();
            }
        }
        
        //Avatar button clicked
        public void onAvatar()
        {
            lobbyManager.ChangeTo(lobbyAvatar);
            avatarName.text = PlayerPrefs.GetString("avatarName");
            avatarColour.value = PlayerPrefs.GetInt("avatarValue");
        }

        //Avatar colour changed
        public void ChangeAvatarColour()
        {
            playerObject.avatarColor = Colors[avatarColour.value];
            playerObject.updateColor();
        }

        //Leaderboard button clicked
        public void onLeaderboard()
        {
            lobbyManager.ChangeTo(lobbyLeaderboard);
        }

        //Avatar settings saved
        public void onSave()
        {
            lobbyPlayer.playerName = avatarName.text;
            lobbyPlayer.playerColor = Colors[avatarColour.value];

            PlayerPrefs.SetString("avatarName", avatarName.text);
            PlayerPrefs.SetInt("avatarValue", avatarColour.value);
            PlayerPrefs.Save();
            lobbyManager.ChangeTo(lobbyMainMenu);
        }

        public void onFailure()
        {
            failurePanel.gameObject.SetActive(false);
        }

        // Program exit button clicked
        public void onExit()
        {
            Debug.Log("Exit");
            Application.Quit();
        }

    }
}
