using Artemis.Interface;
using Microsoft.Xna.Framework;

namespace Engine.Component {
    // A view contains the needed data in order 
    // to place an visible object on the map relative to the viewer
    class Projection : IComponent {
        public float AspectRatio { get; set; }
        public float FieldOfView { get; set; }
        public float NearClipPlane { get; set; }
        public float FarClipPlane { get; set; }

        public Matrix Matrix() {
            //return Microsoft.Xna.Framework.Matrix.CreatePerspectiveFieldOfView(
            //    FieldOfView, AspectRatio, NearClipPlane, FarClipPlane);

            float yScale = 1.0f / (float)System.Math.Tan(FieldOfView / 2);
            float xScale = yScale / AspectRatio;
            float nearmfar = NearClipPlane - FarClipPlane;

            Matrix result;
            result.M11 = xScale;
            result.M12 = 0.0f;
            result.M13 = 0.0f;
            result.M14 = 0.0f;

            result.M21 = 0.0f;
            result.M22 = yScale;
            result.M23 = 0.0f;
            result.M24 = 0.0f;

            result.M31 = 0.0f;
            result.M32 = 0.0f;
            result.M33 = (FarClipPlane + NearClipPlane) / nearmfar;
            result.M34 = -1.0f;

            result.M41 = 0.0f;
            result.M42 = 0.0f;
            result.M43 = 2 * FarClipPlane * NearClipPlane / nearmfar;
            result.M44 = 0.0f;
            return result;
        }
    }
}
