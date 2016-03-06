using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePhysicsTest
{
    public class Cursor
    {
        private readonly CursorTextures _textures;
        private PlayDirection _state = PlayDirection.Horizontal;
        private bool _enabled;
        private readonly Vector2 _origin = new Vector2(10,10);
        private Vector2 _position;
        private readonly Map _map;

        public Cursor(Map map, CursorTextures textures)
        {
            _map = map;
            _textures = textures;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_textures.Get(_state), _position, origin: _origin );
        }
        public void Update(GameTime gameTime)
        {
            int colIndex, rowIndex;
            var mouse = Mouse.GetState();
            if (!_map.TryNormalizeCoordinates(mouse.X, mouse.Y, out colIndex, out rowIndex))
            {
                return;
            }
            _position.X = _map.TileSize.X * colIndex;
            _position.Y = _map.TileSize.Y * rowIndex;
            if (_map.HasTileAtIndex(colIndex, rowIndex))
            {
                _enabled = false;
                return;
            }
            _enabled = true;
            switch (_state)
            {
                case PlayDirection.Vertical:
                case PlayDirection.Up:
                case PlayDirection.Down:
                    var canDown = !_map.HasTileAtIndex(colIndex, rowIndex + 1);
                    var canUp = !_map.HasTileAtIndex(colIndex, rowIndex - 1);
                    if (canDown && canUp)
                    {
                        _state = PlayDirection.Vertical;
                    } else if (canDown)
                    {
                        _state = PlayDirection.Down;
                    } else if (canUp)
                    {
                        _state = PlayDirection.Up;
                    }
                    break;
                case PlayDirection.Horizontal:
                case PlayDirection.Left:
                case PlayDirection.Right:
                    var canRight = colIndex + 1 < 128 && !_map.HasTileAtIndex(colIndex + 1, rowIndex);
                    var canLeft = colIndex - 1 > 1 && !_map.HasTileAtIndex(colIndex - 1, rowIndex);
                    if (canRight && canLeft)
                    {
                        _state = PlayDirection.Horizontal;
                    } else if (canRight)
                    {
                        _state = PlayDirection.Right;
                    } else if (canLeft)
                    {
                        _state = PlayDirection.Left;
                    }
                    break;
            }
        }

        public void ToggleAxis()
        {
            if (_state == PlayDirection.Vertical)
            {
                _state = PlayDirection.Horizontal;
            } else if (_state == PlayDirection.Horizontal)
            {
                _state = PlayDirection.Vertical;
            }
        }

        public void StartDivider()
        {
            if (!_enabled)
            {
                return;
            }

            int colIndex, rowIndex;
            if (!_map.TryNormalizeCoordinates(_position.X, _position.Y, out colIndex, out rowIndex))
            {
                return;
            }
            switch (_state)
            {
                case PlayDirection.Vertical:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerUp(colIndex,rowIndex);
                    goDividerDown(colIndex,rowIndex);
                    break;
                case PlayDirection.Up:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerUp(colIndex,rowIndex);
                    break;
                case PlayDirection.Down:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerDown(colIndex,rowIndex);
                    break;
                case PlayDirection.Horizontal:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerLeft(colIndex,rowIndex);
                    goDividerRight(colIndex,rowIndex);
                    break;
                case PlayDirection.Left:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerLeft(colIndex,rowIndex);
                    break;
                case PlayDirection.Right:
                    _map.SetAtIndex(colIndex, rowIndex, true);
                    goDividerRight(colIndex,rowIndex);
                    break;
            }
        }

        private void goDividerDown(int colIndex, int rowIndex)
        {
            rowIndex ++;
            while (!_map.HasTileAtIndex(colIndex, rowIndex))
            {
                _map.SetAtIndex(colIndex, rowIndex, true);
                rowIndex ++;
            }
        }

        private void goDividerUp(int colIndex, int rowIndex)
        {
                rowIndex --;
            while (!_map.HasTileAtIndex(colIndex, rowIndex))
            {
                _map.SetAtIndex(colIndex, rowIndex, true);
                rowIndex --;
            }
        }

        private void goDividerLeft(int colIndex, int rowIndex)
        {
            colIndex --;
            while (!_map.HasTileAtIndex(colIndex, rowIndex))
            {
                _map.SetAtIndex(colIndex, rowIndex, true);
                colIndex --;
            }
        }

        private void goDividerRight(int colIndex, int rowIndex)
        {
            colIndex ++;
            while (!_map.HasTileAtIndex(colIndex, rowIndex))
            {
                _map.SetAtIndex(colIndex, rowIndex, true);
                colIndex ++;
            }
        }
    }

    public enum PlayDirection
    {
        Up, Down, Left, Right, Vertical, Horizontal
    }
}