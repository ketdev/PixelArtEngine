using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Engine.Component;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.RenderSystem {
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.Overlay)]
    class UITextureRender : EntityComponentProcessingSystem<TextureSprite, Transform2D> {
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
