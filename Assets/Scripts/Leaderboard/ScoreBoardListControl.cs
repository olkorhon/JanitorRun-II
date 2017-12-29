
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Class that contains code for creating 20 best scores to leaderboard.
/// Instantiates leaderboard entry prefab with results from App42 database for 20 best scores.
/// 
/// @Author: Mikael Martinviita
/// </summary>
public class ScoreBoardListControl : MonoBehaviour {

    //Leaderboard entry prefab
    public GameObject playerScoreEntryPrefab;

    // Use this for adding score entries to list
    public void AddScores(string[][] scoreList, int max)
    {
        // Create <max> number of entries to leaderboard list
        for (int i = 0; i < max; i++)
        {
            GameObject entry = (GameObject)Instantiate(playerScoreEntryPrefab);
            entry.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(0, 0, 0);
            entry.transform.SetParent(this.transform, false);
            entry.transform.Find("Username").GetComponent<Text>().text = scoreList[i][0]; // Set name
            entry.transform.Find("Time").GetComponent<Text>().text = scoreList[i][1]; // Set time score
        }
    }
}
