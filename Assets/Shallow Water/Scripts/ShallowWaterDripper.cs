using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyWater
{
    public class ShallowWaterDripper : ShallowWaterObject
    {
//        [SerializeField] Range _dripIntervalRange = new Range(0.5f, 1.5f);
//
//        [Header("Debug")] [SerializeField] bool _drawRay;
//
//        Timer _dripTimer;
//        ShallowWater _lastWater;
//
//
//        void Awake()
//        {
//            _dripTimer = new Timer(_dripIntervalRange.randomValue);
//        }
//
//        protected override void Update()
//        {
//            base.Update();
//
//            _dripTimer.Tick();
//            if (_dripTimer.complete)
//            {
//                Drip();
//                _dripTimer = new Timer(_dripIntervalRange.randomValue);
//            }
//        }
//
//        void Drip()
//        {
//            Ray ray = new Ray(transform.position, Vector3.down);
//
//            if (_drawRay)
//                Debug.DrawRay(ray.origin, ray.direction * 25);
//
//            RaycastHit hit;
//            if (Physics.Raycast(ray, out hit, Mathf.Infinity, _waterLayers))
//            {
//                ShallowWater water = hit.collider.GetComponent<ShallowWater>();
//                if (water)
//                {
//                    water.SetDripPosition(hit.textureCoord, _inputSize);
//                    _lastWater = water;
//                    Invoke(nameof(ClearDrip), 0.06f);
//                    return;
//                }
//            }
//        }
//
//        void ClearDrip()
//        {
//            _lastWater.ClearDrip();
//        }
    }
}