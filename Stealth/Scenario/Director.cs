using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stealth.Core.Render;
using Stealth.Kernel;
using Stealth.Map;
using Stealth.Play;
using System;
using System.Collections.Generic;

namespace Stealth.Scenario {

    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Update,
        Layer = (int)Settings.PriorityLayer.GameLogic)]
    class Director : EntityComponentProcessingSystem<Scenario> {

        private void CreateUnit(string model, string texture, int x, int y, int z, int sizeX, int sizeY) {
            var unit = Scene.Current().Content.LoadModel(model, texture);
            var entity = EntityWorld.CreateEntity();
            entity.AddComponent(unit);
            // size offset
            var sizeOffset = new Vector3((float)(sizeX - 1) * 0.5f, (float)(sizeY - 1) * 0.5f, 0);

            entity.AddComponent(new Transform3D {
                Position = new Vector3(x, y, (float)(z) * 0.25f),
                //Forward = -Vector3.UnitX
            });
        }

        private void CreateShadow(string model, int x, int y, int z, int sizeX, int sizeY) {
            var unit = Scene.Current().Content.LoadModel(model, null);
            var shadow = new Shadow {
                Model = unit.Model,
                Animation = unit.Animation
            };
            var entity = EntityWorld.CreateEntity();
            entity.AddComponent(shadow);
            // size offset
            var sizeOffset = new Vector3((float)(sizeX - 1) * 0.5f, (float)(sizeY - 1) * 0.5f, 0);

            entity.AddComponent(new Transform3D {
                Position = new Vector3(x, y, (float)(z)*0.25f),
                //Forward = -Vector3.UnitX
            });
        }

        public override void LoadContent() {
            // Set initial scene
            var introScene = EntityWorld.CreateEntity();
            introScene.AddComponent(new Scenario {
                Name = "Intro",
                Background = Color.DodgerBlue
            });
            
            var scene = Scene.Current();
            var font = scene.Content.Load<SpriteFont>("Arial");
            var texture = scene.Content.Load<Texture2D>("tile_grass1");
            
            introScene.AddComponent(new StringSprite {
                spriteFont = font,
                color = Color.White,
                effects = SpriteEffects.None,
                text = introScene.GetComponent<Scenario>().Name
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
                Effect = new BasicEffect(scene.GraphicsDevice) {
                    DiffuseColor = new Vector3(0.3f),
                },
                PrimitiveType = PrimitiveType.LineList
            });
            grid.AddComponent(new Transform3D {
                Position = new Vector3(-.5f, -.5f, 0), // tiles have 0 at middle
            });

            // shared camera object
            scene.Camera.Transform.Position = new Vector3(0, -12, 12);
            scene.Camera.Transform.LookAt = new Vector3(gridSize.X / 2, gridSize.Y / 2, 0);


            // Input:
            //  - Tiles (Pooled): Transform3D, Model(Mesh), View

            const float Voxel = 1.0f / 32.0f;

            // Create some tiles
            var palette = "map\\obj\\palette";
            var grass1Tile = scene.Content.LoadModel("map\\tile\\grass1", palette);
            var grass2Tile = scene.Content.LoadModel("map\\tile\\grass2", palette);            
            var cursorTile = scene.Content.LoadModel("map\\tile\\cursor", palette, "Cursor|Hover");            


            var cursor = EntityWorld.CreateEntity();
            cursor.AddComponent(cursorTile);
            cursor.AddComponent(new Transform3D { Position = new Vector3(6, 1, 2* Voxel) });
            cursor.AddComponent(new Cursor());
                        

            CreateUnit("map\\obj\\bench_2x1x2", "map\\obj\\bench_2x1x2_tex", 4, 1, 1, 2, 1);
            CreateUnit("map\\obj\\bookshelf_2x1x2\\model", "map\\obj\\bookshelf_2x1x2\\texture", 6, 2, 1, 2, 1);

            CreateUnit("map\\obj\\tile_floor\\model", "map\\obj\\tile_floor\\texture", 1, 1, 0, 1, 1);
            CreateUnit("map\\obj\\tile_floor\\model", "map\\obj\\tile_floor\\texture", 2, 3, 0, 1, 1);
            CreateUnit("map\\obj\\tile_floor\\model", "map\\obj\\tile_floor\\texture", 2, 4, 0, 1, 1);

            CreateUnit("map\\decor\\book\\book", "map\\decor\\book\\book_texture", 6, 1, 2, 1, 1);
            
            CreateUnit("map\\decor\\vase\\vase", "map\\decor\\vase\\vase_texture", 7, 1, 2, 1, 1);
            CreateShadow("map\\decor\\vase\\vase_shadow", 7, 1, 2, 1, 1);

            var rand = new Random();
            for (int y = 0; y < gridSize.Y; y++) {
                for (int x = 0; x < gridSize.X; x++) {
                    //var entity = EntityWorld.CreateEntity();
                    //var tile = (rand.Next() % 2 == 0) ? grass1Tile : grass2Tile;
                    var xRot = (rand.Next() % 3) - 1;
                    var yRot = (rand.Next() % 2) * 2 - 1;
                    //entity.AddComponent(tile);
                    //entity.AddComponent(new Transform3D { Position = new Vector3(x, y, 0), Forward = new Vector3(xRot, xRot == 0 ? yRot : 0, 0) });
                    
                    //CreateUnit("map\\obj\\tile_floor\\model", "map\\obj\\tile_floor\\texture", x, y, 0, 1, 1);
                }
            }

            // Add skybox
            var skybox = EntityWorld.CreateEntity();
            skybox.AddComponent(new Skybox {
                Texture = scene.Content.Load<Texture2D>("map\\skybox\\skybox")
            });
            

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

        }

        public override void Process(Entity entity, Scenario scene) {
            //Debug.WriteLine($"Scene: {scene.Name}");
        }
    }
    
}
