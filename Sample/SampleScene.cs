using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Engine;
using Engine.Component;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Sample {
    [ArtemisEntitySystem(GameLoopType = GameLoopType.Update, Layer = (int)Engine.Layer.GameLogic)]
    class SampleScene : EntitySystem {

        public override void LoadContent() {
            // Get our scene object
            var scene = Scene.Current();

            Console.WriteLine("Hello Console!");

            // Get our font asset
            var font = scene.Content.Load<SpriteFont>("Arial");

            // add test entity
            var test = EntityWorld.CreateEntity();
            test.AddComponent(new StringSprite {
                spriteFont = font,
                color = Color.CornflowerBlue,
                effects = SpriteEffects.None,
                text = "Hello World!"
            });
            test.AddComponent(new Transform2D {
                position = new Vector2(10, 10),
                rotation = 0,
                origin = new Vector2(),
                scale = new Vector2(1, 1),
                layerDepth = 0
            });

            // Create some tiles
            const float Voxel = 1.0f / 32.0f;

            // Add animated cursor
            var cursor = EntityWorld.CreateEntity();
            cursor.AddComponent(scene.Content.LoadModel("cursor", "palette", "Cursor|Hover"));
            cursor.AddComponent(new Transform3D { Position = new Vector3(0, 0, 2 * Voxel) });

            // Add bench
            var bench = EntityWorld.CreateEntity();
            bench.AddComponent(scene.Content.LoadModel("bench_2x1x2\\model", "bench_2x1x2\\tex"));
            bench.AddComponent(new Transform3D { Position = new Vector3(1, 1, 0) });

            // Add bookshelf
            var bookshelf = EntityWorld.CreateEntity();
            bookshelf.AddComponent(scene.Content.LoadModel("bookshelf_2x1x2\\model", "bookshelf_2x1x2\\texture"));
            bookshelf.AddComponent(new Transform3D { Position = new Vector3(-2, 2, 0), Forward = new Vector3(-0.3f, 0.5f, 0) });

            // shared camera object
            scene.Camera.Transform.Position = new Vector3(6, -6, 6);
            scene.Camera.Transform.LookAt = new Vector3(0, 0, 0);
        }
    }
}
