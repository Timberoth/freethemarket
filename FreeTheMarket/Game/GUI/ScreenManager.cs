using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using GarageGames.Torque.GUI;
using GarageGames.Torque.Platform;
using GarageGames.Torque.Core;
using GarageGames.Torque.Core.Xml;
using GarageGames.Torque.Sim;

namespace FreeTheMarket.GUI
{
    public sealed class ScreenManager
    {
        // Create singleton instace
        static readonly ScreenManager instance = new ScreenManager();

        // ScreenManger variables.

        // Keep a ref to the current screen
        IGUIScreen _currentScreen;

        // Explicit static constructor to tell C# compiler
        // not to mark type as beforefieldinit
        static ScreenManager()
        {

        }

        ScreenManager()
        {

        }

        public static ScreenManager Instance
        {
            get
            {
                return instance;
            }
        }

        public void InitializeNewScreen(IGUIScreen screen)
        {
            GUICanvas.Instance.SetContentControl((GUIControl)screen);            
            _currentScreen = screen;
        }

        public void LoadNewScene( string newScene )
        {
            // Unload the previous scene
            Game.Instance.SceneLoader.UnloadLastScene();
            
            // Load new scene
            Game.Instance.SceneLoader.Load(newScene);

            //create a renderable canvas for the scene     
            GUIStyle stylePlayGui = new GUIStyle();
            GUISceneview playGui = new GUISceneview();
            playGui.Style = stylePlayGui;

            //switch over to the new canvas     
            GUICanvas.Instance.SetContentControl(playGui);  
        }
    }    
}
