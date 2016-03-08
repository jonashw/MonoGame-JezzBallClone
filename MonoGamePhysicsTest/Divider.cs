using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    public class Divider
    {
        private readonly Map _map;
        private readonly Texture2D _texture;
        private readonly PlayDirection _direction;
        private readonly TileSlot _startingSlot;
        private readonly Vector2 _position;
        private readonly List<DividerLeg> _legs = new List<DividerLeg>();
        public DividerState State { get; private set; }

        public Divider(Map map, Texture2D texture, TileSlot startingSlot, PlayDirection direction)
        {
            _map = map;
            _startingSlot = startingSlot;
            _position = startingSlot.ToPosition();
            _texture = texture;
            _direction = direction;
            State = DividerState.StillGrowing;

            switch (direction)
            {
                case PlayDirection.Vertical:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.Y, false, texture));
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.Y, true, texture));
                    break;
                case PlayDirection.Up:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.Y, false, texture));
                    break;
                case PlayDirection.Down:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.Y, true, texture));
                    break;
                case PlayDirection.Horizontal:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.X, false, texture));
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.X, true, texture));
                    break;
                case PlayDirection.Left:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.X, false, texture));
                    break;
                case PlayDirection.Right:
                    _legs.Add(new DividerLeg(map, _startingSlot, BallPhysics.Axis.X, true, texture));
                    break;
            }
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

        public void Update(GameTime gameTime)
        {
            if (State != DividerState.StillGrowing || !canUpdateThisTick())
            {
                return;
            }

            foreach (var leg in _legs)
            {
                leg.Update(gameTime);
            }

            if (_legs.Any(l => l.State == DividerLegState.StillGrowing))
            {
                return;
            }

            if (_legs.All(l => l.State == DividerLegState.Failed))
            {
                _legs.Clear();
                State = DividerState.TotalFailure;
                return;
            }

            if (_legs.All(l => l.State == DividerLegState.Succeeded))
            {
                _map.SetAt(_startingSlot, true);
                State = DividerState.TotalSuccess;
                return;
            }

            State = DividerState.PartialSuccess;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var leg in _legs)
            {
                leg.Draw(spriteBatch);
            }
            spriteBatch.Draw(_texture, _position);
        }
    }
}