using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Core.Xml;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GUI;
using GarageGames.Torque.MathUtil;


namespace StarterGame2D
{
    class GuiHelpScreen : GUIBitmap, IGUIScreen
    {
        public GuiHelpScreen()
        {
            Name = "GuiHelpScreen";

            //create the style for the main menu background
            GUIBitmapStyle bitmapStyle = new GUIBitmapStyle();
            bitmapStyle.SizeToBitmap = true;
            Style = bitmapStyle;

            Bitmap = @"data\images\HelpScreen";

            //setup the input map
            SetupInputMap();
        }



        public void OnMainScreenWake(GUIControl mainGUI)
        {
            // perform your custom screen-activation tasks
        }



        private void SetupInputMap()
        {
            // bind the first gamepad’s A and B buttons to start and exit the
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.A, null, On_Accept);
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.Back, null, On_Accept);

            // bind the keyboards enter and escape keys to start and exit
            int keyboardId = InputManager.Instance.FindDevice("keyboard");
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.A, null, On_Accept);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Enter, null, On_Accept);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Escape, null, On_Accept);
        }



        private void On_Accept()
        {
            //return to main menu
            GuiMainMenu mainMenu = new GuiMainMenu();
            GUICanvas.Instance.SetContentControl(mainMenu);
        }
    }
}
