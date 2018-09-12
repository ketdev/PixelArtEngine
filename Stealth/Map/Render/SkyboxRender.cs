using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;

namespace Stealth.Map.Render {

    class Skybox : IComponent {
        public Texture2D Texture { get; set; }
    }

    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.Skybox)]
    class SkyboxRender : EntityComponentProcessingSystem<Skybox> {
        private ContentManager c;
        private GraphicsDeviceManager g;
        private RenderData r;
        
        private readonly float size = 5.0f;
        private VertexPosition[] box;
        private DepthStencilState dss;
        private Effect effect;

        public override void LoadContent() {
            c = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            g = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            r = BlackBoard.GetEntry<RenderData>(Settings.RenderData);

            box = new VertexPosition[] {
                new VertexPosition(new Vector3(-size, size, size)),     // invert winding [duplicated first]
                new VertexPosition(new Vector3(-size, size, size)),     // Front-top-left
                new VertexPosition(new Vector3(size, size, size)),      // Front-top-right
                new VertexPosition(new Vector3(-size, -size, size)),    // Front-bottom-left
                new VertexPosition(new Vector3(size, -size, size)),     // Front-bottom-right
                new VertexPosition(new Vector3(size, -size, -size)),    // Back-bottom-right
                new VertexPosition(new Vector3(size, size, size)),      // Front-top-right
                new VertexPosition(new Vector3(size, size, -size)),     // Back-top-right
                new VertexPosition(new Vector3(-size, size, size)),     // Front-top-left
                new VertexPosition(new Vector3(-size, size, -size)),    // Back-top-left
                new VertexPosition(new Vector3(-size, -size, size)),    // Front-bottom-left
                new VertexPosition(new Vector3(-size, -size, -size)),   // Back-bottom-left
                new VertexPosition(new Vector3(size, -size, -size)),    // Back-bottom-right
                new VertexPosition(new Vector3(-size, size, -size)),    // Back-top-left
                new VertexPosition(new Vector3(size, size, -size))      // Back-top-right
            };
            effect = c.Load<Effect>("shaders\\skybox");
        }
        protected override void Begin() {
            r.SetOutput(false);
        }

        public override void Process(Entity entity, Skybox skybox) {
            var worldMat = Matrix.CreateTranslation(r.Camera.Transform.Position);
            var viewMat = r.Camera.Transform.ViewMatrix();
            var projMat = r.Camera.Projection.Matrix();            
            var wvp = worldMat * viewMat * projMat;
            
            effect.Parameters["texure"].SetValue(skybox.Texture);
            effect.Parameters["wvp"].SetValue(wvp);
            effect.Parameters["world"].SetValue(worldMat);
            effect.Parameters["cameraPosition"].SetValue(r.Camera.Transform.Position);
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                g.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleStrip, box, 0, 13);
            }            
        }
        
    }

}
