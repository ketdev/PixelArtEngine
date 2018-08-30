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
        Layer = (int)Settings.PriorityLayer.Map)]
    class GeometryRender : EntityProcessingSystem {
        private ContentManager contentManager;
        private GraphicsDeviceManager graphics;

        public GeometryRender() : base(
            Aspect.All(typeof(Transform3D), typeof(Camera)).
            GetOne(typeof(Geometry<VertexPosition>), 
                typeof(Geometry<VertexPositionColor>), 
                typeof(Geometry<VertexPositionColorTexture>), 
                typeof(Geometry<VertexPositionNormalTexture>),
                typeof(Geometry<VertexPositionTexture>)
                )) { }

        public override void LoadContent() {            
            contentManager = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            graphics = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
        }

        public override void Process(Entity entity) {
            var vp = entity.GetComponent<Geometry<VertexPosition>>();
            var vpc = entity.GetComponent<Geometry<VertexPositionColor>>();
            var vpct = entity.GetComponent<Geometry<VertexPositionColorTexture>>();
            var vpnt = entity.GetComponent<Geometry<VertexPositionNormalTexture>>();
            var vpt = entity.GetComponent<Geometry<VertexPositionTexture>>();
            var transform = entity.GetComponent<Transform3D>();
            var camera = entity.GetComponent<Camera>();            
            DrawGeometry(vp, transform, camera);
            DrawGeometry(vpc, transform, camera);
            DrawGeometry(vpct, transform, camera);
            DrawGeometry(vpnt, transform, camera);
            DrawGeometry(vpt, transform, camera);
        }

        private void DrawGeometry<T>(Geometry<T> geometry, Transform3D transform, Camera camera) where T : struct, IVertexType {
            if(geometry != null) {
                // apply matrices
                if (geometry.Effect is IEffectMatrices effect) {
                    effect.World = transform.WorldMatrix();
                    effect.View = camera.Transform.ViewMatrix();
                    effect.Projection = camera.Projection.Matrix();
                }

                // compute number of primitives
                var count = geometry.Vertices.Length / 3;
                if(geometry.PrimitiveType == PrimitiveType.LineList 
                    || geometry.PrimitiveType == PrimitiveType.LineStrip) {
                    count = geometry.Vertices.Length / 2;
                }

                // render by passes
                foreach (var pass in geometry.Effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    graphics.GraphicsDevice.DrawUserPrimitives(geometry.PrimitiveType, geometry.Vertices, 0, count);
                }
            }
        }
    }
    
}
