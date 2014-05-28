using System;
using System.Collections;
using System.Collections.Generic;
namespace AssemblyCSharp
{
		//TetrisBitArray is responsible for
		// - providing convient way to access bit array (allowing (x,y) syntax)
		// - checking which rows are full
		// - deleting full rows
		// - updating internal bit array
		public class TetrisGrid
		{				
				private BitArray mBitArray;
				private byte[] mRowBytes; //A grouping of 8 bits, used to make things like detecting a full row easier
				private int mRowCount;
				private int mColumnCount;				

				private TetrisGrid ()
				{
				}

				public TetrisGrid (int rowCount, int columnCount)
				{
						mBitArray = new BitArray (rowCount * columnCount);
						mRowCount = rowCount;
						mColumnCount = columnCount;
						mRowBytes = new byte[rowCount];//assuming 1 row = 8 bytes 
				}
			
				public void ClearGrid ()
				{
						mBitArray.SetAll (false);
				}

				public int GetCountOfFilledBlocksInRow (int row)
				{
						int filledBlockCount = 0;
						for (int i = 0; i < mColumnCount; ++i) {
								if (this [row, i])
										++filledBlockCount;
						}
						return filledBlockCount;
				}

				public bool this [int rowIndex, int columnIndex] {
						get {
								rowIndex = Math.Abs (rowIndex);
								if (rowIndex < 0 || rowIndex >= mRowCount)
										throw new ArgumentOutOfRangeException ("rowIndex");
				
								if (columnIndex < 0 || columnIndex >= mColumnCount)
										throw new ArgumentOutOfRangeException ("columnIndex");
				
								int pos = rowIndex * mColumnCount + columnIndex;
								return mBitArray [pos];
						}
						set {
								rowIndex = Math.Abs (rowIndex);
								if (rowIndex < 0 || rowIndex >= mRowCount)
										throw new ArgumentOutOfRangeException ("rowIndex");
				
								if (columnIndex < 0 || columnIndex >= mColumnCount)
										throw new ArgumentOutOfRangeException ("columnIndex");
				
								int pos = rowIndex * mColumnCount + columnIndex;
				
								mBitArray [pos] = value;
						}
				}
		
				public List<int> GetFullRows ()
				{
						List<int> fullRows = new List<int> ();
			
						//Check for full rows using bit masks
						for (int i = 0; i < mRowCount; ++i) {
								TetrisGrid fullRowMask = new TetrisGrid (mRowCount, mColumnCount);
								fullRowMask.mRowBytes [i] = Byte.MaxValue;
								fullRowMask.UpdateBitArrayBasedOnRowBytes ();				
								BitArray clone = (BitArray)this.mBitArray.Clone (); //need to clone because AND will modify left-hand arg
				
								if (fullRowMask.Equals (clone.And (fullRowMask.mBitArray)))
										fullRows.Add (i);
						}
			
						return fullRows;
				}
		
				public bool Equals (BitArray b)
				{
						if (this.mBitArray.Count != b.Count)
								throw new ArgumentException ("Can't compare BitArrays of different length");
			
						//Not a good way to compare equality of bit arrays in C#
						//At least this does 32 times less compares than comparing bit by
						int[] ints1 = ConvertToInts (this.mBitArray);
						int[] ints2 = ConvertToInts (b);
						for (int i = 0; i < ints1.Length; ++i)
								if (ints1 [i] != ints2 [i])
										return false;
			
						return true;
				}
		
				private int[] ConvertToInts (BitArray bits)
				{
						int arraySize = Convert.ToInt32 (Math.Ceiling ((double)(bits.Count / 32)));
						int[] ints = new int[arraySize];
						bits.CopyTo (ints, 0);
						return ints;
				}

				public void UpdateRowBytes ()
				{						
						mBitArray.CopyTo (mRowBytes, 0);						
				}

				private void UpdateBitArrayBasedOnRowBytes ()
				{
						mBitArray = new BitArray (mRowBytes);
				}
				
				//Delete Row by preserving the rows below the one being deleted, shifting the rows, and then combining those two
				public void DeleteRow (int row)
				{
						row = Math.Abs (row);
									
						BitArray preShiftedGrid = (BitArray)this.mBitArray.Clone ();
						BitArray bottomRowsMask = new BitArray (mColumnCount * mRowCount);
						for (int i = ((row + 1) * mColumnCount); i < (mRowCount * mColumnCount); ++i) { //bottomRowsMask = everything below full row
								bottomRowsMask [i] = true; //-192 > -192								
						}
									
						//UnityEngine.Debug.Log ("Before shift" + ++mDebugId);
						//this.PrintBitArray ();
			
						//shift - CAN'T do via mask because I don't have one consecutive array of bits in C#.
						//They are split up across ints, which makes shifting a bitch - I still have to copy between ints
						//(last part of int to first part of next int...), so it is easier just to use byte array.
						for (int i = mRowCount -1; i > 0; --i) {
								this.mRowBytes [i] = this.mRowBytes [i - 1];
						}
						this.mRowBytes [0] = 0; //explicitly set top row to 0s now that we shifted down
						this.UpdateBitArrayBasedOnRowBytes ();
												
						//UnityEngine.Debug.Log ("After shift" + ++mDebugId);
						//this.PrintBitArray ();
			
						BitArray topRowsMask = new BitArray (mColumnCount * mRowCount);
						for (int i = (((row +1) * mColumnCount)-1); i >= 0; --i) { //includes the full row, and all rows above it
								topRowsMask [i] = true;
						}								

						TetrisGrid ans = new TetrisGrid (mRowCount, mColumnCount);
						ans.mBitArray = (preShiftedGrid.And (bottomRowsMask)).Or (this.mBitArray.And (topRowsMask)); //
						this.mBitArray = ans.mBitArray;

						//UnityEngine.Debug.Log ("Answer" + ++mDebugId);
						//ans.PrintBitArray ();
				}

				//Debug function
				public void PrintBitArray ()
				{
						string output = string.Empty;
			
						for (int i = 0; i < mRowCount; ++i) {
								string space = " ";
								if (i < 10)
										space = "  ";
				
								output += Environment.NewLine + "Row " + i + space;
								for (int j = 0; j < mColumnCount; ++j) {
										output += Convert.ToInt32 (this [i, j]) + " ";								
								}								
						}
						UnityEngine.Debug.Log (output);
				}
		}
}

