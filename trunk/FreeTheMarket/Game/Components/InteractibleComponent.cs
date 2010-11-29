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

using Microsoft.Xna.Framework;

using GarageGames.Torque.Core;
using GarageGames.Torque.Util;
using GarageGames.Torque.Sim;
using GarageGames.Torque.T2D;
using GarageGames.Torque.SceneGraph;
using GarageGames.Torque.MathUtil;

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

        public T2DSceneObject SceneObject
        {
            get { return Owner as T2DSceneObject; }
        }

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
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
                        System.Console.WriteLine("Could Interact Now");
                    }
                    else
                    {
                        System.Console.WriteLine("");
                    }
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


        #endregion

        //======================================================
        #region Private, protected, internal fields
        
        // Keep pointer to scene object
        T2DSceneObject _sceneObject;
        
        #endregion
    }
}
