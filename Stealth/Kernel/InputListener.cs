using Artemis.Attributes;
using Artemis.Interface;
using Artemis.Manager;
using Artemis.System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;

namespace Stealth.Kernel {
    
    struct PlayerInput : IComponent {
        public PlayerIndex index;

    }

    // Captures user input and maps according to current button settings
    [ArtemisEntitySystem(
        GameLoopType = GameLoopType.Update,
        Layer = (int)Settings.PriorityLayer.Kernel)]
    class InputListener : EntitySystem {

        // TODO: Input mapping

        public override void Process() {

            // Poll for current state
            var gamepad = GamePad.GetState(PlayerIndex.One);
            var gamepadCap = GamePad.GetCapabilities(PlayerIndex.One);
            var keyb = Keyboard.GetState();
            var mouse = Mouse.GetState();
            var touchCol = TouchPanel.GetState();

            //if (gamepad.Buttons.Back == ButtonState.Pressed
            //    || keyb.IsKeyDown(Keys.Escape)) {
            //    Exit();
            //}
            //
            //// TODO: Add your update logic here
            //
            //if (Keyboard.GetState().IsKeyDown(Keys.Space)) {
            //    if (SoundEffect.MasterVolume == 0.0f)
            //        SoundEffect.MasterVolume = 1.0f;
            //    else
            //        SoundEffect.MasterVolume = 0.0f;
            //}
            //
            //// gamepad
            //if (gamepadCap.HasAButton && gamepad.Buttons.A == ButtonState.Pressed) {
            //    var inst = sfx.CreateInstance();
            //    listener.Position = new Vector3(-1, 0, 0);
            //    inst.Apply3D(listener, emitter);
            //    sfx.Play();
            //}
            //
            //if (gamepadCap.HasLeftXThumbStick && gamepadCap.HasLeftYThumbStick) {
            //    var d = gamepad.ThumbSticks.Left * 10.0f;
            //    d.Y = -d.Y; // invert Y
            //    position += d;
            //}
            //if (gamepad.DPad.Right == ButtonState.Pressed)
            //    position.X += 10;
            //if (gamepad.DPad.Left == ButtonState.Pressed)
            //    position.X -= 10;
            //if (gamepad.DPad.Up == ButtonState.Pressed)
            //    position.Y -= 10;
            //if (gamepad.DPad.Down == ButtonState.Pressed)
            //    position.Y += 10;
            //
            ////// mouse
            ////position.X = mouse.X;
            ////position.Y = mouse.Y;
            //
            //// touch
            //foreach (var touch in touchCol) {
            //    position = touch.Position;
            //}
            
        }

    }
}
