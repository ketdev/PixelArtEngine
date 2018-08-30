using Artemis.Interface;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using SkinnedModel;

namespace Stealth.Map {
    class Unit : IComponent {
        public Model Model { get; set; }
        public AnimationPlayer Animation { get; set; }
    }
    
    static class UnitLoader {
        public static Unit LoadModel(this ContentManager cm, string modelPath, string texturePath, string clipName = null) {
            var unit = new Unit {
                Model = cm.Load<Model>(modelPath)
            };
            var tex = cm.Load<Texture2D>(texturePath);
            foreach (var mesh in unit.Model.Meshes) {
                foreach (var effect in mesh.Effects) {

                    // Light models
                    if(effect is IEffectLights lights) {
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
                    }
                }
            }

            // load animation if any
            if (clipName != null && unit.Model.Tag is SkinningData animation) {
                unit.Animation = new AnimationPlayer(animation);
                AnimationClip clip = animation.AnimationClips[clipName];
                unit.Animation.StartClip(clip);
            }

            return unit;
        }
    }
}
