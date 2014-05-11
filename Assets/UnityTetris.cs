using UnityEngine;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		public class UnityTetris : MonoBehaviour
		{
				public static AssemblyCSharp.SceneManager sceneMgr;
				//private Queue<action> actionQueue = new Queue ();
		
				// Use this for initialization
				void Start ()
				{
						Debug.Log ("Start called!");
						sceneMgr = new AssemblyCSharp.SceneManager ();
				}
		
		
		
				// Update is called once per frame
				int frameCounter = 0;
				void Update ()
				{
			
						//eh, this depends on the frame time through, I will need to switch this to time #TODO
						if (frameCounter == 60) {
								//foreach (var foo in actionQueue) {
								//check to make sure we still have time this frame?
								//do Foo (call menu, update scene object, etc)
				
								//}
				
								sceneMgr.Tick ();
								//s.Rotate90Degrees ();
								frameCounter = 0;
						} else {
								frameCounter++;
						}
				}		
		}
}