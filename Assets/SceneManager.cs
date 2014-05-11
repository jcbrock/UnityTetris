//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. 
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;
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

		
		//Apparently Structs are more different than classes in C# than they are in C++
		//Yet, their member variables default to private like classes do, (which is different than c++)
		public struct SceneRequestInfo
		{
				public enum Type
				{
						None,
						ClearGame,
						EndGame,
						PauseGame,
						ResumeGame,
						StartGame
						
				}

				public struct ClearGameData
				{
						
				};
				public struct EndGameData
				{
			
				};
				public struct PauseGameData
				{
			
				};
				public struct ResumeGameData
				{
			
				};
				public struct StartGameData
				{
			
				};
				
				public string debugName;// = "foo";
				public	SceneRequestInfo.Type type;
				public	ClearGameData clearGame;
				public	EndGameData endGame;
				public	PauseGameData pauseGame;
				public	ResumeGameData resumeGame;
				public	StartGameData startGame;				
		}



		//also fix some using of member varibles as globals more like
		public class SceneManager
		{
				enum GameState
				{
						None,
						Paused,
						Running
				}

				//For these member variables surfaced outside of class, only expose as read-only
				public Shape CurrentShape { get { return m_CurrentShape; } }
				public List<LeaderboardScore> HighScores { get { return m_HighScores; } }
				public int PlacedBlockCount { get { return m_PlacedBlockCount; } }
		
				private ShapeFactory m_Factory;
				private Shape m_CurrentShape;
				private Shape previewShape;
				private List<LeaderboardScore> m_HighScores = new List<LeaderboardScore> ();
				private int m_PlacedBlockCount = 0;
							
				private System.Collections.ArrayList m_ListOfShapes = new System.Collections.ArrayList ();
				private int m_RowWidth = 10; //must match the Unity grid


				private Queue<SceneRequestInfo> requestQueue = new Queue<SceneRequestInfo> ();
				private GameState gameState;

				public SceneManager ()
				{
						m_Factory = new ShapeFactory ();
						LoadLeaderboardScores ();						
				}

				
				public void UpdateQueuedRequests ()
				{						
						if (requestQueue.Count == 0)
								return;

						//pop request
						SceneRequestInfo request = requestQueue.Dequeue (); //TODO - throttle?						
									
						switch (request.type) {
						case SceneRequestInfo.Type.EndGame:
								{
										HandleEndGameRequest (request);
										break;
								}
						case SceneRequestInfo.Type.ClearGame:
								{
										HandleClearGameRequest (request);
										break;
								}
						case SceneRequestInfo.Type.PauseGame:
								{
										HandlePauseGameRequest (request);
										break;
								}		
						case SceneRequestInfo.Type.ResumeGame:
								{
										HandleResumeGameRequest (request);
										break;
								}		
						case SceneRequestInfo.Type.StartGame:
								{
										HandleStartGameRequest (request);
										break;
								}
						default:
								{
										UnityEngine.Debug.LogWarning ("No type sent on GameState request... this is probably bad!");
										break;
								}
						}


				}

				private void HandleClearGameRequest (SceneRequestInfo request)
				{
						foreach (Shape s in m_ListOfShapes) {
								s.DeleteShape ();
						}
						if (m_CurrentShape != null)
								m_CurrentShape.DeleteShape ();
						if (previewShape != null)
								previewShape.DeleteShape ();
						m_ListOfShapes.Clear ();						
						m_CurrentShape = null;
						m_PlacedBlockCount = 0;
				}
				private void HandleEndGameRequest (SceneRequestInfo request)
				{
						if (!m_IsGameOver) {
								SaveLeaderboardScores ();
								previewShape.DeleteShape ();				
								m_IsGameOver = true;								
						}
						gameState = GameState.Paused;
				}
				private void HandlePauseGameRequest (SceneRequestInfo request)
				{
						m_IsGamePaused = true;
						gameState = GameState.Paused;
				}
				private void HandleResumeGameRequest (SceneRequestInfo request)
				{
						m_IsGamePaused = false;
						gameState = GameState.Running;
				}

				private void HandleStartGameRequest (SceneRequestInfo request)
				{
						m_CurrentShape = m_Factory.SpawnRandomizedTetrisShape ();
						m_CurrentShape.TranslateToInitialPlacement ();
						m_CurrentShape.enablePlayerControls ();
						previewShape = m_Factory.SpawnRandomizedTetrisShape ();
						previewShape.disablePlayerControls ();
						m_IsGamePaused = false;
						m_IsGameOver = false;
						gameState = GameState.Running;
				}


				public void StartNewGame ()
				{		
						SceneRequestInfo startRequest;
						startRequest.debugName = "start";
						startRequest.type = SceneRequestInfo.Type.StartGame;
						requestQueue.Enqueue (startRequest);
				}

				public void PauseGame ()
				{
						SceneRequestInfo pauseRequest;
						pauseRequest.type = SceneRequestInfo.Type.PauseGame;
						pauseRequest.debugName = "pause";
						requestQueue.Enqueue (pauseRequest);												
				}
				public void ResumeGame ()
				{
						SceneRequestInfo resumeRequest;
						resumeRequest.type = SceneRequestInfo.Type.ResumeGame;
						resumeRequest.debugName = "resume";
						requestQueue.Enqueue (resumeRequest);						
				}
				public void EndGame ()
				{
						SceneRequestInfo endRequest;
						endRequest.type = SceneRequestInfo.Type.EndGame;
						endRequest.debugName = "end";
						requestQueue.Enqueue (endRequest);					
				}
				public void ClearGame ()
				{
						SceneRequestInfo clearRequest;
						clearRequest.type = SceneRequestInfo.Type.ClearGame;
						clearRequest.debugName = "Clear request!";
						requestQueue.Enqueue (clearRequest);
				}

				private void SaveLeaderboardScores ()
				{
						List<LeaderboardScore> highScores = LoadLeaderboardScores ();
						try {
								highScores.Add (new LeaderboardScore (System.Environment.MachineName, m_PlacedBlockCount, DateTime.Now, "1.0.0"));
								m_HighScores = highScores; //update public exposed leaderboard for GUI
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
								m_HighScores = highScores; //update public exposed leaderboard for GUI
						} catch (Exception ex) {
								UnityEngine.Debug.LogWarning ("Error loading leaderboard scores: " + ex.Message);
						}
						return highScores;
				}

				public bool DoAnyShapesCollideInScene (UnityEngine.Vector3 movementVector)
				{
						foreach (Shape shape in m_ListOfShapes) { //for each object in the scene that is colliable
								if (m_CurrentShape.collides (shape, movementVector))
										return true;
						}
						return false;
				}
		
				public void Tick ()
				{			
						UpdateQueuedRequests ();

						if (gameState == GameState.Paused || m_CurrentShape == null)
								return;
						//currentShape.Tick();
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, -1.0f, 0);
						if (AssemblyCSharp.UnityTetris.sceneMgr.m_CurrentShape.CheckCollisionWithBotWall (movementVector) || DoAnyShapesCollideInScene (movementVector)) {
								m_CurrentShape.PlayCollisionAudio ();
								++m_PlacedBlockCount;
								m_ListOfShapes.Add (m_CurrentShape);

								//Handle game end condition
								if (AssemblyCSharp.UnityTetris.sceneMgr.m_CurrentShape.CheckCollisionWithTopWall (0, 0)) {
										UnityEngine.GameObject.Find ("background").audio.Play ();
										EndGame ();
										return;
								}

								//My approach to deleting rows: detect full rows, delete (and shift shapes down) top to bottom
								//Note: Actual object destruction is always delayed until after the current Update loop, but will always be done before rendering.
								//https://docs.unity3d.com/Documentation/ScriptReference/Object.Destroy.html

								//Add up number of blocks in each row
								Dictionary<int, int> rowCounts = new Dictionary<int, int> ();
								foreach (Shape s in m_ListOfShapes) {
										foreach (int row in s.GetRowValuesOfSubBlocks()) {
												if (rowCounts.ContainsKey (row))
														rowCounts [row]++;
												else
														rowCounts.Add (row, 1);
										}
								}
								//Make sure it is ordered top to bottom
								rowCounts = rowCounts.OrderByDescending (x => x.Key).ToDictionary (x => x.Key, x => x.Value);

								//Detect full rows and delete/shift
								foreach (int row in rowCounts.Keys) {
										if (rowCounts [row] == m_RowWidth) {
												UnityEngine.Debug.Log ("Row " + row + " is full. Deleting now...");
												DeleteRow (row);
										} else {
												UnityEngine.Debug.LogWarning ("Row contains over " + m_RowWidth + " blocks");
										}
								}

								//Switch to previewed Shape and generate a new one
								m_CurrentShape.disablePlayerControls ();
								m_CurrentShape = previewShape;								
								m_CurrentShape.enablePlayerControls ();
								m_CurrentShape.TranslateToInitialPlacement ();
								previewShape = m_Factory.SpawnRandomizedTetrisShape ();
						} else {
								m_CurrentShape.translate (movementVector);
						}
				}

				private void DeleteRow (int row)
				{
						List<Shape> shapesToRemove = new List<Shape> ();
						foreach (Shape s in m_ListOfShapes) {								
								if (s.DeleteBlocksInRow (row) == 0) {
										shapesToRemove.Add (s); //can't modify list while I'm iterating through it, mark for delete
								}
						}

						foreach (Shape s in shapesToRemove) {
								UnityEngine.Debug.Log (s.Name + " has been completely destroyed.");
								m_ListOfShapes.Remove (s);
								s.DeleteShape ();
						}
									
						foreach (Shape s2 in m_ListOfShapes) {										
								s2.ShiftBlocksAboveDeletedRow (row);										
						}
						
						//Debug printing...
						string shapeList = "List of shapes(" + m_ListOfShapes.Count + "): " + Environment.NewLine;
						foreach (Shape s in m_ListOfShapes) {
								shapeList += s.Name + Environment.NewLine;
						}
						UnityEngine.Debug.Log (shapeList);
				}


				//Debug helper function
				public int GetRowCount (int row)
				{
						Dictionary<int, int> rowCounts = new Dictionary<int, int> ();
						foreach (Shape s in m_ListOfShapes) {
				
								//UnityEngine.Debug.Log ("Block count for shape: " + s.BlockCount ());
				
								foreach (int rownum in s.GetRowValuesOfSubBlocks()) {
										if (rowCounts.ContainsKey (rownum))
												rowCounts [rownum]++;
										else
												rowCounts.Add (rownum, 1);
								}
						}
			
						if (!rowCounts.ContainsKey (row))
								return -1;
			
						return rowCounts [row];
				}
		}
}

