using Artemis.Interface;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Stealth.Scenario {
    struct Scenario : IComponent {
        public String Name { get; set; }
        public Color Background { get; set; }
    }
}
