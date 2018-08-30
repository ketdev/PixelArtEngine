using Artemis;
using Artemis.Attributes;
using Microsoft.Xna.Framework;

namespace Stealth.Map {
    [ArtemisComponentPool()]
    class Transform3D : ComponentPoolable {
        private Vector3 position, forward, upward, scale;

        public Vector3 Position {
            get { return position; }
            set { position = value; }
        }
        public Vector3 Forward {
            get { return forward; }
            set { forward = value; forward.Normalize(); }
        }
        public Vector3 Upward {
            get { return upward; }
            set { upward = value; upward.Normalize(); }
        }
        public Vector3 Scale {
            get { return scale; }
            set { scale = value; }
        }
        // Alternative way of defining for forward direction
        public Vector3 LookAt {
            get { return Position + Forward; }
            set { Forward = value - Position; }
        }

        public Transform3D() {
            forward = Vector3.UnitY;
            upward = Vector3.UnitZ;
            scale = Vector3.One;
        }

        // Create matrices
        public Matrix ViewMatrix() {
            var inverseScale = new Vector3(1.0f / Scale.X, 1.0f / Scale.Y, 1.0f / Scale.Z);
            return Matrix.CreateLookAt(position, LookAt, upward) * Matrix.CreateScale(inverseScale);
        }
        public Matrix WorldMatrix() {
            // Recalculate up vector
            Vector3.Cross(ref forward, ref upward, out Vector3 right);
            Vector3.Cross(ref right, ref forward, out Vector3 up);

            Matrix result;
            result.M11 = right.X * scale.X;
            result.M12 = right.Y * scale.X;
            result.M13 = right.Z * scale.X;
            result.M14 = 0.0f;

            result.M21 = forward.X * scale.Y;
            result.M22 = forward.Y * scale.Y;
            result.M23 = forward.Z * scale.Y;
            result.M24 = 0.0f;

            result.M31 = up.X * scale.Y;
            result.M32 = up.Y * scale.Y;
            result.M33 = up.Z * scale.Y;
            result.M34 = 0.0f;

            result.M41 = position.X;
            result.M42 = position.Y;
            result.M43 = position.Z;
            result.M44 = 1.0f;
            return result;
        }
        
    }    
}
