using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace Stealth.Map {
    // A view contains the needed data in order 
    // to place an visible object on the map relative to the viewer
    class Projection : IComponent {
        public float AspectRatio { get; set; }
        public float FieldOfView { get; set; }
        public float NearClipPlane { get; set; }
        public float FarClipPlane { get; set; }

        public Matrix Matrix() {
            return Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(
                FieldOfView, AspectRatio, NearClipPlane, FarClipPlane);
        }
    }
}
