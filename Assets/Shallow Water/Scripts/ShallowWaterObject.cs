using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyWater
{
    public class ShallowWaterObject : MonoBehaviour
    {
        [SerializeField] bool _useFixedUpdate;
        [SerializeField] protected float _inputSize = 20;
        [SerializeField] protected float _minInputSize = 5;
        [SerializeField] protected bool _inputPush = false;



        protected virtual void Update()
        {
            if (!_useFixedUpdate)
                DoUpdate();
        }
        void FixedUpdate()
        {
            if (_useFixedUpdate)
                DoUpdate();
        }

        protected virtual void DoUpdate()
        {

        }
    }
}