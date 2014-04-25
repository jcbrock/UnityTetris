using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace AssemblyCSharp
{
	enum TetrisShape
	{
		square,
		longShape,
		tShape,
		zShapeRight,
		zShapeLeft,
		lShapeRight,
		lShapeLeft,
		unknown
	}
	public class ShapeFactory
	{
		private List<UnityEngine.GameObject> m_PossibleGameObjectsForShapes = new List<UnityEngine.GameObject> ();
		private int m_DebugCounter = 0;
				
		public ShapeFactory ()
		{
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("square"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("longShape"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("tShape"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("zShapeRight"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("zShapeLeft"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("lShapeRight"));
			m_PossibleGameObjectsForShapes.Add (UnityEngine.GameObject.Find ("lShapeLeft"));
		}
				
		public Shape SpawnRandomizedTetrisShape ()
		{
			TetrisShape randomShape = (TetrisShape)UnityEngine.Random.Range (0, 7); //7 = number of possible shapes
			if (randomShape == TetrisShape.unknown)
				throw new System.Exception ("unknown tetris shape generated!");

			float xStart = UnityEngine.Random.Range (1, 10); //10 = length of tetris board (x)
			xStart -= (float)0.5;
			int rotation = UnityEngine.Random.Range (0, 3); //Rotation possiblities
			UnityEngine.Vector3 temp = new UnityEngine.Vector3 (-5.0f, (float)-0.5, 0);

			Shape newShape = new Shape (SpawnNewBlock (m_PossibleGameObjectsForShapes [(int)randomShape], temp), ConvertBlockToRotationStyle (randomShape), xStart); //eventually replace with random shape...;
			for (int i = 0; i < rotation; ++i) {
				//use shape's rotation functions since they do things like making sure not to rotate out of a wall
				//todo - ehh... if i'm spawning outside of the walls, it won't matter...
				newShape.Rotate ();
			}
			
			return newShape;
		}			
	
		private RotationStyles ConvertBlockToRotationStyle (TetrisShape shape)
		{
			switch (shape) {
			case TetrisShape.square:
				return RotationStyles.none;
			case TetrisShape.longShape:
				return RotationStyles.flip90;
			case TetrisShape.tShape:
				return RotationStyles.full360;
			case TetrisShape.zShapeRight:
				return RotationStyles.flip90;
			case TetrisShape.zShapeLeft:
				return RotationStyles.flip90;
			case TetrisShape.lShapeRight:
				return RotationStyles.flip90;
			case TetrisShape.lShapeLeft:
				return RotationStyles.flip90;
			}
			return RotationStyles.none;
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
