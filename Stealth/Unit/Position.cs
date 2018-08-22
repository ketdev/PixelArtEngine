using Artemis;
using Artemis.Attributes;
using Artemis.Interface;

namespace Stealth.Unit {

    //class Position : IComponent {  
    [Artemis.Attributes.ArtemisComponentPool(
        InitialSize = 5, 
        IsResizable = true, 
        ResizeSize = 20, 
        IsSupportMultiThread = false)]
    class Position : ComponentPoolable {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Position() {}
        public Position(int x, int y) {
            this.X = x;
            this.Y = y;
        }
    }
    
}
