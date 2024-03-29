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
using System.Threading;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.Util;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.MathUtil;
using GarageGames.Torque.Platform;
using GarageGames.Torque.Core.Xml;
using GarageGames.Torque.GUI;

using FreeTheMarket.GUI;

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

        public enum InteractionDirections { Up, Down, Left, Right }
        public enum InteractionTypes { OneShot, Continuous }
        
        public int PlayerNumber
        {
            get { return _playerNumber; }
            set { _playerNumber = value; }
        }

        [TorqueXmlSchemaType(DefaultValue = "Space")]
        public Keys InteractionKey
        {
            get { return _kbControlInteraction; }
            set { _kbControlInteraction = value; }
        }

        public InteractionDirections InteractionDirection
        {
            get { return _interactionDirection; }
            set { _interactionDirection = value; }
        }

        [TorqueXmlSchemaType(DefaultValue = "OneShot")]
        public InteractionTypes InteractionType
        {
            get { return _interactionType; }
            set { _interactionType = value; }
        }

        [TorqueXmlSchemaType(DefaultValue = "10")]
        public float InteractionDistance
        {
            get { return _interactionDistance; }
            set { _interactionDistance = value; }
        }

        public String CustomStringData
        {
            get { return _customStringData; }
            set { _customStringData = value; }
        }

        // Define delegate method
        public delegate void OnInteractionBeginDelegate(T2DSceneObject ourObject, T2DSceneObject theirObject);
        public delegate void OnInteractionEndDelegate(T2DSceneObject ourObject, T2DSceneObject theirObject);

        // Get and sets for delegate methods
        public OnInteractionBeginDelegate InteractionBeginDelegate
        {
            get { return _onInteractionBegin; }
            set { _onInteractionBegin = value; }
        }

        public OnInteractionEndDelegate InteractionEndDelegate
        {
            get { return _onInteractionEnd; }
            set { _onInteractionEnd = value; }
        }

        // Define function to be used as delegated
        public static OnInteractionBeginDelegate DragBegin
        {
            get { return DragBeginInteraction; }
        }

        public static OnInteractionEndDelegate DragEnd
        {
            get { return DragEndInteraction; }
        }

        public static OnInteractionBeginDelegate ChangeSceneDelegate
        {
            get { return ChangeSceneInteraction; }
        }

        public static OnInteractionBeginDelegate Destroy
        {
            get { return DestroyInteraction; }
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
             
        }
      
        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public override void CopyTo(TorqueComponent obj)
        {
            base.CopyTo(obj);
            InteractibleComponent obj2 = obj as InteractibleComponent;
            obj2.PlayerNumber = this.PlayerNumber;
            obj2.InteractionKey = this.InteractionKey;
            obj2.InteractionDirection = this.InteractionDirection;
            obj2.InteractionType = this.InteractionType;
            obj2.InteractionDistance = this.InteractionDistance;
            obj2.CustomStringData = this.CustomStringData;
            obj2.InteractionBeginDelegate = this.InteractionBeginDelegate;
            obj2.InteractionEndDelegate = this.InteractionEndDelegate;

            // TODO copy all private fields
        }

        // Mark object as active
        public void Activate()
        {
            this._interactionActive = true;
        }

        // Mark object as inactive
        public void Deactivate()
        {
            this._interactionActive = false;
        }

        // Check if object is active
        public bool IsActive()
        {
            return this._interactionActive;
        }

        // Check if interactor is in range of this object
        public bool IsInRange(T2DSceneObject theirObject)
        {
            MovementComponent moveComponent = theirObject.Components.FindComponent<MovementComponent>();
            T2DSceneObject ourObject = this.SceneObject;
            if (moveComponent != null)
            {
                if (this.InteractionDirection == InteractionDirections.Left &&
                    moveComponent.PlayerFacing == MovementComponent.Facing.Right)
                {
                    // If at correct distance
                    if (theirObject.Position.X >= ourObject.Position.X - this.InteractionDistance &&
                        theirObject.Position.X < ourObject.Position.X &&
                        theirObject.Position.Y > ourObject.Position.Y - ourObject.Size.Y / 4 &&
                        theirObject.Position.Y < ourObject.Position.Y + ourObject.Size.Y / 4)
                    {
                        return true;
                    }
                }
                else if (this.InteractionDirection == InteractibleComponent.InteractionDirections.Right &&
                    moveComponent.PlayerFacing == MovementComponent.Facing.Left)
                {
                    // If at correct distance
                    if (theirObject.Position.X <= ourObject.Position.X + this.InteractionDistance &&
                        theirObject.Position.X > ourObject.Position.X &&
                        theirObject.Position.Y > ourObject.Position.Y - ourObject.Size.Y / 4 &&
                        theirObject.Position.Y < ourObject.Position.Y + ourObject.Size.Y / 4)
                    {
                        return true;
                    }
                }
                else if (this.InteractionDirection == InteractibleComponent.InteractionDirections.Up &&
                        moveComponent.PlayerFacing == MovementComponent.Facing.Down)
                {
                    // If at correct distance
                    if (theirObject.Position.Y >= ourObject.Position.Y - this.InteractionDistance &&
                        theirObject.Position.Y < ourObject.Position.Y &&
                        theirObject.Position.X > ourObject.Position.X - ourObject.Size.X / 4 &&
                        theirObject.Position.X < ourObject.Position.X + ourObject.Size.X / 4)
                    {
                        return true;
                    }
                }
                else if (this.InteractionDirection == InteractibleComponent.InteractionDirections.Down &&
                        moveComponent.PlayerFacing == MovementComponent.Facing.Up)
                {
                    // If at correct distance
                    if (theirObject.Position.Y <= ourObject.Position.Y + this.InteractionDistance &&
                        theirObject.Position.Y > ourObject.Position.Y &&
                        theirObject.Position.X > ourObject.Position.X - ourObject.Size.X / 4 &&
                        theirObject.Position.X < ourObject.Position.X + ourObject.Size.X / 4)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Destroy the object being interacted with.
        public static void DestroyInteraction(T2DSceneObject ourObject, T2DSceneObject theirObject)
        {
            ourObject.MarkForDelete = true;
        }

        public static void DragBeginInteraction(T2DSceneObject ourObject, T2DSceneObject theirObject)
        {
            Vector2 newPos = new Vector2();
            newPos.Y = theirObject.Position.Y + theirObject.Size.Y / 2 + ourObject.Size.Y / 2;
            newPos.X = theirObject.Position.X;
            ourObject.SetPosition(newPos, true);
            ourObject.CollisionsEnabled = false;

            MovementComponent theirMove = theirObject.Components.FindComponent<MovementComponent>();
            theirMove.PreventFaceChange();
        }

        public static void DragEndInteraction(T2DSceneObject ourObject, T2DSceneObject theirObject)
        {
            MovementComponent theirMove = theirObject.Components.FindComponent<MovementComponent>();
            theirMove.AllowFaceChange();

            ourObject.CollisionsEnabled = true;
        }

        // Change the scene after the interaction.
        public static void ChangeSceneInteraction(T2DSceneObject ourObject, T2DSceneObject theirObject)
        {
             InteractibleComponent component = ourObject.Components.FindComponent<InteractibleComponent>();

            // On proceed further if this sceneobject actually has the required component.
            if (component != null)
            {
                // TODO Change the scene using the SceneManager
                String newScene = component._customStringData;
                
                ScreenManager.Instance.LoadNewScene(newScene);


                
            }            
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

            ProcessList.Instance.AddTickCallback(Owner, this);
        }


        #endregion

        //======================================================
        #region Private, protected, internal fields
        
        // Keep pointer to scene object
        T2DSceneObject _sceneObject;

        // Track player controlling this object, if any
        int _playerNumber = -1;

        // Keeps track of active status
        bool _interactionActive = false;

        // Keep track of how long the interaction should be frozen.
        float _interactionTimer = 0.0f;

        // Distance for interaction
        float _interactionDistance;

        // Custom String Data for various uses
        String _customStringData;

        // Direction of activation (up, down, left, right)
        InteractionDirections _interactionDirection;
        // Type of interaction: OneShot / Continuous
        InteractionTypes _interactionType;

        // Key to listen to
        Keys _kbControlInteraction;

        // Delegate method for interaction activation
        OnInteractionBeginDelegate _onInteractionBegin;
        // Delegate method after interaction activation
        OnInteractionEndDelegate _onInteractionEnd;
        
        #endregion
    }
}
