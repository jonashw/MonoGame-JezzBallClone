using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class DividerTextures
    {
        public readonly Texture2D Middle;
        public readonly Texture2D Positive;
        public readonly Texture2D Negative;

        public DividerTextures(Texture2D middle, Texture2D positive, Texture2D negative)
        {
            Middle = middle;
            Positive = positive;
            Negative = negative;
        }
    }
}