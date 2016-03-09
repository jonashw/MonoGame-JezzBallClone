using System.Diagnostics;
using System.Diagnostics.Contracts;
using Microsoft.Xna.Framework;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    [DebuggerDisplay("({ColumnIndex}, {RowIndex})")]
    public struct TileSlot
    {
        public const int RowCount = 72;
        public const int ColumnCount = 128;
        public const int TotalSlots = RowCount*ColumnCount;
        public const int Size = 10;

        public readonly int ColumnIndex;
        public readonly int RowIndex;

        private TileSlot(int columnIndex, int rowIndex)
        {
            ColumnIndex = columnIndex;
            RowIndex = rowIndex;
        }

        public static readonly TileSlot TopLeftSlot = new TileSlot(0, 0);
        public static readonly Vector2 TopLeftPosition = TopLeftSlot.ToPosition();

        public static readonly TileSlot TopRightSlot = new TileSlot(ColumnCount - 1, 0);
        public static readonly Vector2 TopRightPosition = TopRightSlot.ToPosition();

        public static readonly TileSlot BottomLeftSlot = new TileSlot(0, RowCount - 1);
        public static readonly Vector2 BottomLeftPosition = BottomLeftSlot.ToPosition();

        public static readonly TileSlot BottomRightSlot = new TileSlot(ColumnCount - 1, RowCount - 1);
        public static readonly Vector2 BottomRightPosition = BottomRightSlot.ToPosition();

        [Pure]
        public Vector2 ToPosition()
        {
            return new Vector2(
                ColumnIndex*Size,
                RowIndex*Size);
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

        public static TileSlot? TryCreate(int column, int row)
        {
            if (row < 0 || row >= RowCount)
            {
                return null;
            }
            if (column < 0 || column >= ColumnCount)
            {
                return null;
            }
            return new TileSlot(column, row);
        }

        public static TileSlot? TryGetForPosition(Vector2 position)
        {
            var colIndex = (int) (position.X/Size);
            var rowIndex = (int) (position.Y/Size);
            return TryCreate(colIndex, rowIndex);
        }

        public static TileSlot? TryGetForPosition(int x, int y)
        {
            var colIndex = x/Size;
            var rowIndex = y/Size;
            return TryCreate(colIndex, rowIndex);
        }

        //Equality Members.
        public bool Equals(TileSlot other)
        {
            return ColumnIndex == other.ColumnIndex && RowIndex == other.RowIndex;
        }
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            return obj is TileSlot && Equals((TileSlot) obj);
        }
        public override int GetHashCode()
        {
            unchecked
            {
                return (ColumnIndex*397) ^ RowIndex;
            }
        }
        public static bool operator ==(TileSlot a, TileSlot b) { return a.Equals(b); }
        public static bool operator !=(TileSlot a, TileSlot b) { return !a.Equals(b); }
    }
}