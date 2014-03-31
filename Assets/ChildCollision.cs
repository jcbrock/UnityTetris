using UnityEngine;
using System.Collections;

public class ChildCollision : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void OnCollisionEnter2D (Collider2D other)
		{
		UnityEngine.Debug.Log ("OnCollisionEnter2D called!");
		CollisionManager.on
		}
		void OnCollisionStay2D (Collider2D other)
		{
		UnityEngine.Debug.Log ("OnCollisionStay2D called!");			
		}
		void OnCollisionExit2D (Collider2D other)
		{
		UnityEngine.Debug.Log ("OnCollisionExit2D called!");		
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
				UnityEngine.Debug.Log ("OnTriggerEnter2D! Stop object...");				
				//spawn new object

		}
}
