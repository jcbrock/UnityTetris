using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		public class UnityTetris : MonoBehaviour, IInputObserver, IMenuObserver, ISceneRulesObserver
		{
				//Apparently Structs are more different than classes in C# than they are in C++
				//Their member variables default to private like classes do, (which is different than c++)		

				public struct SceneRequestInfo
				{
						public enum Type
						{
								RotateShapeRequest,
								TranslateShapeRequest,
								ChangeGameStateRequest
						}

						public struct TranslateShapeData
						{
								public Vector3 movementVector;
						};
									
						public	SceneRequestInfo.Type type;						
						public	TranslateShapeData translationData;		
						public GameState newGameState;						
				}

				private Queue<SceneRequestInfo> requestQueue = new Queue<SceneRequestInfo> ();												
				private GameState currentGameState = GameState.None;
				public SceneRules scene; //can make private?
				private AssemblyCSharp.Leaderboard mLeaderboard = new Leaderboard ();
						
				void Start ()
				{
						Application.runInBackground = true; 												
						scene = new SceneRules ();
						scene.RegisterObserver (this);

						//Register this class as an observer with the input and menu controller
						GameObject go = GameObject.Find ("GameObject");
						PlayerControl inputController = (PlayerControl)go.GetComponent (typeof(PlayerControl));
						inputController.RegisterObserver (this);										
						DelegateMenu gameStateController = (DelegateMenu)go.GetComponent (typeof(DelegateMenu));
						gameStateController.RegisterObserver (this);
				}										

				
				public void Translate (Vector3 movementVector)
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.TranslateShapeRequest;
						request.translationData.movementVector = movementVector;						
						requestQueue.Enqueue (request);
				}
				public void Rotate ()
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.RotateShapeRequest;																	
						requestQueue.Enqueue (request);
				}
				public void ChangeGameState (GameState newState)
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.ChangeGameStateRequest;	
						request.newGameState = newState;
						requestQueue.Enqueue (request);
				}

				private void UpdateQueuedRequests ()
				{						
						if (requestQueue.Count == 0)
								return;
			
						SceneRequestInfo request = requestQueue.Dequeue (); //TODO - throttle?

						switch (request.type) {
						case SceneRequestInfo.Type.RotateShapeRequest:
								{
										if ((currentGameState & AssemblyCSharp.GameState.Running) == AssemblyCSharp.GameState.Running) 
												HandleRotateShapeRequest (request);
										break;
								}
						case SceneRequestInfo.Type.TranslateShapeRequest:
								{
										if ((currentGameState & AssemblyCSharp.GameState.Running) == AssemblyCSharp.GameState.Running) 
												HandleTranslateShapeRequest (request);
										break;
								}
						case SceneRequestInfo.Type.ChangeGameStateRequest:
								{
										HandleChangeGameStateRequest (request);
										break;
								}
						default:
								{
										UnityEngine.Debug.LogWarning ("No type sent on GameState request... this is probably bad!");
										break;
								}
						}					
				}
				private void HandleChangeGameStateRequest (SceneRequestInfo request)
				{																
						UnityEngine.Debug.Log (" currentGameState: " + currentGameState.ToString () + " request.newGameState: " + request.newGameState.ToString ());
						if (request.newGameState == AssemblyCSharp.GameState.Running && 
								((currentGameState & (AssemblyCSharp.GameState.None | AssemblyCSharp.GameState.Ended)) != 0)) {
								scene.Initialize (24, 8, 0); //start new game
						}
						if (request.newGameState == AssemblyCSharp.GameState.Ended) {
								mLeaderboard.AddHighScore (scene.GetCurrentScore ());
								scene.Cleanup (); //cleanup game
						}

						currentGameState = request.newGameState;
				}

				private void HandleRotateShapeRequest (SceneRequestInfo request)
				{										
						scene.HandleRotateRequest ();
				}
				private void HandleTranslateShapeRequest (SceneRequestInfo request)
				{									
						scene.HandleTranslateRequest (request.translationData.movementVector);
				}
		
				void IInputObserver.notify (UnityEngine.KeyCode pressedKey)
				{					
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
						if (pressedKey == KeyCode.LeftArrow) {
								movementVector.x = -1.0f;
						} else if (pressedKey == KeyCode.RightArrow) { 
								movementVector.x = 1.0f;
						} else if (pressedKey == KeyCode.DownArrow) { 
								movementVector.y = -1.0f;
						}
						if (movementVector.x != 0 || movementVector.y != 0) {
								Translate (movementVector);
						}						
						if (pressedKey == KeyCode.UpArrow) {							
								Rotate ();											
						}
				}
		
				void IMenuObserver.notify (GameState gameState)
				{					
						ChangeGameState (gameState);
				}
				void ISceneRulesObserver.notify (SceneInfo sceneInfo)
				{					
						if (sceneInfo.StateUpdate == StateUpdate.GameEnded)
								currentGameState = AssemblyCSharp.GameState.Ended;
				}

				// Update is called once per frame
				int frameCounter = 0;
				void Update ()
				{						
						UpdateQueuedRequests ();						
						
						//eh, this depends on the frame time through, I will need to switch this to time #TODO
						//Every so often, tick object down
						if (frameCounter == 5) {								
								Translate (new Vector3 (0, -1f, 0));											
								frameCounter = 0;
						} else {
								frameCounter++;
						}
				}

				public int GetCurrentScore ()
				{
						return scene.GetCurrentScore ();
				}

				public List<LeaderboardScore> GetCurrentHighScores ()
				{
						return mLeaderboard.GetHighScores ();
				}
		}
}