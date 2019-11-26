using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyWater
{
    public class ShallowWaterCharacter : ShallowWaterObject
    {
        ShallowWater _lastWater;

        [SerializeField] protected LayerMask _waterLayers;
        [Header("Debug")] [SerializeField] bool _drawRay;

        protected override void DoUpdate()
        {
            Ray ray = new Ray(transform.position + transform.up * 5, -transform.up);

            if (_drawRay)
                Debug.DrawRay(ray.origin, ray.direction * 5, Color.red);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 5, _waterLayers))
            {
                ShallowWater water = hit.collider.GetComponent<ShallowWater>();
                if (water)
                {
                    Debug.DrawRay(hit.point, hit.normal * 10, Color.green);
                    water.SetInputPosition(hit.textureCoord, _inputSize, _minInputSize, _inputPush);
                    _lastWater = water;
                    return;
                }
            }

            if (_lastWater != null)
            {
                _lastWater.ClearInput();
                _lastWater = null;
            }
        }
    }
}
