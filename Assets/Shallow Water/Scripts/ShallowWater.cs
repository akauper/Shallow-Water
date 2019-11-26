using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SkyWater
{
    [RequireComponent(typeof(Renderer))]
    public class ShallowWater : MonoBehaviour
    {
        public enum UpdateMode
        {
            Update,
            FixedUpdate,
            RTUpdate
        }
        
        public const string RelativeFolderPath = "Assets/";

        static readonly int ShallowWaveBufferID = Shader.PropertyToID("_ShallowWaveBuffer");
        static readonly int ShallowWaveResolution = Shader.PropertyToID("_ShallowWaveResolution");
        static readonly int DampeningID = Shader.PropertyToID("_dampening");
        
        static readonly int InputSizeID = Shader.PropertyToID("_inputSize");
        static readonly int MinInputSizeID = Shader.PropertyToID("_minInputSize");
        static readonly int GotInputID = Shader.PropertyToID("_gotInput");
        static readonly int InputPushID = Shader.PropertyToID("_inputPush");
        static readonly int InputXID = Shader.PropertyToID("_inputX");
        static readonly int InputYID = Shader.PropertyToID("_inputY");

        static readonly int DripSizeID = Shader.PropertyToID("_dripSize");
        static readonly int GotDripID = Shader.PropertyToID("_gotDrip");
        static readonly int DripInputXID = Shader.PropertyToID("_dripInputX");
        static readonly int DripInputYID = Shader.PropertyToID("_dripInputY");

        MeshCollider _meshCollider;
        public MeshCollider meshCollider => _meshCollider != null ? _meshCollider : _meshCollider = GetComponent<MeshCollider>();

        [SerializeField] Shader _updateShader;
        [SerializeField] Shader _initShader;
        Renderer _renderer;
        MaterialPropertyBlock _propertyBlock;


        [Space(10)]
        
        [SerializeField] Vector2 _resolution = new Vector2(512, 512);
        Vector2 _prevResolution;
        [SerializeField, Range(0.97f, 0.999f)] float _dampening = 0.99f;
        float _prevDampening;

        [Space(10)]
        
        [SerializeField] UpdateMode _updateMode = UpdateMode.FixedUpdate;
        [SerializeField, Range(1, 8)] int _iterationsPerUpdate = 2;


        
        Material _updateMat;
        Material _initMat;
        CustomRenderTexture _rt;
        
        
        Vector2 _curInputPosition;
        float _curInputSize;
        float _minInputSize;
        bool _didHit;
        bool _inputPush;
        float _hitTime;
        float _clearHitTime;

        Vector2 _curDripPosition;
        float _curDripSize;
        bool _didDrip;


        void Awake()
        {
            _renderer = GetComponent<Renderer>();
            _propertyBlock = new MaterialPropertyBlock();
        }
        
        void Start()
        {
            _initMat = new Material(_initShader);
            _updateMat = new Material(_updateShader);

            _rt = CreateRenderTexture();
            _updateMat.SetFloat(DampeningID, _dampening);

            _prevResolution = _resolution;
            _prevDampening = _dampening;

            InitializeRT();

            _renderer.GetPropertyBlock(_propertyBlock);
            _propertyBlock.SetTexture(ShallowWaveBufferID, _rt);
            _renderer.SetPropertyBlock(_propertyBlock);
        }

        CustomRenderTexture CreateRenderTexture()
        {
            var newRenderTexture = new CustomRenderTexture((int) _resolution.x, (int) _resolution.y, RenderTextureFormat.RGFloat, RenderTextureReadWrite.Linear)
            {
                initializationMaterial = _initMat,
                initializationMode = CustomRenderTextureUpdateMode.OnDemand,
                initializationSource = CustomRenderTextureInitializationSource.Material,
                material = _updateMat,
                updateMode = CustomRenderTextureUpdateMode.OnDemand,
                doubleBuffered = true,
                autoGenerateMips = true,
                useMipMap = true,
                filterMode = FilterMode.Trilinear
            };
            
            newRenderTexture.updateMode = _updateMode == UpdateMode.RTUpdate ? CustomRenderTextureUpdateMode.Realtime : CustomRenderTextureUpdateMode.OnDemand;
            return newRenderTexture;
        }
        void InitializeRT()
        {
            if (_rt)
                _rt.Initialize();
        }

        void Update()
        {
            CheckValueChanges();
            UpdateMaterialValues();
            
            
            if (_updateMode == UpdateMode.Update)
                UpdateRenderTexture();

            if (_didHit && !Mathf.Approximately(_clearHitTime, -1))
            {
                if (Time.time > _hitTime + _clearHitTime)
                    ClearInput();
            }
        }

        void CheckValueChanges()
        {
            if (!Mathf.Approximately(_prevDampening, _dampening))
            {
                _updateMat.SetFloat(DampeningID, _dampening);
                _prevDampening = _dampening;
            }

            if (_resolution != _prevResolution)
            {
                _updateMat.SetVector(ShallowWaveResolution, _resolution);
                _prevResolution = _resolution;
            }
        }
        

        void FixedUpdate()
        {
            if (_updateMode == UpdateMode.FixedUpdate)
                UpdateRenderTexture();
        }

        void UpdateMaterialValues()
        {
            _updateMat.SetFloat(InputXID, _curInputPosition.x);
            _updateMat.SetFloat(InputYID, _curInputPosition.y);
            _updateMat.SetFloat(InputSizeID, _curInputSize);
            _updateMat.SetFloat(MinInputSizeID, _minInputSize);
            _updateMat.SetFloat(GotInputID, _didHit ? 1 : 0);
            _updateMat.SetFloat(InputPushID, _inputPush ? 1 : 0);
            _updateMat.SetFloat(DripInputXID, _curDripPosition.x);
            _updateMat.SetFloat(DripInputYID, _curDripPosition.y);
            _updateMat.SetFloat(DripSizeID, _curDripSize);
            _updateMat.SetFloat(GotDripID, _didDrip ? 1 : 0);
        }

        void UpdateRenderTexture()
        {
            _rt.Update(_iterationsPerUpdate);
        }
        
        public void SetInputPosition(Vector2 texCoordPosition, float inputSize, float minInputSize, bool inputPush, float clearHitTime = -1)
        {
            _curInputPosition = texCoordPosition;
            _curInputSize = inputSize;
            _minInputSize = minInputSize;
            _didHit = true;
            _inputPush = inputPush;
            _hitTime = Time.time;
            _clearHitTime = clearHitTime;
        }

        public void ClearInput()
        {
            _didHit = false;
        }

        public void SetDripPosition(Vector2 texCoordPosition, float inputSize)
        {
            _curDripPosition = texCoordPosition;
            _curDripSize = inputSize;
            _didDrip = true;
        }

        public void ClearDrip()
        {
            _didDrip = false;
        }

        public void OnValidate()
        {
#if UNITY_EDITOR
            if (_initShader == null)
                _initShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>(RelativeFolderPath + "/Shallow Water/ShallowWaterRTInit.shader");
            if (_updateShader == null)
                _updateShader = UnityEditor.AssetDatabase.LoadAssetAtPath<Shader>(RelativeFolderPath + "/Shallow Water/ShallowWaterRTUpdate.shader");
#endif
        }
        
        #if UNITY_EDITOR
        [ContextMenu("Generate Mesh Collider")]
        void GenerateMeshCollider()
        {
            var oldRot = transform.rotation;
            var oldPos = transform.position;
            transform.rotation = Quaternion.identity;
            transform.position = Vector3.zero;
            
            
            MeshFilter[] _meshFilters = GetComponentsInChildren<MeshFilter>();
            CombineInstance[] combine = new CombineInstance[_meshFilters.Length];

            int i = 0;
            while (i < _meshFilters.Length)
            {
                combine[i].subMeshIndex = 0;
                combine[i].mesh = _meshFilters[i].sharedMesh;
                combine[i].transform = _meshFilters[i].transform.localToWorldMatrix;
                i++;
            }
            Mesh newMesh = new Mesh();
            newMesh.name = "Combined Mesh";
            newMesh.CombineMeshes(combine, true, true, false);

            MeshCollider meshCollider = GetComponent<MeshCollider>();
            if (meshCollider)
                DestroyImmediate(meshCollider.sharedMesh);
            else
                meshCollider = gameObject.AddComponent<MeshCollider>();

            meshCollider.sharedMesh = newMesh;
            //_col = meshCollider;

            transform.rotation = oldRot;
            transform.position = oldPos;
        }
        #endif
    }
}