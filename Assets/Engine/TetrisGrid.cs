using System;
using System.Linq;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		//grid concept / collision detection, but no concept of current / preview shapes
		public class TetrisGrid
		{				
				public int PlacedShapeCount { get { return mPlacedShapeCount; } } //Exposing for ? class
				public int RowCount { get { return mRowCount; } } //Exposing for ? class
				public int ColumnCount { get { return mColumnCount; } } //Exposing for ? class
				public bool WasShapeAddedToScene { get { return mWasShapeAddedToScene; } } //Exposing for ? class
				private System.Collections.ArrayList mListOfShapes; //used for updating the UI
				private TetrisBitArray mSceneGrid;
				private int mPlacedShapeCount = 0;								
				private int mColumnCount;
				private int mRowCount;										
				private bool mWasShapeAddedToScene = false;
				private static int mDebugId = 0; //Used to make debug print statements unique					
					
				//todo - figure out a better way
				public int GetHighestRowContainingBlock ()
				{
						int row = -1;
						for (int i = 0; i < mRowCount; ++i) {

								if (mSceneGrid.GetCountOfFilledBlocksInRow (i) != 0) {
										row = i;
										break;
								}
						}
						return row;
				}

				public void HandleTranslateRequest (Shape mCurrentShape, UnityEngine.Vector3 movementVector) //feed in shape into this scene
				{								
						mWasShapeAddedToScene = false;
						if (CheckCollisionWithLeftWall (mCurrentShape, movementVector) ||
								CheckCollisionWithRightWall (mCurrentShape, movementVector))
								return; //no action

						else if (CheckCollisionWithBotWall (mCurrentShape, movementVector) || DoAnyShapesCollideInScene (mCurrentShape, movementVector)) {								
								//m_CurrentShape.PlayCollisionAudio ();								
								mWasShapeAddedToScene = true;
								++mPlacedShapeCount;				
								mListOfShapes.Add (mCurrentShape);											
								AddCurrentShapeToSceneBitGrid (mCurrentShape, true);//??
								mSceneGrid.UpdateRowBytes ();																														
				
								mSceneGrid.PrintBitArray (); //debug print															
						} else
								mCurrentShape.translate (movementVector);						
				}
				public List<int> GetFullRows ()
				{
						return mSceneGrid.GetFullRows ();
				}

				//Delete row in UI and in the grid
				public void DeleteRow (int row)
				{
						DeleteRowInUI (row + 1); //need to -1 because row positions for the shapes are -1 to -25, not 0 to -24, gets converted to negative in func
						mSceneGrid.DeleteRow (row);
				}

				public void HandleRotateRequest (Shape shape)
				{
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
						shape.Rotate ();
						if (CheckCollisionWithAnyWall (shape, movementVector) || DoAnyShapesCollideInScene (shape, movementVector)) {								
								shape.Rotate (true);								
						}
				}
		
				public void Initialize (int rowCount, int columnCount)
				{
						mRowCount = rowCount;	
						mColumnCount = columnCount;						
						mPlacedShapeCount = 0;
						mListOfShapes = new System.Collections.ArrayList ();
						mSceneGrid = new TetrisBitArray (mRowCount, mColumnCount);
				}
		
				public void Finalize2 ()
				{			
						foreach (Shape s in mListOfShapes) {
								s.DeleteShape ();
						}						
						mSceneGrid.ClearGrid ();
						mListOfShapes.Clear ();												
						mPlacedShapeCount = 0;
				}				
		
				private void AddCurrentShapeToSceneBitGrid (Shape shape, bool val)
				{		
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition (); //todo - convert to an array of 4
						Coordinate[] foo = filledGridPositions.ToArray (); //todo - how to only make it 4 elements?
						foreach (Coordinate pos in filledGridPositions) {
								mSceneGrid [pos.row, pos.column] = val;
						}
				}
		
				private void DeleteRowInUI (int row)
				{
						//Note: Actual object destruction is always delayed until after the current Update loop, but will always be done before rendering.
						//https://docs.unity3d.com/Documentation/ScriptReference/Object.Destroy.html
			
						UnityEngine.Debug.Log ("(Row + " + (row - 1) + ") Deleting x = : " + row + " in the UI." + ++mDebugId);
						List<Shape> shapesToRemove = new List<Shape> ();
						foreach (Shape s in mListOfShapes) {								
								if (s.DeleteBlocksInRow (row * -1) == 0) {//TODO, fix all my -1 crap. I have to translate this to acutal game pos, which is negative
										shapesToRemove.Add (s); //can't modify list while I'm iterating through it, mark for delete
								}
						}
			
						foreach (Shape s in shapesToRemove) {
								UnityEngine.Debug.Log (s.Name + " has been completely destroyed.");
								mListOfShapes.Remove (s);
								s.DeleteShape ();
						}
			
						foreach (Shape s2 in mListOfShapes) {										
								s2.ShiftBlocksAboveDeletedRow (row * -1);										
						}
				}

				public List<CellInformation> GetCellInformation (Shape s, UnityEngine.Vector3 movementVector)
				{
						List<CellInformation> neighbors = new List<CellInformation> ();
						List<Coordinate> filledGridPositions = s.GetCurrentGridPosition ();
						foreach (AssemblyCSharp.Coordinate rowCol in filledGridPositions) {
								CellInformation info = new CellInformation ();
								info.coordinate.row = rowCol.row + (int)movementVector.y;
								info.coordinate.column = rowCol.column + (int)movementVector.x;
								if (info.coordinate.row > 0 || info.coordinate.row <= -24 || info.coordinate.column < 0 || info.coordinate.column > 7)
										return null;
				
								int right = info.coordinate.column + 1;
								int left = info.coordinate.column - 1;
								int up = info.coordinate.row + 1;
								int down = info.coordinate.row - 1;								
								if (up <= 0 && mSceneGrid [up, info.coordinate.column] == true) {
										info.TopNeighborStatus = TetrisGridCellStatus.Filled;
								}
				
								if (right < 8 && mSceneGrid [info.coordinate.row, right] == true) {
										info.RightNeighborStatus = TetrisGridCellStatus.Filled;
								}
								if (right == 8) //boost a little bit on the sides of scores don't clump in the middle
										info.RightNeighborStatus = TetrisGridCellStatus.Wall;
								if (left == -1)
										info.LeftNeighborStatus = TetrisGridCellStatus.Wall;
								if (down > -24 && mSceneGrid [down, info.coordinate.column] == true) {
										info.BotNeighborStatus = TetrisGridCellStatus.Filled;
								}										
								if (down > -24 && mSceneGrid [down, info.coordinate.column] == false && filledGridPositions.Count (x => x.row == (rowCol.row - 1) && x.column == rowCol.column) == 0) {												
										info.BotNeighborStatus = TetrisGridCellStatus.Open;
								}
								if (down == -24) {
										info.BotNeighborStatus = TetrisGridCellStatus.Wall;
								}
								if (left >= 0 && mSceneGrid [info.coordinate.row, left] == true) {
										info.LeftNeighborStatus = TetrisGridCellStatus.Filled;
								}
																													
								neighbors.Add (info);
						}
						return neighbors;
				}

				//	private int GetBlockNeightborCount ()
				//{
				//
				//	}


				//These collision detection functions are made public so they can be accessed by the AI
				public bool DoAnyShapesCollideInScene (Shape shape, UnityEngine.Vector3 movementVector)
				{
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition ();
						foreach (Coordinate pos in filledGridPositions) {
								if (mSceneGrid [pos.row + (int)movementVector.y, pos.column + (int)movementVector.x] == true)
										return true;
						}
						return false;
				}					
				public bool CheckCollisionWithLeftWall (Shape shape, UnityEngine.Vector3 movementVector)
				{
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition ();
						foreach (Coordinate pos in filledGridPositions) {																											
								if ((pos.column + movementVector.x) < 0) {
										return true;
								}
						}
						return false;
				}
				public bool CheckCollisionWithRightWall (Shape shape, UnityEngine.Vector3 movementVector)
				{
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition ();
						foreach (Coordinate pos in filledGridPositions) {
								if (pos.column + movementVector.x >= mColumnCount) {
										return true;
								}
						}
						return false;
				}
				public bool CheckCollisionWithBotWall (Shape shape, UnityEngine.Vector3 movementVector)
				{
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition ();
						foreach (Coordinate pos in filledGridPositions) {
								if (Math.Abs (pos.row + movementVector.y) >= mRowCount)
										return true;
						}
						return false;
				}
				public bool CheckCollisionWithTopWall (Shape shape, UnityEngine.Vector3 movementVector)
				{
						List<Coordinate> filledGridPositions = shape.GetCurrentGridPosition ();
						foreach (Coordinate pos in filledGridPositions) {
								if (pos.row + movementVector.y > 0) {
										return true;
								}
						}
						return false;
				}						
				public bool CheckCollisionWithAnyWall (Shape shape, UnityEngine.Vector3 movementVector)
				{
						if (CheckCollisionWithLeftWall (shape, movementVector) ||
								CheckCollisionWithRightWall (shape, movementVector) ||
								CheckCollisionWithBotWall (shape, movementVector)) {
								return true;	
			
						}
						return false;
				}			
		}
}

