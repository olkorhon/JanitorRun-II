
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Class that maintains where each player is going on the race path of checkpoints
/// </summary>

/*
    Class that maintains where each player is going on the race path of checkpoints.
    Holds a list of checkpoints that the players must go through in order to finish the race
    Has a dictionary that uses the players InstaceId's to keep track of what checkpoints have been cleared.
    
    TODO: 
    Validation and automatic ordering of checkpoints.

    @Author: Olli Korhonen
*/

public class ProgressScript : MonoBehaviour
{
    [System.Serializable]
    public delegate void CheckpointEvent(CheckpointScript a);
    public CheckpointEvent correct_checkpoint_hit;
    public CheckpointEvent wrong_checkpoint_hit;

    private GameManagerScript game_manager;
    public CheckpointScript[] checkpoints;

    Dictionary<int, int> player_progress;

    private AudioSource audiosource;

    // Use this for initialization
    void Start()
    {
        // Ensure provided checkpoints are ordered and ok
        validateCheckpoints();

        // Find gamemanager that will handle game ending after someone finishes
        this.game_manager = GetComponentInParent<GameManagerScript>();
        if (this.game_manager == null)
            Debug.LogWarning("ProgressScript: GameManagerSript could not be found");

        // Initialize tracked players list
        fetchPlayers();
        if (this.player_progress.Count < 1)
            Debug.LogWarning("ProgressScript: No valid players found");
        else
            Debug.Log("ProgressScript: Found " + this.player_progress.Count + " players.");
        audiosource = this.GetComponent<AudioSource>();
    }

    private void validateCheckpoints()
    {
        // Make sure there are checkpoints to clear
        if (checkpoints.Length < 0)
        {
            Debug.LogWarning("No checkpoints defined");
            return;
        }

        // Define the checkpoint orders based on the list
        for (int i = 0; i < checkpoints.Length; i++)
        {
            checkpoints[i].setOrder(i + 1);
        }
    }

    private void fetchPlayers()
    {
        // Initialize PlayerProgress list
        this.player_progress = new Dictionary<int, int>();

        // Fetch all the players from the scene
        GameObject[] players_list = GameObject.FindGameObjectsWithTag("Player");

        // Go through players and add them to the PlayerProgress list if they are valid
        PlayerObject player_obj;
        foreach (GameObject obj in players_list)
        {
            player_obj = obj.GetComponent<PlayerObject>();

            if (player_obj == null)
                Debug.LogError("Player without PlayerObject script cannot be tracked!");
            else
                this.player_progress.Add(obj.GetInstanceID(), 0);
        }
    }

    public void objectHitCheckpoint(int order, GameObject other)
    {
        Debug.Log("Player " + other.GetInstanceID() + " has hit checkpoint");

        if (player_progress.ContainsKey(other.GetInstanceID()))
        {
            #region valid_key
            // If we hit the correct checkpoint, move the progress to the next checkpoint
            if (order == player_progress[other.GetInstanceID()] + 1)
            {
                if (correct_checkpoint_hit != null)
                    correct_checkpoint_hit.Invoke(checkpoints[player_progress[other.GetInstanceID()]]);

                // If the checkpoint is the last one, start the finishing procedure
                if (order == checkpoints[checkpoints.Length - 1].getOrder())
                {
                    handlePlayerFinish(other);
                }
                else
                {
                    // Advance the players progress in the path and carry on
                    player_progress[other.GetInstanceID()] += 1;
                    Debug.Log("Next checkpoint is " + player_progress[other.GetInstanceID()]);
                }
            }
            else
            {
                if (wrong_checkpoint_hit != null && player_progress[other.GetInstanceID()] != order + 1)
                    wrong_checkpoint_hit.Invoke(checkpoints[player_progress[other.GetInstanceID()]]);

                Debug.Log("Player passed the wrong checkpoint");
            }

            
            #endregion
        }
        else
        {
            #region Generate debug for missing key
            // Initialize error message
            string error_message = "ProgressScript: Tried to advance an undefined player with id: " + other.GetInstanceID();

            // Collect and append all keys
            error_message += "\n + Existing keys: ";
            foreach (int key in new List<int>(player_progress.Keys))
                error_message += "\n" + key.ToString();

            // Log the error
            Debug.LogError(error_message);
            #endregion
        }
    }

    private void handlePlayerFinish(GameObject finisher)
    {
        // Disable steering if possible
        Steering steering = finisher.GetComponent<Steering>();
        if (steering != null)
        {
            steering.enabled = false;
            finisher.GetComponent<AudioSource>().mute = true;
            audiosource.Play();
        }
        else
        {
            Debug.Log("ProgressScript: cannot process object crossing finish line, it does not have a PlayerControl component.");
        }

        // Notify game manager that a player has finished
        game_manager.playerCrossesFinish(finisher);
    }

    public void setListenerRightCheckpoint(CheckpointEvent callback)
    {
        this.correct_checkpoint_hit = callback;
    }
    public void setListenerWrongCheckpoint(CheckpointEvent callback)
    {
        this.wrong_checkpoint_hit = callback;
    }
}
