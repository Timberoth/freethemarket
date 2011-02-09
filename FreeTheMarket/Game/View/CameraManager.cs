using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using GarageGames.Torque.Core;
using GarageGames.Torque.Util;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.MathUtil;
using GarageGames.Torque.GUI;

namespace FreeTheMarket.View
{

    /// <summary>
    /// Enumerations defining the type of camera operations
    /// </summary>
    // Note: When adding new operation types, make sure to add them to the appropriate array in the 
    //      CameraManager class
    public enum CameraOperationTypes { FollowObject, LookAtObject, LookAtPosition, StopMove, StopEffect, Rotate, Shake };
    
    /// <summary>
    /// Structure containing the commands and parameters for camera operations
    /// </summary>
    public class CameraOperation
    {
        public CameraOperation(int paramcount) { Parameters = new float[paramcount]; }
        public CameraOperationTypes OperationType;
        public float [] Parameters;
        public T2DSceneObject Object;
        public bool Blocking; // does the operation prevent the next from running
        public InterpolationMode interpolationMode; // Mode of interpolation for animating camera movements
    }

    /// <summary>
    /// Contains a camera and its queues
    /// </summary>
    public class CameraData
    {
        public CameraData() {MovementQueue = new Queue<CameraOperation>(); 
                             EffectQueue = new Queue<CameraOperation>();}
        public T2DSceneCamera Camera;
        public Queue<CameraOperation> MovementQueue;
        public CameraOperation CurMovOperation;
        public bool isPaused;

        // Effect operations should only be used for non-movement related functions (including camera shakes)
        // Used to show an effect while moving the camera
        public Queue<CameraOperation> EffectQueue;
        public CameraOperation CurEffOperation; 
    }

    [TorqueXmlSchemaType]
    public class CameraManager : TorqueObject, ITickObject
    {
        //======================================================
        #region Static methods, fields, constructors

        public static CameraManager Instance
        {
            get { return _instance; }
        }
        #endregion

        //======================================================
        #region Constructors

        private CameraManager()
        {
            // So we get called every tick
            ProcessList.Instance.AddTickCallback(this);
            _active = false;
            Cameras = new List<CameraData>();

            // Build the operation lists to make it easier to determine the type of operation (movement or effect)
            _moveOperations = new List<CameraOperationTypes>();
            _moveOperations.Add(CameraOperationTypes.FollowObject);
            _moveOperations.Add(CameraOperationTypes.LookAtObject);
            _moveOperations.Add(CameraOperationTypes.LookAtPosition);
            _moveOperations.Add(CameraOperationTypes.StopMove);
            // Shake is a movement op because it doesn't work well with the other movement operations
            _moveOperations.Add(CameraOperationTypes.Shake); 

            _effectOperations = new List<CameraOperationTypes>();
            _effectOperations.Add(CameraOperationTypes.Rotate);
            _effectOperations.Add(CameraOperationTypes.StopEffect);
        }

        #endregion

        //======================================================
        #region Public properties, operators, constants, and enums

        /// <summary>
        /// Returns true if the camera manager is controlling cameras
        /// </summary>
        public bool Active
        {
            get { return _active; }
            set 
            {
                if (value)
                    _active = value;
                else
                {
                    for (int x = 0; x < Cameras.Count; x++)
                    {
                        StopCameraEffects(Cameras[x]);
                        StopCameraMovements(Cameras[x]);
                    }
                    _active = value;
                }
            }
        }

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
            // Variables to make the code more readable
            T2DSceneCamera Camera;
            float [] mParams;
            float [] eParams;
            CameraOperation CurMovOperation;
            CameraOperation CurEffOperation;
            bool newop;

            // Don't do anything if the manager isn't active
            if (!_active)
                return;

