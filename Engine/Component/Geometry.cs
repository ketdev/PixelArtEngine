using Artemis.Interface;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Component {
    class Geometry<T> : IComponent where T : IVertexType {
        public T[] Vertices { get; set; }
        public Effect Effect { get; set; }
        public PrimitiveType PrimitiveType { get; set; }

        public Geometry() {
            Vertices = null;
            Effect = null;
            PrimitiveType = PrimitiveType.TriangleList;
        }
    }
}
