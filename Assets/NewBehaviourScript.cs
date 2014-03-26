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
						var txtToPrint = "Start called!"; 
						Debug.Log (txtToPrint);
						sceneMgr = new AssemblyCSharp.SceneManager ();
						sceneMgr.currentObject = UnityEngine.GameObject.Find ("tempblock");
				}
	
				// Update is called once per frame
				int frameCounter = 0;
				void Update ()
				{
						//eh, this depends on the frame through, I will need to switch this to time #TODO
						//UnityEngine.Debug.Log ("Current obj: " + sceneMgr.currentObject.name);
						if (frameCounter == 60) {
								sceneMgr.Tick ();
								frameCounter = 0;
						} else {
								frameCounter++;
						}
						//var txtToPrint="Update called!"; 
						//Debug.Log(txtToPrint);
				}
		
		}
}
