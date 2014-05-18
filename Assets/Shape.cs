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
		//Different shapes have different rotation styles
		public enum RotationStyles
		{
				none,
				flip90,
				full360
	}
		;

		public class Shape
		{	
				public string Name; //for debugging

				private UnityEngine.GameObject m_CompositeGameObject;
				private RotationStyles m_RotationStyle;
				private bool m_FlipRotation = true;
				private float m_InitialXPos = 0;
				private int m_BlockCount = 4; //assuming 4 blocks per tetris block

				//Hide default constructor
				private Shape ()
				{
				}

				public Shape (UnityEngine.GameObject compositeGameObject, RotationStyles rotationStyle, float initialXPos)
				{
						if (compositeGameObject == null)
								throw new ArgumentNullException ("A shape MUST contain a game object!");

						Name = compositeGameObject.name;
						this.m_CompositeGameObject = compositeGameObject;
						this.m_InitialXPos = initialXPos;
						m_RotationStyle = rotationStyle;
				}

				//debug func
				public void ShadeSubBlock (int blockIndex)
				{
						if (m_CompositeGameObject.transform.childCount > blockIndex) {
								m_CompositeGameObject.transform.GetChild (blockIndex).GetComponent<UnityEngine.SpriteRenderer> ().color = UnityEngine.Color.magenta;
						}		
				}
		
		
				//cleans up UI of shape
				public void DeleteShape ()
				{
						UnityEngine.GameObject.Destroy (m_CompositeGameObject);
				}

				//returns block count remaining after deletion
				public int DeleteBlocksInRow (int row)
				{
						for (int i = 0; i < m_CompositeGameObject.transform.childCount; ++i) {
								if (Convert.ToInt32 (Math.Floor (m_CompositeGameObject.transform.GetChild (i).transform.position.y)) == row) {
										UnityEngine.GameObject.Destroy (m_CompositeGameObject.transform.GetChild (i).gameObject);
										--m_BlockCount;
								}
						}
						return m_BlockCount;
				}

				public void TranslateToInitialPlacement ()
				{
						translate (m_InitialXPos + 5, 0, 0); //TODO - replace 5 with preview translate variable
				}

				public bool ShiftBlocksAboveDeletedRow (int row)
				{
						bool shiftedSomethingDown = false;
						for (int i = 0; i < m_CompositeGameObject.transform.childCount; ++i) {
								if (Convert.ToInt32 (Math.Floor (m_CompositeGameObject.transform.GetChild (i).transform.position.y)) > row) {
										m_CompositeGameObject.transform.GetChild (i).transform.Translate (new UnityEngine.Vector3 (0, -1, 0), UnityEngine.Space.World);
										shiftedSomethingDown = true;
								}
						}
						if (shiftedSomethingDown)
								return true;

						return false;
				}
		
				public void Rotate ()
				{
						//TODO - collision detection within Rotate is inconsisant with PlayerControl
						UnityEngine.Vector3 rotation;	
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
						switch (m_RotationStyle) {
						case RotationStyles.none:
								break;
						case RotationStyles.flip90:
								rotation = m_CompositeGameObject.transform.eulerAngles;
								rotation.z = (rotation.z + (90 * (m_FlipRotation ? 1 : -1)));
								m_CompositeGameObject.transform.eulerAngles = rotation;
								m_FlipRotation = !m_FlipRotation;
				
								if (this.CheckCollisionWithAnyWall (movementVector) || UnityTetris.sceneMgr.DoAnyShapesCollideInScene (movementVector)) {
										//flip back
										rotation = m_CompositeGameObject.transform.eulerAngles;
										rotation.z = (rotation.z + (90 * (m_FlipRotation ? 1 : -1)));
										m_CompositeGameObject.transform.eulerAngles = rotation;
										m_FlipRotation = !m_FlipRotation;
								}
								break;
						case RotationStyles.full360:
								rotation = m_CompositeGameObject.transform.eulerAngles;
								rotation.z = (rotation.z + (90 * (true ? 1 : -1)));
								m_CompositeGameObject.transform.eulerAngles = rotation;
								if (this.CheckCollisionWithAnyWall (movementVector) || UnityTetris.sceneMgr.DoAnyShapesCollideInScene (movementVector)) {
										//flip back
										rotation = m_CompositeGameObject.transform.eulerAngles;
										rotation.z = (rotation.z + (90 * (false ? 1 : -1)));
										m_CompositeGameObject.transform.eulerAngles = rotation;
								}
								break;
						}
			
				}
				public void PlayCollisionAudio ()
				{
						m_CompositeGameObject.audio.Play ();
				}

				public void translate (float x, float y, float z)
				{
						translateInWorldSpace (new UnityEngine.Vector3 (x, y, z));								
				}
				public void translate (UnityEngine.Vector3 vec)
				{
						translateInWorldSpace (vec);
				}
				private void translateInWorldSpace (UnityEngine.Vector3 vec)
				{
						m_CompositeGameObject.transform.Translate (vec, UnityEngine.Space.World);
				}				
				
				public List<int> GetRowValuesOfSubBlocks ()
				{
						List<int> rows = new List<int> ();
						for (int i = 0; i < m_CompositeGameObject.transform.childCount; ++i) {

								double rowValue = m_CompositeGameObject.transform.GetChild (i).transform.position.y;
								//UnityEngine.Debug.Log ("Block row value: " + rowValue.ToString ());
								rows.Add (Convert.ToInt32 (Math.Floor (rowValue)));
						}
						return rows;
				}
				public List<KeyValuePair<int, int>> GetFilledGridValues ()
				{
						List<KeyValuePair<int, int>> rows = new List<KeyValuePair<int, int>> ();
						for (int i = 0; i < m_CompositeGameObject.transform.childCount; ++i) {
				
								
								double rowValue = m_CompositeGameObject.transform.GetChild (i).transform.position.y;
								double colValue = m_CompositeGameObject.transform.GetChild (i).transform.position.x;
								//UnityEngine.Debug.Log ("Block row value: " + rowValue.ToString ());
								//rows.Add (Convert.ToInt32 (Math.Floor (rowValue)));
								//rows -> ceiling cuz they are negative, cols -> floor cuz they are positive
								rows.Add (new KeyValuePair<int, int> (Convert.ToInt32 (Math.Ceiling (rowValue)), Convert.ToInt32 (Math.Floor (colValue))));
						}
						return rows;
				}
						
				public bool collides (Shape shape, UnityEngine.Vector3 movementVector)
				{						
						var shape1 = this.m_CompositeGameObject.transform;
						var shape2 = shape.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform block1 in shape1) {								
								foreach (UnityEngine.Transform block2 in shape2) {
										if (((UnityEngine.Mathf.Abs ((block1.position.x + movementVector.x) - block2.position.x) * 2) < ((((UnityEngine.BoxCollider)block1.collider).size.x + ((UnityEngine.BoxCollider)block2.collider).size.x) - .1) &&
												(UnityEngine.Mathf.Abs ((block1.position.y + movementVector.y) - block2.position.y) * 2) < ((((UnityEngine.BoxCollider)block1.collider).size.y + ((UnityEngine.BoxCollider)block2.collider).size.y) - .1))) {
												return true;			
										}
								}
						}
						return false;
				}
		

				//TODO - fix this up, wall should not be hard coded
				public bool CheckCollisionWithLeftWall (UnityEngine.Vector3 movementVector)
				{
						var foo = this.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform child1 in foo) {
								if (((child1.position.x - (child1.renderer.bounds.size.x / 2.0)) + movementVector.x) < -.1) { //jitter (why it isnt 0)
										return true;	
								}
						}
						return false;
				}
		
				public bool CheckCollisionWithRightWall (UnityEngine.Vector3 movementVector)
				{
						var foo = this.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform child1 in foo) {
								if (((child1.position.x + (child1.renderer.bounds.size.x / 2.0)) + movementVector.x) > 8.1) {
										//UnityEngine.Debug.LogWarning ("Collided with right wall. pos: " + child1.position.x + " bounds size: " + ); //todo - replace with an assert of some kind
										return true;	
								}
						}
						return false;
				}
				public bool CheckCollisionWithBotWall (UnityEngine.Vector3 movementVector)
				{
						var foo = this.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform child1 in foo) {
								if (((child1.position.y - (child1.renderer.bounds.size.y / 2.0) + movementVector.y)) < -24.1) {
										return true;	
								}
						}
						return false;
				}
		
				public bool CheckCollisionWithAnyWall (UnityEngine.Vector3 movementVector)
				{
						var foo = this.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform child1 in foo) {
								if (CheckCollisionWithLeftWall (movementVector) ||
										CheckCollisionWithRightWall (movementVector) ||
										CheckCollisionWithBotWall (movementVector)) {
										return true;	
								}
						}
						return false;
				}
				public bool CheckCollisionWithTopWall (float xDelta, float yDelta) // used for end of game measuring
				{
						var foo = this.m_CompositeGameObject.transform;
						foreach (UnityEngine.Transform child1 in foo) {
								if ((child1.position.y + (child1.renderer.bounds.size.y / 2.0)) > 0) {
										return true;	
								}
						}
						return false;
				}
		}
}

