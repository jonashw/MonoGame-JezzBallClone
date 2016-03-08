using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class StringDrawer
    {
        private readonly SpriteFont _font;

        public StringDrawer(SpriteFont font)
        {
            _font = font;
        }

        public void Draw(SpriteBatch spriteBatch, string text, Vector2? position = null, Color? color = null)
        {
            spriteBatch.DrawString(_font, text, position ?? new Vector2(0,0), color ?? Color.Black);
        }
    }
}
