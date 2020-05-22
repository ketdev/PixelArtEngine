using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace Engine.Component {
    class Model3D : IComponent {
        public Model Model { get; set; }
        public Texture2D Texture { get; set; }
        public AnimationPlayer Animation { get; set; }
    }
    class Shadow : IComponent {
        public Model Model { get; set; }
        public AnimationPlayer Animation { get; set; }
    }

    static class Model3DLoader {
        public static Model3D LoadModel(this ContentManager cm, string modelPath, string texturePath, string clipName = null) {
            var model = new Model3D {
                Model = cm.Load<Model>(modelPath),
            };
            Texture2D tex = null;
            if (texturePath != null) {
                tex = cm.Load<Texture2D>(texturePath);
                model.Texture = tex;
            }
            foreach (var mesh in model.Model.Meshes) {
                foreach (var effect in mesh.Effects) {

                    // Light models
                    if (effect is IEffectLights lights) {
                        //lights.EnableDefaultLighting();
                        lights.DirectionalLight0.Enabled = false;
                        lights.AmbientLightColor = Vector3.One;
                    }

                    // Static models
                    if (effect is BasicEffect basic) {
                        basic.TextureEnabled = true;
                        basic.Texture = tex;
                        basic.DiffuseColor = Vector3.One;
                        basic.PreferPerPixelLighting = true;
                    }

                    // Animated models
                    if (effect is SkinnedEffect skinned) {
                        skinned.Texture = tex;
                        skinned.DiffuseColor = Vector3.One;
                        skinned.PreferPerPixelLighting = true;

                        var weights = skinned.WeightsPerVertex;
                    }
                }
            }

            // load animation if any
            if (clipName != null && model.Model.Tag is SkinningData animation) {
                model.Animation = new AnimationPlayer(animation);
                AnimationClip clip = animation.AnimationClips[clipName];
                model.Animation.StartClip(clip);
            }

            return model;
        }
    }
}
