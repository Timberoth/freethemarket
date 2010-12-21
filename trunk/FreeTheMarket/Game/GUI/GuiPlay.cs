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

            _testDialogWindow = new DialogWindow(this, "First Line that is really long and should be broken up and go onto a second line.", 0.0f, 300.0f, 520.0f, 120.0f);
        }


        public override void OnRender(Vector2 offset, GarageGames.Torque.MathUtil.RectangleF updateRect)
        {            
            base.OnRender(offset, updateRect);
        }

    }
}
