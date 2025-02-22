using UnityEngine;

public class ImpactEffect : MonoBehaviour
{
    public void SetColor(Color color)
    {
        var line = GetComponentInChildren<LineRenderer>();
        if (line)
        {
            var gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] 
                { 
                    new GradientColorKey(color, 0.0f), 
                }, 
                new GradientAlphaKey[] 
                { 
                    new GradientAlphaKey(color.a, 0.0f), 
                });
            line.colorGradient = gradient;
        }
    }

}
