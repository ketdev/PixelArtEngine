using Artemis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Stealth.Map;
using Artemis.System;
using Artemis.Blackboard;
using SkinnedModel;
using System;

namespace Stealth.Kernel {
    class Scene {
        public ContentManager Content { get; private set; }
        public EntityWorld World { get; private set; }
        public GraphicsDeviceManager GraphicsManager { get; private set; }
        public GraphicsDevice GraphicsDevice {
            get {
                return GraphicsManager.GraphicsDevice;
            }
        }

        // Rendering configuration
        public const int PixelSize = 3;
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Rendering targets
        public RenderTarget2D OutputRT { get; private set; }
        public RenderTarget2D BorderRT { get; private set; }
        public RenderTarget2D DepthRT { get; private set; }

        // camera [single camera supported]
        public Camera Camera { get; private set; }
        
        // fullscreen rendering
        private VertexPositionTexture[] fullscreen;
        private BasicEffect effect;

        // common config
        private const string blackboardId = "Scene";
        private Viewport outputViewport, displayViewport;
        private DepthStencilState depth, noDepth;
        private RasterizerState ccwCull;

        public Scene(GraphicsDeviceManager graphics, ContentManager content) {
            GraphicsManager = graphics;
            Content = content;

            Width = GraphicsDevice.PresentationParameters.BackBufferWidth / PixelSize;
            Height = GraphicsDevice.PresentationParameters.BackBufferHeight / PixelSize;

            // Create G-Buffer
            OutputRT = new RenderTarget2D(
                GraphicsDevice, Width, Height, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24Stencil8, 0, RenderTargetUsage.PreserveContents);
            BorderRT = new RenderTarget2D(
                GraphicsDevice, Width, Height, false,
                GraphicsDevice.PresentationParameters.BackBufferFormat,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            DepthRT = new RenderTarget2D(
                GraphicsDevice, Width, Height, false,
                //graphics.PresentationParameters.BackBufferFormat,
                SurfaceFormat.Single, // Highest quality single channel [NOT SUPPORTED on ANDROID]
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);

            // Create shared camera object
            Camera = new Camera {
                Transform = new Transform3D {
                    Position = Vector3.Zero,
                    LookAt = Vector3.UnitY,
                    Upward = Vector3.UnitZ
                },
                Projection = new Projection {
                    AspectRatio = Width / (float)Height,
                    FieldOfView = MathHelper.PiOver4,
                    NearClipPlane = 0.1f,
                    FarClipPlane = 1000
                }
            };
            
            // Create required stuff for fullscreen rendering
            fullscreen = new VertexPositionTexture[]{
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };
            effect = new BasicEffect(GraphicsManager.GraphicsDevice) {
                TextureEnabled = true,
                Texture = OutputRT,
            };


            // set constant settings
            outputViewport = new Viewport(0, 0, Width, Height);
            displayViewport = new Viewport(0, 0, 
                GraphicsDevice.PresentationParameters.BackBufferWidth,
                GraphicsDevice.PresentationParameters.BackBufferHeight);

            depth = new DepthStencilState() {
                DepthBufferEnable = true,
                DepthBufferWriteEnable = true,
                DepthBufferFunction = CompareFunction.LessEqual
            };            
            noDepth = new DepthStencilState() {
                DepthBufferEnable = false
            };
            
            ccwCull = new RasterizerState {
                CullMode = CullMode.CullCounterClockwiseFace
            };

            GraphicsDevice.RasterizerState = ccwCull;
            GraphicsDevice.DepthStencilState = depth;
            GraphicsDevice.BlendState = BlendState.AlphaBlend;

            // Initialize systems after all graphics are initialized
            World = new EntityWorld();
            EntitySystem.BlackBoard.SetEntry(blackboardId, this);
            World.InitializeAll(System.Reflection.Assembly.GetExecutingAssembly());
        }

        static public Scene Current() {
            return EntitySystem.BlackBoard.GetEntry<Scene>(blackboardId);
        }

        public void Update() {
            World.Update();
        }

        public void Draw() {
            // Clear all targets to white for depth and shadow buffers
            GraphicsDevice.SetRenderTargets(OutputRT, BorderRT, DepthRT);
            GraphicsDevice.Viewport = outputViewport;
            GraphicsDevice.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
                Color.White, 1.0f, 0);
            GraphicsDevice.RasterizerState = ccwCull;
            
            // Run rendering systems
            World.Draw();

            // Output to screen
            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Viewport = displayViewport;
            DrawFullscreen(effect);

            //// Draw G-Buffer
            //var w = displayViewport.Width;
            //var h = displayViewport.Height;
            //
            //effect.Texture = OutputRT;
            //g.Viewport = new Viewport(0, 0, w / 2, h / 2);
            //DrawFullscreen(effect);
            //
            //effect.Texture = BorderRT;
            //g.Viewport = new Viewport(w / 2, 0, w / 2, h / 2);
            //DrawFullscreen(effect);
            //
            //effect.Texture = DepthRT;
            //g.Viewport = new Viewport(0, h / 2, w / 2, h / 2);
            //DrawFullscreen(effect);
            //
            ////effect.Texture = ShadowRT;
            ////g.Viewport = new Viewport(w / 2, h / 2, w / 2, h / 2);
            ////DrawFullscreen(effect);
        }



        // --

        public void SetGBuffer() {
            GraphicsDevice.SetRenderTargets(OutputRT, BorderRT, DepthRT);
            GraphicsDevice.Viewport = outputViewport;
            GraphicsDevice.RasterizerState = ccwCull;
            GraphicsDevice.DepthStencilState = depth;
        }
        
        public void SetOutput(bool depthTest = true) {
            GraphicsDevice.SetRenderTargets(OutputRT);
            GraphicsDevice.Viewport = outputViewport;
            GraphicsDevice.RasterizerState = ccwCull;
            if (depthTest)
                GraphicsDevice.DepthStencilState = depth;
            else
                GraphicsDevice.DepthStencilState = noDepth;
        }
        
        public void DrawFullscreen(Effect effect) {
            GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, fullscreen, 0, 2);
            }
        }
        
        public void DrawMesh(Effect effect, Transform3D transform, Model model, Texture2D texture, AnimationPlayer animation = null) {
            // TODO: update animation once, in separate system?

            // update animation if any
            Matrix[] bones = null;
            if (animation != null) {
                animation.Update(
                    new TimeSpan(World.Delta), true, Matrix.Identity);
                bones = animation.GetSkinTransforms();
            }

            // compute matrices for camera
            var worldMat = transform.WorldMatrix();
            var viewMat = Camera.Transform.ViewMatrix();
            var projMat = Camera.Projection.Matrix();
            var wvp = worldMat * viewMat * projMat;

            effect.Parameters["WVP"].SetValue(wvp);
            if (texture != null) {
                effect.Parameters["Texture"].SetValue(texture);
            }

            foreach (var mesh in model.Meshes) {
                foreach (var part in mesh.MeshParts) {
                    // TODO: set bones
                    part.Effect = effect;
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
