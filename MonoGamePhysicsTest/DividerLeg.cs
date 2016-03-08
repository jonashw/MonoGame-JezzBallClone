using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    public class DividerLeg
    {
        private readonly Map _map;
        private readonly BallPhysics.Axis _axis;
        private readonly bool _positive;
        private readonly Texture2D _texture;
        private readonly List<Vector2> _tilePositions = new List<Vector2>();
        private readonly List<TileSlot> _tileSlots = new List<TileSlot>();
        public DividerLegState State { get; private set; }
        private TileSlot _slot;
        public DividerLeg(Map map, TileSlot startingSlot, BallPhysics.Axis axis, bool positive, Texture2D texture)
        {
            _map = map;
            _slot = startingSlot;
            _axis = axis;
            _positive = positive;
            _texture = texture;
            State = DividerLegState.StillGrowing;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (State != DividerLegState.StillGrowing)
            {
                return;
            }

            foreach (var pos in _tilePositions)
            {
                spriteBatch.Draw(_texture, pos);
            }
        }

        public void Update(GameTime gameTime)
        {
            if (State != DividerLegState.StillGrowing)
            {
                return;
            }
            var maybeNextSlot = _slot.Next(_axis, _positive);
            if (!maybeNextSlot.HasValue)
            {
                succeeded();
                return;
            }
            _slot = maybeNextSlot.Value;

            if (_map.HasTileIn(_slot))
            {
                succeeded();
                return;
            }

            _tileSlots.Add(_slot);
            _tilePositions.Add(_slot.ToPosition());
        }

        private void succeeded()
        {
            State = DividerLegState.Succeeded;
            foreach (var slot in _tileSlots)
            {
                _map.SetAt(slot, true);
            }
            _tileSlots.Clear();
            _tilePositions.Clear();
        }

        private void failed()
        {
            State = DividerLegState.Failed;
            _tileSlots.Clear();
            _tilePositions.Clear();
        }
    }
}