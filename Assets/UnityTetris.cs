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
									
						//From input
						public struct RotateShapeData
						{
						};
						public struct TranslateShapeData
						{
								public Vector3 movementVector;
						};

						//From menu
						public struct ChangeGameStateData
						{
								public ChangeGameState changeGameStateTo;
						};
									
						public	SceneRequestInfo.Type type;
						public	RotateShapeData rotationData;			
						public	TranslateShapeData translationData;
						public ChangeGameStateData gameStateData;
						public bool AIModeOn;
				}
		
				public static AssemblyCSharp.SceneManager sceneMgr;
				private Queue<SceneRequestInfo> requestQueue = new Queue<SceneRequestInfo> ();				
				private bool AIModeTurnedOn = false;
				private bool freezeMode = false; //debug mode variable
						
				void Start ()
				{
						Application.runInBackground = true; 						
						sceneMgr = new AssemblyCSharp.SceneManager ();

						//Register this class as an observer with the input and menu controller
						GameObject go = GameObject.Find ("GameObject");
						PlayerControl inputController = (PlayerControl)go.GetComponent (typeof(PlayerControl));
						inputController.RegisterObserver (this);										
						DelegateMenu menu = (DelegateMenu)go.GetComponent (typeof(DelegateMenu));
						menu.RegisterObserver (this);
				}										

				
				public void Translate (Vector3 movementVector)
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.TranslateShapeRequest;
						request.translationData.movementVector = movementVector;
						request.AIModeOn = AIModeTurnedOn;

						if (freezeMode)
								request.translationData.movementVector = new Vector3 ();

						requestQueue.Enqueue (request);
				}
				public void Rotate ()
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.RotateShapeRequest;						
						request.AIModeOn = AIModeTurnedOn;
						//request.rotationData = null;						
						requestQueue.Enqueue (request);
				}
				public void ChangeGameState (ChangeGameState newState)
				{		
						UnityTetris.SceneRequestInfo request = new SceneRequestInfo ();						
						request.type = UnityTetris.SceneRequestInfo.Type.ChangeGameStateRequest;											
						request.gameStateData.changeGameStateTo = newState;
						request.AIModeOn = AIModeTurnedOn;
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
										HandleRotateShapeRequest (request);
										break;
								}
						case SceneRequestInfo.Type.TranslateShapeRequest:
								{
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
				private void HandleRotateShapeRequest (SceneRequestInfo request)
				{						
						sceneMgr.SendSceneRequest (request);
				}
				private void HandleTranslateShapeRequest (SceneRequestInfo request)
				{				
						sceneMgr.SendSceneRequest (request);
				}
				private void HandleChangeGameStateRequest (SceneRequestInfo request)
				{						
						sceneMgr.SendSceneRequest (request);
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
						if (pressedKey == KeyCode.P)
								freezeMode = !freezeMode;
						if (pressedKey == KeyCode.A) {
								AIModeTurnedOn = !AIModeTurnedOn;
								UnityEngine.Debug.Log ("AI mode turned on: " + AIModeTurnedOn.ToString ());
						}
				}
		
				void IMenuObserver.notify (ChangeGameState newState)
				{					
						ChangeGameState (newState);
				}

				// Update is called once per frame
				int frameCounter = 0;
				void Update ()
				{						
						UpdateQueuedRequests ();
						sceneMgr.UpdateQueuedRequests ();
						
						//eh, this depends on the frame time through, I will need to switch this to time #TODO
						//Every so often, tick object down
						if (frameCounter == 5) {								
								Translate (new Vector3 (0, -1f, 0));											
								frameCounter = 0;
						} else {
								frameCounter++;
						}
				}
		}
}