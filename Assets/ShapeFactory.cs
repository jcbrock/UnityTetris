using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
		public class ShapeFactory
		{
				private List<UnityEngine.GameObject> listOfPossibleShapes = new List<UnityEngine.GameObject> ();
				
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
						UnityEngine.Vector3 temp = new UnityEngine.Vector3 (xStart, (float)-.5, 0);
						RotationStyles rotationStyle = RotationStyles.full360;
						if (randomShape == 1 || randomShape == 3)
								rotationStyle = RotationStyles.flip90;
						else if (randomShape == 2)
								rotationStyle = RotationStyles.none; //square

						return new Shape (SpawnNewBlock (listOfPossibleShapes [randomShape], temp, rotation), rotationStyle); //eventually replace with random shape...
				}
	
				private UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, UnityEngine.Vector3 position, int rotation)
				{
		
						UnityEngine.GameObject newObj = (UnityEngine.GameObject)UnityEngine.MonoBehaviour.Instantiate (objectShape,
		                                                                                               position,
		                                                                                               UnityEngine.Quaternion.identity);
		
						UnityEngine.Vector3 currentRotation;		
						currentRotation = newObj.transform.eulerAngles;
						currentRotation.z = (currentRotation.z + (90 * rotation * -1));
						newObj.transform.eulerAngles = currentRotation;
						return newObj;
				}
		}
}
