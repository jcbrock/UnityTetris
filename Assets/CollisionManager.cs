using UnityEngine;
using System.Collections;

public static class CollisionManager
{

	// Use this for initialization
	//	void Start ()
	//{
	//		UnityEngine.Debug.Log ("Start collisionmanager!");				
	//}
	
	// Update is called once per frame
	//	void Update ()
	//	{
	//		
	//	}
	/*
		public void OnCollisionEnter2DChild (Collider2D other)
		{
				UnityEngine.Debug.Log ("OnCollisionEnter2DChild called!");		
		}
		public  void OnCollisionStay2DChild (Collider2D other)
		{
				UnityEngine.Debug.Log ("OnCollisionStay2DChild called!");			
		}
		public void OnCollisionExit2DChild (Collider2D other)
		{
				UnityEngine.Debug.Log ("OnCollisionExit2DChild called!");		
		}

		void OnTriggerExit2D (Collider2D other)
		{
				Debug.Log ("Something has exited this zone.");    
		} 
		void OnTriggerStay2D (Collider2D other)
		{
				Debug.Log ("Something has entered this zone.");    
		} 
	
		void OnCollisionEnter2D (Collision2D collision)
		{
				Debug.Log ("something has hit me");
		} 
	
		void OnTriggerEnter2D (UnityEngine.Collider2D other)
		{
				//Destroy(other.gameObject);
				UnityEngine.Debug.Log ("Collision! Stop object...");				
				//spawn new object
				AssemblyCSharp.NewBehaviourScript.sceneMgr.currentObject = SpawnRandomizedTetrisBlock ();
		}*/

	//not the right place for it..
	/*static UnityEngine.GameObject SpawnRandomizedTetrisBlock ()
		{
				int foo = Random.Range (0, 10); //10 = number of possible shapes
				float xStart = Random.Range (0.0F, 10.0F); //10 = length of tetris board (x)
				int rotation = Random.Range (0, 3); //Rotation possiblities
				UnityEngine.Debug.Log ("Random values! " + foo + " " + xStart + " " + rotation);

				UnityEngine.GameObject currentObject = UnityEngine.GameObject.Find ("CompositeGO");				

				Vector3 temp = new Vector3 (xStart, 0, 0);
				//rotation *= 90;
				//Quaternion newRotation = new Quaternion ();

				//0 = 0,0,0
				//90 = 0,-1,0
				//180 = -1,0,0
				//270 = 0,1,0
				//newRotation = Quaternion.Euler (new Vector3 (0, 0, 270));
			
				return SpawnNewBlock (currentObject, temp, rotation);
		}*/
	/*
		static  UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, Vector3 position, int rotation)
		{
		
				GameObject newObj = (GameObject)Instantiate (objectShape,
		                                             position,
		                                             Quaternion.identity);
				newObj.AddComponent ("CollisionManager");

		
				Vector3 currentRotation;		
				currentRotation = newObj.transform.eulerAngles;
				currentRotation.z = (currentRotation.z + (90 * rotation * -1));
				newObj.transform.eulerAngles = currentRotation;
				return newObj;
		}
*/
	public static bool isColliding (AssemblyCSharp.Block b1, AssemblyCSharp.Block b2)
	{
		return (Mathf.Abs (b1.x () - b2.x ()) * 2 < (b1.width () + b2.width ()) &&
			Mathf.Abs (b1.y () - b2.y ()) * 2 < (b1.height () + b2.height ()));
	}

	public static bool isColliding (AssemblyCSharp.Shape s1, AssemblyCSharp.Shape s2)
	{
		//todo - current brute force, check if every block collides with another block. short circuit if collision
		foreach (AssemblyCSharp.Block b1 in s1.blocks) {
			foreach (AssemblyCSharp.Block b2 in s2.blocks) {
				if (isColliding (b1, b2))
					return true;
			}
		}
		return false;
	}
}
