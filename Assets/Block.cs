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
		public class Block
		{
				//public float x;
				//public float y;
				//public float width;
				//public float height;
				public UnityEngine.GameObject gameObject;

				public float x ()
				{
						//((UnityEngine.BoxCollider2D)gameObject.collider).size.x;
						return gameObject.transform.position.x;
				}
				public float y ()
				{
						return gameObject.transform.position.y;
				}
				public float width ()
				{
						//return gameObject.renderer.bounds.size.x;
						return ((UnityEngine.BoxCollider)gameObject.collider).size.x;
				}
				public float height ()
				{
						//return gameObject.renderer.bounds.size.y;
						return ((UnityEngine.BoxCollider)gameObject.collider).size.y;
						//((UnityEngine.BoxCollider2D)gameObject.GetComponent<UnityEngine.BoxCollider2D>).
				}

				public Block ()
				{
					
				}
				public Block (UnityEngine.GameObject gameObj)
				{
						gameObject = gameObj;
						//x = gameObj.transform.position.x; 
						//y = gameObj.transform.position.y;
						//width = gameObj.renderer.bounds.size.x; 
						//height = gameObj.renderer.bounds.size.y;

				}
				public void Tick () //eventually override this in the child class, once I start creating different types of blocks\
				{
						//I need to move down this object, assuming it is the current obj
						//rigidbody2D.transform.position.y -= .1;
					
				}				
		}
}

