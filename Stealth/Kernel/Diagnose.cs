using Artemis.Attributes;
using Artemis.Interface;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using System;
using System.Diagnostics;

namespace Stealth.Kernel {
    // Measure frame rate and cpu usage
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Update,
        Layer = (int)Settings.PriorityLayer.Kernel)]
    class Diagnose : EntitySystem {

        // Measure frame rate
        private long elapsedTime;
        private int frameCounter;
        private int frameRate;

        public override void Process() {
            // Measure FPS
            ++frameCounter;
            elapsedTime += EntityWorld.Delta;
            if (elapsedTime > TimeSpan.TicksPerSecond) {
                elapsedTime -= TimeSpan.TicksPerSecond;
                frameRate = frameCounter;
                frameCounter = 0;
            }

#if DEBUG
            string fps = string.Format("fps: {0}", frameRate);
            string entityCount = string.Format("Active entities: {0}", EntityWorld.EntityManager.ActiveEntities.Count);
            string removedEntityCount = string.Format("Removed entities: {0}", EntityWorld.EntityManager.TotalRemoved);
            string totalEntityCount = string.Format("Total entities: {0}", EntityWorld.EntityManager.TotalCreated);
            Debug.WriteLine(fps);
            Debug.WriteLine(entityCount);
            Debug.WriteLine(removedEntityCount);
            Debug.WriteLine(totalEntityCount);
#endif
        }
    }
}
