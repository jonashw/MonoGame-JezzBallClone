using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class Map
    {
        private readonly bool[,] _tilesByRowsThenColumns;
        private readonly Texture2D _tileTexture;
        public readonly Vector2 TileSize;
        private readonly ILogger _logger;

        public Map(Texture2D tileTexture, ILogger logger, bool[,] tilesByRowsThenColumns)
        {
            _tilesByRowsThenColumns = tilesByRowsThenColumns;
            _logger = logger;
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

        public bool HasTileIn(TileSlot index)
        {
            return _tilesByRowsThenColumns[index.RowIndex, index.ColumnIndex];
        }

        public Vector2 GetPositionOf(TileSlot slot)
        {
            return new Vector2(
                slot.ColumnIndex*TileSize.X,
                slot.RowIndex*TileSize.Y);
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

        public void SetAt(TileSlot slot, bool hasTile)
        {
            _tilesByRowsThenColumns[slot.RowIndex, slot.ColumnIndex] = hasTile;
        }

        public float GetFillPercent()
        {
            var filledSlots = 0f; 
            for (var r = 0; r < _tilesByRowsThenColumns.GetLength(0); r++)
            {
                for (var c = 0; c < _tilesByRowsThenColumns.GetLength(1); c++)
                {
                    if (_tilesByRowsThenColumns[r, c])
                    {
                        filledSlots++;
                    }
                }
            }
            return (100 * filledSlots / TileSlot.TotalSlots);
        }

        private bool indexInBounds(int colIndex, int rowIndex)
        {
            return rowIndex >= 0 && rowIndex < _tilesByRowsThenColumns.GetLength(0)
                   && colIndex >= 0 && colIndex < _tilesByRowsThenColumns.GetLength(1);
        }

        private void floodFillPrivate(TileSlot slot)
        {
            if (_tilesByRowsThenColumns[slot.RowIndex, slot.ColumnIndex])
            {
                return;
            }
            var slotsToFill = new List<TileSlot>();
            var firstEdgeSlot = findEdge(slot, MooreNeighborRelation.Up);
            slotsToFill.Add(firstEdgeSlot);
            var previousEdgeSlot = firstEdgeSlot;
            while (true)
            {
                foreach (var neighborRelation in MooreNeighborRelation.CARDINALS)
                {
                    _logger.Log("Looking for neighbor: " + neighborRelation.Name);
                    var maybeNeighbor = neighborRelation.TryGetNeighborFor(previousEdgeSlot);
                    if (!maybeNeighbor.HasValue)
                    {
                        _logger.Log("Hit end of map while looking for a neighbor, going " + neighborRelation.Name);
                        continue;
                    }
                    var neighbor = maybeNeighbor.Value;
                    if (_tilesByRowsThenColumns[neighbor.RowIndex, neighbor.ColumnIndex])
                    {
                        _logger.Log("Neighbor is filled: " + neighborRelation.Name);
                        continue;
                    }
                    var foundNextEdgeSlot = false;
                    foreach (var subNeighborRelation in MooreNeighborRelation.ALL)
                    {
                        var maybeSubNeighbor = subNeighborRelation.TryGetNeighborFor(neighbor);
                        if (!maybeSubNeighbor.HasValue)
                        {
                            _logger.Log("Hit end of map while looking for a sub-neighbor, going " + subNeighborRelation.Name);
                            continue;
                        }
                        var subNeighbor = maybeSubNeighbor.Value;
                        if (_tilesByRowsThenColumns[subNeighbor.RowIndex, subNeighbor.ColumnIndex])
                        {
                            _logger.Log("Found edge by going " + subNeighborRelation.Name + " from " + neighborRelation.Name + " Neighbor");
                            slotsToFill.Add(neighbor);
                            previousEdgeSlot = neighbor;
                            foundNextEdgeSlot = true;
                            break;
                        }
                    }
                    if (foundNextEdgeSlot)
                    {
                        break;
                    }
                }
                if (previousEdgeSlot == firstEdgeSlot)
                {
                    _logger.Log("Terminating because previousEdgeSlot == firstEdgeSlot");
                    break;
                }
            }
            foreach (var s in slotsToFill)
            {
                _tilesByRowsThenColumns[s.RowIndex, s.ColumnIndex] = true;
            }
        }

        private TileSlot findEdge(TileSlot slot, MooreNeighborRelation direction)
        {
            while (true)
            {
                var maybeNextSlot = direction.TryGetNeighborFor(slot);
                if (!maybeNextSlot.HasValue)
                {
                    return slot;
                }
                var nextSlot = maybeNextSlot.Value;
                if (_tilesByRowsThenColumns[nextSlot.RowIndex, nextSlot.ColumnIndex])
                {
                    return slot;
                }
                slot = nextSlot;
            }
        }
    }
}