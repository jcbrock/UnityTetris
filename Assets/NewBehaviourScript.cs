using UnityEngine;
using System.Collections;

public class NewBehaviourScript : MonoBehaviour
{
		AssemblyCSharp.SceneManager sceneMgr;

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
