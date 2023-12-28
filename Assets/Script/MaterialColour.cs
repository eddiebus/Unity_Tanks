using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class MaterialColour : MonoBehaviour
{
    public Color Colour;
    private GameObject rootObj;
    private Renderer[] meshR;
    // Start is called before the first frame update
    void Start()
    {
        rootObj = transform.root.gameObject;
        meshR = rootObj.GetComponentsInChildren<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        _UpdateMaterials();
    }

    private void _UpdateMaterials(){
        if (!Application.isPlaying) return;
        foreach (var meshRenderer in meshR){
            var mat = meshRenderer.material;
            mat.SetColor("_Color",Colour);
        }
    }
}
