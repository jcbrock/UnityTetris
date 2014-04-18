using UnityEngine;
using System.Collections;

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

				menuFunction = anyKey;
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

		
		//GUI.Button startGame = new GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.1f, 
		//                                          buttonWidth, buttonHeight), "Start Game");
		void mainMenu ()
		{
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.1f, 
		                                                     buttonWidth, buttonHeight), "Start New Game")) {
						//make sure to kick off new game
						menuFunction = inGameHUD;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.StartNewGame ();
				}
				if (GUI.Button (new Rect ((screenWidth - buttonWidth) * 0.5f, screenHeight * 0.5f, 
		                        buttonWidth, buttonHeight), "Quit Game")) {
						Application.Quit ();
				}
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
		}

		void inGameHUD ()
		{
				if (Input.GetKeyDown (KeyCode.Escape)) {						
						menuFunction = mainMenuWithResume;
						AssemblyCSharp.NewBehaviourScript.sceneMgr.PauseGame ();
				}
				GUI.Label (new Rect (screenWidth * 0.8f, screenHeight * 0.1f, 
		                    screenWidth * 0.2f, screenHeight * 0.1f), 
		          "Placed blocks: " + AssemblyCSharp.NewBehaviourScript.sceneMgr.placedBlockCount);
		}

}
