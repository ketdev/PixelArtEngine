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
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.ModelRender)]
    class ModelRender : EntityComponentProcessingSystem<Model3D, Transform3D> {
        private Scene scene;
        private ModelEffect modelEffect;

        public override void LoadContent() {
            scene = Scene.Current();
            modelEffect = new ModelEffect();
        }
        protected override void Begin() {
            scene.SetGBuffer();
        }
        public override void Process(Entity entity, Model3D unit, Transform3D transform) {
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
            if (unit.Animation != null) {
                unit.Animation.Update(
                    new TimeSpan(scene.World.Delta), true, Matrix.Identity);
                bones = unit.Animation.GetSkinTransforms();
                if (bones != null) {
                    modelEffect.IsSkinned = true;
                    modelEffect.SetBoneTransforms(bones);
                }
            }

            if (unit.Texture != null) {
                modelEffect.Texture = unit.Texture;
            }

            foreach (var mesh in unit.Model.Meshes) {
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
