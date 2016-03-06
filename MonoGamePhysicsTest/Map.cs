using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class Map
    {
        private readonly bool[,] _tilesByRowsThenColumns;
        private readonly Texture2D _tileTexture;
        public readonly Vector2 TileSize;

        public Map(Texture2D tileTexture, bool[,] tilesByRowsThenColumns)
        {
            _tilesByRowsThenColumns = tilesByRowsThenColumns;
            _tileTexture = tileTexture;
            TileSize = new Vector2(_tileTexture.Width, _tileTexture.Height);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            for (var r = 0; r < _tilesByRowsThenColumns.GetLength(0); r++)
            {
                for (var c = 0; c < _tilesByRowsThenColumns.GetLength(1); c++)
                {
                    if (!_tilesByRowsThenColumns[r, c])
                    {
                        continue;
                    }
                    spriteBatch.Draw(_tileTexture, new Vector2(c, r) * TileSize);
                }
            }
        }

        public bool HasTileAtCoordinates(float x, float y)
        {
            int colIndex, rowIndex;
            return TryNormalizeCoordinates(x, y, out colIndex, out rowIndex)
                   && _tilesByRowsThenColumns[rowIndex, colIndex];
        }

        public void ToggleAtCoordinates(float x, float y)
        {
            int colIndex, rowIndex;
            if (!TryNormalizeCoordinates(x, y, out colIndex, out rowIndex))
            {
                return;
            }
            _tilesByRowsThenColumns[rowIndex, colIndex] = !_tilesByRowsThenColumns[rowIndex, colIndex];
        }

        public bool TryNormalizeCoordinates(float x, float y, out int colIndex, out int rowIndex)
        {
            var ri = (int)(y/TileSize.Y);
            var ci = (int) (x/TileSize.X);
            if (!indexInBounds(ci, ri))
            {
                colIndex = 0;
                rowIndex = 0;
                return false;
            }
            rowIndex = ri;
            colIndex = ci;
            return true;
        }

        public bool HasTileAtIndex(int colIndex, int rowIndex)
        {
            return indexInBounds(colIndex,rowIndex)
                   && _tilesByRowsThenColumns[rowIndex, colIndex];
        }

        public void ToggleAtIndex(int colIndex, int rowIndex)
        {
            if (!indexInBounds(colIndex, rowIndex))
            {
                return;
            }
            _tilesByRowsThenColumns[rowIndex, colIndex] = !_tilesByRowsThenColumns[rowIndex, colIndex];
        }

        public void SetAtIndex(int colIndex, int rowIndex, bool hasTile)
        {
            if (!indexInBounds(colIndex, rowIndex))
            {
                return;
            }
            _tilesByRowsThenColumns[rowIndex, colIndex] = hasTile;
        }

        private bool indexInBounds(int colIndex, int rowIndex)
        {
            return rowIndex >= 0 && rowIndex < _tilesByRowsThenColumns.GetLength(0)
                   && colIndex >= 0 && colIndex < _tilesByRowsThenColumns.GetLength(1);
        }
    }
}