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
		public class AIMoveEvaluation
		{
				public int Row;
				public int Column;
				public int NumberOfRotations;
				public float Score;
				public bool PathClear;
				public PlacementStatus Status;
				public string DebugText;				

				public string Print ()
				{
						string stat = string.Empty;
						switch (Status) {
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
						return "PlacementEval - row: " + Row + " column: " + Column + " score: " + Score + " numberOfRotations: " + NumberOfRotations + " pathClear: " + PathClear + " status: " + stat + " debug: " + DebugText;
				}
		}

		public class AI : MonoBehaviour, ISceneRulesObserver, IInputObserver
		{
				UnityTetris tetris;
				bool AIModeOn = false;
				private static int debugId = 0; //Used to make debug print statements unique
				public void Start ()
				{
						GameObject go = GameObject.Find ("GameObject");
						((PlayerControl)go.GetComponent (typeof(PlayerControl))).RegisterObserver (this);
						tetris = (UnityTetris)go.GetComponent (typeof(UnityTetris));
						tetris.scene.RegisterObserver (this);
				}
	
				private AIMoveEvaluation GetBestMove (SceneRules scene)
				{						
						List<AssemblyCSharp.AIMoveEvaluation> possibleScores = new List<AIMoveEvaluation> ();		
						
						//For each column, compute score if we let the piece fall in this column (for each rotation of the piece too)
						AssemblyCSharp.Coordinate anchor = scene.GetCurrentShape ().GetAnchorCoordinate ();
						int shiftedColumnIndex = anchor.column * -1;								
						for (int j = shiftedColumnIndex; j < (shiftedColumnIndex + scene.mTetrisGrid.GetGridColumnCount()); ++j) {
								for (int i = 0; i < 4; ++i) {
										AIMoveEvaluation score = ComputeScoreForColumn (scene, j);										
										if (score != null) {
												score.NumberOfRotations = i;
												score.Column = j + (shiftedColumnIndex * -1);
												possibleScores.Add (score); //change from being current shape to prediction...															
										}
										scene.GetCurrentShape ().Rotate ();
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

				private	AIMoveEvaluation ComputeScoreForColumn (SceneRules scene, int columnDelta)
				{					
						List<AssemblyCSharp.Coordinate> filledGridPositions = scene.GetCurrentShape ().GetCurrentGridPosition ();						
						//s.ShadeSubBlock (0); //shade anchor block for debugging

						AIMoveEvaluation placement = new AIMoveEvaluation ();

						//Verify this is a column placement, skip computation if not						
						Vector3 movementVector = new Vector3 (columnDelta, 0, 0);
						if (!scene.mTetrisGrid.CheckCollisionWithLeftWall (scene.GetCurrentShape (), movementVector) &&
								!scene.mTetrisGrid.CheckCollisionWithRightWall (scene.GetCurrentShape (), movementVector)) {		
								
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
				private void SetTranslationVectorToRestingPosition (SceneRules scene, ref Vector3 movementVector)
				{
						//Find the resting place for the shape in this column. This loop is safe because eventually it'll hit the bot wall															
						while (true) {								
								movementVector.y -= 1;
								if (scene.mTetrisGrid.CheckCollisionWithBotWall (scene.GetCurrentShape (), movementVector) || scene.mTetrisGrid.DoAnyShapesCollideInScene (scene.GetCurrentShape (), movementVector)) {
										movementVector.y += 1; //move back up to the position before the collision																							
										return;
								}
						}						
				}

				private float GetScore (SceneRules scene, Vector3 movementVector)
				{
						//Get neighbor information from the scene for current shape
						List<NeighborInfo> neighbors = scene.mTetrisGrid.GetBlockNeighborCount (scene.GetCurrentShape (), movementVector);
			
						if (neighbors == null) //invalid shape, make sure it never gets picked
								return float.MinValue;
			
						//Score computed by adding points for neighbors and deducting points if covering up gap / higher blocks
						float score = 0f;
						foreach (NeighborInfo neighbor in neighbors) {
								//Add a point for each shape neighbor
								if (neighbor.TopNeighborStatus == GridInfo.Filled) {
										++score;
								}				
								if (neighbor.RightNeighborStatus == GridInfo.Filled) {
										++score;														
								}
								if (neighbor.BotNeighborStatus == GridInfo.Filled) {
										++score;
								}		
								if (neighbor.LeftNeighborStatus == GridInfo.Filled) {
										++score;
								}		
				
								//Add points for each wall neighbor
								if (neighbor.RightNeighborStatus == GridInfo.Wall) { //boost a little bit on the sides of scores don't clump in the middle
										score += .5f;
								}							
								if (neighbor.LeftNeighborStatus == GridInfo.Wall) {
										score += .5f;
								}
								if (neighbor.BotNeighborStatus == GridInfo.Wall) {
										++score;																																					
								}
				
								//Remove a point if covering up an open hole
								if (neighbor.BotNeighborStatus == GridInfo.Open) {
										--score;
								}
				
								//Remove points for higher rows (give priority to lower rows)
								float fudge = (neighbor.row + 24) * .1f;
								score -= fudge;								
				
						}
						return score;
				}

				private void ComputeAIForCurrentPiece ()
				{
						AIMoveEvaluation bestMove = GetBestMove (tetris.scene);
			
						if (bestMove == null) {
								UnityEngine.Debug.LogWarning ("Couldn't compute BestMove for some reason");
								return;
						}
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, -1, 0);
						for (int rot = 0; rot < bestMove.NumberOfRotations; ++rot) {
								tetris.Rotate ();
						}
			
						AssemblyCSharp.Coordinate anchor = tetris.scene.GetCurrentShape ().GetAnchorCoordinate ();
						if (bestMove.Column < anchor.column) {
								int moveLeftCount = anchor.column - bestMove.Column;								
								UnityEngine.Debug.Log (++debugId + "Start column: " + anchor.column + " Added " + moveLeftCount + " translate lefts");
								for (int i = 0; i < moveLeftCount; ++i) {
										tetris.Translate (new Vector3 (-1, 0, 0));
								}
						}
						if (bestMove.Column > anchor.column) {
								int moveRightCount = bestMove.Column - anchor.column;								
								UnityEngine.Debug.Log (++debugId + "Start column: " + anchor.column + " Added " + moveRightCount + " translate rights");
								for (int i = 0; i < moveRightCount; ++i) {
										tetris.Translate (new Vector3 (1, 0, 0));
								}
						}						
						UnityEngine.Debug.Log (++debugId + "score: " + bestMove.Score + " rowTarget: " + bestMove.Row + " columnTarget: " + bestMove.Column + " rotation: " + bestMove.NumberOfRotations);
				}

				//Handle updates from the Scene and Input
				void ISceneRulesObserver.notify (SceneInfo sceneInfo)
				{					
						if (sceneInfo.StateUpdate == StateUpdate.GeneratedNewShape && AIModeOn) {						
								ComputeAIForCurrentPiece ();
						}
				}
				void AssemblyCSharp.IInputObserver.notify (UnityEngine.KeyCode pressedKey)
				{											
						if (pressedKey == KeyCode.A) {
								
								AIModeOn = !AIModeOn;
								UnityEngine.Debug.Log ("AI mode is turned on: " + AIModeOn.ToString ());
								if (AIModeOn)
										ComputeAIForCurrentPiece ();
						}
				}
		}
}