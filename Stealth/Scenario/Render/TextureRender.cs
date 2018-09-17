using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
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
    class TextureRender : EntityComponentProcessingSystem<TextureSprite, Transform2D> {
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
        public override void Process(Entity entity, TextureSprite sprite, Transform2D transform) {
            spriteBatch.Draw(sprite.texture, transform.position, sprite.clipping, sprite.color,
                transform.rotation, transform.origin, transform.scale, sprite.effects, transform.layerDepth);
        }
    }
}
