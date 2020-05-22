using Engine.Component;

namespace Engine {
    class Camera {
        public Transform3D Transform { get; set; }
        public Projection Projection { get; set; }
    }
}
