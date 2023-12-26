using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct PostShaderSetup
{
    public int Pixels;
    public float OutlineSize;
    public float CameraDepth;
}

public class PostProcessController : MonoBehaviour
{
    public Material PostProcessMat;
    public PostShaderSetup ShaderSetup;

    // Start is called before the first frame update
    void Start()
    {
        Shader targetShader = Shader.Find("Custom/Sh_PostPixelation");
        PostProcessMat = new Material(targetShader);
        PostProcessMat.name = "My Shader";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (!PostProcessMat) return;
        else
        {
            PostProcessMat.SetInt("_Pixels", ShaderSetup.Pixels);
            PostProcessMat.SetFloat("_OutlineLength", ShaderSetup.OutlineSize);
            Graphics.Blit(src, dest, PostProcessMat);
        }
    }
}
