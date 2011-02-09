using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using GarageGames.Torque.Core;
using GarageGames.Torque.T2D;
using GarageGames.Torque.Sim;
using GarageGames.Torque.Platform;

namespace FreeTheMarket
{
    [TorqueXmlSchemaType]
    [TorqueXmlSchemaDependency(Type = typeof(T2DPhysicsComponent))]    

    public class MovementComponent : TorqueComponent, ITickObject
    {        
        //======================================================
        #region Constructors
        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        public enum Facing
        {
            Right,
            Up,
            Left,
            Down,            
        }
        
        public int PlayerNumber
        {
            get { return _playerNumber; }
            set { _playerNumber = value; }
        }

        public Facing PlayerFacing
        {
            get { return _currentFacing; }
        }
        
        /*
         * Animation Variables 
         */
        public T2DAnimationData IdleUp
        {
            get { return _idleUp; }
            set { _idleUp = value; }
        }

        public T2DAnimationData MoveUp
        {
            get { return _moveUp; }
            set { _moveUp = value; }
        }


        public T2DAnimationData IdleDown
        {
            get { return _idleDown; }
            set { _idleDown = value; }
        }

        public T2DAnimationData MoveDown
        {
            get { return _moveDown; }
            set { _moveDown = value; }
        }

        public T2DAnimationData IdleLeft
        {
            get { return _idleLeft; }
            set { _idleLeft = value; }
        }

        public T2DAnimationData MoveLeft
        {
            get { return _moveLeft; }
            set { _moveLeft = value; }
        }

        public T2DAnimationData IdleRight
        {
            get { return _idleRight; }
            set { _idleRight = value; }
        }

        public T2DAnimationData MoveRight
        {
            get { return _moveRight; }
            set { _moveRight = value; }
        }
        
        #endregion

        
        //======================================================
        #region Public Methods
        public void InterpolateTick(float k)
        {            
        }

        public void ProcessTick(Move move, float elapsed)
        {            
            if (move != null && move.Sticks.Count > 0)
            {               
                T2DAnimatedSprite currentAnim = _sceneObject as T2DAnimatedSprite;

                // set our test object's Velocity based on stick/keyboard input
                _sceneObject.Physics.VelocityX = move.Sticks[0].X * 20.0f;
                _sceneObject.Physics.VelocityY = -move.Sticks[0].Y * 20.0f;                

                // If there's no stick movement, then switch to idle if not already in it
                if (_sceneObject.Physics.VelocityX == 0.0f && _sceneObject.Physics.VelocityY == 0.0f)
                {
                    // Check to see if the character was moving last frame, if so switch to idle anim
                    if ( (currentAnim != null) && ( _prevVelocity.X != 0.0f || _prevVelocity.Y != 0.0 ) )
                    {
                        if( currentAnim.AnimationData == _moveUp )
                        {
                            currentAnim.PlayAnimation(_idleUp);
                        }
                        else if ( currentAnim.AnimationData == _moveDown )
                        {
                            currentAnim.PlayAnimation(_idleDown);
                        }
                        else if ( currentAnim.AnimationData == _moveLeft )
                        {
                            currentAnim.PlayAnimation(_idleLeft);
                        }
                        else if ( currentAnim.AnimationData == _moveRight )
                        {
                            currentAnim.PlayAnimation(_idleRight);
                        }
                    }
                }

                // If there is stick movement, then check if we need to switch animations
                else if (!_preventChange)
                {
                    // Check the character's current facing direction, if it has changed, switch animations.
                    // Note that VelocityY is swapped.
                    Vector2 tempVector;
                    tempVector.X = _sceneObject.Physics.VelocityX;
                    tempVector.Y = -_sceneObject.Physics.VelocityY;
                    double facingAngle = CalculateFacingAngle( tempVector );
                    
                    _currentFacing = CalculateFacing(facingAngle);
                                                           
                    if (_currentFacing == Facing.Right && currentAnim.AnimationData != _moveRight)
                    {
                        currentAnim.PlayAnimation(_moveRight);
                    }
                    else if (_currentFacing == Facing.Up && currentAnim.AnimationData != _moveUp)
                    {
                        currentAnim.PlayAnimation(_moveUp);
                    }
                    else if (_currentFacing == Facing.Left && currentAnim.AnimationData != _moveLeft)
                    {
                        currentAnim.PlayAnimation(_moveLeft);
                    }
                    else if (_currentFacing == Facing.Down && currentAnim.AnimationData != _moveDown)
                    {
                        currentAnim.PlayAnimation(_moveDown);
                    }
                }

                // Keep track of previous frame velocity to determine which animation to play
                _prevVelocity.X = _sceneObject.Physics.VelocityX;
                _prevVelocity.Y = _sceneObject.Physics.VelocityY;
                
                //T2DSceneCamera cam = T2DSceneGraph.Camera as T2DSceneCamera;                
            }
        }

