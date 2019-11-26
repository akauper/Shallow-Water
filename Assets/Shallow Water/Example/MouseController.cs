using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SkyWater
{
    public class MouseController : ShallowWaterObject
    {
        [SerializeField] ShallowWater _shallowWater;

        protected override void DoUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (_shallowWater.meshCollider.Raycast(ray, out hit, Mathf.Infinity))
                {
                    _shallowWater.SetInputPosition(hit.textureCoord, _inputSize, _minInputSize, _inputPush);
                }
                else
                {
                    _shallowWater.ClearInput();
                }
            }
            else
            {
                _shallowWater.ClearInput();
            }
        }
    }
}