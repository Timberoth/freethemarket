//**btp_replace(namespace GarageGames.VCSTemplates.ItemTemplates,namespace FreeTheMarket.Components)
//**btp_replace(class TestClass,public class InteractorComponent)
using System;
using System.Collections.Generic;
using System.Text;

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
    public class InteractorComponent : TorqueComponent, ITickObject
    {
        //======================================================
        #region Static methods, fields, constructors
        #endregion

        //======================================================
        #region Constructors
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        public T2DSceneObject SceneObject
        {
            get { return Owner as T2DSceneObject; }
        }

        [TorqueXmlSchemaType(DefaultValue = "0")]
        public int Player
        {
            get { return _player; }
            set { _player = value; }
        }

        [TorqueXmlSchemaType(DefaultValue = "Space")]
        public Keys InteractionKey
        {
            get { return _kbControlInteraction; }
            set { _kbControlInteraction = value; }
        }

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
            // todo: perform processing for component here
            if (move != null)
            {
                // if the button is not being pressed/has been let go of
                if (!move.Buttons[0].Pushed)
                {
                    this._isHeld = false;
                    foreach (T2DSceneObject obj in this._objectsActivated)
                    {
                        InteractibleComponent component = obj.Components.FindComponent<InteractibleComponent>();
                        if (component != null)
                        {
                            component.InteractionEndDelegate(component.SceneObject, this.SceneObject);
                        }
                    }
                }

                // if player is currently starting to fire action
                if (move.Buttons[0].Pushed)
                {
                    List<T2DSceneObject> sceneObjects = TorqueObjectDatabase.Instance.FindObjects<T2DSceneObject>();
                    for (int i = 0; i < sceneObjects.Count; i++)
                    {
                        T2DSceneObject current = sceneObjects[i];
                        InteractibleComponent component = current.Components.FindComponent<InteractibleComponent>();
                        if (component != null)
                        {
                            // If this interactor's action key is equivalent to a component's key
                            // And if the component is just firing or continuous
                            if (component.InteractionKey == this.InteractionKey &&
                                ((component.InteractionType == InteractibleComponent.InteractionTypes.OneShot &&
                                    !_isHeld) ||
                                component.InteractionType == InteractibleComponent.InteractionTypes.Continuous))
                            {

                                // If there is a delegate method set up
                                if (component.InteractionBeginDelegate != null && this.Owner != null )
                                {
                                    // TODO if required conditions are true
                                    MovementComponent moveComponent = this.Owner.Components.FindComponent<MovementComponent>();
                                    if (moveComponent != null)
                                    {
                                        T2DSceneObject parentObject = this.SceneObject;
                                        if (component.InteractionDirection == InteractibleComponent.InteractionDirections.Left &&
                                                moveComponent.PlayerFacing == MovementComponent.Facing.Right)
                                        {
                                            // If at correct distance
                                            if (parentObject.Position.X >= current.Position.X - component.InteractionDistance &&
                                                parentObject.Position.X < current.Position.X &&
                                                parentObject.Position.Y > current.Position.Y - current.Size.Y / 4 &&
                                                parentObject.Position.Y < current.Position.Y + current.Size.Y / 4)
                                            {
                                                // Then fire that method
                                                component.InteractionBeginDelegate(current, parentObject);
                                                // If continuous, restore properties after button release
                                                if (component.InteractionType == InteractibleComponent.InteractionTypes.Continuous)
                                                {
                                                    this._objectsActivated.Add(current);
                                                }
                                            }
                                        }
                                        if (component.InteractionDirection == InteractibleComponent.InteractionDirections.Right &&
                                                moveComponent.PlayerFacing == MovementComponent.Facing.Left)
                                        {
                                            // If at correct distance
                                            if (parentObject.Position.X <= current.Position.X + component.InteractionDistance &&
                                                parentObject.Position.X > current.Position.X &&
                                                parentObject.Position.Y > current.Position.Y - current.Size.Y / 4 &&
                                                parentObject.Position.Y < current.Position.Y + current.Size.Y / 4)
                                            {
                                                // Then fire that method
                                                component.InteractionBeginDelegate(current, parentObject);
                                                // If continuous, restore properties after button release
                                                if (component.InteractionType == InteractibleComponent.InteractionTypes.Continuous)
                                                {
                                                    this._objectsActivated.Add(current);
                                                }
                                            }
                                        }
                                        if (component.InteractionDirection == InteractibleComponent.InteractionDirections.Up &&
                                                moveComponent.PlayerFacing == MovementComponent.Facing.Down)
                                        {
                                            // If at correct distance
                                            if (parentObject.Position.Y >= current.Position.Y - component.InteractionDistance &&
                                                parentObject.Position.Y < current.Position.Y &&
                                                parentObject.Position.X > current.Position.X - current.Size.X / 4 &&
                                                parentObject.Position.X < current.Position.X + current.Size.X / 4)
                                            {
                                                // Then fire that method
                                                component.InteractionBeginDelegate(current, parentObject);
                                                // If continuous, restore properties after button release
                                                if (component.InteractionType == InteractibleComponent.InteractionTypes.Continuous)
                                                {
                                                    this._objectsActivated.Add(current);
                                                }
                                            }
                                        }
                                        if (component.InteractionDirection == InteractibleComponent.InteractionDirections.Down &&
                                                moveComponent.PlayerFacing == MovementComponent.Facing.Up)
                                        {
                                            // If at correct distance
                                            if (parentObject.Position.Y <= current.Position.Y + component.InteractionDistance &&
                                                parentObject.Position.Y > current.Position.Y &&
                                                parentObject.Position.X > current.Position.X - current.Size.X / 4 &&
                                                parentObject.Position.X < current.Position.X + current.Size.X / 4)
                                            {
                                                // Then fire that method
                                                component.InteractionBeginDelegate(current, parentObject);
                                                // If continuous, restore properties after button release
                                                if (component.InteractionType == InteractibleComponent.InteractionTypes.Continuous)
                                                {
                                                    this._objectsActivated.Add(current);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // After activating any object, action is now considered held
                    this._isHeld = true;
                }
            }
        }

        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public override void CopyTo(TorqueComponent obj)
        {
            base.CopyTo(obj);
            InteractorComponent obj2 = obj as InteractorComponent;
            obj2.Player = Player;
            obj2.InteractionKey = InteractionKey;
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(owner is T2DSceneObject))
                return false;

            // todo: perform initialization for the component
            PlayerManager.Instance.GetPlayer(_player).ControlObject = SceneObject;
            InputMap map = new InputMap();

            int gamepadId = InputManager.Instance.FindDevice("gamepad" + _player);
            int keyboardId = InputManager.Instance.FindDevice("keyboard");

            if (gamepadId >= 0)
            {
                map.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.A, MoveMapTypes.Button, 0);
            }

            if (keyboardId >= 0)
            {
                map.BindMove(keyboardId, (int)Keys.Space, MoveMapTypes.Button, 0);
            }

            PlayerManager.Instance.GetPlayer(_player).InputMap = map;

            // activate tick callback for this component.
            ProcessList.Instance.AddTickCallback(Owner, this);


            // todo: look up interfaces exposed by other components
            // E.g., 
            // _theirInterface = Owner.Components.GetInterface<ValueInterface<float>>("float", "their interface name");            

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


        #endregion

        //======================================================
        #region Private, protected, internal fields

        // The player number from owner
        int _player;
        // Key that is to be used for interaction
        Keys _kbControlInteraction;
        // Used to see if button was just pressed or is being held
        bool _isHeld = false;
        // List of objects that have been activated
        List<T2DSceneObject> _objectsActivated = new List<T2DSceneObject>();

        #endregion
    }
}
