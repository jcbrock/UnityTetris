using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;
using System.Linq;

namespace AssemblyCSharp
{
		public class LeaderboardScore
		{
				public string Name { get; set; }
				public int Score { get; set; }
				public DateTime Date  { get; set; }
				public string Version { get; set; }
	
				public LeaderboardScore (string name, int score, DateTime date, string version)
				{
						Name = name;
						Score = score;
						Date = date;
						Version = version;
				}
		}
			
		//Thread-safe - uses lock when updating Leaderboard.txt file.
		
		public class Leaderboard
		{
				FileIO fileHelper = new FileIO ();
				public int CurrentScore;
				private List<LeaderboardScore> mHighScores = new List<LeaderboardScore> (); //Note: Assumed ordered descending
				public List<LeaderboardScore> GetHighScores ()
				{						
						return mHighScores;
				}

				public Leaderboard ()
				{
						CurrentScore = 0;
						LoadHighScores ();
				}
							
				public void AddHighScore (int blockCount)
				{												
						LoadHighScores (); //Before updating file on disk, pick up any changes
						mHighScores.Add (new LeaderboardScore (System.Environment.MachineName, blockCount, DateTime.Now, "1.0.0"));
						mHighScores = mHighScores.OrderByDescending (x => x.Score).ToList ();								
						string json = JsonConvert.SerializeObject (mHighScores, Formatting.Indented);
						fileHelper.WriteToFile (@"Leaderboard.txt", json);													
				}
				private void  LoadHighScores ()
				{																			
						string json = fileHelper.ReadFromFile (@"Leaderboard.txt");							
						mHighScores = JsonConvert.DeserializeObject<List<LeaderboardScore>> (json);
						mHighScores = mHighScores.OrderByDescending (x => x.Score).ToList ();																			
				}
		}
}
