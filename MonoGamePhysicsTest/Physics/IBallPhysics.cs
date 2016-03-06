using Microsoft.Xna.Framework;

namespace MonoGamePhysicsTest.Physics
{
    /// <summary>
    /// This interface is mostly read-only.  It is meant to be used by the Ball game entity.
    /// Any mutation operations should be semantic and respectful of the Physics system as a whole.
    /// </summary>
    public interface IBallPhysics
    {
        Vector2 Position { get; }
        float Rotation { get; }
        void Update(GameTime gameTime);
    }
}