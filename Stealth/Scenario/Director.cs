using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Kernel;
using Stealth.Map;
using Stealth.Play;
using System;
using System.Collections.Generic;

namespace Stealth.Scenario {

    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Update,
        Layer = (int)Settings.PriorityLayer.GameLogic)]
    class Director : EntityComponentProcessingSystem<Scene> {

        public override void LoadContent() {
            // Set initial scene
            var introScene = EntityWorld.CreateEntity();
            introScene.AddComponent(new Scene {
                Name = "Intro",
                Background = Color.DodgerBlue
            });

            var contentManager = BlackBoard.GetEntry<ContentManager>(Settings.ContentManager);
            var graphicsManager = BlackBoard.GetEntry<GraphicsDeviceManager>(Settings.GraphicsManager);
            var font = contentManager.Load<SpriteFont>("Arial");
            var texture = contentManager.Load<Texture2D>("tile_grass1");
            
            introScene.AddComponent(new StringSprite {
                spriteFont = font,
                color = Color.White,
                effects = SpriteEffects.None,
                text = introScene.GetComponent<Scene>().Name
            });
            introScene.AddComponent(new TextureSprite {
                texture = texture,
                clipping = null,
                color = Color.White,
                effects = SpriteEffects.None
            });
            introScene.AddComponent(new Transform2D {
                position = new Vector2(10, 10),
                rotation = 0,
                origin = new Vector2(),
                scale = new Vector2(1, 1),
                layerDepth = 0
            });


            // shared camera object
            var camera = new Camera {
                Transform = new Transform3D {
                    Position = new Vector3(0, -8, 8),
                    LookAt = Vector3.Zero,
                    Upward = Vector3.UnitZ
                },
                Projection = new Projection {
                    AspectRatio = graphicsManager.PreferredBackBufferWidth /
                    (float)graphicsManager.PreferredBackBufferHeight,
                    FieldOfView = MathHelper.PiOver4,
                    NearClipPlane = 0.1f,
                    FarClipPlane = 1000
                }
            };
            
            // map grid
            var grid = EntityWorld.CreateEntity();
            var gridSize = new Vector2(15,10);
            var lines = new List<VertexPosition>();
            for (int i = 0; i <= gridSize.Y; i++) {
                lines.Add(new VertexPosition { Position = new Vector3(0, i, 0) });
                lines.Add(new VertexPosition { Position = new Vector3(gridSize.X, i, 0) });
            }
            for (int i = 0; i <= gridSize.X; i++) {
                lines.Add(new VertexPosition { Position = new Vector3(i, 0, 0) });
                lines.Add(new VertexPosition { Position = new Vector3(i, gridSize.Y, 0) });
            }
            grid.AddComponent(new Geometry<VertexPosition> {
                Vertices = lines.ToArray(),
                Effect = new BasicEffect(graphicsManager.GraphicsDevice) {
                    DiffuseColor = new Vector3(0.3f),
                },
                PrimitiveType = PrimitiveType.LineList
            });
            grid.AddComponent(new Transform3D {
                Position = new Vector3(-.5f, -.5f, 0), // tiles have 0 at middle
            });
            grid.AddComponent(camera); // use shared camera
            

            // Look at middle of grid
            camera.Transform.LookAt = new Vector3(gridSize.X / 2, gridSize.Y / 2, 0);


            // Input:
            //  - Tiles (Pooled): Transform3D, Model(Mesh), View

            const float Voxel = 1.0f / 32.0f;

            // Create some tiles
            var palette = "map\\obj\\palette";
            var grass1Tile = contentManager.LoadModel("map\\tile\\grass1", palette);
            var grass2Tile = contentManager.LoadModel("map\\tile\\grass2", palette);
            grass2Tile.Border = grass1Tile.Border = new Color(0x49, 0x93, 0x24);
            
            var cursorTile = contentManager.LoadModel("map\\tile\\cursor", palette, "Cursor|Hover");
            cursorTile.Border = new Color(0x18, 0x4d, 0xd6);
            
            var shelfTile = contentManager.LoadModel("map\\obj\\shelf_bench", "map\\obj\\shelf_bench_tex");
            shelfTile.Border = new Color(0x6b,0x2b,0x13);
            

            var cursor = EntityWorld.CreateEntity();
            cursor.AddComponent(cursorTile);
            cursor.AddComponent(new Transform3D { Position = new Vector3(9, 6, 2* Voxel) });
            cursor.AddComponent(camera); // use shared camera
            cursor.AddComponent(new Cursor());
            
            var shelfObj = EntityWorld.CreateEntity();
            shelfObj.AddComponent(shelfTile);
            shelfObj.AddComponent(new Transform3D { Position = new Vector3(3, 3, 0 * Voxel) });
            shelfObj.AddComponent(camera); // use shared camera


            var rand = new Random();
            for (int y = 0; y < gridSize.Y; y++) {
                for (int x = 0; x < gridSize.X; x++) {
                    var entity = EntityWorld.CreateEntity();
                    var tile = (rand.Next() % 2 == 0) ? grass1Tile : grass2Tile;
                    var xRot = (rand.Next() % 3) - 1;
                    var yRot = (rand.Next() % 2) * 2 - 1;

                    entity.AddComponent(tile);
                    entity.AddComponent(new Transform3D { Position = new Vector3(x, y, 0), Forward = new Vector3(xRot, xRot == 0 ? yRot : 0, 0) });
                    entity.AddComponent(camera); // use shared camera
                }
            }

            //// Create animated fox entity
            //var item2 = EntityWorld.CreateEntity();
            //var unit = contentManager.LoadModel(
            //        "obj\\untitled", "obj\\texture",
            //        "Armature|Armature|Take 001|BaseLayer");
            //item2.AddComponent(unit);
            //item2.AddComponent(new Transform3D {
            //    Scale = new Vector3(1.0f / 30.0f),
            //    Position = new Vector3(2, 0, 0),
            //});            
            //item2.AddComponent(camera); // use shared camera

        }

        public override void Process(Entity entity, Scene scene) {
            //Debug.WriteLine($"Scene: {scene.Name}");
        }
    }
    
}
