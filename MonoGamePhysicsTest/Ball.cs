using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGamePhysicsTest.Physics;

namespace MonoGamePhysicsTest
{
    public class Ball
    {
        private readonly Texture2D _sprite;
        private readonly Vector2 _origin;
        private readonly IBallPhysics _physics;

        public Ball(Texture2D sprite, IBallPhysics physics)
        {
            _sprite = sprite;
            _physics = physics;
            _origin = new Vector2(_sprite.Width/2f, _sprite.Height/2f);
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(
                _sprite,
                _physics.Position,
                origin: _origin,
                rotation: _physics.Rotation);
        }

        public void Update(GameTime gameTime)
        {
            _physics.Update(gameTime);
        }
    }
}