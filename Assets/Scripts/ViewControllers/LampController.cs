using UnityEngine;
using System.Collections;

public class LampController : MonoBehaviour {
    
    private Renderer renderer;
    private Color initialAlbedo;

    private Color lightColor = Color.white;
    private bool turnedOn = false;

    void Awake()
    {
        renderer = GetComponent<Renderer>();
        initialAlbedo = renderer.sharedMaterials[1].color;
    }
    
    public void SetLightColor(Color color)
    {
        // performance optimization
        if (lightColor == color)
            return;

        lightColor = color;
        UpdateView();
    }

    public void SetTurnedOn(bool flag)
    {
        // performance optimization
        if (turnedOn == flag)
            return;

        turnedOn = flag;
        UpdateView();
    }

    private void UpdateView()
    {
        Material glassMat = renderer.materials[1];

        if (turnedOn)
        {
            // set albedo to black so the emission is more visible
            glassMat.color = Color.black;
            // set emission color and make sure the shader is processing emissions
            glassMat.SetColor("_EmissionColor", lightColor * 0.7f);
            glassMat.EnableKeyword("_EMISSION");
        }
        else
        {
            // reset to initial state
            glassMat.color = initialAlbedo;
            glassMat.SetColor("_EmissionColor", Color.black);
            glassMat.DisableKeyword("_EMISSION");
        }
    }

}
