using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AssemblyCSharp;

namespace AssemblyCSharp
{
		public class ChangeThis
		{
				public int numberOfRotations;
				public int score;
		}


		public class AI
		{
				private const int m_RowCount = 24;
				private const int m_ColumnCount = 8;
	
				//my heurestic - forget rotation for now
				public	int ComputeScore (Shape s, TetrisBitArray m_SceneGrid)
				{
						List<KeyValuePair<int, int>> rowCols = s.GetFilledGridValues ();
						foreach (KeyValuePair<int,int> rowCol in rowCols) {
								if (m_SceneGrid [rowCol.Key, rowCol.Value] == true) {
										return -1;
								}
						}

						//Compute score - for now, count neighbors
						int neighborCount = 0;
						foreach (KeyValuePair<int,int> rowCol in rowCols) {
								int right = rowCol.Value + 1;
								int left = rowCol.Value - 1;
								int up = rowCol.Key + 1;
								int down = rowCol.Key - 1;
								if (up <= 0 && m_SceneGrid [up, rowCol.Value] == true) {
										++neighborCount;
								}
								if (right < 8 && m_SceneGrid [rowCol.Key, right] == true) {
										++neighborCount;
								}
								if ((down > -24 && m_SceneGrid [down, rowCol.Value] == true) || down == -24) {
										++neighborCount;
								}
								if (left >= 0 && m_SceneGrid [rowCol.Key, left] == true) {
										++neighborCount;
								}
						}
						return neighborCount;
				}
		
			
				//ok, this shifting must be done an easier way...
				static int foo = 0;
				public	ChangeThis ComputeScore (Shape s, TetrisBitArray m_SceneGrid, int rowTarget, int columnTarget)
				{
						List<ChangeThis> scoreOfRots = new List<ChangeThis> ();
						scoreOfRots.Add (new ChangeThis ());
						scoreOfRots.Add (new ChangeThis ());
						scoreOfRots.Add (new ChangeThis ());
						scoreOfRots.Add (new ChangeThis ());

						int rotatedTimes = 0;
						for (int rot = 0; rot < 4; ++rot) {
								rowTarget *= -1;
								var blah = 0;
								if (rowTarget == -23 && columnTarget == 0)
										blah++;

								List<KeyValuePair<int, int>> rowCols = s.GetFilledGridValues ();
								int shiftRow = rowTarget - rowCols [0].Key;
								int shiftCol = columnTarget - rowCols [0].Value;
								s.ShadeSubBlock (0);

								List<KeyValuePair<int, int>> newRowCols = new List<KeyValuePair<int, int>> ();
								foreach (KeyValuePair<int,int> rowCol in rowCols) {		
										newRowCols.Add (new KeyValuePair<int, int> (rowCol.Key - shiftRow, rowCol.Value + shiftCol));
								}

								bool shouldContinue = false;
								foreach (KeyValuePair<int,int> rowCol in newRowCols) {
										if ((rowCol.Key <= -24 || rowCol.Key > 0) || (rowCol.Value >= 8 || rowCol.Value < 0) || m_SceneGrid [rowCol.Key, rowCol.Value] == true) {
												shouldContinue = true;												
												//return -1;
												//	ChangeThis returnthis = new ChangeThis ();
												//	returnthis.numberOfRotations = 0;
												//	returnthis.score = -1;
												//	return returnthis;
										}
								}

								if (shouldContinue) {
										s.Rotate ();
										rotatedTimes++;
										continue;
										
								}
				
								//todo - damn, I don't detect if the piece can "rest" at that spot. oh well... cna do that later... it'll just reach that spot, then continue falling.

								//Compute score - for now, count neighbors
								int neighborCount = 0;
								foreach (KeyValuePair<int,int> rowCol in newRowCols) {
										int right = rowCol.Value + 1;
										int left = rowCol.Value - 1;
										int up = rowCol.Key + 1;
										int down = rowCol.Key - 1;
										if (up <= 0 && m_SceneGrid [up, rowCol.Value] == true) {
												++neighborCount;
										}
										if (right < 8 && m_SceneGrid [rowCol.Key, right] == true) {
												++neighborCount;
										}
										if ((down > -24 && m_SceneGrid [down, rowCol.Value] == true) || down == -24) {
												++neighborCount;
										}
										if (left >= 0 && m_SceneGrid [rowCol.Key, left] == true) {
												++neighborCount;
										}
								}
								
								scoreOfRots [rot].numberOfRotations = rot;
								scoreOfRots [rot].score = neighborCount;
								rotatedTimes++;
								s.Rotate ();
								//			return neighborCount;
						}

						if (scoreOfRots.Count == 0) {
								ChangeThis returnthis = new ChangeThis ();
								returnthis.numberOfRotations = 0;
								returnthis.score = -1;
								return returnthis;
						}

						ChangeThis answer = scoreOfRots.OrderByDescending (x => x.score).First ();
						//get back to normal orientation
						foo++;
						//	UnityEngine.Debug.Log (foo + "rotatedTimes: " + rotatedTimes);
						UnityEngine.Debug.Log ("rotatedTimes: " + rotatedTimes);
						for (int i = rotatedTimes; i < 4; ++i)
								s.Rotate ();
						return answer;
				}
		}
}