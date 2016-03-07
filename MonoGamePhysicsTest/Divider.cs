using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    public enum DividerState
    {
        Done, StillGrowing
    }

    public class Divider
    {
        private readonly Map _map;
        private readonly BallPhysics.Axis _axis;
        private readonly bool _positive;
        private readonly Texture2D _texture;
        private readonly List<Vector2> _tilePositions = new List<Vector2>();
        private readonly List<TileSlot> _tileSlots = new List<TileSlot>();
        private TileSlot _slot;

        public Divider(Map map, TileSlot startingSlot, BallPhysics.Axis axis, bool positive, Texture2D texture)
        {
            _map = map;
            _slot = startingSlot;
            _axis = axis;
            _positive = positive;
            _texture = texture;
        }

        private const int UpdateEveryNTicks = 3;
        private int _tickCount = 0;
        private bool canUpdateThisTick()
        {
            _tickCount++;
            var can = _tickCount == (UpdateEveryNTicks - 1);
            if (can)
            {
                _tickCount = 0;
            }
            return can;
        }

        public DividerState Update(GameTime gameTime)
        {
            if (!canUpdateThisTick())
            {
                return DividerState.StillGrowing;
            }

            var maybeNextSlot = _slot.Next(_axis, _positive);
            if (!maybeNextSlot.HasValue)
            {
                return done();
            }
            _slot = maybeNextSlot.Value;

            if (_map.HasTileIn(_slot))
            {
                return done();
            }

            _tileSlots.Add(_slot);
            _tilePositions.Add(_map.GetPositionOf(_slot));

            return DividerState.StillGrowing;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var pos in _tilePositions)
            {
                spriteBatch.Draw(_texture, pos);
            }
        }

        private DividerState done()
        {
            foreach (var slot in _tileSlots)
            {
                _map.SetAt(slot, true);
            }
            return DividerState.Done;
        }
    }
}