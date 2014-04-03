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
		private List<UnityEngine.GameObject> listOfPossibleShapes = new List<UnityEngine.GameObject> ();

		public SceneManager ()
		{
			//grid = new int[10, 25];					
			listOfShapes.Add (new Shape (UnityEngine.GameObject.Find ("TestCD2")));
			listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape1"));
			listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape2"));
			listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape3"));
			listOfPossibleShapes.Add (UnityEngine.GameObject.Find ("Shape4"));
		}
			
		public void Tick ()
		{			
			//currentShape.Tick();
			//UnityEngine.Debug.Log (currentBlock.gameObject.transform.position);
			bool collided = false;
			if (AnyCollisions ()) {
				listOfShapes.Add (currentShape); //might need to copy it explictly
				currentShape.disablePlayerControls ();
				currentShape = new Shape (SpawnRandomizedTetrisBlock ());

			} else {
				currentShape.translate (0, -1, 0);
			}
		}

		bool AnyCollisions ()
		{
			foreach (Shape shape in listOfShapes) { //for each object in the scene that is colliable
				if (CollisionManager.isColliding (currentShape, shape))
					return true;
			}

			return false;
		}

		UnityEngine.GameObject SpawnRandomizedTetrisBlock ()
		{
			int randomShape = UnityEngine.Random.Range (0, 4); //4 = number of possible shapes
			float xStart = UnityEngine.Random.Range (0.0F, 10.0F); //10 = length of tetris board (x)
			int rotation = UnityEngine.Random.Range (0, 3); //Rotation possiblities
			UnityEngine.Vector3 temp = new UnityEngine.Vector3 (xStart, 0, 0);

			return SpawnNewBlock (listOfPossibleShapes [randomShape], temp, rotation); //eventually replace with random shape...
		}
		
		UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, UnityEngine.Vector3 position, int rotation)
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

