using UnityEngine;

public class BlurRender : MonoBehaviour
{
    public Material blurMaterial;
    public string _textureName = "_RenderTex";

    void OnEnable()
    {
        if (blurMaterial.GetTexture(_textureName) == null)
        {
            var texture = new RenderTexture(Screen.width, Screen.height, 24, RenderTextureFormat.ARGB32, 1);
            blurMaterial.SetTexture(_textureName, texture);
        }

        var renderTexture = blurMaterial.GetTexture(_textureName) as RenderTexture;

        var camera = Camera.main;
        
        if(camera == null || renderTexture == null)
        {
            return;
        }

        camera.targetTexture = renderTexture;
        camera.Render();
        camera.targetTexture = null;
    }
}
