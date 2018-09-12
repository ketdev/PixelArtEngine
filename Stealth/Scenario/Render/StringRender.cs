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
        private ContentManager c;
        private SpriteBatch spriteBatch;
        private RenderData r;

        public override void LoadContent() {
            c = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            r = BlackBoard.GetEntry<RenderData>(Settings.RenderData);
            var g = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            spriteBatch = new SpriteBatch(g.GraphicsDevice);
        }
        protected override void Begin() {
            r.SetOutput();
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
