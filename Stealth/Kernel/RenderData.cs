using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Map;

namespace Stealth.Kernel {
    class RenderData {
        // graphics device reference
        private GraphicsDevice g;

        // Rendering configuration
        public int Width { get; private set; }
        public int Height { get; private set; }

        // Rendering targets
        public RenderTarget2D OutputRT { get; private set; }
        public RenderTarget2D BorderRT { get; private set; }
        public RenderTarget2D DepthRT { get; private set; }

        // camera [single camera supported]
        public Camera Camera { get; private set; }
        
        // shadow direction [single shadow supported]
        public RenderTarget2D ShadowRT { get; private set; }
        public Matrix LightVP { get; private set; }
        
        private const int shadowMapSize = 2048;
        private const int shadowPixelsPerUnit = 32;
        private readonly Vector3 tileOffset = new Vector3(0.5f, 0.5f, 0);
        private readonly Vector3 mapSize = new Vector3(
            shadowMapSize / shadowPixelsPerUnit,
            shadowMapSize / shadowPixelsPerUnit,
            sizeof(float));

        // fullscreen rendering
        private VertexPositionTexture[] fullscreen;
        private BasicEffect effect;

        // common config
        private DepthStencilState depth, noDepth;
        private RasterizerState ccwCull, cwCull;

        public RenderData(GraphicsDevice graphics, int pixelSize) {
            g = graphics;
            Width = g.PresentationParameters.BackBufferWidth / pixelSize;
            Height = g.PresentationParameters.BackBufferHeight / pixelSize;

            // Create G-Buffer
            OutputRT = new RenderTarget2D(
                g, Width, Height, false,
                g.PresentationParameters.BackBufferFormat,
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);
            BorderRT = new RenderTarget2D(
                g, Width, Height, false,
                g.PresentationParameters.BackBufferFormat,
                DepthFormat.None, 0, RenderTargetUsage.PreserveContents);
            DepthRT = new RenderTarget2D(
                g, Width, Height, false,
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

            // Create Shadow RT
            ShadowRT = new RenderTarget2D(
                g, shadowMapSize, shadowMapSize, false,
                //g.PresentationParameters.BackBufferFormat,
                SurfaceFormat.Single, // Highest quality single channel [NOT SUPPORTED on ANDROID]
                DepthFormat.Depth24, 0, RenderTargetUsage.PreserveContents);

            // Create required stuff for fullscreen rendering
            fullscreen = new VertexPositionTexture[]{
                new VertexPositionTexture(new Vector3(-1, -1, 0), new Vector2(0, 1)),
                new VertexPositionTexture(new Vector3(-1, 1, 0), new Vector2(0, 0)),
                new VertexPositionTexture(new Vector3(1, -1, 0), new Vector2(1, 1)),
                new VertexPositionTexture(new Vector3(1, 1, 0), new Vector2(1, 0))
            };
            effect = new BasicEffect(g) {
                TextureEnabled = true,
                Texture = OutputRT,
            };
            

            // set constant settings
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
            cwCull = new RasterizerState {
                CullMode = CullMode.CullClockwiseFace
            };

            g.RasterizerState = ccwCull;
            g.DepthStencilState = depth;
            g.BlendState = BlendState.AlphaBlend;
        }

        public void Clear() {
            SetShadowMap();
            g.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil,
                Color.White, 1.0f, 0);

            // Clear to white for depth buffer
            SetGBuffer();
            g.Clear(
                ClearOptions.Target | ClearOptions.DepthBuffer | ClearOptions.Stencil, 
                Color.White, 1.0f, 0);
        }

        public void SetOutput(bool depthTest = true) {
            g.SetRenderTargets(OutputRT);
            g.Viewport = new Viewport(0, 0, Width, Height);
            g.RasterizerState = ccwCull;
            if (depthTest)
                g.DepthStencilState = depth;
            else
                g.DepthStencilState = noDepth;
        }
        public void SetGBuffer() {
            g.SetRenderTargets(OutputRT, BorderRT, DepthRT);
            g.Viewport = new Viewport(0, 0, Width, Height);
            g.DepthStencilState = depth;
            g.RasterizerState = ccwCull;
        }
        public void SetShadowMap() {
            g.SetRenderTargets(ShadowRT);
            g.Viewport = new Viewport(0, 0, ShadowRT.Width, ShadowRT.Height);
            g.DepthStencilState = depth;
            g.RasterizerState = ccwCull;
        }

        public void Display() {
            var w = g.PresentationParameters.BackBufferWidth;
            var h = g.PresentationParameters.BackBufferHeight;
            g.SetRenderTarget(null);
            g.Viewport = new Viewport(0, 0, w, h);
            DrawFullscreen(effect);

            //// Draw G-Buffer
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
            //effect.Texture = ShadowRT;
            //g.Viewport = new Viewport(w / 2, h / 2, w / 2, h / 2);
            //DrawFullscreen(effect);
        }

        public void DrawFullscreen(Effect effect) {
            g.SamplerStates[0] = SamplerState.PointClamp;
            foreach (var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                g.DrawUserPrimitives(PrimitiveType.TriangleStrip, fullscreen, 0, 2);
            }
        }

        public void UpdateLightDepthVPT() {
            var shadowChunk = new Vector3(0, 0, 0);
            
            var lightView = Matrix.CreateTranslation(
                shadowChunk * mapSize + mapSize * -0.5f + tileOffset);            
            var lightProjection = Matrix.CreateOrthographic(
                mapSize.X, mapSize.Y, -mapSize.Z, mapSize.Z);

            LightVP = lightView * lightProjection;
        }

    }
}
