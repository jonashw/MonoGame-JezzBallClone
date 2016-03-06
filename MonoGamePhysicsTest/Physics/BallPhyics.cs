using Microsoft.Xna.Framework;

namespace MonoGamePhysicsTest.Physics
{
    /// <summary>
    /// This class is meant to be used by the physics engine only.
    /// </summary>
    public class BallPhysics : IBallPhysics
    {
        private readonly Map _map;
        private Vector2 _position;
        private Vector2 _velocity;
        private float _rotation = 0;
        private bool _rotateClockwise = true;
        private readonly int _width;
        private readonly int _height;

        public BallPhysics(Map map, Vector2 position, Vector2 velocity, int width, int height)
        {
            _map = map;
            _position = position;
            _velocity = velocity;
            _width = width;
            _height = height;
        }

        public Vector2 Position { get { return _position; } }
        public Vector2 Velocity { get { return _velocity; } }
        public float Rotation { get { return _rotation; } }
        public void Update(GameTime gameTime)
        {
            var nextX = _position.X + _velocity.X;
            var nextXEdge = nextX + (_velocity.X > 0 ? _width : -_width)/2f;
            //Horizontal collision
            if (_map.HasTileAtCoordinates(nextXEdge, _position.Y))
            {
                _velocity.X = -_velocity.X;
                _rotateClockwise = newRotationIsClockwise(Axis.X, _velocity);
            }
            else
            {
                _position.X = nextX;
            }
            //Vertical collision
            var nextY = _position.Y + _velocity.Y;
            var nextYEdge = nextY + (_velocity.Y > 0 ? _height : -_height)/2f;
            if (_map.HasTileAtCoordinates(_position.X, nextYEdge))
            {
                _velocity.Y = -_velocity.Y;
                _rotateClockwise = newRotationIsClockwise(Axis.Y, _velocity);
            }
            else
            {
                _position.Y = nextY;
            }
            _rotation += 0.1f * (_rotateClockwise ? 1 : -1);
        }

        public enum Axis
        {
            X, Y
        }

        private static bool newRotationIsClockwise(Axis axisOfCollision, Vector2 velocity)
        {
            if (axisOfCollision == Axis.X)
            {
                return velocity.X > 0
                    ? velocity.Y <= 0
                    : velocity.Y > 0;
            }
            return velocity.X > 0
                ? velocity.Y > 0
                : velocity.Y <= 0;
        }
    }
}