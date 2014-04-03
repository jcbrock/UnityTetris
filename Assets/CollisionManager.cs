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

	public static bool isColliding (AssemblyCSharp.Block b1, AssemblyCSharp.Block b2)
	{
		return (Mathf.Abs (b1.x () - b2.x ()) * 2 < (b1.width () + b2.width ()) &&
			Mathf.Abs (b1.y () - b2.y ()) * 2 < (b1.height () + b2.height ()));
	}

	public static bool isColliding (AssemblyCSharp.Shape s1, AssemblyCSharp.Shape s2)
	{
		//TODO - clean this up... can't assume that Shape's GO is always a composite. Sometimes it is just a plain GO
		var foo = s1.GetGameObjectTransform ();
		var bar = s2.GetGameObjectTransform ();
		//UnityEngine.Debug.Log (bar);
		foreach (UnityEngine.Transform child1 in foo) {
			//UnityEngine.Debug.Log ("In outer loop");
			if (bar.childCount > 0) {
				foreach (UnityEngine.Transform child2 in bar) {
					//	UnityEngine.Debug.Log ("In inner loop");
					if ((Mathf.Abs (child1.position.x - child2.position.x) * 2 < (((UnityEngine.BoxCollider)child1.collider).size.x + ((UnityEngine.BoxCollider)child2.collider).size.x) &&
						Mathf.Abs (child1.position.y - child2.position.y) * 2 < (((UnityEngine.BoxCollider)child1.collider).size.y + ((UnityEngine.BoxCollider)child2.collider).size.y))) {
						return true;			
					}
				}
			} else {
				//UnityEngine.Debug.Log ("Just checking parent");
				if ((Mathf.Abs (child1.position.x - bar.position.x) * 2 < (child1.renderer.bounds.size.x + bar.renderer.bounds.size.x) &&
					Mathf.Abs (child1.position.y - bar.position.y) * 2 < (child1.renderer.bounds.size.y + bar.renderer.bounds.size.y))) {
					return true;			
				}
			}

		}
		return false;
	}
}
