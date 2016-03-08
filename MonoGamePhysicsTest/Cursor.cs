using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace MonoGamePhysicsTest
{
    public class Cursor
    {
        private readonly CursorTextures _textures;
        private readonly StringDrawer _stringDrawer;
        private PlayDirection _state = PlayDirection.Horizontal;
        private bool _enabled;
        private readonly Vector2 _origin = new Vector2(10,10);
        private Vector2 _position;
        private TileSlot _slot;
        private readonly Map _map;
        private readonly List<Divider> _dividers = new List<Divider>();
        private readonly Texture2D _dividerTexture;

        public Cursor(Map map, CursorTextures textures, Texture2D dividerTexture, StringDrawer stringDrawer)
        {
            _map = map;
            _textures = textures;
            _dividerTexture = dividerTexture;
            _stringDrawer = stringDrawer;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            drawDebugInfo(spriteBatch);
            spriteBatch.Draw(getTexture(), _position, origin: _origin );
            foreach (var d in _dividers)
            {
                d.Draw(spriteBatch);
            }
        }

        private void drawDebugInfo(SpriteBatch spriteBatch)
        {
            _stringDrawer.Draw(spriteBatch, string.Format("Cursor Position: {0}, {1}", _slot.ColumnIndex, _slot.RowIndex), new Vector2(20,20));
            _stringDrawer.Draw(spriteBatch, string.Format("Dividers in-progress: {0}", _dividers.Count), TileSlot.TopRightPosition - new Vector2(230, -20));
        }

        private readonly List<Divider> _dividersToRemove = new List<Divider>();
        private void updateDividers(GameTime gameTime)
        {
            foreach (var d in _dividers)
            {
                d.Update(gameTime);
                switch (d.State)
                {
                    case DividerState.TotalSuccess:
                    case DividerState.TotalFailure:
                    case DividerState.PartialSuccess:
                        _dividersToRemove.Add(d);
                        break;
                }
            }
            foreach (var d in _dividersToRemove)
            {
                _dividers.Remove(d);
            }
            _dividersToRemove.Clear();
        }

        public void Update(GameTime gameTime)
        {
            updateDividers(gameTime);
            var mouse = Mouse.GetState();
            var maybeNewSlot = TileSlot.TryGetForPosition(mouse.X, mouse.Y);
            if (!maybeNewSlot.HasValue)
            {
                return;
            }
            var slot = maybeNewSlot.Value;
            _slot = slot;

            _position.X = _map.TileSize.X * slot.ColumnIndex;
            _position.Y = _map.TileSize.Y * slot.RowIndex;


            if (_map.HasTileIn(slot))
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
                    var canDown = !_map.HasTileAtIndex(slot.ColumnIndex, slot.RowIndex + 1);
                    var canUp = !_map.HasTileAtIndex(slot.ColumnIndex, slot.RowIndex - 1);
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
                    var canRight = !_map.HasTileAtIndex(slot.ColumnIndex + 1, slot.RowIndex);
                    var canLeft = !_map.HasTileAtIndex(slot.ColumnIndex - 1, slot.RowIndex);
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

        private Texture2D getTexture()
        {
            if (!_enabled)
            {
                return _textures.Disabled;
            }
            switch (_state)
            {
                case PlayDirection.Up: return _textures.Up;
                case PlayDirection.Down: return _textures.Down;
                case PlayDirection.Left: return _textures.Left;
                case PlayDirection.Right: return _textures.Right;
                case PlayDirection.Horizontal: return _textures.Horizontal;
                case PlayDirection.Vertical: return _textures.Vertical;
            }
            throw new ArgumentException("Unable to return a cursor sprite for PlayDirection: " + _state);
        }

        public void ToggleAxis()
        {
            switch (_state)
            {
                case PlayDirection.Vertical:
                    _state = PlayDirection.Horizontal;
                    break;
                case PlayDirection.Horizontal:
                    _state = PlayDirection.Vertical;
                    break;
            }
        }

        public void StartDivider()
        {
            if (!_enabled)
            {
                return;
            }

            var maybeSlot = TileSlot.TryGetForPosition(_position);
            if (!maybeSlot.HasValue)
            {
                return;
            }

            var slot = maybeSlot.Value;
            _dividers.Add(new Divider(_map, _dividerTexture, slot, _state));
        }
    }

    public enum PlayDirection
    {
        Up, Down, Left, Right, Vertical, Horizontal
    }
}