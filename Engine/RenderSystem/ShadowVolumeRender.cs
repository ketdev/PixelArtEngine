using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Engine.Component;
using Engine.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Engine.RenderSystem {

    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.ShadowVolumeRender)]
    class ShadowVolumeRender : EntityComponentProcessingSystem<Shadow, Transform3D> {
        private Scene scene;
        private ModelEffect modelEffect;
        private DepthStencilState depthStencilState;
        private RasterizerState rasterizerState;

        public override void LoadContent() {
            scene = Scene.Current();
            modelEffect = new ModelEffect {
                IsShadow = true
            };

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
            // TODO: optimize matrix multiplication, do once per render for camera

            // compute matrices for camera
            var world = transform.WorldMatrix();
            var view = scene.Camera.Transform.ViewMatrix();
            var proj = scene.Camera.Projection.Matrix();
            modelEffect.WorldViewProjection = world * view * proj;

            // TODO: update animation once, in separate system?

            // update animation if any
            Matrix[] bones = null;
            modelEffect.IsSkinned = false;
            if (shadow.Animation != null) {
                shadow.Animation.Update(
                    new TimeSpan(scene.World.Delta), true, Matrix.Identity);
                bones = shadow.Animation.GetSkinTransforms();
                if (bones != null) {
                    modelEffect.IsSkinned = true;
                    modelEffect.SetBoneTransforms(bones);
                }
            }

            foreach (var mesh in shadow.Model.Meshes) {
                foreach (var part in mesh.MeshParts) {
                    if (part.Effect is SkinnedEffect skinned) {
                        modelEffect.WeightsPerVertex = skinned.WeightsPerVertex;
                    }
                    part.Effect = modelEffect;
                }
                mesh.Draw();
            }
        }
    }

}
