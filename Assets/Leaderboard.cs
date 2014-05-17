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
	
		public class Leaderboard
		{
				public void SaveLeaderboardScores (int blockCount)
				{
						List<LeaderboardScore> highScores = LoadLeaderboardScores ();
						try {
								highScores.Add (new LeaderboardScore (System.Environment.MachineName, blockCount, DateTime.Now, "1.0.0"));
								//m_HighScores = highScores; //update public exposed leaderboard for GUI
								string json = JsonConvert.SerializeObject (highScores, Formatting.Indented);
								System.IO.File.WriteAllText (@"Leaderboard.txt", json);
						} catch (Exception ex) {
								UnityEngine.Debug.LogWarning ("Error writing leaderboard scores: " + ex.Message);
						}
				}
				public List<LeaderboardScore> LoadLeaderboardScores () //todo - make private, fix issue where leaderboard doesn't update right after a game for some reason...
				{
						List<LeaderboardScore> highScores = new List<LeaderboardScore> ();
						try {
								string json;
								using (System.IO.StreamReader file = new System.IO.StreamReader(@"Leaderboard.txt", true)) { 
										json = file.ReadToEnd ();
								}
								highScores = JsonConvert.DeserializeObject<List<LeaderboardScore>> (json);
								highScores = highScores.OrderByDescending (x => x.Score).ToList ();
								//m_HighScores = highScores; //update public exposed leaderboard for GUI
						} catch (Exception ex) {
								UnityEngine.Debug.LogWarning ("Error loading leaderboard scores: " + ex.Message);
						}
						return highScores;
				}
		}
}
