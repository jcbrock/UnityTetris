using UnityEngine;
using System.Collections;

public class Example : MonoBehaviour
{
		void Start ()
		{
				//Debug.Log ("Trigger: " + collider.isTrigger);
		}
		void OnTriggerEnter2D (Collider2D other)
		{
				//Destroy(other.gameObject);
				Debug.Log ("Something has entered this zone.");    
		}
		void OnTriggerExit2D (Collider2D other)
		{
				Debug.Log ("Something has exited this zone.");    
		} 
		void OnTriggerStay2D (Collider2D other)
		{
				//		Debug.Log ("Something has entered this zone.");    
		} 
	
		void OnCollisionEnter2D (Collision2D collision)
		{
				Debug.Log ("something has hit me");
		} 
}