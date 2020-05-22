using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Engine.Component;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.RenderSystem {

    [ArtemisEntitySystem(GameLoopType = GameLoopType.Draw, Layer = (int)Layer.Overlay)]
    class UIGeometryRender : EntityProcessingSystem {
        private Scene scene;

        public UIGeometryRender() : base(
            Aspect.All(typeof(Transform3D)).
            GetOne(typeof(Geometry<VertexPosition>),
                typeof(Geometry<VertexPositionColor>),
                typeof(Geometry<VertexPositionColorTexture>),
                typeof(Geometry<VertexPositionNormalTexture>),
                typeof(Geometry<VertexPositionTexture>)
                )) { }

        public override void LoadContent() {
            scene = Scene.Current();
        }

        protected override void Begin() {
            scene.SetOutput();
        }
        public override void Process(Entity entity) {
            var vp = entity.GetComponent<Geometry<VertexPosition>>();
            var vpc = entity.GetComponent<Geometry<VertexPositionColor>>();
            var vpct = entity.GetComponent<Geometry<VertexPositionColorTexture>>();
            var vpnt = entity.GetComponent<Geometry<VertexPositionNormalTexture>>();
            var vpt = entity.GetComponent<Geometry<VertexPositionTexture>>();
            var transform = entity.GetComponent<Transform3D>();
            DrawGeometry(vp, transform);
            DrawGeometry(vpc, transform);
            DrawGeometry(vpct, transform);
            DrawGeometry(vpnt, transform);
            DrawGeometry(vpt, transform);
        }

        private void DrawGeometry<T>(Geometry<T> geometry, Transform3D transform) where T : struct, IVertexType {
            if (geometry != null) {
                // apply matrices
                if (geometry.Effect is IEffectMatrices effect) {
                    effect.World = transform.WorldMatrix();
                    effect.View = scene.Camera.Transform.ViewMatrix();
                    effect.Projection = scene.Camera.Projection.Matrix();
                }

                // compute number of primitives
                var count = geometry.Vertices.Length / 3;
                if (geometry.PrimitiveType == PrimitiveType.LineList
                    || geometry.PrimitiveType == PrimitiveType.LineStrip) {
                    count = geometry.Vertices.Length / 2;
                }

                // render by passes
                foreach (var pass in geometry.Effect.CurrentTechnique.Passes) {
                    pass.Apply();
                    scene.GraphicsDevice.DrawUserPrimitives(geometry.PrimitiveType, geometry.Vertices, 0, count);
                }
            }
        }
    }

}
