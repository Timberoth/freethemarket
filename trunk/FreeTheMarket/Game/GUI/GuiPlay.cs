using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GarageGames.Torque.GUI;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Core;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;




namespace FreeTheMarket
{
    public class GuiPlay : GUISceneview, IGUIScreen
    {        
        DialogWindow _testDialogWindow;        
        
        // Used for camera schooling
        Vector2 _tempPosition;

        // Used in FPS calculation and display        
        float _timeCounter;
        int _frameCount;
        GUITextStyle _FPSTextStyle;        
        GUIText _FPSText;        

        public GuiPlay()
        {
            GUIStyle playStyle = new GUIStyle();
            Name = "GuiPlay";
            Style = playStyle;
            Size = new Vector2(1280, 720);
            
            // This assignment is critical to the render process when reloading scenes for some reason.
            Folder = GUICanvas.Instance;            

            
            _testDialogWindow = new DialogWindow(this, 
                "Let's see how the dialog window handles this longer piece of text with different spacing.  It looks like this string has been broken up into multiple lines, but the question is how many?  Here is more text that should be put onto another screen.  Here's more text that will continue to roll onto another screen or two after it reaches the end of the window and rolls over.",
                0.0f, 300.0f, 520.0f, 120.0f);
            
            // FPS Stuff
            _frameCount = 0;
            _timeCounter = 0.0f;

            // Create the text style for all the text.
            _FPSTextStyle = new GUITextStyle();
            _FPSTextStyle.FontType = "Arial14"; // @"data\images\MyFont";
            _FPSTextStyle.TextColor[0] = Color.White;
            _FPSTextStyle.SizeToText = true;
            _FPSTextStyle.Alignment = TextAlignment.JustifyLeft;
            _FPSTextStyle.PreserveAspectRatio = true;

            // Create all the GUI text lines.
            _FPSText = new GUIText();
            _FPSText.Style = _FPSTextStyle;
            // The text needs to be inside the dialog window so the position should
            // be offset from the dialog window's position.
            _FPSText.Position = new Vector2(20.0f, 20.0f);
            _FPSText.Visible = true;
            _FPSText.Folder = this;
             
        }

        ~GuiPlay()
        {
            bool stop = true;
        }

        public override void OnRender(Vector2 offset, GarageGames.Torque.MathUtil.RectangleF updateRect)
        {            
            base.OnRender(offset, updateRect);
            
            _frameCount++;

            // Time in milliseconds
            float time = Game.Instance.Engine.GameTime.ElapsedGameTime.Milliseconds;
            _timeCounter += time;

            if (_timeCounter >= 1000.0)
            {               
                // FPS in ms = (1/time)
                // FPS in seconds = FPS in ms * 1000.0
                float fps = (_frameCount/_timeCounter)*1000.0f;

                // Display FPS
                _FPSText.Text = "FPS: " + fps.ToString();

                // Zero out variables
                _frameCount = 0;
                _timeCounter = 0.0f;
            }
             
            
            T2DSceneCamera camera = (T2DSceneCamera)this.Camera;
            T2DSceneObject player = (T2DSceneObject)TorqueObjectDatabase.Instance.FindObject("Player");
            if( camera != null && player != null )
            {
                _tempPosition = player.Position;
                _tempPosition.Y = camera.CenterPosition.Y;
                camera.CenterPosition = _tempPosition;
            }                      
        }
    }
}
