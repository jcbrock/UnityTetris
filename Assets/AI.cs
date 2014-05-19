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
				public float score;
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
						s.ShadeSubBlock (0);
						List<AssemblyCSharp.AIPlacementEval> gridScores = new List<AIPlacementEval> ();
						/*for (int i = 0; i < sceneGrid.GetRowCount(); ++i) {
								for (int j = 0; j < sceneGrid.GetColumnCount(); ++j) {					
										gridScores.AddRange (ComputeScore (s, sceneGrid, i, j)); //change from being current shape to prediction...															
								}
						}*/
						for (int j = 0; j < sceneGrid.GetColumnCount(); ++j) {
								for (int i = 0; i < 4; ++i) {
										AIPlacementEval score = ComputeScore3 (s, sceneGrid, j);										
										if (score != null) {
												score.numberOfRotations = i;
												gridScores.Add (score); //change from being current shape to prediction...															
										}
										s.Rotate ();
								}
						}
						//s.Rotate ();

						foreach (AIPlacementEval colScore in gridScores)
								UnityEngine.Debug.Log (colScore.print ());

									
						List<AIPlacementEval> bestMoves = gridScores.Where (x => x.status == PlacementStatus.NoCollision && x.pathClear == true).OrderByDescending (x => x.score).ToList ();
						AIPlacementEval bestMove = null;
						if (bestMoves.Count > 0)
								bestMove = bestMoves.Where (x => x.score == bestMoves [0].score).OrderBy (x => x.row).FirstOrDefault ();

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

								//List<KeyValuePair<int, int>> rowCols = s.GetFilledGridValues ();
								List<Coordinate> rowCols = s.GetCurrentGridPosition ();
								int shiftRow = rowTarget - rowCols [0].row;
								int shiftCol = columnTarget - rowCols [0].column;
								//s.ShadeSubBlock (0);


								List<KeyValuePair<int, int>> newRowCols = new List<KeyValuePair<int, int>> ();
								foreach (Coordinate rowCol in rowCols) {		
										newRowCols.Add (new KeyValuePair<int, int> (rowCol.row + shiftRow, rowCol.column + shiftCol));
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
								float neighborCount = -1;
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

				//makes sense to pass in:
				//Shape, 
				//Coordinate (position of shape)
				//grid? or someway to detect grid filled

				private float GetScore (Shape s, int movementY, int columnIndex, TetrisBitArray m_SceneGrid)
				{
						//Score computed by adding number neighbors and deducting points if covering up gap / higher blocks
						float score = 0f;
						List<Coordinate> filledGridPositions = s.GetCurrentGridPosition ();
						foreach (AssemblyCSharp.Coordinate rowCol in filledGridPositions) {
								int newRow = rowCol.row + movementY;
								int newColumn = rowCol.column - filledGridPositions [0].column + columnIndex;
								int right = newColumn + 1;
								int left = newColumn - 1;
								int up = newRow + 1;
								int down = newRow - 1;
								try {
										if (up <= 0 && m_SceneGrid [up, newColumn] == true) {
												++score;
										}

										if (right < 8 && m_SceneGrid [newRow, right] == true) {
												++score;														
										}
										if (right == 8) //boost a little bit on the sides of scores don't clump in the middle
												score += .5f;
										if (left == -1)
												score += .5f;
										if (down > -24 && m_SceneGrid [down, newColumn] == true) {
												++score;
										}
										//if (down > -24 && m_SceneGrid [down, newColumn] == false && filledGridPositions.Count (x => x.row == down && x.column == newColumn) == 0) {
										if (down > -24 && m_SceneGrid [down, newColumn] == false && filledGridPositions.Count (x => x.row == (rowCol.row - 1) && x.column == rowCol.column) == 0) {
												//make sure to not -1 when there is a block as part of the same shape below it...
												--score; //removing a point for covering up a hole, including those created by the shape, think upside down L
												//placement.debug += " -1 for covering up hole. down = " + down + " newColumn = " + newColumn;
										}
										if (down == -24) {
												++score;																											
										}
					
										if (left >= 0 && m_SceneGrid [newRow, left] == true) {
												++score;
										}
					
										float fudge = (newRow + 24) * .1f;
										score -= fudge;
								} catch (System.ArgumentOutOfRangeException ex) {
										UnityEngine.Debug.LogError ("break");
								}							
						}
						return score;
				}

				public	AIPlacementEval ComputeScore3 (Shape s, TetrisBitArray m_SceneGrid, int columnIndex)
				{
						List<AssemblyCSharp.Coordinate> filledGridPositions = s.GetCurrentGridPosition ();

						//assuming [0] column is column index
						StringBuilder log = new StringBuilder ();
						List<AIPlacementEval> scores = new List<AIPlacementEval> ();												
									
						//does this shape go out of the grid?
						foreach (Coordinate pos in filledGridPositions) {
								if ((columnIndex + (pos.column - filledGridPositions [0].column)) < 0 || (columnIndex + pos.column - filledGridPositions [0].column) > 7)
										return null;								
						}
						
						int rotatedTimes = 0;
						Vector3 movementVector = new Vector3 (0, 0, 0);
						//for (int column = 0; column < 8; ++column) {
						//for (int rot = 0; rot < 4; ++rot) {										

						//Find the resting place for this shape in this column
						AIPlacementEval placement = new AIPlacementEval ();
						int row = 0;						
						bool collided = false;
						while (!collided) {
								movementVector.y += -1;								
								foreach (Coordinate pos in filledGridPositions) {
										if (pos.row + (int)movementVector.y <= -24 || m_SceneGrid [pos.row + (int)movementVector.y, pos.column - filledGridPositions [0].column + columnIndex] == true) {
												collided = true;
												movementVector.y += 1;
												row = pos.row + (int)movementVector.y;
												break;
										}
								}

						}
						
						//found its resting spot, compute score
						float neighborCount = -1;
						if (collided) {
								placement.status = PlacementStatus.NoCollision;
								placement.score = GetScore (s, (int)movementVector.y, columnIndex, m_SceneGrid);
								/*neighborCount = 0;								
								foreach (AssemblyCSharp.Coordinate rowCol in filledGridPositions) {
										int newRow = rowCol.row + (int)movementVector.y;
										int newColumn = rowCol.column - filledGridPositions [0].column + columnIndex;
										int right = newColumn + 1;
										int left = newColumn - 1;
										int up = newRow + 1;
										int down = newRow - 1;
										try {
												if (up <= 0 && m_SceneGrid [up, newColumn] == true) {
														++neighborCount;
												}
												if (right < 8)
														placement.root += m_SceneGrid [newRow, right].ToString ();
												if (right < 8 && m_SceneGrid [newRow, right] == true) {
														++neighborCount;														
												}
												if (right == 8) //boost a little bit on the sides of scores don't clump in the middle
														neighborCount += .5f;
												if (left == -1)
														neighborCount += .5f;
												if (down > -24 && m_SceneGrid [down, newColumn] == true) {
														++neighborCount;
												}
												//if (down > -24 && m_SceneGrid [down, newColumn] == false && filledGridPositions.Count (x => x.row == down && x.column == newColumn) == 0) {
												if (down > -24 && m_SceneGrid [down, newColumn] == false && filledGridPositions.Count (x => x.row == (rowCol.row - 1) && x.column == rowCol.column) == 0) {
														//make sure to not -1 when there is a block as part of the same shape below it...
														--neighborCount; //removing a point for covering up a hole, including those created by the shape, think upside down L
														placement.debug += " -1 for covering up hole. down = " + down + " newColumn = " + newColumn;
												}
												if (down == -24) {
														++neighborCount;																											
												}
							
												if (left >= 0 && m_SceneGrid [newRow, left] == true) {
														++neighborCount;
												}

												float fudge = (newRow + 24) * .1f;
												neighborCount -= fudge;
										} catch (System.ArgumentOutOfRangeException ex) {
												foreach (Coordinate pos in filledGridPositions) {
														if (pos.row + (int)movementVector.y <= -24 || m_SceneGrid [pos.row + (int)movementVector.y, columnIndex] == true) {
																collided = true;
																//movementVector.y += 1;
																//int row22 = pos.row + (int)movementVector.y;
																//coor.row = pos.row;
																//coor.column = pos.column;
																break;
														}
												}
												UnityEngine.Debug.LogError ("break");
										}
								}*/											

								placement.pathClear = true;
								//placement.numberOfRotations = rot;
								placement.numberOfRotations = 0;								
								placement.row = row;
								placement.column = columnIndex;
								//placement.status = status;
								//rotatedTimes++;
								//s.Rotate ();								
								//scores.Add (placement);
								return placement;

											
						} else {
								UnityEngine.Debug.LogError ("Should never happen");
						}
																	
						string path = @"log.txt";									
						if (!File.Exists (path)) {															
								File.WriteAllText (path, log.ToString ());
						} else {						
								File.AppendAllText (path, log.ToString ());
						}
			
						return null;
						//return scores;
				}
		}
}