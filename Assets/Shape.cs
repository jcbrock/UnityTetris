//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------
using System;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	public class Shape
	{
		public List<Block> blocks;
		public UnityEngine.GameObject compositeGameObject;

		public Shape ()
		{
		}

		public Shape (UnityEngine.GameObject compositeGameObject)
		{
			this.compositeGameObject = compositeGameObject;
			blocks = new List<Block>{new Block(compositeGameObject)};
		}

		public void Rotate90Degrees (bool clockwise)
		{
			UnityEngine.Vector3 rotation;		
			rotation = compositeGameObject.transform.eulerAngles;
			rotation.z = (rotation.z + (90 * (clockwise ? 1 : -1)));
			compositeGameObject.transform.eulerAngles = rotation;


		}


	}
}

