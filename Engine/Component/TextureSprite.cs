using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Component {
    struct TextureSprite : IComponent {
        public Texture2D texture;
        public Rectangle? clipping;
        public Color color;
        public SpriteEffects effects;
    }
}
