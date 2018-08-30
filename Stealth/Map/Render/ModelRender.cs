using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;
using System;

namespace Stealth.Map.Render {        
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.Map)]
    class ModelRender : EntityComponentProcessingSystem<Unit, Transform3D, Camera> {
        private ContentManager contentManager;
        private GraphicsDeviceManager graphicsManager;

        public override void LoadContent() {
            contentManager = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            graphicsManager = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
        }
        protected override void Begin() {
            // Enable depth buffer for all models
            graphicsManager.GraphicsDevice.DepthStencilState = new DepthStencilState() {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
                DepthBufferFunction = CompareFunction.LessEqual
            };
        }
        public override void Process(Entity entity, Unit unit, Transform3D transform, Camera camera) {
            // compute once
            var worldMat = transform.WorldMatrix();
            var viewMat = camera.Transform.ViewMatrix();
            var projMat = camera.Projection.Matrix();

            // update animation if any
            Matrix[] bones = null;
            if (unit.Animation != null) {
                unit.Animation.Update(
                    new TimeSpan(EntityWorld.Delta), true, Matrix.Identity);
                bones = unit.Animation.GetSkinTransforms();
            }

            // assign for all
            foreach (var mesh in unit.Model.Meshes) {
                // Apply matrices
                foreach (IEffectMatrices effect in mesh.Effects) {
                    effect.World = worldMat;
                    effect.View = viewMat;
                    effect.Projection = projMat;

                    // update animation if any
                    if (bones != null && effect is SkinnedEffect skinned) {
                        skinned.SetBoneTransforms(bones);
                    }
                }
                mesh.Draw();
            }
        }        
    }

}
