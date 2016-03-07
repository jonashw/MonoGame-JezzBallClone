using Microsoft.Xna.Framework;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    public struct TileSlot
    {
        public int ColumnIndex;
        public int RowIndex;

        private TileSlot(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        public TileSlot? Next(BallPhysics.Axis axis, bool positive)
        {
            int nextRow, nextCol;
            if (axis == BallPhysics.Axis.X)
            {
                nextCol = ColumnIndex + (positive ? 1 : -1);
                nextRow = RowIndex;
            }
            else
            {
                nextCol = ColumnIndex;
                nextRow = RowIndex + (positive ? -1 : 1);
            }
            return TryCreate(nextCol, nextRow);
        }

        public const int TileSize = 10;
        public static TileSlot? TryCreate(int column, int row)
        {
            if (row < 0 || row >= 72)
            {
                return null;
            }
            if (column < 0 || column >= 128)
            {
                return null;
            }
            return new TileSlot(column, row);
        }

        public static TileSlot? TryGetForPosition(Vector2 position)
        {
            var colIndex = (int) (position.X/TileSize);
            var rowIndex = (int) (position.Y/TileSize);
            return TryCreate(colIndex, rowIndex);
        }

        public static TileSlot? TryGetForPosition(int x, int y)
        {
            var colIndex = x/TileSize;
            var rowIndex = y/TileSize;
            return TryCreate(colIndex, rowIndex);
        }
    }
}