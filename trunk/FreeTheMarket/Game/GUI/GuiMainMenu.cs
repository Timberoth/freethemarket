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
    class GuiMainMenu : GUIBitmap, IGUIScreen
    {
        public GuiMainMenu()
        {
            Name = "GuiMainMenu";

            //create the style for the main menu background
            GUIBitmapStyle bitmapStyle = new GUIBitmapStyle();
            Style = bitmapStyle;
            Bitmap = @"data\images\MainMenuScreen";

            //create the style for the menu buttons
            GUIButtonStyle buttonStyle = new GUIButtonStyle();
            buttonStyle.FontType = "Arial22"; //@"data\images\MyFont";
            buttonStyle.TextColor[CustomColor.ColorBase] = new Color(100, 100, 100, 255); //normal menu text color
            buttonStyle.TextColor[CustomColor.ColorHL] = Color.Red; //highlighter color
            buttonStyle.TextColor[CustomColor.ColorNA] = Color.Silver; //disabled color
            buttonStyle.TextColor[CustomColor.ColorSEL] = Color.DarkRed; //select color
            buttonStyle.Alignment = TextAlignment.JustifyCenter;
            buttonStyle.Focusable = true;

            float positionX = 640;
            float positionY = 400;

            GUIButton option1 = new GUIButton();
            option1.Style = buttonStyle;
            option1.Size = new Vector2(520, 100);
            option1.Position = new Vector2(positionX - (option1.Size.X / 2), positionY);
            option1.Visible = true;
            option1.Folder = this;
            option1.ButtonText = "Start New Game";
            option1.OnSelectedDelegate = On_Option1_Select;
            option1.OnGainFocus(null);
            _buttons.Add(option1);

            GUIButton option2 = new GUIButton();
            option2.Style = buttonStyle;
            option2.Size = new Vector2(500, 100);
            option2.Position = new Vector2(positionX - (option1.Size.X / 2), positionY + 50);
            option2.Visible = true;
            option2.Folder = this;
            option2.ButtonText = "How to Play";
            option2.OnSelectedDelegate = On_Option2_Select;
            _buttons.Add(option2);

            GUIButton option3 = new GUIButton();
            option3.Style = buttonStyle;
            option3.Size = new Vector2(500, 100);
            option3.Position = new Vector2(positionX - (option1.Size.X / 2), positionY + 100);
            option3.Visible = true;
            option3.Folder = this;
            option3.ButtonText = "Credits";
            option3.OnSelectedDelegate = On_Option3_Select;
            _buttons.Add(option3);

            GUIButton option4 = new GUIButton();
            option4.Style = buttonStyle;
            option4.Size = new Vector2(500, 100);
            option4.Position = new Vector2(positionX - (option1.Size.X / 2), positionY + 150);
            option4.Visible = true;
            option4.Folder = this;
            option4.ButtonText = "Quit";
            option4.OnSelectedDelegate = On_Option4_Select;
            _buttons.Add(option4);

            //setup the input map
            SetupInputMap();
        }



        public void OnMainScreenWake(GUIControl mainGUI)
        {
        }



        private void SetupInputMap()
        {
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.A, null, On_Option1_Select);
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.B, null, On_Option2_Select);
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.X, null, On_Option3_Select);
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.Y, null, On_Option4_Select);
            InputMap.BindCommand(Game.Instance.Player1ControllerIndex, (int)XGamePadDevice.GamePadObjects.Back, null, Game.Instance.Exit);

            int keyboardId = InputManager.Instance.FindDevice("keyboard");
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Down, null, MoveDown);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Up, null, MoveUp);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Enter, null, Select);
            InputMap.BindCommand(keyboardId, (int)Microsoft.Xna.Framework.Input.Keys.Escape, null, On_Option3_Select);
        }



        private void MoveDown()
        {
            if (_currentSelection + 1 < _buttons.Count)
            {
                //clear all other options
                for (int index = 0; index < _buttons.Count; index++)
                    ((GUIButton)_buttons[index]).OnLoseFocus(null);

                _currentSelection++;
                ((GUIButton)_buttons[_currentSelection]).OnGainFocus(null);
            }
        }



        private void MoveUp()
        {
            if (_currentSelection - 1 >= 0)
            {
                //clear all other options
                for (int index = 0; index < _buttons.Count; index++)
                    ((GUIButton)_buttons[index]).OnLoseFocus(null);

                _currentSelection--;
                ((GUIButton)_buttons[_currentSelection]).OnGainFocus(null);
            }
        }



        public override void OnRender(Vector2 offset, RectangleF updateRect)
        {
            base.OnRender(offset, updateRect);
        }



        private void Select()
        {
            ((GUIButton)_buttons[_currentSelection]).OnSelectedDelegate();
        }



        private void On_Option1_Select()
        {
            GuiPlay playGUI = new GuiPlay();
            GUICanvas.Instance.SetContentControl(playGUI);
            
            Game.Instance.SceneLoader.Load(@"data\levels\Gameplay.txscene");
        }



        private void On_Option2_Select()
        {
            //show the help screen
            GuiHelpScreen helpScreen = new GuiHelpScreen();
            GUICanvas.Instance.SetContentControl(helpScreen);
        }



        private void On_Option3_Select()
        {
            //show the credits screen
            GuiCreditsScreen creditsScreen = new GuiCreditsScreen();
            GUICanvas.Instance.SetContentControl(creditsScreen);
        }



        private void On_Option4_Select()
        {
            //shutdown the game
            GarageGames.Torque.XNA.TorqueEngineComponent.Instance.Exit();
        }


        GUIButton option3 = new GUIButton();

        bool _showBuy = false;
        int _currentSelection = 0;
        ArrayList _buttons = new ArrayList();
    }
}
