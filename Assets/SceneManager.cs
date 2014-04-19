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
namespace AssemblyCSharp
{
		public class SceneManager// : UnityEngine.MonoBehaviour
		{
				public Shape currentShape;
				private System.Collections.ArrayList listOfShapes = new System.Collections.ArrayList ();
				public int placedBlockCount = 0;
				private ShapeFactory factory;
				private bool isGamePaused = false;

				public SceneManager ()
				{
						//grid = new int[10, 25];					
						//listOfShapes.Add (new Shape (UnityEngine.GameObject.Find ("TestCD2")));
						factory = new ShapeFactory ();
				}

				public void StartNewGame ()
				{												
						currentShape = factory.SpawnRandomizedTetrisShape ();
				}

				public void PauseGame ()
				{
						isGamePaused = true;
				}

				public void ResumeGame ()
				{
						isGamePaused = false;
				}

				public void EndGame ()
				{
						//todo - write score somewhere for the leaderboard						
						foreach (Shape s in listOfShapes) {
								s.DeleteShape ();
						}
						currentShape.DeleteShape ();
						listOfShapes.Clear ();						
						currentShape = null;
						placedBlockCount = 0;
				}

				public void Tick ()
				{			
						if (isGamePaused || currentShape == null)
								return;
						//currentShape.Tick();
						//UnityEngine.Debug.Log (currentBlock.gameObject.transform.position);
						//bool collided = false;

						if (AnyCollisions (0, -1)) {
								++placedBlockCount;
								listOfShapes.Add (currentShape); //might need to copy it explictly
								currentShape.disablePlayerControls ();

								//check to see if a row can be deleted
								//I'm going to do it a horrible way first - check every row, see if any of them add up to 10 (my width) (if I find a row of 0, short-circuit)
								//Once I find a row of 10, go back through and delete blocks that are in 10
								//I'll need to figure out how to shift down, but I can do that later...

								//Destroy (AssemblyCSharp.NewBehaviourScript.sceneMgr.currentShape.compositeGameObject.transform.FindChild ("mid").gameObject);
								Dictionary<int, int> rowCounts = new Dictionary<int, int> ();
								foreach (Shape s in listOfShapes) {
				
										//UnityEngine.Debug.Log ("Block count for shape: " + s.BlockCount ());

										foreach (int row in s.GetRowValuesOfSubBlocks()) {
												if (rowCounts.ContainsKey (row))
														rowCounts [row]++;
												else
														rowCounts.Add (row, 1);
										}
								}
								foreach (int row in rowCounts.Keys) {
										//UnityEngine.Debug.Log ("Row: " + row.ToString () + " Count: " + rowCounts [row].ToString ());
										if (rowCounts [row] == 10) {
												//row is full, delete any block in that row
												foreach (Shape s in listOfShapes) {
														s.DeleteBlocksInRow (row);
														
												}
												List<Shape> shapesToRemove = new List<Shape> ();
												foreach (Shape s3 in listOfShapes) {
														if (s3.BlockCount () == 0) {
																shapesToRemove.Add (s3);
																
														}
												}
												foreach (Shape s in shapesToRemove) {
														listOfShapes.Remove (s);
														UnityEngine.Debug.Log (s.Name + " has been completely destroyed.");
												}
												List<Shape> debugList = new List<Shape> ();
												foreach (Shape s2 in listOfShapes) {
							
														//if (s2.ContainsBlockAboveDeletedRow (row)) {
														if (s2.ShiftBlocksAboveDeletedRow (row))
																debugList.Add (s2);
														//s2.translate (0, -1, 0);
														//}
												}
												//TODO - after shifting, I should theoretically test for a complete row again...
												foreach (Shape s in debugList) {
														UnityEngine.Debug.Log (s.Name + " was shifted down");
												}
												string shapeList = "List of shapes(" + listOfShapes.Count + "): " + Environment.NewLine;
												foreach (Shape s in listOfShapes) {
														shapeList += s.Name + Environment.NewLine;
												}
												UnityEngine.Debug.Log (shapeList);
										} else if (rowCounts [row] > 10) {
												//throw assert or exception
												UnityEngine.Debug.LogWarning ("ROW " + row + " HAS MORE THAN 10, WTF");

										}
								}
								
								



								currentShape = factory.SpawnRandomizedTetrisShape ();

						} else {
								currentShape.translate (0, -1, 0);
						}
				}

				//todo - cant just be on tick, gotta be on control movement too...
				public bool AnyCollisions (float xDelta, float yDelta)
				{
						if (currentShape.isCollidingWithBotWall ())
								return true;

						foreach (Shape shape in listOfShapes) { //for each object in the scene that is colliable
								if (currentShape.collides (shape, xDelta, yDelta))
										return true;
						}

						return false;
				}
		}
}

