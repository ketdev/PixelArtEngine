using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;
using Stealth.Map;

namespace Stealth.Core.Render {

    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.ShadowVolumeRender)]
    class ShadowVolumeRender : EntityComponentProcessingSystem<Shadow, Transform3D> {
        private Scene scene;
        private Effect effect;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;

        public override void LoadContent() {
            scene = Scene.Current();          
            effect = scene.Content.Load<Effect>("shaders\\shadow");
            
            depthStencilState = new DepthStencilState() {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = false,
                DepthBufferFunction = CompareFunction.LessEqual,
                StencilEnable = true,
                TwoSidedStencilMode = true,
                StencilFunction = CompareFunction.Always,
                ReferenceStencil = 0,
                StencilMask = 0xff,
                CounterClockwiseStencilFail = StencilOperation.Keep,
                CounterClockwiseStencilDepthBufferFail = StencilOperation.Increment,
                CounterClockwiseStencilPass = StencilOperation.Keep,
                StencilFail = StencilOperation.Keep,
                StencilDepthBufferFail = StencilOperation.Decrement,
                StencilPass = StencilOperation.Keep
            };
            rasterizerState = new RasterizerState {
                CullMode = CullMode.None
            };
        }

        protected override void Begin() {
            scene.SetGBuffer();
            scene.GraphicsDevice.RasterizerState = rasterizerState;
            scene.GraphicsDevice.DepthStencilState = depthStencilState;
        }

        public override void Process(Entity entity, Shadow shadow, Transform3D transform) {
            scene.DrawMesh(effect, transform, shadow.Model, null, shadow.Animation);            
        }
    }

}
