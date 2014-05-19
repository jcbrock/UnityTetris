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
						//s.ShadeSubBlock (0);
						List<AssemblyCSharp.AIPlacementEval> gridScores = new List<AIPlacementEval> ();					
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

						foreach (AIPlacementEval colScore in gridScores)
								UnityEngine.Debug.Log (colScore.print ());

									
						List<AIPlacementEval> bestMoves = gridScores.Where (x => x.status == PlacementStatus.NoCollision && x.pathClear == true).OrderByDescending (x => x.score).ToList ();
						AIPlacementEval bestMove = null;
						if (bestMoves.Count > 0)
								bestMove = bestMoves.Where (x => x.score == bestMoves [0].score).OrderBy (x => x.row).FirstOrDefault ();								

						return bestMove;
				}
						
				static int foo = 0;
			
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
								if (down > -24 && m_SceneGrid [down, newColumn] == false && filledGridPositions.Count (x => x.row == (rowCol.row - 1) && x.column == rowCol.column) == 0) {												
										--score; //removing a point for covering up a hole, including those created by the shape, think upside down L												
								}
								if (down == -24) {
										++score;																											
								}
					
								if (left >= 0 && m_SceneGrid [newRow, left] == true) {
										++score;
								}
					
								float fudge = (newRow + 24) * .1f;
								score -= fudge;												
						}
						return score;
				}

				public	AIPlacementEval ComputeScore3 (Shape s, TetrisBitArray m_SceneGrid, int columnIndex)
				{
						List<AssemblyCSharp.Coordinate> filledGridPositions = s.GetCurrentGridPosition ();														
						AIPlacementEval placement = new AIPlacementEval ();
						placement.status = PlacementStatus.None; //Defaults to this, but being explicit

						//does this shape go out of the grid? - TODO, make this way more clear
						foreach (Coordinate pos in filledGridPositions) {
								if ((columnIndex + (pos.column - filledGridPositions [0].column)) < 0 || (columnIndex + pos.column - filledGridPositions [0].column) > 7)
										return null;								
						}
		
		
						//Find the resting place for the shape in this column				
						int yTranslation = 0;						
						while (placement.status == PlacementStatus.None) {
								yTranslation += -1;								
								foreach (Coordinate pos in filledGridPositions) {
										if (pos.row + yTranslation <= -24 || m_SceneGrid [pos.row + yTranslation, pos.column - filledGridPositions [0].column + columnIndex] == true) {												
												yTranslation += 1;
												placement.row = pos.row + yTranslation;
												placement.status = PlacementStatus.NoCollision;
												break;
										}
								}
						}
						
						//found its resting spot, compute score																		
						placement.score = GetScore (s, yTranslation, columnIndex, m_SceneGrid);								
						placement.pathClear = true;								
						placement.numberOfRotations = 0;																 
						placement.column = columnIndex;								
						return placement;																						
				}
		}
}