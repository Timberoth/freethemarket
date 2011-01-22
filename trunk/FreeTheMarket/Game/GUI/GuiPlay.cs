using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GarageGames.Torque.GUI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace FreeTheMarket
{
    public class GuiPlay : GUISceneview, IGUIScreen
    {        
        DialogWindow _testDialogWindow;

        public GuiPlay()
        {
            GUIStyle playStyle = new GUIStyle();
            Name = "GuiPlay";
            Style = playStyle;
            Size = new Vector2(1280, 720);
            Folder = GUICanvas.Instance;

            _testDialogWindow = new DialogWindow(this, 
                "Let's see how the dialog window handles this longer piece of text with different spacing.  It looks like this string has been broken up into multiple lines, but the question is how many?  Here is more text that should be put onto another screen.  Here's more text that will continue to roll onto another screen or two after it reaches the end of the window and rolls over.",
                0.0f, 300.0f, 520.0f, 120.0f);
        }


        public override void OnRender(Vector2 offset, GarageGames.Torque.MathUtil.RectangleF updateRect)
        {            
            base.OnRender(offset, updateRect);
        }

    }
}
