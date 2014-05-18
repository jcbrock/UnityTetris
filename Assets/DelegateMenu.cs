using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DelegateMenu : MonoBehaviour, AssemblyCSharp.IInputObserver
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

		private List<AssemblyCSharp.IMenuObserver> registeredObservers = new List<AssemblyCSharp.IMenuObserver> ();


		// Use this for initialization
		private void Start ()
		{
				screenHeight = Screen.height;
				screenWidth = Screen.width;

				buttonHeight = screenHeight * 0.2f;
				buttonWidth = screenWidth * 0.4f;

				menuFunction = mainMenu;

				//Register with the input controller so I observer updates
				GameObject go = GameObject.Find ("GameObject");
				AssemblyCSharp.PlayerControl inputController = (AssemblyCSharp.PlayerControl)go.GetComponent (typeof(AssemblyCSharp.PlayerControl));
				inputController.RegisterObserver (this);
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
						NotifyObservers (AssemblyCSharp.ChangeGameState.ClearGame);
						NotifyObservers (AssemblyCSharp.ChangeGameState.StartGame);
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
						NotifyObservers (AssemblyCSharp.ChangeGameState.ResumeGame);			
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.4f, 
		                          buttonWidth, buttonHeight), "Start New Game")) {						
						menuFunction = inGameHUD;
						NotifyObservers (AssemblyCSharp.ChangeGameState.EndGame);
						NotifyObservers (AssemblyCSharp.ChangeGameState.ClearGame);
						NotifyObservers (AssemblyCSharp.ChangeGameState.StartGame);
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
				//AssemblyCSharp.UnityTetris.sceneMgr.LoadLeaderboardScores ();
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
				AddBlockCount ();
				AddLeaderboard ();
		}

		void AssemblyCSharp.IInputObserver.notify (UnityEngine.KeyCode pressedKey)
		{					
				UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
				if (Input.GetKeyDown (KeyCode.Escape)) {
						menuFunction = mainMenuWithResume;
						NotifyObservers (AssemblyCSharp.ChangeGameState.PauseGame);
				}
		}

		public void RegisterObserver (AssemblyCSharp.IMenuObserver observer)
		{
				registeredObservers.Add (observer);
		}
		public void UnregisterObserver (AssemblyCSharp.IMenuObserver observer)
		{
				registeredObservers.Remove (observer);
		}
		private void NotifyObservers (AssemblyCSharp.ChangeGameState newState)
		{
				foreach (AssemblyCSharp.IMenuObserver observer in registeredObservers) {
						observer.notify (newState);
				}
		
		}		
}
