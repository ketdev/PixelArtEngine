using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace Engine.Component {
    struct Transform2D : IComponent {
        public Vector2 position;
        public float rotation;
        public Vector2 origin;
        public Vector2 scale;
        public float layerDepth; // between 0.0 and 1.0
    }
}
