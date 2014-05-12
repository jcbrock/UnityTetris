using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		public enum ChangeGameState
		{
				None,
				ClearGame,
				EndGame,
				PauseGame,
				ResumeGame,
				StartGame			
		}

		public class UnityTetris : MonoBehaviour, IInputObserver, IMenuObserver
		{
				public struct UnityRequestInfo
				{
						public enum Type
						{
								RotateShapeRequest,
								TranslateShapeRequest,
								ChangeGameStateRequest
						}

				
			
						//From input
						public struct RotateShapeData
						{
						};
						public struct TranslateShapeData
						{
								public Vector3 movementVector;
						};

						//From Game State (which could come from menu too)
						public struct ChangeGameStateData
						{
								public ChangeGameState changeGameStateTo;
						};
			
						public string debugName;
						public	UnityRequestInfo.Type type;
						public	RotateShapeData rotationData;			
						public	TranslateShapeData translationData;
						public ChangeGameStateData gameStateData;
				}


				public static AssemblyCSharp.SceneManager sceneMgr;
				//private Queue<action> actionQueue = new Queue ();
		
				// Use this for initialization
				void Start ()
				{
						Debug.Log ("Start called!");
						sceneMgr = new AssemblyCSharp.SceneManager ();

						//Register this class as an observer with the input and menu controller
						GameObject go = GameObject.Find ("GameObject");
						PlayerControl inputController = (PlayerControl)go.GetComponent (typeof(PlayerControl));
						inputController.RegisterObserver (this);										
						DelegateMenu menu = (DelegateMenu)go.GetComponent (typeof(DelegateMenu));
						menu.RegisterObserver (this);
				}
			
				CurrentGameState currentGameState = new CurrentGameState ();//???
				private Queue<UnityRequestInfo> requestQueue = new Queue<UnityRequestInfo> ();

				private void UpdateQueuedRequests ()
				{						
						if (requestQueue.Count == 0)
								return;
			
						//pop request
						UnityRequestInfo request = requestQueue.Dequeue (); //TODO - throttle?						
			
						switch (request.type) {
						case UnityRequestInfo.Type.RotateShapeRequest:
								{
										HandleRotateShapeRequest (request);
										break;
								}
						case UnityRequestInfo.Type.TranslateShapeRequest:
								{
										HandleTranslateShapeRequest (request);
										break;
								}
						case UnityRequestInfo.Type.ChangeGameStateRequest:
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
				private void HandleRotateShapeRequest (UnityRequestInfo request)
				{
						//update current game state, publish to subscribers
						sceneMgr.RotateShape (request);


				}
				private void HandleTranslateShapeRequest (UnityRequestInfo request)
				{
						//update current game state, publish to subscribers
						sceneMgr.TranslateShape (request);
				}
				private void HandleChangeGameStateRequest (UnityRequestInfo request)
				{
						//update current game state, publish to subscribers
						sceneMgr.ChangeGameState (request);
				}
				public void Translate (Vector3 movementVector)
				{		
						UnityTetris.UnityRequestInfo request = new UnityRequestInfo ();
						request.debugName = "Translate";
						request.type = UnityTetris.UnityRequestInfo.Type.TranslateShapeRequest;
						request.translationData.movementVector = movementVector;
						//request.rotationData = null;
						//request.gameStateData = null;
						requestQueue.Enqueue (request);
				}
				public void Rotate ()
				{		
						//	SceneRequestInfo startRequest;
						//	startRequest.debugName = "start";
						//	startRequest.type = SceneRequestInfo.Type.StartGame;
						//requestQueue2.Enqueue (request);
				}
				public void ChangeGameState (ChangeGameState newState)
				{		
						UnityTetris.UnityRequestInfo request = new UnityRequestInfo ();
						request.debugName = "ChangeGameState";
						request.type = UnityTetris.UnityRequestInfo.Type.ChangeGameStateRequest;
						//request.translationData.movementVector = movementVector;
						//request.rotationData = null;
						request.gameStateData.changeGameStateTo = newState;
						requestQueue.Enqueue (request);
				}
		
				// Update is called once per frame
				int frameCounter = 0;
				void Update ()
				{
						currentGameState.UpdateQueuedRequests ();
						UpdateQueuedRequests ();
						sceneMgr.UpdateQueuedRequests ();
				

						//eh, this depends on the frame time through, I will need to switch this to time #TODO
						if (frameCounter == 60) {
								//foreach (var foo in actionQueue) {
								//check to make sure we still have time this frame?
								//do Foo (call menu, update scene object, etc)
				
								//}
								
								//sceneMgr.Tick ();
								Translate (new Vector3 (0, -1f, 0));

								//s.Rotate90Degrees ();
								frameCounter = 0;
						} else {
								frameCounter++;
						}
				}

				void IInputObserver.notify (UnityEngine.KeyCode pressedKey)
				{					
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
						if (Input.GetKeyDown (KeyCode.LeftArrow)) {
								movementVector.x = -1.0f;
						} else if (Input.GetKeyDown (KeyCode.RightArrow)) { 
								movementVector.x = 1.0f;
						} else if (Input.GetKeyDown (KeyCode.DownArrow)) { 
								movementVector.y = -1.0f;
						}
						if (movementVector.x != 0 || movementVector.y != 0) {
								Translate (movementVector);
						}
			
						if (Input.GetKeyDown (KeyCode.UpArrow)) {							
								Rotate ();
						}
				}

				void IMenuObserver.notify (ChangeGameState newState)
				{					
						ChangeGameState (newState);
				}
		}
}