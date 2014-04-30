using UnityEngine;
using System.Collections;
namespace AssemblyCSharp
{
		public class NewBehaviourScript : MonoBehaviour
		{
				public static AssemblyCSharp.SceneManager sceneMgr;

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
								sceneMgr.Tick ();
								//s.Rotate90Degrees ();
								frameCounter = 0;
						} else {
								frameCounter++;
						}
				}		
		}
}