        public double CalculateFacingAngle(Vector2 currentVelocity)
        {
            Vector2 tempVector;
            tempVector.X = 1.0f;
            tempVector.Y = 0.0f;
            double angle = CalculateAngleBetweenVectors( tempVector, currentVelocity);

            // If in Quad III or IV convert to 180 to 360
            if (currentVelocity.Y < 0)
            {
                return (360.0 - angle);
            }
            else
            {
                return angle;
            }
        }

        public double CalculateAngleBetweenVectors( Vector2 v1, Vector2 v2 )
        {   
            // Normalize both vectors
            v1.Normalize();
            v2.Normalize();

            // Create var to hold dot product result
            float dotResult = 0.0f;
            Vector2.Dot( ref v1, ref v2, out dotResult );

            // Convert to degrees
            return Math.Acos(dotResult) * 180.0 / Math.PI;                       
        }

        public Facing CalculateFacing(double angle)
        {
            if (angle < 45.0f || angle >= 315.0f )
            {
                return Facing.Right;
            }
            else if( angle >= 45.0f && angle < 135.0f )
            {
                return Facing.Up;
            }
            else if (angle >= 135.0f && angle < 225.0f)
            {
                return Facing.Left;
            }
            else if (angle >= 225.0f && angle < 315.0f)
            {
                return Facing.Down;
            }
            
            // Should never get here
            return Facing.Down;            
        }

        public void PreventFaceChange()
        {
            this._preventChange = true;
        }

        public void AllowFaceChange()
        {
            this._preventChange = false;
        }
        
        #endregion    
    
        //======================================================
        #region Private, protected, internal methods
        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(Owner is T2DSceneObject))
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

        void _OnBackButton(float val)
        {
            if (val > 0.0f)
                Game.Instance.Exit();
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
                inputMap.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.LeftThumbX, MoveMapTypes.StickAnalogHorizontal, 0);
                inputMap.BindMove(gamepadId, (int)XGamePadDevice.GamePadObjects.LeftThumbY, MoveMapTypes.StickAnalogVertical, 0);
                inputMap.BindAction(gamepadId, (int)XGamePadDevice.GamePadObjects.Back, _OnBackButton);
            }

            // keyboard controls
            int keyboardId = InputManager.Instance.FindDevice(keyboard);
            if (keyboardId >= 0)
            {
                inputMap.BindMove(keyboardId, (int)Keys.Right, MoveMapTypes.StickDigitalRight, 0);
                inputMap.BindMove(keyboardId, (int)Keys.Left, MoveMapTypes.StickDigitalLeft, 0);
                inputMap.BindMove(keyboardId, (int)Keys.Up, MoveMapTypes.StickDigitalUp, 0);
                inputMap.BindMove(keyboardId, (int)Keys.Down, MoveMapTypes.StickDigitalDown, 0);
                // WASD
                inputMap.BindMove(keyboardId, (int)Keys.D, MoveMapTypes.StickDigitalRight, 0);
                inputMap.BindMove(keyboardId, (int)Keys.A, MoveMapTypes.StickDigitalLeft, 0);
                inputMap.BindMove(keyboardId, (int)Keys.W, MoveMapTypes.StickDigitalUp, 0);
                inputMap.BindMove(keyboardId, (int)Keys.S, MoveMapTypes.StickDigitalDown, 0);
            }
        }


        #endregion

        //======================================================
        #region Private, protected, internal fields
        
        // Keep pointer to scene object
        T2DSceneObject _sceneObject;
        
        // Track player controlling this object
        int _playerNumber = -1;
        
        // Animations to play when facing or moving a certain direction Up, Down, Left or Right
        T2DAnimationData _idleUp;
        T2DAnimationData _moveUp;
        T2DAnimationData _idleRight;
        T2DAnimationData _moveRight;
        T2DAnimationData _idleDown;
        T2DAnimationData _moveDown;
        T2DAnimationData _idleLeft;
        T2DAnimationData _moveLeft;
        
        // Track facing direction
        Facing _currentFacing;
        // Should we change face
        bool _preventChange = false;

        // Track previous velocity values as a vector
        Vector2 _prevVelocity;

        #endregion
    }
}