            // Cycle through all the cameras
            for (int x = 0; x < Cameras.Count; x++)
            {
                newop = false;

                // Bypass the camera's operation if it is paused
                if (Cameras[x].isPaused)
                    continue;

                // First check to make sure if the current operation is finished
                if (Cameras[x].CurMovOperation != null)
                {
                    if (Cameras[x].CurMovOperation.OperationType != CameraOperationTypes.FollowObject)
                    {
                        // Time/duration is the first parameter
                        if (Cameras[x].CurMovOperation.Parameters[0] < 0)
                            Cameras[x].CurMovOperation = null;
                    }
                }
                if (Cameras[x].CurEffOperation != null)
                {
                    // Time/duration is the first parameter
                    if (Cameras[x].CurEffOperation.Parameters[0] < 0)
                        Cameras[x].CurEffOperation = null;
                }

                // If no movement operation is currently happening, or a non-blocking operation is happening, 
                //   load the next operation.
                if (Cameras[x].CurMovOperation == null)
                    newop = true;
                else if (Cameras[x].CurMovOperation.Blocking == false && Cameras[x].MovementQueue.Count > 0)
                    newop = true;

                // Make the current movement operation the next operation
                if (newop && Cameras[x].MovementQueue.Count > 0)
                    Cameras[x].CurMovOperation = Cameras[x].MovementQueue.Dequeue();
                newop = false;

                // If no effect operation is currently happening, or a non-blocking operation is happening, 
                //   load the next operation.
                if (Cameras[x].CurEffOperation == null)
                    newop = true;
                else if (Cameras[x].CurEffOperation.Blocking == false && Cameras[x].EffectQueue.Count > 0)
                    newop = true;

                // Make the current Effect operation the next operation
                if (newop && Cameras[x].EffectQueue.Count > 0)
                    Cameras[x].CurEffOperation = Cameras[x].EffectQueue.Dequeue();
                newop = false;

                // Now process the operations
                Camera = Cameras[x].Camera;
                CurEffOperation = Cameras[x].CurEffOperation;
                CurMovOperation = Cameras[x].CurMovOperation;
                if (CurMovOperation != null)
                    mParams = Cameras[x].CurMovOperation.Parameters;
                else
                    mParams = null;
                if (CurEffOperation != null)
                    eParams = Cameras[x].CurEffOperation.Parameters;
                else
                    eParams = null;

                // Movement operation first
                if (CurMovOperation != null)
                {
                    switch (CurMovOperation.OperationType)
                    {
                        case CameraOperationTypes.LookAtPosition:
                            ProcessLookAtPositionOperation(Cameras[x], dt);
                            break;

                        case CameraOperationTypes.LookAtObject:
                            ProcessLookAtObjectOperation(Cameras[x], dt);
                            break;

                        case CameraOperationTypes.FollowObject:
                            ProcessFollowObjectOperation(Cameras[x], dt);
                            break;

                        case CameraOperationTypes.Shake:
                            ProcessShakeOperation(Cameras[x], dt);
                            break;

                        case CameraOperationTypes.StopMove:
                            // Stopmove is functioning as a delay, so just reduce the time
                            mParams[0] -= dt;
                            break;
                    }
                }

                // Process the effect operations
                if (CurEffOperation != null)
                {
                    switch (CurEffOperation.OperationType)
                    {
                        case CameraOperationTypes.Rotate:
                            ProcessRotateOperation(Cameras[x], dt);
                            break;

                        case CameraOperationTypes.StopEffect:
                            // StopEffect is functioning as a delay, so just reduce the time
                            mParams[0] -= dt;
                            break;
                    }
                }

              
            }
        }

        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public CameraData GetCameraData(string cameraName)
        {
            if (cameraName == "")
                return null;

            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera.Name == cameraName)
                    return Cameras[x];

