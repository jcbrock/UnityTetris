//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using UnityEngine;
using System.Collections;
namespace AssemblyCSharp
{
	public class PlayerControl : MonoBehaviour
	{		
		void Update ()
		{
			UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
			if (Input.GetKeyDown (KeyCode.LeftArrow)) {
				movementVector.x = -1.0f;
			} else if (Input.GetKeyDown (KeyCode.RightArrow)) { 
				movementVector.x = 1.0f;
			} else if (Input.GetKeyDown (KeyCode.DownArrow)) { 
				movementVector.y = -1.0f;
			}
			if (movementVector.x != 0 || movementVector.y != 0) {
				if (!NewBehaviourScript.sceneMgr.Collides (NewBehaviourScript.sceneMgr.CurrentShape, movementVector)) {
					transform.Translate (movementVector, UnityEngine.Space.World);							
				}
			}

			if (Input.GetKeyDown (KeyCode.UpArrow)) {				
				NewBehaviourScript.sceneMgr.CurrentShape.Rotate ();
			}
		}
	}
}


