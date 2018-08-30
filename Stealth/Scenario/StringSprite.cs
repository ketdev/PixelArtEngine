using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Stealth.Scenario {
    struct StringSprite : IComponent {
        public SpriteFont spriteFont;
        public string text;
        public Color color;
        public SpriteEffects effects;
    }
}
