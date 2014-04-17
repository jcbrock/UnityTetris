/*using UnityEngine;
using System.Collections;

public static class CollisionManager
{
		public static bool isCollidingWithLeftWall (AssemblyCSharp.Shape s)
		{
				var foo = s.GetGameObjectTransform ();
				foreach (UnityEngine.Transform child1 in foo) {
						if (((child1.position.x - (child1.renderer.bounds.size.x / 2.0)) - 1.0) < -.1) { //jitter (why it isnt 0)
								return true;	
						}
				}
				return false;
		}

		public static bool isCollidingWithRightWall (AssemblyCSharp.Shape s)
		{
				var foo = s.GetGameObjectTransform ();
				foreach (UnityEngine.Transform child1 in foo) {
						if (((child1.position.x + (child1.renderer.bounds.size.x / 2.0)) + 1.0) > 10.1) {
								return true;	
						}
				}
				return false;
		}
		public static bool isCollidingWithBotWall (AssemblyCSharp.Shape s)
		{
				var foo = s.GetGameObjectTransform ();
				foreach (UnityEngine.Transform child1 in foo) {
						if (((child1.position.y - (child1.renderer.bounds.size.y / 2.0) - 1.0)) < -25.1) {
								return true;	
						}
				}
				return false;
		}
	
		public static bool isCollidingWithWall (AssemblyCSharp.Shape s)
		{
				var foo = s.GetGameObjectTransform ();
				foreach (UnityEngine.Transform child1 in foo) {
						if (child1.position.x < 0 || child1.position.x > 10 || child1.position.y < -25) {
								return true;	
						}
				}
				return false;
		}

		//public static bool isColliding (AssemblyCSharp.Block b1, AssemblyCSharp.Block b2)
		//{
		//			return (Mathf.Abs (b1.x () - b2.x ()) * 2 < (b1.width () + b2.width ()) &&
//						Mathf.Abs (b1.y () - b2.y ()) * 2 < (b1.height () + b2.height ()));
		//}

		public static bool isColliding (AssemblyCSharp.Shape s1, AssemblyCSharp.Shape s2, float xDelta, float yDelta)
		{			
				//TODO - clean this up... can't assume that Shape's GO is always a composite. Sometimes it is just a plain GO
				var foo = s1.GetGameObjectTransform ();
				var bar = s2.GetGameObjectTransform ();
				//UnityEngine.Debug.Log (bar);
				if (xDelta == -1) {
						//UnityEngine.Debug.Log ("Moving Left...");
				}
				foreach (UnityEngine.Transform child1 in foo) {
						//UnityEngine.Debug.Log ("In outer loop");
						//if (bar.childCount > 0) {
						if (xDelta == -1) {
								//UnityEngine.Debug.Log ("Checking a block for current shape...");
						}
						foreach (UnityEngine.Transform child2 in bar) {
								//	UnityEngine.Debug.Log ("In inner loop");
								//Move -delta AFTER x2
								if (xDelta == -1) {
										//UnityEngine.Debug.Log ("Child1 x,y: " + child1.position.x + ", " + child1.position.y);
										//UnityEngine.Debug.Log ("Child2 x,y: " + child2.position.x + ", " + child2.position.y);
										//UnityEngine.Debug.Log (((Mathf.Abs ((child1.position.x + xDelta) - child2.position.x) * 2)));
								}
								if (((Mathf.Abs ((child1.position.x + xDelta) - child2.position.x) * 2) < ((((UnityEngine.BoxCollider)child1.collider).size.x + ((UnityEngine.BoxCollider)child2.collider).size.x) - .1) &&
										(Mathf.Abs ((child1.position.y + yDelta) - child2.position.y) * 2) < ((((UnityEngine.BoxCollider)child1.collider).size.y + ((UnityEngine.BoxCollider)child2.collider).size.y) - .1))) {
										return true;			
								}
						}
						//} 
						/*else {
								//UnityEngine.Debug.Log ("Just checking parent");
								if (((Mathf.Abs (child1.position.x - bar.position.x) - xDelta) * 2 < ((child1.renderer.bounds.size.x + bar.renderer.bounds.size.x) - .1) &&
										(Mathf.Abs (child1.position.y - 1 - bar.position.y) - yDelta) * 2 < ((child1.renderer.bounds.size.y + bar.renderer.bounds.size.y) - .1))) {
										return true;			
								}
						}*//*

				}
				return false;
		}
}*/
