//**btp_replace(namespace GarageGames.VCSTemplates.ItemTemplates,namespace FreeTheMarket.Components)
//**btp_replace(class TestClass,public class InteractibleComponent)

/***
 * InteractibleComponent.cs
 * Component that contains all the functionality to make a Torque2D object interactible.
 * What the component actually does when interacted with is determined by the params passed in.
 * Possible interactions include talking, searching, opening, fighting, 
 */

using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.Util;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.MathUtil;
using GarageGames.Torque.Platform;

namespace FreeTheMarket.Components
{
    [TorqueXmlSchemaType]
    public class InteractibleComponent : TorqueComponent, ITickObject
    {
        //======================================================
        #region Static methods, fields, constructors
        #endregion

        //======================================================
        #region Constructors
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums
        
        public int PlayerNumber
        {
            get { return _playerNumber; }
            set { _playerNumber = value; }
        }       

        public T2DSceneObject SceneObject
        {
            get { return Owner as T2DSceneObject; }
        }

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
            // Need valid move to proceed
            if (move == null)
                return;

            List<T2DSceneObject> sceneObjects = TorqueObjectDatabase.Instance.FindObjects<T2DSceneObject>();
            for (int i = 0; i < sceneObjects.Count; ++i)
            {
                T2DSceneObject current = sceneObjects[i];

                // Don't process anything if we're looking at our own pointer.
                if (current == _sceneObject)
                {
                    continue;
                }

                InteractibleComponent component = current.Components.FindComponent<InteractibleComponent>();

                // On proceed further if this sceneobject actually has the required component.
                if (component != null)
                {
                    // See if we are close enough to interact with it.
                    Vector2 distanceVector = current.Position - _sceneObject.Position;
                    float INTERACT_DISTANCE = 3.0f;
                    if (Math.Abs(distanceVector.Length()) < INTERACT_DISTANCE)
                    {                        
                        // Only interact when in range and the Interact button is pressed.
                        if ( move.Buttons.Count > 0 && move.Buttons[0].Pushed )
                        {                            
                            if (!_interactionActive)
                            {
                                System.Console.WriteLine("Start Interaction");

                                // DO SOMETHING HERE, the function called here must be able to be called
                                // mutliple times without breaking anything.
                                // InteractionFunction();

                                _interactionActive = true;

                                // Set a timer so the interact call isn't called a bunch of times.
                                Timer timer = new Timer();
                                timer.Elapsed += new ElapsedEventHandler(FinishInteraction);
                                timer.Interval = 1750;
                                timer.Start();
                            }
                        }
                    }                    
                }
            }
        }

        private void FinishInteraction(object source, ElapsedEventArgs args)
        {
            _interactionActive = false;
        }

        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public override void CopyTo(TorqueComponent obj)
        {
            base.CopyTo(obj);
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(owner is T2DSceneObject))
                return false;

            // retain a reference to this component's owner object
            _sceneObject = owner as T2DSceneObject;

            // Don't register any input if the _playerNumber is not between 0 and 3
            if (_playerNumber >= 0 && _playerNumber <= 3)
            {
                _SetupInputMap(_sceneObject, _playerNumber, "gamepad" + _playerNumber, "keyboard");
            }
            
            // tell the process list to notifiy us with ProcessTick and InterpolateTick events
            ProcessList.Instance.AddTickCallback(Owner, this);

            return true;
        }

        protected override void _OnUnregister()
        {
            // todo: perform de-initialization for the component

            base._OnUnregister();
        }

        protected override void _RegisterInterfaces(TorqueObject owner)
        {
            base._RegisterInterfaces(owner);

            // todo: register interfaces to be accessed by other components
            // E.g.,
            // Owner.RegisterCachedInterface("float", "interface name", this, _ourInterface);
        }

        private void _SetupInputMap(TorqueObject player, int playerIndex, String gamePad, String keyboard)
        {
            // Set player as the controllable object
            PlayerManager.Instance.GetPlayer(playerIndex).ControlObject = player;

            // Get input map for this player and configure it
            InputMap inputMap = PlayerManager.Instance.GetPlayer(playerIndex).InputMap;

            int gamepadId = InputManager.Instance.FindDevice(gamePad);
            if (gamepadId >= 0)
            {
                // A button for interactions
                inputMap.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.A, MoveMapTypes.Button, 0);                
            }

            // keyboard controls
            int keyboardId = InputManager.Instance.FindDevice(keyboard);
            if (keyboardId >= 0)
            {
                // Space for interactions
                inputMap.BindMove(keyboardId, (int)Keys.Space, MoveMapTypes.Button, 0);                
            }
        }


        #endregion

        //======================================================
        #region Private, protected, internal fields
        
        // Keep pointer to scene object
        T2DSceneObject _sceneObject;

        // Track player controlling this object, if any
        int _playerNumber = -1;

        // Make sure interaction isn't hit multiple times
        bool _interactionActive = false;
        
        #endregion
    }
}
