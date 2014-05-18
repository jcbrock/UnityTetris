using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;
using System.Text;
using System.IO;

namespace AssemblyCSharp
{
		public enum PlacementStatus
		{
				None,
				CollidedWithWall,
				CollidedWithShape,
				CollidedWithWallAndShape,
				NoCollision
		}
		public class AIPlacementEval
		{
				public int row;
				public int column;
				public int numberOfRotations;
				public int score;
				public bool pathClear;
				public PlacementStatus status;
				public string debug;
				public string root;

				public string print ()
				{
						string stat = string.Empty;
						switch (status) {
						case PlacementStatus.None:
								stat = "None";
								break;
						case PlacementStatus.CollidedWithWall:
								stat = "CollidedWithWall";
								break;
						case PlacementStatus.CollidedWithShape:
								stat = "CollidedWithShape";
								break;
						case PlacementStatus.CollidedWithWallAndShape:
								stat = "CollidedWithWallAndShape";
								break;
						case PlacementStatus.NoCollision:
								stat = "NoCollision";
								break;
						}
						return "PlacementEval - row: " + row + " column: " + column + " score: " + score + " numberOfRotations: " + numberOfRotations + " pathClear: " + pathClear + " status: " + stat + " root: " + root + " debug: " + debug;
				}
		}


		public class AI
		{
				public AIPlacementEval GetBestMove (Shape s, TetrisBitArray sceneGrid)
				{
						List<AssemblyCSharp.AIPlacementEval> gridScores = new List<AIPlacementEval> ();
						for (int i = 0; i < sceneGrid.GetRowCount(); ++i) {
								for (int j = 0; j < sceneGrid.GetColumnCount(); ++j) {					
										gridScores.AddRange (ComputeScore (s, sceneGrid, i, j)); //change from being current shape to prediction...															
								}
						}
									
						List<AIPlacementEval> bestMoves = gridScores.Where (x => x.status == PlacementStatus.NoCollision && x.pathClear == true).OrderByDescending (x => x.score).ToList ();
						AIPlacementEval bestMove = null;
						if (bestMoves.Count > 0)
								bestMove = bestMoves.OrderBy (x => x.row).FirstOrDefault ();

						//DEBUG PRINTING
						string path = @"ai.txt";
						File.Delete (path);
						if (bestMove != null)
								File.AppendAllText (path, "BEST MOVE: " + bestMove.print () + "\n\n");
						else
								File.AppendAllText (path, "No valid move found for this block!\n\n");
			
			
						foreach (AIPlacementEval placement in gridScores) {
								File.AppendAllText (path, placement.print () + "\n");
						}

						return bestMove;
				}
			
