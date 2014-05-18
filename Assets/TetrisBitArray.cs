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
		//TetrisBitArray is responsible for
		// - providing convient way to access bit array (allowing (x,y) syntax)
		// - checking which rows are full
		// - deleting full rows
		// - updating internal bit array
		public class TetrisBitArray
		{
				private BitArray mBitArray;
				public byte[] mRowBytes;
				private int mRowCount;
				private int mColumnCount;
				private TetrisBitArray ()
				{
				}
				public TetrisBitArray (int rowCount, int columnCount)
				{
						mBitArray = new BitArray (rowCount * columnCount);
						mRowCount = rowCount;
						mColumnCount = columnCount;
						mRowBytes = new byte[rowCount];//assuming 1 row = 8 bytes 

				}
			
				public int GetRowCount ()
				{
						return mRowCount;
				}
				public int GetColumnCount ()
				{
						return mColumnCount;
				}
				public void ClearGrid ()
				{
						mBitArray.SetAll (false);
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
								TetrisBitArray fullRowMask = new TetrisBitArray (mRowCount, mColumnCount);
								fullRowMask.mRowBytes [i] = Byte.MaxValue;
								fullRowMask.UpdateBitArrayBasedOnRowBytes ();				
								BitArray clone = (BitArray)this.mBitArray.Clone (); //need to close because AND will modify left-hand arg
				
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
			
						//TODO - remove debug check
						if (bits.Count != 192) {
								UnityEngine.Debug.LogWarning ("Grid length not 192");
						}
			
						int[] ints = new int[arraySize];
						bits.CopyTo (ints, 0);
						return ints;
				}
		
				public void UpdateRowBytes ()
				{
						if (mBitArray.Count != 192) {
								throw new ArgumentException ("# of bits is wrong for row bytes");
						}
						//byte[] bytes = new byte[24];
						mBitArray.CopyTo (mRowBytes, 0);
						//return bytes;
				}
				public void UpdateBitArrayBasedOnRowBytes ()
				{
						mBitArray = new BitArray (mRowBytes);
				}
		
		
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
		
				static int foo = 0;
				public void DeleteRow (int row)
				{
						row = Math.Abs (row);
			
						//TODO - clean up all extra bit arrays and crap.
			
						//row 0 = top row
						//row 23 = bot row
						BitArray x = (BitArray)this.mBitArray.Clone ();
						BitArray yBot = new BitArray (mColumnCount * mRowCount);
						for (int i = ((row + 1) * mColumnCount); i < (mRowCount * mColumnCount); ++i) { //yBot = everything below full row, hence -1
								yBot [i] = true; //-192 > -192
								//-184 > -192, -185 > -192, -186 > 192
						}
			
						//Debug - print stuff
						TetrisBitArray p2 = new TetrisBitArray (mRowCount, mColumnCount);
						/*p2.m_data = yBot;
			++foo;
			UnityEngine.Debug.Log ("yBot" + foo);
			p2.PrintBitArray ();*/
						++foo;
						UnityEngine.Debug.Log ("Before shift" + foo);
						this.PrintBitArray ();
			
						//shift - CAN'T do via mask because I don't have one consecutive array of bits in C#.
						//They are split up across ints, which makes shifting a bitch - I still have to copy between ints
						//(last part of int to first part of next int...), so it is easier just to use byte array.
						for (int i = mRowCount -1; i > 0; --i) {
								this.mRowBytes [i] = this.mRowBytes [i - 1];
						}
						this.mRowBytes [0] = 0;
						this.UpdateBitArrayBasedOnRowBytes ();
			
						//Debug print stuff
						++foo;
						UnityEngine.Debug.Log ("After shift" + foo);
						this.PrintBitArray ();
			
						BitArray yTop = new BitArray (mColumnCount * mRowCount);
						for (int i = (((row +1) * mColumnCount)-1); i >= 0; --i) { //includes the full row, hence the -1 (+1 is for 0-191, not 1-192)
								yTop [i] = true;
						}
						/*++foo;
			UnityEngine.Debug.Log ("After yTop construction" + foo);

			//Debug - print
			MyBitArray p = new MyBitArray (height, m_RowWidth);
			p.m_data = yTop;
			++foo;
			UnityEngine.Debug.Log ("yTop" + foo);
			p.PrintBitArray ();*/
						++foo;
						UnityEngine.Debug.Log ("Answer" + foo);
			
						TetrisBitArray ans = new TetrisBitArray (mRowCount, mColumnCount);
						ans.mBitArray = (x.And (yBot)).Or (this.mBitArray.And (yTop)); //Take bottom or original grid, add it to the top of the shifted down grid
						ans.PrintBitArray ();
						this.mBitArray = ans.mBitArray;
				}	
		}
}

