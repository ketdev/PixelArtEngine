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

        private Effect modelShader;
        private Effect borderShader;

        private RenderTarget2D modelOutput;
        private RenderTarget2D colorAux;
        private RenderTarget2D depthAux;

        private SpriteBatch spriteBatch;

        private Geometry<VertexPositionTexture> fullscreen;

        public override void LoadContent() {
            contentManager = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            graphicsManager = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);

            // Set a higher pixel size for whole scene
            var width = graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth / 3;
            var height = graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight / 3;

            // Load custom shaders
            modelShader = contentManager.Load<Effect>("shaders\\model");
            borderShader = contentManager.Load<Effect>("shaders\\border");            
            borderShader.Parameters["ScreenPixel"].SetValue(new Vector2(1.0f / width, 1.0f / height));

            // Create render targets
            modelOutput = new RenderTarget2D(
                graphicsManager.GraphicsDevice,
                width, height, false,
                graphicsManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            colorAux = new RenderTarget2D(
                graphicsManager.GraphicsDevice,
                width, height, false,
                graphicsManager.GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None, 0, RenderTargetUsage.DiscardContents);
            depthAux = new RenderTarget2D(
                graphicsManager.GraphicsDevice,
                width, height, false,
                SurfaceFormat.Single, // Highest quality single channel
                DepthFormat.None, 0, RenderTargetUsage.DiscardContents);


            // rendering geometry
            spriteBatch = new SpriteBatch(graphicsManager.GraphicsDevice);
                       
            fullscreen = new Geometry<VertexPositionTexture> {
                Vertices = new VertexPositionTexture[]{
                    new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                    new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                    new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                    new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
                },
                PrimitiveType = PrimitiveType.TriangleStrip
            };

        }
        protected override void Begin() {
            // Setup configuration
            graphicsManager.GraphicsDevice.SetRenderTargets(modelOutput, colorAux, depthAux);
            graphicsManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, Color.TransparentBlack, 1, 0);
            graphicsManager.GraphicsDevice.RasterizerState = new RasterizerState { CullMode = CullMode.CullCounterClockwiseFace };
            graphicsManager.GraphicsDevice.DepthStencilState = new DepthStencilState() {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
                DepthBufferFunction = CompareFunction.LessEqual
            };            
        }
        protected override void End() {
            // Drop the render target
            graphicsManager.GraphicsDevice.SetRenderTarget(modelOutput);
            graphicsManager.GraphicsDevice.Viewport = new Viewport(0, 0, modelOutput.Width, modelOutput.Height);
            
            // Post-effect, create pixel perfect borders
            fullscreen.Effect = borderShader;
            borderShader.Parameters["ColorTexture"].SetValue(colorAux);
            borderShader.Parameters["DepthTexture"].SetValue(depthAux);
            foreach (var pass in fullscreen.Effect.CurrentTechnique.Passes) {
                pass.Apply();
                graphicsManager.GraphicsDevice.DrawUserPrimitives(fullscreen.PrimitiveType, fullscreen.Vertices, 0, 2);
            }

            // Draw everything to the scene
            // TODO: more efficient, use fullscreen
            graphicsManager.GraphicsDevice.SetRenderTarget(null);
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend,
                        SamplerState.PointClamp, DepthStencilState.Default,
                        RasterizerState.CullNone);
            spriteBatch.Draw(modelOutput, new Rectangle(0, 0,
                graphicsManager.GraphicsDevice.PresentationParameters.BackBufferWidth,
                graphicsManager.GraphicsDevice.PresentationParameters.BackBufferHeight), Color.White);
            spriteBatch.End();
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


            foreach (var mesh in unit.Model.Meshes) {
                foreach (var part in mesh.MeshParts) {
                    var wvp = worldMat * viewMat * projMat;
            
                    // TODO: set bones

                    // configure shader
                    part.Effect = modelShader;
                    modelShader.Parameters["wvp"].SetValue(wvp);
                    modelShader.Parameters["tex"].SetValue(unit.Texture);
                    modelShader.Parameters["id"].SetValue(unit.Border.ToVector3());
                }            
                mesh.Draw();
            }
            
        }
    }

}
