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
        Layer = (int)Settings.PriorityLayer.Border)]
    class BorderPass : EntitySystem {
        private ContentManager c;
        private GraphicsDeviceManager g;
        private RenderData r;
        
        private Effect borderEffect;
        
        public override void LoadContent() {
            c = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            g = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            r = BlackBoard.GetEntry<RenderData>(Settings.RenderData);            
            borderEffect = c.Load<Effect>("shaders\\border");
            borderEffect.Parameters["ScreenPixel"].SetValue(
                new Vector2(1.0f / r.Width, 1.0f / r.Height)
            );
        }
        
        public override void Process() {
            r.SetOutput(false);
            borderEffect.Parameters["BorderTexture"].SetValue(r.BorderRT);
            borderEffect.Parameters["DepthTexture"].SetValue(r.DepthRT);            
            r.DrawFullscreen(borderEffect);            
        }        
    }

}
