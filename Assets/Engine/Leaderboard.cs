using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace AssemblyCSharp
{
		//Thread-safe - uses lock when updating Leaderboard.txt file.		
		public class Leaderboard
		{
				public int CurrentScore { get { return mCurrentScore; } }
				public List<LeaderboardScore> HighScores { get { return mHighScores; } }
				private int mCurrentScore;
				private FileIO mFileHelper = new FileIO ();				
				private List<LeaderboardScore> mHighScores = new List<LeaderboardScore> (); //Note: Assumed ordered descending				

				public Leaderboard ()
				{
						mCurrentScore = 0;
						LoadHighScores ();
				}
							
				public void AddHighScore (int blockCount)
				{												
						LoadHighScores (); //Before updating file on disk, pick up any changes
						mHighScores.Add (new LeaderboardScore (System.Environment.MachineName, blockCount, DateTime.Now, "1.0.0"));
						mHighScores = mHighScores.OrderByDescending (x => x.Score).ToList ();								
						string json = JsonConvert.SerializeObject (mHighScores, Formatting.Indented);
						mFileHelper.WriteToFile (@"Leaderboard.txt", json);													
				}
				private void  LoadHighScores ()
				{																			
						string json = mFileHelper.ReadFromFile (@"Leaderboard.txt");							
						mHighScores = JsonConvert.DeserializeObject<List<LeaderboardScore>> (json);
						mHighScores = mHighScores.OrderByDescending (x => x.Score).ToList ();																			
				}
		}
}
