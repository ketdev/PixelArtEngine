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
        Layer = (int)Settings.PriorityLayer.Models)]
    class ModelRender : EntityComponentProcessingSystem<Unit, Transform3D> {
        private ContentManager c;
        private GraphicsDeviceManager g;
        private RenderData r;

        private Effect modelShader;
        private Matrix lightVPT;
                        
        public override void LoadContent() {
            c = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            g = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            r = BlackBoard.GetEntry<RenderData>(Settings.RenderData);
            modelShader = c.Load<Effect>("shaders\\model");  
        }

        protected override void Begin() {
            r.SetGBuffer();
            
            // transforms -1,1 to UV coordinates 0,1 (and inverts y axis)
            var uvMatrix = new Matrix(
                0.5f, 0.0f, 0.0f, 0.0f,
                0.0f, -0.5f, 0.0f, 0.0f,
                0.0f, 0.0f, 1.0f, 0.0f,
                0.5f, 0.5f, 0.0f, 1.0f
                );
            lightVPT = r.LightVP * uvMatrix;            
            modelShader.Parameters["ShadowTexture"].SetValue(r.ShadowRT);            
        }

        public override void Process(Entity entity, Unit unit, Transform3D transform) {
            // TODO: update animation once, in separate system?
            
            // update animation if any
            Matrix[] bones = null;
            if (unit.Animation != null) {
                unit.Animation.Update(
                    new TimeSpan(EntityWorld.Delta), true, Matrix.Identity);
                bones = unit.Animation.GetSkinTransforms();
            }

            // compute matrices for camera
            var worldMat = transform.WorldMatrix();
            var viewMat = r.Camera.Transform.ViewMatrix();
            var projMat = r.Camera.Projection.Matrix();
            var wvp = worldMat * viewMat * projMat;

            modelShader.Parameters["WVP"].SetValue(wvp);
            modelShader.Parameters["Texture"].SetValue(unit.Texture);
            modelShader.Parameters["DepthWVPT"].SetValue(worldMat * lightVPT);

            foreach (var mesh in unit.Model.Meshes) {
                foreach (var part in mesh.MeshParts) {            
                    // TODO: set bones
                    part.Effect = modelShader;
                }            
                mesh.Draw();
            }
            
            //// assign for all
            //foreach (var mesh in unit.Model.Meshes) {
            //    // Apply matrices
            //    foreach (IEffectMatrices effect in mesh.Effects) {
            //        effect.World = worldMat;
            //        effect.View = viewMat;
            //        effect.Projection = projMat;
            //
            //        // update animation if any
            //        if (bones != null && effect is SkinnedEffect skinned) {
            //            skinned.SetBoneTransforms(bones);
            //        }
            //    }
            //    mesh.Draw();
            //}
        }
        

    }

}
