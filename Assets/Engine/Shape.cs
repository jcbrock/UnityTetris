using System;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{		
		public class Shape
		{	
				public string Name; //for debugging				
				private UnityEngine.GameObject mCompositeGameObject;
				private RotationStyles mRotationStyle;
				private bool mFlipRotation = true;
				private float mInitialXPos = 0;
				private int mBlockCount = 4; //assuming 4 blocks per tetris block

				//Hide default constructor
				private Shape ()
				{
				}				

				public Shape (UnityEngine.GameObject compositeGameObject, RotationStyles rotationStyle, float initialXPos)
				{
						if (compositeGameObject == null)
								throw new ArgumentNullException ("A shape MUST contain a game object!");

						Name = compositeGameObject.name;
						mCompositeGameObject = compositeGameObject;
						mInitialXPos = initialXPos;
						mRotationStyle = rotationStyle;
				}

				public Shape Clone (UnityEngine.Vector3 previewLocation, float initialXPos, string name)
				{
			
						UnityEngine.GameObject newGameObject = (UnityEngine.GameObject)UnityEngine.MonoBehaviour.Instantiate (mCompositeGameObject,
			                                                                                               previewLocation,
			                                                                                               UnityEngine.Quaternion.identity);
						newGameObject.name += name;
						return new Shape (newGameObject, mRotationStyle, initialXPos);
				}


				//debug func
				public void ShadeSubBlock (int blockIndex)
				{
						if (mCompositeGameObject.transform.childCount > blockIndex) {
								mCompositeGameObject.transform.GetChild (blockIndex).GetComponent<UnityEngine.SpriteRenderer> ().color = UnityEngine.Color.magenta;
						}		
				}

				public Coordinate GetAnchorCoordinate ()
				{
						return GetCurrentGridPosition () [0];
				}		
		
				//cleans up UI of shape
				public void DeleteShape ()
				{
						UnityEngine.GameObject.Destroy (mCompositeGameObject);
				}

				//returns block count remaining after deletion
				public int DeleteBlocksInRow (int row)
				{
						for (int i = 0; i < mCompositeGameObject.transform.childCount; ++i) {
								if (Convert.ToInt32 (Math.Ceiling (mCompositeGameObject.transform.GetChild (i).transform.position.y)) == row) {
										UnityEngine.GameObject.Destroy (mCompositeGameObject.transform.GetChild (i).gameObject);
										--mBlockCount;
								}
						}
						return mBlockCount;
				}

				public void TranslateToInitialPosition ()
				{
						Translate (mInitialXPos + 5, 0, 0); //todo - fix magic number of 5 (the x distance of the preview shape location)
				}

				public bool ShiftBlocksAboveDeletedRow (int row)
				{
						bool shiftedSomethingDown = false;
						for (int i = 0; i < mCompositeGameObject.transform.childCount; ++i) {
								if (Convert.ToInt32 (Math.Ceiling (mCompositeGameObject.transform.GetChild (i).transform.position.y)) > row) {
										mCompositeGameObject.transform.GetChild (i).transform.Translate (new UnityEngine.Vector3 (0, -1, 0), UnityEngine.Space.World);
										shiftedSomethingDown = true;
								}
						}
						if (shiftedSomethingDown)
								return true;

						return false;
				}

				public void Rotate ()
				{
						Rotate (false);
				}

				public void Rotate (bool backwards)
				{						
						UnityEngine.Vector3 rotation;	
						UnityEngine.Vector3 movementVector = new UnityEngine.Vector3 (0, 0, 0);
						switch (mRotationStyle) {
						case RotationStyles.none:
								break;
						case RotationStyles.flip90:
								rotation = mCompositeGameObject.transform.eulerAngles;
								rotation.z = (rotation.z + (90 * (mFlipRotation ? 1 : -1)));
								mCompositeGameObject.transform.eulerAngles = rotation;
								mFlipRotation = !mFlipRotation;
								break;
						case RotationStyles.full360:
								rotation = mCompositeGameObject.transform.eulerAngles;
								rotation.z = (rotation.z + (90 * (!backwards ? 1 : -1)));
								mCompositeGameObject.transform.eulerAngles = rotation;
								break;
						}
			
				}
		
				public void PlayCollisionAudio ()
				{
						mCompositeGameObject.audio.Play ();
				}

				public void Translate (float x, float y, float z)
				{
						TranslateInWorldSpace (new UnityEngine.Vector3 (x, y, z));								
				}
				public void Translate (UnityEngine.Vector3 vec)
				{
						TranslateInWorldSpace (vec);
				}
				private void TranslateInWorldSpace (UnityEngine.Vector3 vec)
				{
						mCompositeGameObject.transform.Translate (vec, UnityEngine.Space.World);
				}				
				
				public List<int> GetRowValuesOfSubBlocks ()
				{
						List<int> rows = new List<int> ();
						for (int i = 0; i < mCompositeGameObject.transform.childCount; ++i) {

								double rowValue = mCompositeGameObject.transform.GetChild (i).transform.position.y;
								//UnityEngine.Debug.Log ("Block row value: " + rowValue.ToString ());
								rows.Add (Convert.ToInt32 (Math.Floor (rowValue)));
						}
						return rows;
				}
						
				public List<Coordinate> GetCurrentGridPosition ()
				{
						List<Coordinate> blocks = new List<Coordinate> ();
						List<KeyValuePair<int, int>> rows = new List<KeyValuePair<int, int>> ();
						for (int i = 0; i < mCompositeGameObject.transform.childCount; ++i) {											
								double rowValue = mCompositeGameObject.transform.GetChild (i).transform.position.y;
								double colValue = mCompositeGameObject.transform.GetChild (i).transform.position.x;
															
								//rows -> ceiling because they are negative, cols -> floor because they are positive								
								blocks.Add (new Coordinate (Convert.ToInt32 (Math.Ceiling (rowValue)), Convert.ToInt32 (Math.Floor (colValue))));
						}
						return blocks;
						//return rows;
				}
						
				public bool collides (Shape shape, UnityEngine.Vector3 movementVector)
				{						
						var shape1 = this.mCompositeGameObject.transform;
						var shape2 = shape.mCompositeGameObject.transform;
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
		

			
		}
}

