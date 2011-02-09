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
    public class LifetimeComponent : TorqueComponent, ITickObject
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

        [TorqueXmlSchemaType(DefaultValue = "1.0")]
        public float Lifetime
        {
            get { return _lifetime; }
            set { _lifetime = value; }
        }

        public OnLifetimeDelegate OnLifetime
        {
            get { return _lifetimers; }
            set { _lifetimers = value; }
        }

        public delegate void OnLifetimeDelegate(T2DSceneObject obj);

        #endregion

        //======================================================
        #region Public methods

        public virtual void ProcessTick(Move move, float dt)
        {
            if (_sceneObject != null && _spawnTime.CompareTo(DateTime.Now.AddSeconds(-_lifetime)) < 0)
            {
                if (_lifetimers != null)
                    _lifetimers(_sceneObject);
                TorqueObjectDatabase.Instance.Unregister(_sceneObject);
            }
        }

        public virtual void InterpolateTick(float k)
        {
            // todo: interpolate between ticks as needed here
        }

        public override void CopyTo(TorqueComponent obj)
        {
            LifetimeComponent obj2 = (LifetimeComponent)obj;
            base.CopyTo(obj);

            obj2.Lifetime = _lifetime;
            obj2.OnLifetime = null;
        }

        #endregion

        //======================================================
        #region Private, protected, internal methods

        protected override bool _OnRegister(TorqueObject owner)
        {
            if (!base._OnRegister(owner) || !(owner is T2DSceneObject))
                return false;

            _sceneObject = (T2DSceneObject)owner;
            _spawnTime = DateTime.Now;
            // todo: perform initialization for the component

            // todo: look up interfaces exposed by other components
            // E.g., 
            // _theirInterface = Owner.Components.GetInterface<ValueInterface<float>>("float", "their interface name");            
            ProcessList.Instance.AddTickCallback(Owner, this);

            return true;
        }

        protected override void _OnUnregister()
        {
            // todo: perform de-initialization for the component
            _lifetimers = null;
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
        T2DSceneObject _sceneObject;
        float _lifetime;
        DateTime _spawnTime;
        OnLifetimeDelegate _lifetimers = null;
        #endregion
    }
}
