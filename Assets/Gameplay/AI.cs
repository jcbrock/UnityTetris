using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;
using System.Text;
using System.IO;

namespace AssemblyCSharp
{		
		public class AI : MonoBehaviour, IClassicTetrisStateObserver, IInputObserver
		{
				private UnityTetris mUnityTetris;
				private bool mAIModeOn = false;
				private static int mDebugId = 0; //Used to make debug print statements unique

				public void Start ()
				{
						GameObject go = GameObject.Find ("GameObject");
						((PlayerControl)go.GetComponent (typeof(PlayerControl))).RegisterObserver (this);
						mUnityTetris = (UnityTetris)go.GetComponent (typeof(UnityTetris));
						mUnityTetris.Scene.RegisterObserver (this);
				}

				//Handle updates from the Scene and Input
				void IClassicTetrisStateObserver.notify (ClassicTetrisStateUpdate stateUpdate)
				{					
						if (stateUpdate == ClassicTetrisStateUpdate.GeneratedNewShape && mAIModeOn) {						
								ComputeAIForCurrentPiece ();
						}
				}
				void AssemblyCSharp.IInputObserver.notify (UnityEngine.KeyCode pressedKey)
				{											
						if (pressedKey == KeyCode.A) {							
								mAIModeOn = !mAIModeOn;
								UnityEngine.Debug.Log ("AI mode is turned on: " + mAIModeOn.ToString ());
								if (mAIModeOn)
										ComputeAIForCurrentPiece ();
						}
				}

				//Computes the best move for the current shape and sends those translation requests to the scene
				private void ComputeAIForCurrentPiece ()
				{
						AIMoveEvaluation bestMove = GetBestMove (mUnityTetris.Scene);
			
						if (bestMove == null) {
								UnityEngine.Debug.LogWarning ("Couldn't compute BestMove for some reason");
								return;
						}
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, -1, 0);
						for (int rot = 0; rot < bestMove.NumberOfRotations; ++rot) {
								mUnityTetris.Rotate ();
						}
			
						AssemblyCSharp.Coordinate anchor = mUnityTetris.Scene.CurrentShape .GetAnchorCoordinate ();
						if (bestMove.Column < anchor.column) {
								int moveLeftCount = anchor.column - bestMove.Column;								
								UnityEngine.Debug.Log (++mDebugId + "Start column: " + anchor.column + " Added " + moveLeftCount + " translate lefts");
								for (int i = 0; i < moveLeftCount; ++i) {
										mUnityTetris.Translate (new Vector3 (-1, 0, 0));
								}
						}
						if (bestMove.Column > anchor.column) {
								int moveRightCount = bestMove.Column - anchor.column;								
								UnityEngine.Debug.Log (++mDebugId + "Start column: " + anchor.column + " Added " + moveRightCount + " translate rights");
								for (int i = 0; i < moveRightCount; ++i) {
										mUnityTetris.Translate (new Vector3 (1, 0, 0));
								}
						}						
						UnityEngine.Debug.Log (++mDebugId + "score: " + bestMove.Score + " rowTarget: " + bestMove.Row + " columnTarget: " + bestMove.Column + " rotation: " + bestMove.NumberOfRotations);
				}
	
				private AIMoveEvaluation GetBestMove (ClassicTetrisRules scene)
				{						
						List<AssemblyCSharp.AIMoveEvaluation> possibleScores = new List<AIMoveEvaluation> ();		
						
						//For each column, compute score if we let the piece fall in this column (for each rotation of the piece too)
						AssemblyCSharp.Coordinate anchor = scene.CurrentShape.GetAnchorCoordinate ();
						int shiftedColumnIndex = anchor.column * -1;								
						for (int j = shiftedColumnIndex; j < (shiftedColumnIndex + scene.TetrisGrid.ColumnCount); ++j) {
								for (int i = 0; i < 4; ++i) {
										AIMoveEvaluation score = ComputeScoreForColumn (scene, j);										
										if (score != null) {
												score.NumberOfRotations = i;
												score.Column = j + (shiftedColumnIndex * -1);
												possibleScores.Add (score); //change from being current shape to prediction...															
										}
										scene.CurrentShape .Rotate ();
								}
						}						
			
						foreach (AIMoveEvaluation colScore in possibleScores)
								UnityEngine.Debug.Log (colScore.Print ());
			
						//Calculate the best score
						List<AIMoveEvaluation> bestMoves = possibleScores.Where (x => x.Status == PlacementStatus.NoCollision && x.PathClear == true).OrderByDescending (x => x.Score).ToList ();
						AIMoveEvaluation bestMove = null;
						if (bestMoves.Count > 0)
								bestMove = bestMoves.Where (x => x.Score == bestMoves [0].Score).OrderBy (x => x.Row).FirstOrDefault ();								
			
						return bestMove;
				}													

