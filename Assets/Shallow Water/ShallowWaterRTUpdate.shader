Shader "Skywater/Shallow Water/Self/ShallowWaterRTUpdate"
{
    Properties
    {
        [PowerSlider(0.01)]
        _dampening("Dampening", Range(0.0, 1.0)) = 0.999
		_inputSize("InputSize", Float) = 5
		_inputPush("Input Push", Float) = 0
    }
    CGINCLUDE
    
    #include "UnityCustomRenderTexture.cginc"
    
    float _dampening;
    
    float _inputX;
    float _inputY;
    float _gotInput;
    float _inputSize;
    float _minInputSize;
    float _inputPush;
    
    float _dripInputX;
    float _dripInputY;
    float _gotDrip;
    float _dripSize;
    
    float4 frag(v2f_customrendertexture i) : SV_Target
    {
        //Based on https://www.shadertoy.com/view/Xsd3DB
        float2 resolution = float2(_CustomRenderTextureWidth, _CustomRenderTextureHeight);
        float2 uv = i.globalTexcoord;
        
        float3 e = float3(float2(1.0, 1.0) / resolution.xy, 0.0);
        
        float2 c = tex2D(_SelfTexture2D, uv);
        
        float p10 = tex2D(_SelfTexture2D, uv - e.zy).x;
        float p01 = tex2D(_SelfTexture2D, uv - e.xz).x;
        float p21 = tex2D(_SelfTexture2D, uv + e.xz).x;
        float p12 = tex2D(_SelfTexture2D, uv + e.zy).x;
        
        
        float d = 0.0;
        float screenscale = resolution.x / 640.0;
        
        if(_gotInput > 0.1)
        {
            //d = smoothstep(_inputSize, 0.0, length(float2(_inputX, _inputY) - uv.xy) * resolution);
            
            float val = 0.05 * smoothstep(_inputSize * screenscale, _minInputSize * screenscale, length(float2(_inputX, _inputY) - uv.xy) * resolution);
            if(_inputPush > 0.1f)
                val *= -1.0;
            d += val;
        }
        if(_gotDrip> 0.1)
        {
            //d = 0.2 * smoothstep(_dripSize, 0.0, length(float2(_dripInputX, _dripInputY) - uv.xy) * resolution);
            d += 0.2 * smoothstep(_dripSize * screenscale, 5.0 * screenscale, length(float2(_dripInputX, _dripInputY) - uv.xy) * resolution);
        }
        
        // The actual propagation:
        d += -(c.y - 0.5) * 2.0 + (p10 + p01 + p21 + p12 - 2.0);
        //d += -(c.y - 0.5) * 2.0 + (p10 + p01 + p21 + p12 - 2.0);
        d *= _dampening;
        d = d * 0.5 + 0.5;
        
        return float4(d, c.x, 0, 0);
    }
    
    ENDCG
    
    SubShader
    {
        Cull Off ZWrite Off ZTest Always
    
        Pass
        {
            Name "Update"
            CGPROGRAM
            #pragma vertex CustomRenderTextureVertexShader
            #pragma fragment frag
            ENDCG
        }
    }
}
