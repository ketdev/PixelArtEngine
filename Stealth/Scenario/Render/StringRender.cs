using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;

namespace Stealth.Scenario.Render {
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.Overlay)]
    class StringRender : EntityComponentProcessingSystem<StringSprite, Transform2D> {        
        private ContentManager contentManager;
        private SpriteBatch spriteBatch;
        
        public override void LoadContent() {
            contentManager = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            var graphics = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);
        }
        protected override void Begin() {
            spriteBatch.Begin();
        }
        protected override void End() {
            spriteBatch.End();
        }
        public override void Process(Entity entity, StringSprite sprite, Transform2D transform) {
            spriteBatch.DrawString(sprite.spriteFont, sprite.text, transform.position, sprite.color, 
                transform.rotation, transform.origin, transform.scale, sprite.effects, transform.layerDepth);
        }        
    }
}
