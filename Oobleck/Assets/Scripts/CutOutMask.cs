using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class CutOutMask : Image
{
    //followed code monkey tutorial for this
    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetFloat("_StencilComp", (float)CompareFunction.NotEqual);
            return material;
        }
    }
}
