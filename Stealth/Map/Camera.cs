using Artemis.Interface;

namespace Stealth.Map {
    // A view contains the needed data in order 
    // to place an visible object on the map relative to the viewer
    class Camera : IComponent {
        public Transform3D Transform { get; set; }
        public Projection Projection { get; set; }        
    }
}
