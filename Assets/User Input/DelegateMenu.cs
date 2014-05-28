using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class DelegateMenu : MonoBehaviour, AssemblyCSharp.IInputObserver, AssemblyCSharp.IClassicTetrisStateObserver
{
		private delegate void MenuDelegate ();
		private MenuDelegate menuFunction;

		//OnGUI gets called multiple times a frame
		//so if you're accessing the static Screen class
		//and getting properties from it, that can be expensive
		//so we'll store these values in memory instead
		private float mScreenHeight;
		private float mScreenWidth;
		private float mButtonHeight;
		private float mButtonWidth;				
		private AssemblyCSharp.UnityTetris mUnityTetris;		
		private List<AssemblyCSharp.IMenuObserver> mRegisteredObservers = new List<AssemblyCSharp.IMenuObserver> ();


		// Use this for initialization
		private void Start ()
		{
				mScreenHeight = Screen.height;
				mScreenWidth = Screen.width;

				mButtonHeight = mScreenHeight * 0.2f;
				mButtonWidth = mScreenWidth * 0.4f;

				menuFunction = mainMenu;							

				//Register with the input controller so I observer updates
				GameObject go = GameObject.Find ("GameObject");
				AssemblyCSharp.PlayerControl inputController = (AssemblyCSharp.PlayerControl)go.GetComponent (typeof(AssemblyCSharp.PlayerControl));
				inputController.RegisterObserver (this);
				mUnityTetris = (AssemblyCSharp.UnityTetris)go.GetComponent (typeof(AssemblyCSharp.UnityTetris));									
		}

		private void OnGUI ()
		{
				menuFunction ();
		}
	
		private void mainMenu ()
		{
				if (GUI.Button (new Rect ((mScreenWidth - mButtonWidth) * 0.5f, mScreenHeight * 0.25f, 
		                                                     mButtonWidth, mButtonHeight), "Start New Game")) {
						//make sure to kick off new game
						menuFunction = inGameHUD;
						NotifyObservers (AssemblyCSharp.GameState.Ended);
						NotifyObservers (AssemblyCSharp.GameState.Running);						
				}
				if (GUI.Button (new Rect ((mScreenWidth - mButtonWidth) * 0.5f, mScreenHeight * 0.5f, 
		                        mButtonWidth, mButtonHeight), "Quit Game")) {
						Application.Quit ();
				}
				mUnityTetris.Scene.RegisterObserver (this);
				AddLeaderboard ();
		
		}
		private void mainMenuWithResume ()
		{
				if (GUI.Button (new Rect ((mScreenWidth - mButtonWidth) * 0.5f, mScreenHeight * 0.1f, 
		                          mButtonWidth, mButtonHeight), "Continue Game")) {						
						menuFunction = inGameHUD;
						NotifyObservers (AssemblyCSharp.GameState.Running);						
				}
				if (GUI.Button (new Rect ((mScreenWidth - mButtonWidth) * 0.5f, mScreenHeight * 0.4f, 
		                          mButtonWidth, mButtonHeight), "Start New Game")) {						
						menuFunction = inGameHUD;
						NotifyObservers (AssemblyCSharp.GameState.Ended);
						NotifyObservers (AssemblyCSharp.GameState.Running);
				}
				if (GUI.Button (new Rect ((mScreenWidth - mButtonWidth) * 0.5f, mScreenHeight * 0.7f, 
		                          mButtonWidth, mButtonHeight), "Quit Game")) {
						Application.Quit ();
				}

				AddBlockCount ();
				AddLeaderboard ();
		}
		private void AddLeaderboard ()
		{				
				var highScores = mUnityTetris.GetCurrentHighScores ().Take (5).ToList ();
				if (highScores.Count != 0) {
						GUI.Label (new Rect (mScreenWidth * 0.75f, mScreenHeight * 0.2f, 
		                     mScreenWidth * 0.25f, mScreenHeight * 0.1f), 
		           			 "Leaderboard (score - name)");
		
				
						float height = 0.25f;						
						foreach (AssemblyCSharp.LeaderboardScore score in highScores) {
								GUI.Label (new Rect (mScreenWidth * 0.75f, mScreenHeight * height, 
			                     	mScreenWidth * 0.25f, mScreenHeight * 0.05f), 
			           			 	score.Score + " - " + score.Name);
								height += 0.05f;
						}

				}

		}
		private void AddBlockCount ()
		{
				GUI.Label (new Rect (mScreenWidth * 0.8f, mScreenHeight * 0.1f, 
		                     mScreenWidth * 0.2f, mScreenHeight * 0.1f), 
		           "Placed blocks: " + mUnityTetris.GetCurrentScore ());
		}

		private void inGameHUD ()
		{
				AddBlockCount ();
				AddLeaderboard ();
		}

		void AssemblyCSharp.IInputObserver.notify (UnityEngine.KeyCode pressedKey)
		{									
				if (pressedKey == KeyCode.Escape) {
						menuFunction = mainMenuWithResume;
						//mHighScoresCache = mUnityTetris.GetCurrentHighScores ().Take (5).ToList ();						
						NotifyObservers (AssemblyCSharp.GameState.Paused);
				}
		}
		void AssemblyCSharp.IClassicTetrisStateObserver.notify (AssemblyCSharp.ClassicTetrisStateUpdate stateUpdate)
		{					
				if (stateUpdate == AssemblyCSharp.ClassicTetrisStateUpdate.GameEnded) {						
						menuFunction = mainMenu;						
				}
		}		

		public void RegisterObserver (AssemblyCSharp.IMenuObserver observer)
		{
				mRegisteredObservers.Add (observer);
		}
		public void UnregisterObserver (AssemblyCSharp.IMenuObserver observer)
		{
				mRegisteredObservers.Remove (observer);
		}
		private void NotifyObservers (AssemblyCSharp.GameState newState)
		{
				foreach (AssemblyCSharp.IMenuObserver observer in mRegisteredObservers) {
						observer.notify (newState);
				}		
		}		
}
