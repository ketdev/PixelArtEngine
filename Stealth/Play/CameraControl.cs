﻿using Artemis;
using Artemis.Attributes;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Stealth.Kernel;
using Stealth.Map;
using System.Collections.Generic;

namespace Stealth.Play {
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Update,
        Layer = (int)Settings.PriorityLayer.GameLogic)]
    class CameraControl : EntityComponentProcessingSystem<Camera, Cursor, Transform3D> {

        private GamePadDPad prevState;

        public override void Process(Entity entity, Camera camera, Cursor cursor, Transform3D transform) {

            // Poll for current state
            var gamepad = GamePad.GetState(PlayerIndex.One);
            var gamepadCap = GamePad.GetCapabilities(PlayerIndex.One);
            var keyb = Keyboard.GetState();
            var mouse = Mouse.GetState();
            var touchCol = TouchPanel.GetState();
            
            // move cursor
            var cursorPos = transform.Position;
            if (gamepad.DPad.Right == ButtonState.Released && prevState.Right == ButtonState.Pressed)
                cursorPos.X += 1;
            if (gamepad.DPad.Left == ButtonState.Released && prevState.Left == ButtonState.Pressed)
                cursorPos.X -= 1;
            if (gamepad.DPad.Up == ButtonState.Released && prevState.Up == ButtonState.Pressed)
                cursorPos.Y += 1;
            if (gamepad.DPad.Down == ButtonState.Released && prevState.Down == ButtonState.Pressed)
                cursorPos.Y -= 1;
            transform.Position = cursorPos;
            prevState = gamepad.DPad;

            // orbit camera
            var speed = 0.1f / MathHelper.Pi;
            var dir = camera.Transform.Position - cursorPos;
            if (gamepadCap.HasRightXThumbStick && gamepadCap.HasRightYThumbStick) {
                var d = gamepad.ThumbSticks.Right * speed;
                // direction
                dir = Vector3.Transform(dir, Quaternion.CreateFromAxisAngle(Vector3.UnitZ, d.X));
                // zoom
                var len = dir.Length();
                len -= len * d.Y;
                dir = Vector3.Normalize(dir) * len;
            }
            camera.Transform.Position = dir + cursorPos;
            camera.Transform.LookAt = cursorPos;
            
        }
    }
}