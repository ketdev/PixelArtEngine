using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.RenderSystem {
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.BorderPass)]
    class BorderPass : EntitySystem {
        private static readonly string EffectResource = "Shaders\\border";

        private Scene scene;
        private Effect effect;

        public override void LoadContent() {
            scene = Scene.Current();
            effect = scene.Content.Load<Effect>(EffectResource);
            effect.Parameters["BorderTexture"].SetValue(scene.BorderRT);
            effect.Parameters["DepthTexture"].SetValue(scene.DepthRT);
            effect.Parameters["ScreenPixel"].SetValue(
                new Vector2(1.0f / scene.Width, 1.0f / scene.Height)
            );
        }

        public override void Process() {
            // setup here, not in Begin(), it isn't called for EntitySystems
            scene.SetOutput(false);
            scene.DrawFullscreen(effect);
        }
    }

}
