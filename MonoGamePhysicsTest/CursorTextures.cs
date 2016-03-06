using System;
using Microsoft.Xna.Framework.Graphics;

namespace MonoGamePhysicsTest
{
    public class CursorTextures
    {
        public readonly Texture2D Up;
        public readonly Texture2D Down;
        public readonly Texture2D Left;
        public readonly Texture2D Right;
        public readonly Texture2D Horizontal;
        public readonly Texture2D Vertical;
        public readonly Texture2D Disabled;

        public CursorTextures(Texture2D up, Texture2D down, Texture2D left, Texture2D right, Texture2D horizontal, Texture2D vertical, Texture2D disabled)
        {
            Up = up;
            Down = down;
            Left = left;
            Right = right;
            Horizontal = horizontal;
            Vertical = vertical;
            Disabled = disabled;
        }

        public Texture2D Get(PlayDirection direction)
        {
            switch (direction)
            {
                case PlayDirection.Up: return Up;
                case PlayDirection.Down: return Down;
                case PlayDirection.Left: return Left;
                case PlayDirection.Right: return Right;
                case PlayDirection.Horizontal: return Horizontal;
                case PlayDirection.Vertical: return Vertical;
            }
            throw new ArgumentException("Unable to return a cursor sprite for PlayDirection: " + direction);
        }
    }
}