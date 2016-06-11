using Microsoft.Xna.Framework;

namespace Welt.MonoGame.Extended.Collisions
{
    public class CollisionInfo
    {
        public ICollidable Other { get; internal set; }
        public Vector2 PenetrationVector { get; internal set; }
    }
}