using System;
using System.Collections.Generic;
namespace AssemblyCSharp
{		
		public class ClassicTetrisRules
		{		
				//For these member variables surfaced outside of class, only expose as read-only																
				public Shape CurrentShape { get { return mCurrentShape; } } //Exposing for AI class
				public TetrisGrid TetrisGrid { get { return mTetrisGrid; } } //Exposing for AI class
				private Shape mCurrentShape;
				private Shape mPreviewShape;				
				private TetrisGrid mTetrisGrid = new TetrisGrid ();
				private ShapeFactory mFactory = new ShapeFactory ();				
				private static int mDebugId = 0; //Used to make debug print statements unique					
				private int mRulesetOption = -1;
				private List<AssemblyCSharp.IClassicTetrisStateObserver> mRegisteredObservers = new List<AssemblyCSharp.IClassicTetrisStateObserver> ();

				public int GetCurrentScore ()
				{
						return mTetrisGrid.PlacedShapeCount;
				}

				public void HandleTranslateRequest (UnityEngine.Vector3 movementVector)
				{		
						if (mCurrentShape == null)
								return;

						//Move current shape
						mTetrisGrid.HandleTranslateRequest (mCurrentShape, movementVector);

						//Check for end game condition
						if (mTetrisGrid.GetRowBlockCount (0) > 0) {																
								NotifyObservers (ClassicTetrisStateUpdate.GameEnded);
								return;
						}

						//Check if a shape was placed. If so, check for full rows, spawn new shape
						if (mTetrisGrid.WasShapeAddedToScene) {
								//Delete full rows
								foreach (int row in mTetrisGrid.GetFullRows ()) {
										UnityEngine.Debug.Log ("Row " + row + " is full. Deleting now..." + ++mDebugId);
										NotifyObservers (ClassicTetrisStateUpdate.RowDeleted);
										mTetrisGrid.DeleteRow (row);
								}											

								mCurrentShape = mPreviewShape;																
								mCurrentShape.TranslateToInitialPosition ();
								mPreviewShape = mFactory.SpawnRandomizedTetrisShape (mRulesetOption);	
								NotifyObservers (ClassicTetrisStateUpdate.GeneratedNewShape);
						}

				}
				public void HandleRotateRequest ()
				{
						mTetrisGrid.HandleRotateRequest (mCurrentShape);
				}

				public void Initialize (int rowCount, int columnCount, int rulesetOption)
				{							
						mTetrisGrid.Initialize (rowCount, columnCount);
						mRulesetOption = rulesetOption;
						mCurrentShape = mFactory.SpawnRandomizedTetrisShape (mRulesetOption);
						mCurrentShape.TranslateToInitialPosition ();										
						mPreviewShape = mFactory.SpawnRandomizedTetrisShape (mRulesetOption);																				
				}

				public void Cleanup ()
				{								
						if (mCurrentShape != null)
								mCurrentShape.DeleteShape ();
						if (mPreviewShape != null)
								mPreviewShape.DeleteShape ();													
						mCurrentShape = null;
						mPreviewShape = null;
						mTetrisGrid.Cleanup ();
				}

				public void RegisterObserver (AssemblyCSharp.IClassicTetrisStateObserver observer)
				{
						if (!mRegisteredObservers.Contains (observer))
								mRegisteredObservers.Add (observer);
				}
				public void UnregisterObserver (AssemblyCSharp.IClassicTetrisStateObserver observer)
				{
						mRegisteredObservers.Remove (observer);
				}
				private void NotifyObservers (AssemblyCSharp.ClassicTetrisStateUpdate stateUpdate)
				{
						foreach (AssemblyCSharp.IClassicTetrisStateObserver observer in mRegisteredObservers) {
								observer.notify (stateUpdate);
						}		
				}		
		}
}