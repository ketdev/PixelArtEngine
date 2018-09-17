using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;
using Stealth.Map;

namespace Stealth.Core.Effects {
    class ModelEffect : Effect {
        private static readonly string EffectResource = "shaders\\model";

        public const int MaxBones = 72;

        private EffectParameter textureParam;
        private EffectParameter worldViewProjParam;
        private EffectParameter bonesParam;

        private int _shaderIndex = -1;
        private int weightsPerVertex = 4;
        private bool isSkinned = false;
        private bool dirtyShaderIndex = true;

        public Matrix WorldViewProjection {
            get { return worldViewProjParam.GetValueMatrix(); }
            set { worldViewProjParam.SetValue(value); }
        }
        
        public Texture2D Texture {
            get { return textureParam.GetValueTexture2D(); }
            set { textureParam.SetValue(value); }
        }
        
        public bool Skinned {
            get { return isSkinned; }
            set {
                if (value != isSkinned)
                    dirtyShaderIndex = true;
                isSkinned = value;
            }
        }

        public int WeightsPerVertex {
            get { return weightsPerVertex; }
            set {
                if ((value != 1) &&
                    (value != 2) &&
                    (value != 4)) {
                    throw new ArgumentOutOfRangeException("value");
                }
                weightsPerVertex = value;
                dirtyShaderIndex = true;
            }
        }
        
        public void SetBoneTransforms(Matrix[] boneTransforms) {
            if ((boneTransforms == null) || (boneTransforms.Length == 0))
                throw new ArgumentNullException("boneTransforms");
            if (boneTransforms.Length > MaxBones)
                throw new ArgumentException();
            bonesParam.SetValue(boneTransforms);
        }
        
        public Matrix[] GetBoneTransforms(int count) {
            if (count <= 0 || count > MaxBones)
                throw new ArgumentOutOfRangeException("count");
            Matrix[] bones = bonesParam.GetValueMatrixArray(count);
            // Convert matrices from 43 to 44 format.
            for (int i = 0; i < bones.Length; i++) {
                bones[i].M44 = 1;
            }
            return bones;
        }
        
        
        public ModelEffect() : base(LoadEffectResource(EffectResource)) {
            textureParam = Parameters["Texture"];
            worldViewProjParam = Parameters["WVP"];
            bonesParam = Parameters["Bones"];
            
            Matrix[] identityBones = new Matrix[MaxBones];

            for (int i = 0; i < MaxBones; i++) {
                identityBones[i] = Matrix.Identity;
            }

            SetBoneTransforms(identityBones);
        }

        private static Effect LoadEffectResource(string v) {
            return Scene.Current().Content.Load<Effect>(v);
        }

        protected override void OnApply() {
            if (dirtyShaderIndex) {
                dirtyShaderIndex = false;

                int index = 0;
                if (isSkinned) {
                    index = 1;
                    if (weightsPerVertex == 2) {
                        index = 2;
                    } else if (weightsPerVertex == 4) {
                        index = 3;
                    }
                }
                
                if (_shaderIndex != index) {
                    _shaderIndex = index;
                    CurrentTechnique = Techniques[_shaderIndex];
                }
            }
        }
        
    }
}
