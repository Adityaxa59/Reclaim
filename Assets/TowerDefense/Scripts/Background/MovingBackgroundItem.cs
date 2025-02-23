using UnityEngine;

public class BackgroundItem : ManagedBehaviour
{
    [SerializeField] SpriteRenderer icon;

    [SerializeField] Gradient colorGradient;
    [SerializeField] AnimationCurve scaleCurve;
    [SerializeField] AnimationCurve positionCurve;
    [SerializeField] AnimationCurve rotationCurve;

    public float colorMaxDist = 10;

    public override void ManagedUpdate()
    {
        var cursorPos = Helpers.Camera.ScreenToWorldPoint(Input.mousePosition);
        cursorPos.z = transform.position.z;

        var dir = (cursorPos - transform.position).normalized;
        var dist = Vector3.Distance(transform.position, cursorPos);

        icon.transform.localScale = Vector3.one * scaleCurve.Evaluate(dist);

        //icon.transform.rotation = Quaternion.Euler(0, 0, rotationCurve.Evaluate(dist) * 90);

        icon.transform.localPosition = dir * positionCurve.Evaluate(dist);

        icon.color = colorGradient.Evaluate(dist / colorMaxDist);
    }
}