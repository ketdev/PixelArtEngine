using Artemis;
using Artemis.Attributes;
using Artemis.Interface;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;

namespace Stealth.Core.Render {

    class Skybox : IComponent {
        public Texture2D Texture { get; set; }
    }

    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.SkyboxRender)]
    class SkyboxRender : EntityComponentProcessingSystem<Skybox> {
        private Scene scene;
        
        private readonly float size = 5.0f;
        private VertexPosition[] box;
        private Effect effect;

        public override void LoadContent() {
            scene = Scene.Current();

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
            effect = scene.Content.Load<Effect>("shaders\\skybox");
        }
        protected override void Begin() {
            scene.SetOutput(false);
        }

        public override void Process(Entity entity, Skybox skybox) {
            var worldMat = Matrix.CreateTranslation(scene.Camera.Transform.Position);
            var viewMat = scene.Camera.Transform.ViewMatrix();
            var projMat = scene.Camera.Projection.Matrix();            
            var wvp = worldMat * viewMat * projMat;
            
            effect.Parameters["texure"].SetValue(skybox.Texture);
            effect.Parameters["wvp"].SetValue(wvp);
            effect.Parameters["world"].SetValue(worldMat);
            effect.Parameters["cameraPosition"].SetValue(scene.Camera.Transform.Position);
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                scene.GraphicsDevice.DrawUserPrimitives(
                    PrimitiveType.TriangleStrip, box, 0, 13);
            }            
        }
        
    }

}
