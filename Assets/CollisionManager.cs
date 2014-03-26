using UnityEngine;
using System.Collections;

public class CollisionManager : MonoBehaviour
{

		// Use this for initialization
		void Start ()
		{
	
		}
	
		// Update is called once per frame
		void Update ()
		{
	
		}

		void OnTriggerEnter2D (UnityEngine.Collider2D other)
		{
				//Destroy(other.gameObject);
				UnityEngine.Debug.Log ("Collision! Stop object..."); 
				AssemblyCSharp.NewBehaviourScript.sceneMgr.currentObject = UnityEngine.GameObject.Find ("CompositeGO");
		}
}