				//ok, this shifting must be done an easier way...
				static int foo = 0;
				public	List<AIPlacementEval> ComputeScore (Shape s, TetrisBitArray m_SceneGrid, int rowTarget, int columnTarget)
				{
						StringBuilder log = new StringBuilder ();
						List<AIPlacementEval> scores = new List<AIPlacementEval> ();
						scores.Add (new AIPlacementEval ());
						scores.Add (new AIPlacementEval ());
						scores.Add (new AIPlacementEval ());
						scores.Add (new AIPlacementEval ());
						rowTarget = Mathf.Abs (rowTarget) * -1;

						int rotatedTimes = 0;
						for (int rot = 0; rot < 4; ++rot) {
								PlacementStatus status = PlacementStatus.None;
								log.AppendLine ("RowTarget: " + rowTarget + " ColumnTarget: " + columnTarget + " Rotation: " + rot);
								
								var blah = 0;
								if (rowTarget == -23 && columnTarget == 0)
										blah++;

								List<KeyValuePair<int, int>> rowCols = s.GetFilledGridValues ();
								int shiftRow = rowTarget - rowCols [0].Key;
								int shiftCol = columnTarget - rowCols [0].Value;
								//s.ShadeSubBlock (0);


								List<KeyValuePair<int, int>> newRowCols = new List<KeyValuePair<int, int>> ();
								foreach (KeyValuePair<int,int> rowCol in rowCols) {		
										newRowCols.Add (new KeyValuePair<int, int> (rowCol.Key + shiftRow, rowCol.Value + shiftCol));
								}

								bool collisionDetected = false;
								foreach (KeyValuePair<int,int> rowCol in newRowCols) {
										if ((rowCol.Key <= -24 || rowCol.Key > 0) || (rowCol.Value >= 8 || rowCol.Value < 0)) {
												scores [rot].debug = "rowCol.Key= " + rowCol.Key + " rowCol.Value= " + rowCol.Value + " shiftRow = " + shiftRow + " shiftCol=" + shiftCol; 
												collisionDetected = true;												
												log.AppendLine ("Collision with border! Row: " + rowCol.Key + " Col: " + rowCol.Value + " Bitgrid col == true");																							
												if (status == PlacementStatus.CollidedWithShape)
														status = PlacementStatus.CollidedWithWallAndShape;
												else if (status == PlacementStatus.None)
														status = PlacementStatus.CollidedWithWall;
										} else if (m_SceneGrid [rowCol.Key, rowCol.Value] == true) {
												collisionDetected = true;		
												scores [rot].debug = "Row= " + rowCol.Key + " Col= " + rowCol.Value + " shiftRow = " + shiftRow + " shiftCol=" + shiftCol; 
												log.AppendLine ("Collision with shape! Row: " + rowCol.Key + " Col: " + rowCol.Value + " Bitgrid col == true");
												if (status == PlacementStatus.CollidedWithWall)
														status = PlacementStatus.CollidedWithWallAndShape;
												else if (status == PlacementStatus.None)
														status = PlacementStatus.CollidedWithShape;
										}
								}
							
								//todo - damn, I don't detect if the piece can "rest" at that spot. oh well... cna do that later... it'll just reach that spot, then continue falling.
								bool pathIsCleared = true;
								int neighborCount = -1;
								if (!collisionDetected) {
										//Compute score - for now, count neighbors
										status = PlacementStatus.NoCollision;										
										neighborCount = 0;										
										foreach (KeyValuePair<int,int> rowCol in newRowCols) {
												int right = rowCol.Value + 1;
												int left = rowCol.Value - 1;
												int up = rowCol.Key + 1;
												int down = rowCol.Key - 1;												
												if (up <= 0 && m_SceneGrid [up, rowCol.Value] == true) {
														++neighborCount;
												}
												if (right < 8)
														scores [rot].root += m_SceneGrid [rowCol.Key, right].ToString ();
												if (right < 8 && m_SceneGrid [rowCol.Key, right] == true) {
														++neighborCount;														
												}
												if (down > -24 && m_SceneGrid [down, rowCol.Value] == true) {
														++neighborCount;
												}
												if (down > -24 && m_SceneGrid [down, rowCol.Value] == false && newRowCols.Count (x => x.Key == down && x.Value == rowCol.Value) == 0) {
														--neighborCount; //removing a point for covering up a hole, including those created by the shape, think upside down L
												}
												if (down == -24) {
														++neighborCount;																											
												}

												if (left >= 0 && m_SceneGrid [rowCol.Key, left] == true) {
														++neighborCount;
												}
										
										}
								
										rowTarget = Mathf.Abs (rowTarget) * -1;
										if (rowTarget > 0)
												UnityEngine.Debug.LogWarning ("rowTarget is positive - AI NOT WORKING RIGHT");
										for (int i = 0; ((rowTarget*-1) - i) > 0 + i; ++i) { 
												foreach (KeyValuePair<int,int> rowCol in newRowCols) {
														if (m_SceneGrid [rowCol.Key + i, rowCol.Value] == true) {
																pathIsCleared = false;
																break; //todo - short circuit out of outer loop oto
														}
												}
										}
								}								
								scores [rot].pathClear = pathIsCleared;
								scores [rot].numberOfRotations = rot;
								scores [rot].score = neighborCount;
								scores [rot].row = rowTarget;
								scores [rot].column = columnTarget;
								scores [rot].status = status;
								rotatedTimes++;
								s.Rotate ();								
						}

						string path = @"log.txt";									
						if (!File.Exists (path)) {															
								File.WriteAllText (path, log.ToString ());
						} else {						
								File.AppendAllText (path, log.ToString ());
						}

						for (int i = rotatedTimes; i < 4; ++i)
								s.Rotate ();
						return scores;
				}
		}
}