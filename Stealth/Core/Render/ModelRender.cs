using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;
using Stealth.Map;

namespace Stealth.Core.Render {
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Draw,
        Layer = (int)Settings.PriorityLayer.ModelRender)]
    class ModelRender : EntityComponentProcessingSystem<Unit, Transform3D> {
        private Scene scene;
        private Effect effect;                        
        public override void LoadContent() {
            scene = Scene.Current();
            effect = scene.Content.Load<Effect>("shaders\\model");  
        }
        protected override void Begin() {
            scene.SetGBuffer();
        }
        public override void Process(Entity entity, Unit unit, Transform3D transform) {
            scene.DrawMesh(effect, transform, unit.Model, unit.Texture, unit.Animation);
        }
    }

}
