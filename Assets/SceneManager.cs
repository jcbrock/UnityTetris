//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1022
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated. 
// </auto-generated>
//------------------------------------------------------------------------------
using System;
namespace AssemblyCSharp
{
		public class SceneManager// : UnityEngine.MonoBehaviour
		{
				Block currentObj;
				int[,] grid;
				//UnityEngine.Transform currentObject = UnityEngine.GameObject.Find("tempblock");
				public UnityEngine.GameObject currentObject;// = UnityEngine.GameObject.Find ("tempblock");
				
				public SceneManager ()
				{
						grid = new int[10, 25];					
						currentObj = new Block ();

				}
			
				public void Tick ()
				{
						//UnityEngine.Debug.Log ("Tick!");
						UnityEngine.Debug.Log (currentObject.transform.position);
						currentObj.Tick ();


						currentObject.transform.Translate (0, (float)-1, 0);
						//currentObject.rigidbody2D.transform.position.y -= .1;
						//currentObject.position.y -= (float).1;
						//rigidbody2D.transform.position.y -= .1;
				}
		}
}

