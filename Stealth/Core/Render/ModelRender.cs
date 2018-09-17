using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Core.Effects;
using Stealth.Kernel;
using Stealth.Map;
using System;

namespace Stealth.Core.Render {
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.ModelRender)]
    class ModelRender : EntityComponentProcessingSystem<Unit, Transform3D> {
        private Scene scene;
        private Effect effect;
        private ModelEffect modelEffect;

        public override void LoadContent() {
            scene = Scene.Current();
            effect = scene.Content.Load<Effect>("shaders\\model");
            modelEffect = new ModelEffect();
        }
        protected override void Begin() {
            scene.SetGBuffer();
        }
        public override void Process(Entity entity, Unit unit, Transform3D transform) {
            //scene.DrawMesh(modelEffect, transform, unit.Model, unit.Texture, unit.Animation);

            // TODO: optimize matrix multiplication, do once per render for camera

            // compute matrices for camera
            var world = transform.WorldMatrix();
            var view = scene.Camera.Transform.ViewMatrix();
            var proj = scene.Camera.Projection.Matrix();
            modelEffect.WorldViewProjection = world * view * proj;

            // TODO: update animation once, in separate system?

            // update animation if any
            Matrix[] bones = null;
            modelEffect.Skinned = false;
            if (unit.Animation != null) {
                unit.Animation.Update(
                    new TimeSpan(scene.World.Delta), true, Matrix.Identity);
                bones = unit.Animation.GetSkinTransforms();
                if (bones != null) {
                    modelEffect.Skinned = true;
                    modelEffect.SetBoneTransforms(bones);
                }
            }

            if (unit.Texture != null) {
                modelEffect.Texture = unit.Texture;
            }
            
            foreach (var mesh in unit.Model.Meshes) {
                foreach (var part in mesh.MeshParts) {
                    if(part.Effect is SkinnedEffect skinned) {
                        modelEffect.WeightsPerVertex = skinned.WeightsPerVertex;
                    }
                    part.Effect = modelEffect;
                }
                mesh.Draw();
            }
        }
    }

}
