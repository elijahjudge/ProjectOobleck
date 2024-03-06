using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialChanger : MonoBehaviour
{

    public delegate void MaterialFlash(GameObject gameObject, Material flashMaterial, float duration);
    public delegate void MaterialDelegate(GameObject gameObject, Material flashMaterial);
    public delegate void MaterialDelegate2(GameObject gameObject);

    public static MaterialFlash flashMaterial;
    public static MaterialDelegate changeMaterial;
    public static MaterialDelegate2 returnOriginalMaterial;

    [SerializeField] List<Renderer> renderers = new List<Renderer>();
    private List<Material> originalMaterials = new List<Material>();
    private Coroutine flashing;
    // Start is called before the first frame update
    void Awake()
    {
        foreach (Renderer renderer in renderers)
        {
            originalMaterials.Add(renderer.material);
        }

        flashMaterial += FlashMaterialForDuration;
        changeMaterial += ChangeMaterial;
        returnOriginalMaterial += ReturnOriginalMaterial;
    }
    public void FlashMaterialForDuration(GameObject gameObject, Material flashMaterial, float duration)
    {
        if (this.gameObject.GetRootGameObject() != gameObject.GetRootGameObject())
            return;

        if(flashing != null)
            StopCoroutine(flashing);    

        flashing = StartCoroutine(FlashRoutine(flashMaterial, duration));
    }

    IEnumerator FlashRoutine(Material flashMaterial, float duration)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = flashMaterial;
        }

        yield return new WaitForSeconds(duration);

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }


    public void ChangeMaterial(GameObject gameObject,Material newMaterial)
    {
        if (this.gameObject.GetRootGameObject() != gameObject.GetRootGameObject())
            return;

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = newMaterial;
        }
    }

    public void ReturnOriginalMaterial(GameObject gameObject)
    {
        if (this.gameObject.GetRootGameObject() != gameObject.GetRootGameObject())
            return;

        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].material = originalMaterials[i];
        }
    }
}
