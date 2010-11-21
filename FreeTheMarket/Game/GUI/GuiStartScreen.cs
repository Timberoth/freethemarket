using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Audio;
using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Core.Xml;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GUI;
using GarageGames.Torque.MathUtil;
using Microsoft.Xna.Framework.GamerServices;



namespace FreeTheMarket
{

    /// <summary>
    /// The GuiStartScreen displays the game's title screen and waits for the player to 
    /// press the Start button on the game controller. This is important because we do
    /// not know which device ID the player is using to control the game setup screens.
    /// For example, if the player has a Rock Band Guitar and Drum Set always connected
    /// and signs-in with the game controller as device 3, then we will know the device 
    /// ID since that's the one they used to press the Start button.
    /// </summary>
    class GuiStartScreen : GUIBitmap, IGUIScreen
    {
        public GuiStartScreen()
        {
            Name = "GuiStartScreen";

            //create the style for the main menu background
            GUIBitmapStyle bitmapStyle = new GUIBitmapStyle();
            Style = bitmapStyle;
            Bitmap = @"data\images\StartScreen";

            //setup the input map
            SetupInputMap();
        }



        public void OnMainScreenWake(GUIControl mainGUI)
        {
        }



        private void SetupInputMap()
        {
            InputMap.BindCommand(0, (int)XGamePadDevice.GamePadObjects.Back, null, Game.Instance.Exit);
            InputMap.BindCommand(0, (int)XGamePadDevice.GamePadObjects.Start, null, Select0);
            InputMap.BindCommand(1, (int)XGamePadDevice.GamePadObjects.Back, null, Game.Instance.Exit);
            InputMap.BindCommand(1, (int)XGamePadDevice.GamePadObjects.Start, null, Select1);
            InputMap.BindCommand(2, (int)XGamePadDevice.GamePadObjects.Back, null, Game.Instance.Exit);
            InputMap.BindCommand(2, (int)XGamePadDevice.GamePadObjects.Start, null, Select2);
            InputMap.BindCommand(3, (int)XGamePadDevice.GamePadObjects.Back, null, Game.Instance.Exit);
            InputMap.BindCommand(3, (int)XGamePadDevice.GamePadObjects.Start, null, Select3);

            int keyboardId = InputManager.Instance.FindDevice("keyboard");
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Enter, null, Select0);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Escape, null, Game.Instance.Exit);
        }



        private void Select0()
        {
            Game.Instance.Player1ControllerIndex = 0;

            //load the games' main menu
            GuiMainMenu mainMenu = new GuiMainMenu();
            GUICanvas.Instance.SetContentControl(mainMenu);
        }



        private void Select1()
        {
            Game.Instance.Player1ControllerIndex = 1;

            //load the games' main menu
            GuiMainMenu mainMenu = new GuiMainMenu();
            GUICanvas.Instance.SetContentControl(mainMenu);
        }



        private void Select2()
        {
            Game.Instance.Player1ControllerIndex = 2;

            //load the games' main menu
            GuiMainMenu mainMenu = new GuiMainMenu();
            GUICanvas.Instance.SetContentControl(mainMenu);
        }



        private void Select3()
        {
            Game.Instance.Player1ControllerIndex = 3;

            //load the games' main menu
            GuiMainMenu mainMenu = new GuiMainMenu();
            GUICanvas.Instance.SetContentControl(mainMenu);
        }

    }
}
