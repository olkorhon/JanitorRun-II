
using System;
using com.shephertz.app42.paas.sdk.csharp.game;
using com.shephertz.app42.paas.sdk.csharp;
using UnityEngine;
using System.Collections.Generic;

namespace AssemblyCSharp
{

    /// summary>
    /// Class that contains App42CallBack functionality for leaderboard data request
    /// </summary>

    /*
        App42 cloud callback functionality for leaderboard data request.
        Creates jagged array of result which format is [result[name, time], result[name, time], ...].
        Public bool for others to know when results are done.
        Results are then request by ScoreBoardListControl.

        @Author: Mikael Martinviita
    */

    public class ScoreBoardResponse : App42CallBack
    {
        //Public bool for others to know when results are done.
        public bool callDone = false;

        //Jagged array for results, only 20 best
        private string[][] result = new string[20][];

        //Initialize array
        void InitResult()
        {
            for(int i = 0; i < 20; i++)
            {
                result[i] = new string[2] { " ", " " };
            }
        }
		
        // If database call was succesfull
		public void OnSuccess (object obj)
		{
            //Initialize array
            InitResult();

			if (obj is Game)
            {
				Game gameObj = (Game)obj;
				Debug.Log ("GameName : " + gameObj.GetName ());
				if (gameObj.GetScoreList () != null)
                {
					IList<Game.Score> scoreList = gameObj.GetScoreList ();
                    for (int i = 0; i < scoreList.Count; i++)
                    {
                        //Add results to jagged array in form [result[name, time], result[name, time], ...]
                        result[i][0] = scoreList[i].GetUserName();
                        result[i][1] = scoreList[i].GetValue().ToString();                      
                    }

                    // Results are done
                    callDone = true;
                }
			}
            else
            {
				IList<Game> gameList = (IList<Game>)obj;
				for (int j = 0; j < gameList.Count; j++)
                {
					Debug.Log ("GameName is     : " + gameList[j].GetName ());
					Debug.Log ("Description is  : " + gameList[j].GetDescription ());
				}
			}
					
		}
		
		public void OnException (Exception e)
		{
			Debug.Log ("EXCEPTION : " + e);
		}
		
        // Return results
        public string[][] getResult()
        {
            return result;
        }
	}
}

