using System;
namespace AssemblyCSharp
{
		public class ShapeRuleset
		{		
				public int PossibleXStartPosition { get { return mPossibleXStartPosition; } } //Exposing for ? class
				public int PossibleXEndPosition { get { return mPossibleXEndPosition; } } //Exposing for ? class
				public int NumberOfPossibleRotations { get { return mNumberOfPossibleRotations; } } //Exposing for ? class
				public Shape[] ShapesInRuleset { get { return mShapesInRuleset; } } //Exposing for ? class
				private int mPossibleXStartPosition = 2;
				private int mPossibleXEndPosition = 7;
				private int mNumberOfPossibleRotations = 4;		
				private Shape[] mShapesInRuleset; 
		
				public void SetRuleset (int xStart, int xEnd, int possibleRotations, Shape[] ruleset)
				{
						mPossibleXStartPosition = xStart;
						mPossibleXEndPosition = xEnd;
						mNumberOfPossibleRotations = possibleRotations;
						mShapesInRuleset = ruleset;
				}				
		}
}

