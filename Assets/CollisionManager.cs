using UnityEngine;
using System.Collections;

public class CollisionManager : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
				UnityEngine.Debug.Log ("Start collisionmanager!");				
		}
	
		// Update is called once per frame
		void Update ()
		{
			
		}

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
		}

		//not the right place for it..
		UnityEngine.GameObject SpawnRandomizedTetrisBlock ()
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
		}

		UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, Vector3 position, int rotation)
		{
		
				GameObject newObj = (GameObject)Instantiate (objectShape,
		                                             position,
		                                             new Quaternion ());
				newObj.AddComponent ("CollisionManager");

		
				Vector3 currentRotation;		
				currentRotation = newObj.transform.eulerAngles;
				currentRotation.z = (currentRotation.z + (90 * rotation * -1));
				newObj.transform.eulerAngles = currentRotation;
				return newObj;
		}
}
