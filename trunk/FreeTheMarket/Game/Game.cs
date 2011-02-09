using System;
using System.Collections.Generic;
using GarageGames.Torque.Core;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.Sim;
using GarageGames.Torque.GameUtil;
using GarageGames.Torque.T2D;
using GarageGames.Torque.GUI;
using GarageGames.Torque.Platform;

using FreeTheMarket.GUI;

namespace FreeTheMarket
{
    /// <summary>
    /// This is the main game class for your game
    /// </summary>
    public class Game : TorqueGame
    {
        #region Static methods, fields, constructors
        /// <summary>
        /// A static property that lets you easily get the Game instance from any Game class.
        /// </summary>

        public static Game Instance { get { return _myGame; } }
        public int Player1ControllerIndex { get; set; }
        public int PlayerScore { get; set; }
        public int HealthLevel { get; set; }

        #endregion


        #region Constructors
        #endregion


        #region Public properties, operators, constants, and enums
        #endregion


        #region Public Methods
        public static void Main()
        {
            // Create the static game instance.  
            _myGame = new Game();
            
            // begin the game.  Further setup is done in BeginRun()
            _myGame.Run();
        }



        public void OnFinishedLoadSplashStudio()
        {
            //next show the studio splash screen
            GuiSplashScreen splashScreen = new GuiSplashScreen(@"data\images\TX_BG");

            ScreenManager.Instance.InitializeNewScreen( splashScreen );
        }



        public void OnFinishedLoadSplashGarageGames()
        {
            //load the main menu
            GuiStartScreen startScreen = new GuiStartScreen();
            ScreenManager.Instance.InitializeNewScreen(startScreen);
        }

        #endregion


        #region Private, protected, internal methods
        /// <summary>
        /// Called after the graphics device is created and before the game is about to start running.
        /// </summary>
        protected override void BeginRun()
        {
            base.BeginRun();

            //start by showing the GarageGames splash screen
            GuiSplashScreen splashScreen = new GuiSplashScreen(@"data\images\splashStudio");
            ScreenManager.Instance.InitializeNewScreen(splashScreen);
        }
        #endregion


        #region Private, protected, internal fields
        static Game _myGame;
        #endregion
    }
}