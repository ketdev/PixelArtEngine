using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.RenderSystem {
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.ShadowVolumePass)]
    class ShadowVolumePass : EntitySystem {
        private static readonly string EffectResource = "Shaders\\shadow_pass";

        private Scene scene;
        private Effect effect;
        private DepthStencilState shadowPosteffect;

        public override void LoadContent() {
            scene = Scene.Current();
            effect = scene.Content.Load<Effect>(EffectResource);
            shadowPosteffect = new DepthStencilState() {
                DepthBufferEnable = false,
                StencilEnable = true,
                StencilFunction = CompareFunction.NotEqual,
                ReferenceStencil = 0,
                StencilMask = 0xff,
            };
        }

        public override void Process() {
            // setup here, not in Begin(), it isn't called for EntitySystems
            scene.SetOutput(false);
            scene.GraphicsDevice.DepthStencilState = shadowPosteffect;
            scene.DrawFullscreen(effect);
        }
    }

}
