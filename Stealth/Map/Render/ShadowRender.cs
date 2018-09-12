using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;

namespace Stealth.Map.Render {
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.Shadows)]
    class ShadowRender : EntityComponentProcessingSystem<Unit, Transform3D> {
        private ContentManager c;
        private GraphicsDeviceManager g;
        private RenderData r;

        private Effect shadowShader;

        public override void LoadContent() {
            c = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            g = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            r = BlackBoard.GetEntry<RenderData>(Settings.RenderData);
            shadowShader = c.Load<Effect>("shaders\\shadow");
        }

        protected override void Begin() {
            r.SetShadowMap();
            r.UpdateLightDepthVPT();
        }

        public override void Process(Entity entity, Unit unit, Transform3D transform) {
            // compute matrices for shadow
            var worldMat = transform.WorldMatrix();
            var wvp = worldMat * r.LightVP;

            // render mesh to shadow map
            shadowShader.Parameters["wvp"].SetValue(wvp);
            foreach (var mesh in unit.Model.Meshes) {
                foreach (var part in mesh.MeshParts) {
                    part.Effect = shadowShader;
                }
                mesh.Draw();
            }
        }    
        
    }
}
