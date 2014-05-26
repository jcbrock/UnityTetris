using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
namespace AssemblyCSharp
{
		
		public class ShapeFactory
		{
				private ShapeRulesetFactory rulesetFactory = new ShapeRulesetFactory ();								
				private int m_DebugCounter = 0;				
				
				public ShapeFactory ()
				{
				}			
	
				public Shape SpawnRandomizedTetrisShape (int rulesetOption)
				{
						if (rulesetOption < 0 || rulesetOption > rulesetFactory.GetRulesets ().Count)
								throw new ArgumentOutOfRangeException ("Invalid ruleset specified!");
						
						ShapeRuleset ruleset = rulesetFactory.GetRulesets () [rulesetOption];

						//Choose random shape
						int randomNumber = UnityEngine.Random.Range (0, ruleset.GetPossibleShapes ().Length); //7 = number of possible shapes				
						Shape randomShapeToGenerate = ruleset.GetPossibleShapes () [randomNumber];
						
						//give random start			
						float xStart = UnityEngine.Random.Range (ruleset.GetPossibleXStartPosition (), ruleset.GetPossibleXEndPosition ()); //10 = length of tetris board (x)
						xStart -= (float)0.5;
						UnityEngine.Vector3 previewPosition = new UnityEngine.Vector3 (-5.0f, (float)-0.5, 0); //this is the preview shape position

						//spawn
						Shape newShape = new Shape (SpawnNewBlock (randomShapeToGenerate.GetGameObject (), previewPosition), randomShapeToGenerate.GetRotationStyle (), xStart);

						//give random rotation
						int rotation = UnityEngine.Random.Range (0, ruleset.GetNumberOfPossibleRotations () - 1); //Rotation possiblities
						for (int i = 0; i < rotation; ++i) {
								//use shape's rotation functions since they do things like making sure not to rotate out of a wall
								//todo - ehh... if i'm spawning outside of the walls, it won't matter...
								newShape.Rotate ();
						}

						return newShape;
				}			
			
				private UnityEngine.GameObject SpawnNewBlock (UnityEngine.GameObject objectShape, UnityEngine.Vector3 position)
				{
		
						UnityEngine.GameObject newObj = (UnityEngine.GameObject)UnityEngine.MonoBehaviour.Instantiate (objectShape,
		                                                                                               position,
		                                                                                               UnityEngine.Quaternion.identity);
		
						newObj.name += m_DebugCounter;
						m_DebugCounter++;
						return newObj;
				}
		}
}
