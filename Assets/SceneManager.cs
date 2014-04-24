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


		public class SceneManager
		{
				public Shape currentShape;
				public Shape previewShape;				
				public int placedBlockCount = 0;
				public List<LeaderboardScore> highScores = new List<LeaderboardScore> ();
				public bool IsGameOver { get { return isGameOver; } }
				private ShapeFactory factory;
				private bool isGamePaused = false;
				private bool isGameOver = false;
				private System.Collections.ArrayList listOfShapes = new System.Collections.ArrayList ();
				

				public SceneManager ()
				{
						factory = new ShapeFactory ();
				}

				public void StartNewGame ()
				{												
						currentShape = factory.SpawnRandomizedTetrisShape ();
						currentShape.TranslateToInitialPlacement ();
						currentShape.enablePlayerControls ();
						previewShape = factory.SpawnRandomizedTetrisShape ();
						previewShape.disablePlayerControls ();
						isGamePaused = false;
						isGameOver = false;
				}

				public void PauseGame ()
				{
						isGamePaused = true;
						//LoadLeaderboardScores (); //tested, works
						//UnityEngine.Debug.Log (highScores.Count);
						
				}

				public void ResumeGame ()
				{
						isGamePaused = false;
				}

				public void EndGame ()
				{
						if (!isGameOver) {
								LoadLeaderboardScores ();
								try {
										highScores.Add (new LeaderboardScore (System.Environment.MachineName, placedBlockCount, DateTime.Now, "1.0.0"));				                                 
										string json = JsonConvert.SerializeObject (highScores, Formatting.Indented);
										System.IO.File.WriteAllText (@"Leaderboard.txt", json);												
								} catch (Exception ex) {
								}
								previewShape.DeleteShape ();				
								isGameOver = true;								
						}

				}

				public void ClearGame ()
				{
						foreach (Shape s in listOfShapes) {
								s.DeleteShape ();
						}
						if (currentShape != null)
								currentShape.DeleteShape ();
						if (previewShape != null)
								previewShape.DeleteShape ();
						listOfShapes.Clear ();						
						currentShape = null;
						placedBlockCount = 0;
				}
		
				public void LoadLeaderboardScores ()
				{
						try {
								string json;
								using (System.IO.StreamReader file = new System.IO.StreamReader(@"Leaderboard.txt", true)) { 
										json = file.ReadToEnd ();
								}
								highScores = JsonConvert.DeserializeObject<List<LeaderboardScore>> (json);
								highScores = highScores.OrderByDescending (x => x.Score).ToList ();
						} catch (Exception ex) {

						}
			
				}
		
				public void Tick ()
				{			
						if (isGamePaused || isGameOver || currentShape == null)
								return;
						//currentShape.Tick();
						//UnityEngine.Debug.Log (currentBlock.gameObject.transform.position);
						//bool collided = false;

						if (AssemblyCSharp.NewBehaviourScript.sceneMgr.currentShape.isCollidingWithBotWall (0, -1.0f) || AnyCollisions (0, -1)) {
								currentShape.PlayCollisionAudio ();
								++placedBlockCount;
								if (AssemblyCSharp.NewBehaviourScript.sceneMgr.currentShape.isCollidingWithTopWall (0, 0)) {
										UnityEngine.Debug.Log ("END OF GAME!!!");
										UnityEngine.GameObject.Find ("background").audio.Play ();
										EndGame ();
										return;
								}

								listOfShapes.Add (currentShape); //might need to copy it explictly
								currentShape.disablePlayerControls ();

								Dictionary<int, int> rowCounts = new Dictionary<int, int> ();
								foreach (Shape s in listOfShapes) {
										foreach (int row in s.GetRowValuesOfSubBlocks()) {
												if (rowCounts.ContainsKey (row))
														rowCounts [row]++;
												else
														rowCounts.Add (row, 1);
										}
								}

								//I think the better approach before I delete anything is to do a pass to tell which rows to be deleted.
								//I don't think it is possible for rows to become completed after shifting... they merely shift down.
								//So I don't think I need to do any more checks, I can just delete the rows top to bottom and be fine.
								List<int> fullRows = new List<int> ();
								foreach (int row in rowCounts.Keys) {
										if (IsRowComplete (rowCounts [row])) {
												fullRows.Add (row);
												UnityEngine.Debug.Log ("Row " + row + " is full");
										}
								}
								fullRows = fullRows.OrderByDescending (i => i).ToList ();			
								foreach (int row in fullRows) {
										DeleteRow (row);
								}

								//why do the shapes not get deleted until later???
								//Actual object destruction is always delayed until after the current Update loop, but will always be done before rendering.
								//https://docs.unity3d.com/Documentation/ScriptReference/Object.Destroy.html	
																
								currentShape = previewShape;								
								currentShape.enablePlayerControls ();
								currentShape.TranslateToInitialPlacement ();
								previewShape = factory.SpawnRandomizedTetrisShape ();
						} else {
								currentShape.translate (0, -1, 0);
						}
				}
						
				private bool IsRowComplete (int rowCount)
				{
						if (rowCount == 10)
								return true;
						else
								return false;
				}

				private void DeleteRow (int row)
				{

						foreach (Shape s in listOfShapes) {								
								s.DeleteBlocksInRow (row);								
						}

						List<Shape> shapesToRemove = new List<Shape> ();
						foreach (Shape s3 in listOfShapes) {
								if (s3.BlockCount == 0) {
										shapesToRemove.Add (s3);
					
								}
						}
						foreach (Shape s in shapesToRemove) {
								listOfShapes.Remove (s);
								UnityEngine.Debug.Log (s.Name + " has been completely destroyed.");
								s.DeleteShape ();
						}
									
						foreach (Shape s2 in listOfShapes) {										
								s2.ShiftBlocksAboveDeletedRow (row);										
						}
						
						//Debug printing...
						string shapeList = "List of shapes(" + listOfShapes.Count + "): " + Environment.NewLine;
						foreach (Shape s in listOfShapes) {
								shapeList += s.Name + Environment.NewLine;
						}
						UnityEngine.Debug.Log (shapeList);
				}

				//Debug helper function
				public int GetRowCount (int row)
				{
						Dictionary<int, int> rowCounts = new Dictionary<int, int> ();
						foreach (Shape s in listOfShapes) {
				
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

				public bool AnyCollisions (float xDelta, float yDelta)
				{
						foreach (Shape shape in listOfShapes) { //for each object in the scene that is colliable
								if (currentShape.collides (shape, xDelta, yDelta))
										return true;
						}

						return false;
				}
		}
}

