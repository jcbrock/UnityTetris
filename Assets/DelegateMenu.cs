using UnityEngine;
using System.Collections;
using System.Linq;

public class DelegateMenu : MonoBehaviour
{

		private delegate void MenuDelegate ();
		private MenuDelegate menuFunction;

		//OnGUI gets called multiple times a frame
		//so if you're accessing the static Screen class or w/e
		//and getting properties from it, that can be expensive
		//so we'll store these values in memory instead
		private float screenHeight;
		private float screenWidth;
		private float buttonHeight;
		private float buttonWidth;


		// Use this for initialization
		private void Start ()
		{
				screenHeight = Screen.height;
				screenWidth = Screen.width;

				buttonHeight = screenHeight * 0.2f;
				buttonWidth = screenWidth * 0.4f;

				menuFunction = mainMenu;
		}

		private void OnGUI ()
		{
				menuFunction ();
		}
	
		private void mainMenu ()
		{
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.25f, 
		                                                     buttonWidth, buttonHeight), "Start New Game")) {
						//make sure to kick off new game
						menuFunction = inGameHUD;
						AssemblyCSharp.UnityTetris.sceneMgr.ClearGame ();
						AssemblyCSharp.UnityTetris.sceneMgr.StartNewGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.5f, 
		                        buttonWidth, buttonHeight), "Quit Game")) {
						Application.Quit ();
				}
				AddLeaderboard ();
		
		}
		private void mainMenuWithResume ()
		{
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.1f, 
		                          buttonWidth, buttonHeight), "Continue Game")) {						
						menuFunction = inGameHUD;
						AssemblyCSharp.UnityTetris.sceneMgr.ResumeGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.4f, 
		                          buttonWidth, buttonHeight), "Start New Game")) {						
						menuFunction = inGameHUD;
						AssemblyCSharp.UnityTetris.sceneMgr.EndGame ();
						AssemblyCSharp.UnityTetris.sceneMgr.ClearGame ();
						AssemblyCSharp.UnityTetris.sceneMgr.StartNewGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.7f, 
		                          buttonWidth, buttonHeight), "Quit Game")) {
						Application.Quit ();
				}

				AddBlockCount ();
				AddLeaderboard ();
		}
		private void AddLeaderboard ()
		{
				AssemblyCSharp.UnityTetris.sceneMgr.LoadLeaderboardScores ();
				if (AssemblyCSharp.UnityTetris.sceneMgr.HighScores.Count != 0) {
						GUI.Label (new Rect (screenWidth * 0.75f, screenHeight * 0.2f, 
		                     screenWidth * 0.25f, screenHeight * 0.1f), 
		           "Leaderboard (score - name)");
		
				
						float height = 0.25f;
						foreach (AssemblyCSharp.LeaderboardScore score in AssemblyCSharp.UnityTetris.sceneMgr.HighScores.Take(5)) {
								GUI.Label (new Rect (screenWidth * 0.75f, screenHeight * height, 
			                     screenWidth * 0.25f, screenHeight * 0.05f), 
			           score.Score + " - " + score.Name);
								height += 0.05f;
						}
				}

		}
		private void AddBlockCount ()
		{
				GUI.Label (new Rect (screenWidth * 0.8f, screenHeight * 0.1f, 
		                     screenWidth * 0.2f, screenHeight * 0.1f), 
		           "Placed blocks: " + AssemblyCSharp.UnityTetris.sceneMgr.PlacedBlockCount);
		}

		private void inGameHUD ()
		{
				if (AssemblyCSharp.UnityTetris.sceneMgr.IsGameOver) {
						menuFunction = mainMenu;
				} else if (Input.GetKeyDown (KeyCode.Escape)) {						
						menuFunction = mainMenuWithResume;
						AssemblyCSharp.UnityTetris.sceneMgr.PauseGame ();
				}
				AddBlockCount ();
				AddLeaderboard ();
		}
}