				private	AIMoveEvaluation ComputeScoreForColumn (ClassicTetrisRules scene, int columnDelta)
				{					
						List<AssemblyCSharp.Coordinate> filledGridPositions = scene.CurrentShape .GetCurrentGridPosition ();						
						//s.ShadeSubBlock (0); //shade anchor block for debugging

						AIMoveEvaluation placement = new AIMoveEvaluation ();

						//Verify this is a column placement, skip computation if not						
						Vector3 movementVector = new Vector3 (columnDelta, 0, 0);
						if (!scene.TetrisGrid.CheckCollisionWithLeftWall (scene.CurrentShape, movementVector) &&
								!scene.TetrisGrid.CheckCollisionWithRightWall (scene.CurrentShape, movementVector)) {		
								
								//Find the resting place for the shape in this column												
								SetTranslationVectorToRestingPosition (scene, ref movementVector);

								//Compute score of resting spot																	
								placement.Score = GetScore (scene, movementVector);					
								placement.PathClear = true; //since we're just translating straight down until it hits something, this is true																						
								placement.Status = PlacementStatus.NoCollision;
						} else
								placement.Status = PlacementStatus.CollidedWithWall;
														
						return placement;																						
				}

				//Sets movementVector argument to the resting position
				private void SetTranslationVectorToRestingPosition (ClassicTetrisRules scene, ref Vector3 movementVector)
				{
						//Find the resting place for the shape in this column. This loop is safe because eventually it'll hit the bot wall															
						while (true) {								
								movementVector.y -= 1;
								if (scene.TetrisGrid.CheckCollisionWithBotWall (scene.CurrentShape, movementVector) || 
										scene.TetrisGrid.DoAnyShapesCollideInScene (scene.CurrentShape, movementVector)) {
										movementVector.y += 1; //move back up to the position before the collision																							
										return;
								}
						}						
				}

				private float GetScore (ClassicTetrisRules scene, Vector3 movementVector)
				{
						//Get neighbor information from the scene for current shape
						List<CellInformation> cellInformation = scene.TetrisGrid.GetCellInformation (scene.CurrentShape, movementVector);
			
						if (cellInformation == null) //invalid shape, make sure it never gets picked
								return float.MinValue;
			
						//Score computed by adding points for neighbors and deducting points if covering up gap / higher blocks
						float score = 0f;
						foreach (CellInformation neighbor in cellInformation) {
								//Add a point for each shape neighbor
								if (neighbor.TopNeighborStatus == TetrisGridCellStatus.Filled) {
										++score;
								}				
								if (neighbor.RightNeighborStatus == TetrisGridCellStatus.Filled) {
										++score;														
								}
								if (neighbor.BotNeighborStatus == TetrisGridCellStatus.Filled) {
										++score;
								}		
								if (neighbor.LeftNeighborStatus == TetrisGridCellStatus.Filled) {
										++score;
								}		
				
								//Add points for each wall neighbor
								if (neighbor.RightNeighborStatus == TetrisGridCellStatus.Wall) { //boost a little bit on the sides of scores don't clump in the middle
										score += .5f;
								}							
								if (neighbor.LeftNeighborStatus == TetrisGridCellStatus.Wall) {
										score += .5f;
								}
								if (neighbor.BotNeighborStatus == TetrisGridCellStatus.Wall) {
										++score;																																					
								}
				
								//Remove a point if covering up an open hole
								if (neighbor.BotNeighborStatus == TetrisGridCellStatus.Open) {
										--score;
								}
				
								//Remove points for higher rows (give priority to lower rows)
								float fudge = (neighbor.coordinate.row + 24) * .1f;
								score -= fudge;								
				
						}
						return score;
				}					
		}
}