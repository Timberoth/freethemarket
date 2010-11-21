using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GUI;
using GarageGames.Torque.MathUtil;



namespace FreeTheMarket
{
    class GuiSplashScreen : GUISplash, IGUIScreen
    {
        GUISplashStyle splashStyle = new GUISplashStyle();


        public GuiSplashScreen()
        {
        }



        public GuiSplashScreen(string ImagePath)
        {
            InitializeSplashScreen(ImagePath);
        }



        private void InitializeSplashScreen(string ImagePath)
        {
            splashStyle.FadeInSec = 2f; // black to image 2 seconds
            splashStyle.FadeOutSec = 2f; // image to black 2 seconds
            splashStyle.FadeWaitSec = 3f; // still image for 3 seconds
            splashStyle.Bitmap = ImagePath; //@"data\images\SplashGG";


            // set some info for this control
            Name = "GuiSplashScreen";
            Style = splashStyle;
            OnFadeFinished = OnSplashFinished;
        }



        public void OnSplashFinished()
        {
            if (splashStyle.Bitmap.EndsWith("splashStudio")) //splashStudio.png
                Game.Instance.OnFinishedLoadSplashStudio();
            else
                Game.Instance.OnFinishedLoadSplashGarageGames();
        }

    }
}
