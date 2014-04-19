using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
		public class ShapeFactory
		{
				private List<UnityEngine.GameObject> listOfPossibleShapes = new List<UnityEngine.GameObject> ();
				private int debugCounter = 0;
				
				public ShapeFactory ()
				{
						listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape1"));
						listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape2"));
						listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape3"));
						listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape4"));
				}

				public Shape SpawnRandomizedTetrisShape ()
				{
						int randomShape = UnityEngine.Random.Range (0, 4); //4 = number of possible shapes
						float xStart = UnityEngine.Random.Range (1, 10); //10 = length of tetris board (x)
						xStart -= (float)0.5;
						int rotation = UnityEngine.Random.Range (0, 3); //Rotation possiblities
						UnityEngine.Vector3 temp = new UnityEngine.Vector3 (xStart, (float)-0.5, 0);
						RotationStyles rotationStyle = RotationStyles.full360;
						if (randomShape == 1 || randomShape == 3)
								rotationStyle = RotationStyles.flip90;
						else if (randomShape == 2)
								rotationStyle = RotationStyles.none; //square

						Shape newShape = new Shape (SpawnNewBlock (listOfPossibleShapes [randomShape], temp, rotation), rotationStyle); //eventually replace with random shape...;
						for (int i = 0; i < rotation; ++i) {
								//use shape's rotation functions since they do things like making sure not to rotate out of a wall
								newShape.Rotate ();
						}

						return newShape;
				}			

				//TODO - collapse this function back into 1...
				public Shape SpawnRandomizedTetrisShape2 ()
				{
						int randomShape = UnityEngine.Random.Range (0, 4); //4 = number of possible shapes
						float xStart = UnityEngine.Random.Range (1, 10); //10 = length of tetris board (x)
						xStart -= (float)0.5;
						int rotation = UnityEngine.Random.Range (0, 3); //Rotation possiblities
						UnityEngine.Vector3 temp = new UnityEngine.Vector3 (xStart - 10, (float)-0.5, 0);
						RotationStyles rotationStyle = RotationStyles.full360;
						if (randomShape == 1 || randomShape == 3)
								rotationStyle = RotationStyles.flip90;
						else if (randomShape == 2)
								rotationStyle = RotationStyles.none; //square
			
						Shape newShape = new Shape (SpawnNewBlock (listOfPossibleShapes [randomShape], temp, rotation), rotationStyle); //eventually replace with random shape...;
						for (int i = 0; i < rotation; ++i) {
								//use shape's rotation functions since they do things like making sure not to rotate out of a wall
								newShape.Rotate ();
						}
			
						return newShape;
				}			
	
				private UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, UnityEngine.Vector3 position, int rotation)
				{
		
						UnityEngine.GameObject newObj = (UnityEngine.GameObject)UnityEngine.MonoBehaviour.Instantiate (objectShape,
		                                                                                               position,
		                                                                                               UnityEngine.Quaternion.identity);
		

						/*
			UnityEngine.Vector3 currentRotation;		
						currentRotation = newObj.transform.eulerAngles;
						currentRotation.z = (currentRotation.z + (90 * rotation * -1));
						newObj.transform.eulerAngles = currentRotation;
*/
						newObj.name += debugCounter;
						debugCounter++;
						return newObj;
				}
		}
}