            return null;
        }

        public void PauseCamera(string cameraName)
        {
            CameraData cam = null;
            
            if (cameraName == "")
                return;

            // Find the camera
            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera.Name == cameraName)
                    cam = Cameras[x];

            if (cam == null)
                return;

            // Mark the camera as paused
            cam.isPaused = true;

            StopCameraEffects(cam);
            StopCameraMovements(cam);
        }

        public void UnPauseCamera(string cameraName)
        {
            CameraData cam = null;

            if (cameraName == "")
                return;

            // Find the camera
            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera.Name == cameraName)
                    cam = Cameras[x];

            if (cam == null)
                return;

            // Mark the camera as unpaused
            cam.isPaused = false;
        }

        /// <summary>
        /// Adds a camera to the camera manager
        /// </summary>
        /// <param name="cameraObject">Object for the camera you want to add</param>
        public void AddCamera(T2DSceneCamera cameraObject)
        {
            if (cameraObject == null)
                return;

            // Make sure we aren't adding a duplicate camera
            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera == cameraObject)
                    return;

            CameraData newcamera = new CameraData();
            newcamera.Camera = cameraObject;
            Cameras.Add(newcamera);
        }

        /// <summary>
        /// Adds a camera to the manager based on the camera's name
        /// </summary>
        /// <param name="cameraName">Script name of the camera</param>
        public void AddCamera(string cameraName)
        {
            if (cameraName == "")
                return;

            T2DSceneCamera cameraObject = TorqueObjectDatabase.Instance.FindObject<T2DSceneCamera>(cameraName);
            if (cameraObject == null)
                return;

            // Make sure we aren't adding a duplicate camera
            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera == cameraObject)
                    return;

            CameraData newcamera = new CameraData();
            newcamera.Camera = cameraObject;
            Cameras.Add(newcamera);
        }

        /// <summary>
        /// Remove a camera from the manager
        /// </summary>
        /// <param name="cameraName">Script name of the camera to remove</param>
        public void RemoveCamera(string cameraName)
        {
            if (cameraName == "")
                return;

            // Find the camera
            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera.Name == cameraName)
                    Cameras.RemoveAt(x);

            return;
        }

        /// <summary>
        /// Follows an object's position and optionally its rotation.
        /// </summary>
        /// <param name="cameraName">Script name of the camera to move</param>
        /// <param name="obj">Object for the camera to follow</param>
        /// <param name="delay">Delay (in milliseconds) in camera's movement and rotation</param>
        /// <param name="followRotation">Should the camera rotate with the object</param>
        /// <param name="offsetX">Offset amount from the object on the X-axis </param>
        /// <param name="offsetY">Offset amount from the object on the Y-axis</param>
        /// <param name="mode">Interpolation mode for the camera movements</param>
        /// <param name="queue">False to follow immediately, true for it to be queued </param>
        public void FollowObject(string cameraName, T2DSceneObject obj, float delay, bool followRotation,
                                    float offsetX, float offsetY, InterpolationMode mode, bool queue)
        {
            // NOTE: Camera's world limit prevents the camera's edge from moving outside of the world limit.  This
            //   only prevents the camera's position and has no effect on the camera's rotation.  Care must be taken
            //   to make sure if the camera rotates while at the edge 

            // Make sure an object was passed in
            if (obj == null)
                return;

            float rotate;
            if (followRotation)
                rotate = 1;
            else
                rotate = 0;

            // Note: Follow operations should never be marked as blocking as it never ends
            AddOperation(cameraName, queue, false, obj, CameraOperationTypes.FollowObject, mode, delay, offsetX, 
                            offsetY, rotate);
        }

        /// <summary>
        /// Moves a camera to an object
        /// </summary>
        /// <param name="cameraName">Script name of the camera to move</param>
        /// <param name="obj">Object to move the camera too</param>
        /// <param name="time">Time (in seconds) to get the camera in position </param>
        /// <param name="offsetX">X axis offset from the object's location to move to</param>
        /// <param name="offsetY">Y axis offset from the object's location to move to</param>
        /// <param name="mode">Interpolation mode for the camera movements</param>
        /// <param name="queue">False for instant movement, true for the movement to be queued</param>
        /// <param name="blocking">Should other actions wait for this action to finish</param>
        public void MoveCameraTo2DObject(string cameraName, T2DSceneObject obj, float time, float offsetX, 
                                        float offsetY, InterpolationMode mode, bool queue, bool blocking)
        {
            // If obj is null, we shouldn't queue this operation
            if (obj == null)
                return;

            // Last parameter deontes that we have not started moving the camera yet
            AddOperation(cameraName, queue, blocking, obj, CameraOperationTypes.LookAtObject, mode, 
                            time, offsetX, offsetY, 0);
        }
        
        /// <summary>
        /// Moves a camera to to a position
        /// </summary>
        /// <param name="cameraName">Scripting name of the camera to move</param>
        /// <param name="x">X coordinate for camera</param>
        /// <param name="y">Y coordinate for camera</param>
        /// <param name="time">Time (in seconds) to get to the position</param>
        /// <param name="mode">Interpolation mode for the camera movements</param>
        /// <param name="queue">False for instant movement, true for the movement to be queued</param>
        /// <param name="blocking">Should other actions wait for this action to finish</param>
        public void MoveCameraTo2DPosition(string cameraName, float time, float x, float y, InterpolationMode mode,
                                                bool queue, bool blocking)
        {
            // Last parameter denotes if we have told the camera to move.
            AddOperation(cameraName, queue, blocking, null, CameraOperationTypes.LookAtPosition,mode,time,x,y,0);
        }

        /// <summary>
        /// Rotates the camera to a specific rotation
        /// </summary>
        /// <param name="cameraName">Script name of the camera to rotate</param>
        /// <param name="time">Time (in seconds) for the camera to get to the specified rotation</param>
        /// <param name="rotation">Final rotation angle of the camera</param>
        /// <param name="mode">Interpolation mode for the camera rotation</param>
        /// <param name="queue">False for instant rotation, true for rotation to be queued</param>
        /// <param name="blocking">Should other actions wait for this action to finish</param>
        public void Rotate2DCamera(string cameraName, float time, float rotation, InterpolationMode mode,
                                        bool queue, bool blocking)
        {
            // Note: Rotation is marked as an effect instead of movement so you can rotate the camera while moving.
            // Last variable denotes if we are already rotating the camera or not
            AddOperation(cameraName, queue, blocking, null, CameraOperationTypes.Rotate, mode, time, rotation, 0);
        }

        /// <summary>
        /// Shakes the camera
        /// </summary>
        /// <param name="cameraName">Script name of the camera </param>
        /// <param name="duration">Duration (in seconds) of the screen shake</param>
        /// <param name="magnitude">Magnitude of the screen shake</param>
        /// <param name="queue">False to immediately stop the camera effect, true to queue the stop</param>
        /// <param name="blocking">Should other actions wait for this action to finish</param>
        public void ShakeCamera(string cameraName, float duration, float magnitude, bool queue, bool blocking)
        {
            // Last variable determines if we have started the shake or not
            AddOperation(cameraName, queue, blocking, null, CameraOperationTypes.Shake, InterpolationMode.Linear, 
                            duration, magnitude, 0);
        }

        /// <summary>
        /// Stop the camera from processing effects (can be used as a delay or immediately stop effects)
        /// </summary>
        /// <param name="cameraName">Script name of the camera</param>
        /// <param name="queue">False to immediately stop the camera effect, true to queue the stop</param>
        public void StopEffect(string cameraName, float time, bool queue)
        {
            // Note: IF an effect is immediately stopped the effect is not reversed!
            AddOperation(cameraName, queue, true, null, CameraOperationTypes.StopMove, InterpolationMode.Linear, time);
        }

        /// <summary>
        /// Stop the camera from moving (can be used as a delay or immediately stop movement)
        /// </summary>
        /// <param name="cameraName">Script name of the camera</param>
        /// <param name="queue">False to immediately stop the camera, true to queue the stop</param>
        public void StopMovement(string cameraName, float time, bool queue)
        {
            AddOperation(cameraName,queue,true,null,CameraOperationTypes.StopMove,InterpolationMode.Linear, time);
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        /// <summary>
        /// Adds an operation to a camera's queue
        /// </summary>
        /// <param name="cameraName">Script name of the camera to add the operation to</param>
        /// <param name="queue">False for instant movement, true for the movement to be queued</param>
        /// <param name="blocking">Should other actions wait for this action to finish</param>
        /// <param name="obj">Object the operation deals with</param>
        /// <param name="opType">Type of operation for the camera</param>
        /// <param name="mode">Interpolation mode for the camera movements</param>
        /// <param name="incParams">Parameters for the operation</param>
        protected void AddOperation(string cameraName, bool queue, bool blocking, T2DSceneObject obj, 
                                        CameraOperationTypes opType, InterpolationMode mode, params float[] incParams)
        {
            // Variables
            CameraData data = null;
            CameraOperation newOp;

            // Make sure data was passed in and the manager is "active"
            if (cameraName == "" || !_active)
                return;

            // Find the camera in the list
            T2DSceneCamera tempCamera = TorqueObjectDatabase.Instance.FindObject<T2DSceneCamera>(cameraName);
            if (tempCamera == null)
                return; // Camera is not recognized by Tx

            for (int x = 0; x < Cameras.Count; x++)
                if (Cameras[x].Camera == tempCamera)
                    data = Cameras[x];

            if (data == null)
                return; // Camera is not being managed by the manager

            // If the action shouldn't be queued, stop the current operation and clear the camera's queue
            if (!queue)
            {
                // Stop the current action, remove the current operation, then clear the queue
                if (_moveOperations.Contains(opType))
                {

                    StopCameraMovements(data);
                    data.CurMovOperation = null;
                    data.MovementQueue.Clear();
                }
                else if (_effectOperations.Contains(opType))
                {
                    StopCameraEffects(data);
                    data.CurEffOperation = null;
                    data.EffectQueue.Clear();
                }              
            }

            // Add the operation to the queue
            // Note: "non-queued" operations are put in the queue, and started at the next tick
            newOp = new CameraOperation(incParams.Length);
            newOp.Blocking = blocking;
            newOp.Object = obj;
            newOp.OperationType = opType;
            newOp.Parameters = incParams;
            newOp.interpolationMode = mode;

            // Put effects in the effect queue
            if (_effectOperations.Contains(opType))
                data.EffectQueue.Enqueue(newOp);
            else
                data.MovementQueue.Enqueue(newOp);
        }

        /// <summary>
        /// Stops any camera movements
        /// </summary>
        /// <param name="cam">Information for the camera you want to stop</param>
        public void StopCameraMovements(CameraData cam)
        {
            if (cam == null)
                return;

            // Check if the camera is currently doing an action that should be paused
            if (cam.CurMovOperation != null)
            {
                switch (cam.CurMovOperation.OperationType)
                {
                    case CameraOperationTypes.LookAtObject:
                        cam.CurMovOperation.Parameters[3] = 0;
                        cam.Camera.Position = cam.Camera.Position;
                        break;

                    case CameraOperationTypes.LookAtPosition:
                        cam.CurMovOperation.Parameters[3] = 0;
                        cam.Camera.Position = cam.Camera.Position;
                        break;

                    case CameraOperationTypes.FollowObject:
                        // Make sure the camera stops rotating and moving
                        cam.Camera.Position = cam.Camera.Position;
                        cam.Camera.Rotation = cam.Camera.Rotation;
                        break;

                    case CameraOperationTypes.Shake:
                        cam.CurMovOperation.Parameters[2] = 0;
                        cam.Camera.StartShake(0, 0);
                        break;
                }
            }
        }

        /// <summary>
        /// Stops any camera effects
        /// </summary>
        /// <param name="cam">Information for the camera you want to stop</param>
        public void StopCameraEffects(CameraData cam)
        {
            if (cam == null)
                return;

            if (cam.CurEffOperation != null)
            {
                switch (cam.CurEffOperation.OperationType)
                {
                    case CameraOperationTypes.Rotate:
                        cam.CurEffOperation.Parameters[2] = 0;
                        cam.Camera.Rotation = cam.Camera.Rotation;
                        break;
                }
            }
        }

        protected void ProcessRotateOperation(CameraData cam, float dt)
        {
            if (cam == null)
                return;

            T2DSceneCamera Camera = cam.Camera;
            float[] mParams = cam.CurMovOperation.Parameters;
            float[] eParams = cam.CurEffOperation.Parameters;
            CameraOperation CurMovOperation = cam.CurMovOperation;
            CameraOperation CurEffOperation = cam.CurEffOperation;
            float angle;

            // Make sure we aren't rotating the camera already
            if (eParams[2] == 0)
            {
                eParams[2] = 1;

                // Make sure to rotate the camera correctly
                // If no delay, don't bother animating the rotation
                if (eParams[0] == 0)
                    Camera.Rotation = eParams[1];
                else
                {
                    // Make sure the camera rotates in the correct direction

                    if (eParams[1] > Camera.Rotation)
                    {
                        if (Math.Abs(eParams[1] - Camera.Rotation) > 180f)
                            angle = eParams[1] - 360f;
                        else
                            angle = eParams[1];
                    }
                    else
                    {
                        if (Math.Abs(eParams[1] - Camera.Rotation) > 180f)
                            angle = eParams[1] + 360f;
                        else
                            angle = eParams[1];
                    }
                    Camera.AnimateRotation(angle, eParams[0] * 1000, CurEffOperation.interpolationMode);
                }
            }
            eParams[0] -= dt;
        }

        protected void ProcessLookAtObjectOperation(CameraData cam, float dt)
        {
            if (cam == null)
                return;

            T2DSceneCamera Camera = cam.Camera;
            float[] mParams = cam.CurMovOperation.Parameters;
            CameraOperation CurMovOperation = cam.CurMovOperation;
            Vector2 position;

            // If the object's position (+ offset) is different now, re-animate the camera
            //    so the camera still goes to the correct position
            if ((Camera.Position.X != CurMovOperation.Object.Position.X + mParams[1] ||
                 Camera.Position.Y != CurMovOperation.Object.Position.Y + mParams[2]) ||
                    mParams[3] == 0)
            {
                mParams[3] = 1;

                // Move the camera to the object's new position
                position = new Vector2(CurMovOperation.Object.Position.X + mParams[1],
                                             CurMovOperation.Object.Position.Y + mParams[2]);
                if (mParams[0] == 0)
                    Camera.Position = position;
                else
                    Camera.AnimatePosition(position, mParams[0] * 1000, CurMovOperation.interpolationMode);
            }

            // Decrease the time to move the camera
            mParams[0] -= dt;
        }

        protected void ProcessLookAtPositionOperation(CameraData cam, float dt)
        {
            if (cam == null)
                return;

            T2DSceneCamera Camera = cam.Camera;
            float[] mParams = cam.CurMovOperation.Parameters;
            CameraOperation CurMovOperation = cam.CurMovOperation;
            Vector2 position;

            // Check if the camera is already animating towards the position
            if (mParams[3] == 0)
            {
                mParams[3] = 1;
                position = new Vector2(mParams[1], mParams[2]);

                // If time is 0, don't interpolate
                if (mParams[0] == 0)
                    Camera.Position = position;
                else
                    Camera.AnimatePosition(position, mParams[0] * 1000, CurMovOperation.interpolationMode);
            }

            // Reduce the time by dt
            mParams[0] -= dt;
        }

        protected void ProcessFollowObjectOperation(CameraData cam, float dt)
        {
            if (cam == null)
                return;

            T2DSceneCamera Camera = cam.Camera;
            float[] mParams = cam.CurMovOperation.Parameters;
            CameraOperation CurMovOperation = cam.CurMovOperation;
            Vector2 position;
            Vector2 offset;
            float angle;
            float distance;

            offset.X = mParams[1];
            offset.Y = mParams[2];

            // Now rotate the camera
            if (mParams[3] == 1)
            {
                // Set the camera's new angle to match the object's
                // If no delay, don't bother animating the rotation
                if (mParams[0] == 0)
                    Camera.Rotation = CurMovOperation.Object.Rotation;
                else
                {
                    // Make sure the camera rotates in the correct direction

                    if (CurMovOperation.Object.Rotation > Camera.Rotation)
                    {
                        if (Math.Abs(CurMovOperation.Object.Rotation - Camera.Rotation) > 180f)
                            angle = CurMovOperation.Object.Rotation - 360f;
                        else
                            angle = CurMovOperation.Object.Rotation;
                    }
                    else
                    {
                        if (Math.Abs(CurMovOperation.Object.Rotation - Camera.Rotation) > 180f)
                            angle = 360f + CurMovOperation.Object.Rotation;
                        else
                            angle = CurMovOperation.Object.Rotation;
                    }
                    Camera.AnimateRotation(angle, mParams[0], CurMovOperation.interpolationMode);
                }

                // Calculate angle from offset
                angle = (float)Math.Atan(offset.Y / offset.X);

                // Correct for negative values
                if (offset.X < 0)
                    angle += MathHelper.Pi;

                distance = offset.Length();
                offset.X = (float)Math.Cos(angle + MathHelper.ToRadians(Camera.Rotation)) * distance;
                offset.Y = (float)Math.Sin(angle + MathHelper.ToRadians(Camera.Rotation)) * distance;
            }

            // Move the camera to the object's new position
            position = new Vector2(CurMovOperation.Object.Position.X + offset.X,
                                             CurMovOperation.Object.Position.Y + offset.Y);

            if (mParams[0] == 0)
                Camera.Position = position;
            else
                Camera.AnimatePosition(position, mParams[0], CurMovOperation.interpolationMode);

                            
        }

        protected void ProcessShakeOperation(CameraData cam, float dt)
        {
            if (cam == null)
                return;

            T2DSceneCamera Camera = cam.Camera;
            float[] mParams = cam.CurMovOperation.Parameters;
            CameraOperation CurMovOperation = cam.CurMovOperation;

            // Check to make sure we haven't already told the camera to shake
            if (mParams[2] == 0)
            {
                mParams[2] = 1;
                Camera.StartShake(mParams[1], mParams[0] * 1000);
            }
            mParams[0] -= dt;
        }

        #endregion

        //======================================================
        #region Private, protected, internal fields
        static readonly CameraManager _instance = new CameraManager();
        protected bool _active;
        protected List<CameraData> Cameras;

        // Lists denoting which operations are effects and which are movements, for cleaner code.
        //   Lists are populated in the constructor
        protected List<CameraOperationTypes> _moveOperations;
        protected List<CameraOperationTypes> _effectOperations;
        #endregion
    }
}
