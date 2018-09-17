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
        private SpriteBatch spriteBatch;
        private Scene scene;

        public override void LoadContent() {
            scene = Scene.Current();
            spriteBatch = new SpriteBatch(scene.GraphicsDevice);
        }
        protected override void Begin() {
            scene.SetOutput();
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
