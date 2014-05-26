using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		public class UnityTetris : MonoBehaviour, IInputObserver, IMenuObserver, IClassicTetrisStateObserver
		{			
				private const float USER_GAME_SPEED = .15f;
				private const float AI_GAME_SPEED = .01f;

				public ClassicTetrisRules Scene { get { return mScene; } } //Exposing for AI class			
				private ClassicTetrisRules mScene; //can make private?
				private Queue<GameRequest> mRequestQueue = new Queue<GameRequest> ();												
				private GameState mCurrentGameState = GameState.None;				
				private float mTickFrequency = USER_GAME_SPEED;				
				private AssemblyCSharp.Leaderboard mLeaderboard = new Leaderboard ();
						
				void Start ()
				{
						Application.runInBackground = true; 												
						mScene = new ClassicTetrisRules ();
						mScene.RegisterObserver (this);

						//Register this class as an observer with the input and menu controller
						GameObject go = GameObject.Find ("GameObject");
						PlayerControl inputController = (PlayerControl)go.GetComponent (typeof(PlayerControl));
						inputController.RegisterObserver (this);										
						DelegateMenu gameStateController = (DelegateMenu)go.GetComponent (typeof(DelegateMenu));
						gameStateController.RegisterObserver (this);
				}										
						
				public void Translate (Vector3 movementVector)
				{		
						GameRequest request = new GameRequest ();						
						request.type = GameRequest.Type.TranslateShapeRequest;
						request.translationData.movementVector = movementVector;						
						mRequestQueue.Enqueue (request);
				}
				public void Rotate ()
				{		
						GameRequest request = new GameRequest ();						
						request.type = GameRequest.Type.RotateShapeRequest;																	
						mRequestQueue.Enqueue (request);
				}
				public void ChangeGameState (GameState newState)
				{		
						GameRequest request = new GameRequest ();						
						request.type = GameRequest.Type.ChangeGameStateRequest;	
						request.newGameState = newState;
						mRequestQueue.Enqueue (request);
				}

				public int GetCurrentScore ()
				{
						return mScene.GetCurrentScore ();
				}
		
				public List<LeaderboardScore> GetCurrentHighScores ()
				{
						return mLeaderboard.HighScores;
				}
		
				private void UpdateQueuedRequests ()
				{						
						if (mRequestQueue.Count == 0)
								return;
			
						GameRequest request = mRequestQueue.Dequeue (); //TODO - throttle?

						switch (request.type) {
						case GameRequest.Type.RotateShapeRequest:
								{
										if ((mCurrentGameState & AssemblyCSharp.GameState.Running) == AssemblyCSharp.GameState.Running) 
												HandleRotateShapeRequest (request);
										break;
								}
						case GameRequest.Type.TranslateShapeRequest:
								{
										if ((mCurrentGameState & AssemblyCSharp.GameState.Running) == AssemblyCSharp.GameState.Running) 
												HandleTranslateShapeRequest (request);
										break;
								}
						case GameRequest.Type.ChangeGameStateRequest:
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
				private void HandleChangeGameStateRequest (GameRequest request)
				{																
						UnityEngine.Debug.Log (" currentGameState: " + mCurrentGameState.ToString () + " request.newGameState: " + request.newGameState.ToString ());
						if (request.newGameState == AssemblyCSharp.GameState.Running && 
								((mCurrentGameState & (AssemblyCSharp.GameState.None | AssemblyCSharp.GameState.Ended)) != 0)) {
								mScene.Initialize (24, 8, 0); //start new game
						}
						if (request.newGameState == AssemblyCSharp.GameState.Ended) {
								mLeaderboard.AddHighScore (mScene.GetCurrentScore ());
								mScene.Cleanup (); //cleanup game
						}

						mCurrentGameState = request.newGameState;
				}

				private void HandleRotateShapeRequest (GameRequest request)
				{										
						mScene.HandleRotateRequest ();
				}
				private void HandleTranslateShapeRequest (GameRequest request)
				{									
						mScene.HandleTranslateRequest (request.translationData.movementVector);
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
						if (pressedKey == KeyCode.A) { //AI mode turned on, crank up tick frequency
								if (mTickFrequency == AI_GAME_SPEED)
										mTickFrequency = USER_GAME_SPEED;
								else
										mTickFrequency = AI_GAME_SPEED;
						}
				}
		
				void IMenuObserver.notify (GameState gameState)
				{					
						ChangeGameState (gameState);
				}
				void IClassicTetrisStateObserver.notify (ClassicTetrisStateUpdate stateUpdate)
				{					
						if (stateUpdate == ClassicTetrisStateUpdate.GameEnded)
								mCurrentGameState = AssemblyCSharp.GameState.Ended;
				}
		
				// Capture frame-per-second
				//int lastFrameCount = 0;
				//float lastTime = 0;			
				float timeSinceLastTranslate = 0;			
				void Update ()
				{									
						UpdateQueuedRequests ();						
						
						//TODO - cap frame time if needed (cut AI calculations short probably)
						if (mCurrentGameState == GameState.Running && (Time.realtimeSinceStartup - timeSinceLastTranslate) > mTickFrequency) {
								Translate (new Vector3 (0, -1f, 0));
								timeSinceLastTranslate = Time.realtimeSinceStartup;
						}
									
						//UnityEngine.Debug.Log ("FPS: " + Mathf.RoundToInt ((Time.frameCount - lastFrameCount) / (Time.realtimeSinceStartup - lastTime)));
						//lastFrameCount = Time.frameCount;
						//lastTime = Time.realtimeSinceStartup;
				}	
		}
}