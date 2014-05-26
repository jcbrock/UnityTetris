using System;
namespace AssemblyCSharp
{
		public class CellInformation
		{
				public Coordinate coordinate;
				public TetrisGridCellStatus RightNeighborStatus;
				public TetrisGridCellStatus LeftNeighborStatus;
				public TetrisGridCellStatus TopNeighborStatus;
				public TetrisGridCellStatus BotNeighborStatus;
		}
}

