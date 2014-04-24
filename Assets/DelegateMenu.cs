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
		void Start ()
		{
				screenHeight = Screen.height;
				screenWidth = Screen.width;

				buttonHeight = screenHeight * 0.2f;
				buttonWidth = screenWidth * 0.4f;

				menuFunction = mainMenu;
		}

		void OnGUI ()
		{
				menuFunction ();
		}

		void anyKey ()
		{
				if (Input.anyKey) {
						menuFunction = mainMenu;
				}
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUI.Label (new Rect (screenWidth * 0.45f, screenHeight * 0.45f, 
		                     		screenWidth * 0.1f, screenHeight * 0.1f), 
		           			"Press any key to continue");
		}
	
		void mainMenu ()
		{
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.25f, 
		                                                     buttonWidth, buttonHeight), "Start New Game")) {
						//make sure to kick off new game
						menuFunction = inGameHUD;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.ClearGame ();
						AssemblyCSharp.NewBehaviourScript.sceneMgr.StartNewGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.5f, 
		                        buttonWidth, buttonHeight), "Quit Game")) {
						Application.Quit ();
				}
				AddLeaderboard ();
		
		}
		void mainMenuWithResume ()
		{
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.1f, 
		                          buttonWidth, buttonHeight), "Continue Game")) {						
						menuFunction = inGameHUD;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.ResumeGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.4f, 
		                          buttonWidth, buttonHeight), "Start New Game")) {						
						menuFunction = inGameHUD;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.EndGame ();
						AssemblyCSharp.NewBehaviourScript.sceneMgr.ClearGame ();
						AssemblyCSharp.NewBehaviourScript.sceneMgr.StartNewGame ();
						AssemblyCSharp.NewBehaviourScript.sceneMgr.ResumeGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.7f, 
		                          buttonWidth, buttonHeight), "Quit Game")) {
						Application.Quit ();
				}

				GUI.Label (new Rect (screenWidth * 0.8f, screenHeight * 0.1f, 
		                     screenWidth * 0.2f, screenHeight * 0.1f), 
		           "Placed blocks: " + AssemblyCSharp.NewBehaviourScript.sceneMgr.placedBlockCount);

				AddLeaderboard ();
		}
		void AddLeaderboard ()
		{
				AssemblyCSharp.NewBehaviourScript.sceneMgr.LoadLeaderboardScores ();
				if (AssemblyCSharp.NewBehaviourScript.sceneMgr.highScores.Count != 0) {
						GUI.Label (new Rect (screenWidth * 0.75f, screenHeight * 0.2f, 
		                     screenWidth * 0.25f, screenHeight * 0.1f), 
		           "Leaderboard (score - name)");
		
				
						float height = 0.25f;
						foreach (AssemblyCSharp.LeaderboardScore score in AssemblyCSharp.NewBehaviourScript.sceneMgr.highScores.Take(5)) {
								GUI.Label (new Rect (screenWidth * 0.75f, screenHeight * height, 
			                     screenWidth * 0.25f, screenHeight * 0.05f), 
			           score.Score + " - " + score.Name);
								height += 0.05f;
						}
				}

		}

		void inGameHUD ()
		{
				if (AssemblyCSharp.NewBehaviourScript.sceneMgr.IsGameOver) {
						menuFunction = mainMenu;
				} else if (Input.GetKeyDown (KeyCode.Escape)) {						
						menuFunction = mainMenuWithResume;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.PauseGame ();
				}
				GUI.Label (new Rect (screenWidth * 0.8f, screenHeight * 0.1f, 
		                    screenWidth * 0.2f, screenHeight * 0.1f), 
		          "Placed blocks: " + AssemblyCSharp.NewBehaviourScript.sceneMgr.placedBlockCount);

				AddLeaderboard ();
		}

		/*public void HandleEndOfGame ()
		{
				menuFunction = mainMenu;
		}*/

		//todo - create leaderboard GUI, load it from sceneMgr

}
