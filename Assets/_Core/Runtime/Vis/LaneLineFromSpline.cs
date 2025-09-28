using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineContainer))]
[RequireComponent(typeof(LineRenderer))]
public class LaneLineFromSpline : MonoBehaviour
{
    public float yOffset = 0.05f;
    [Min(0.05f)] public float width = 0.35f;
    [Min(0.1f)] public float samplesPerMeter = 0.8f; // raise for smoother

    SplineContainer _sc;
    LineRenderer _lr;

    void Reset()
    {
        _lr = GetComponent<LineRenderer>();
        _lr.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        _lr.receiveShadows = false;
        _lr.textureMode = LineTextureMode.Stretch;
        _lr.alignment = LineAlignment.View;
        _lr.numCornerVertices = 4;
        _lr.useWorldSpace = true;
        _lr.material = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        _lr.material.SetColor("_BaseColor", new Color(0.2f, 0.9f, 1f, 1f));
    }

    void OnEnable(){ Rebuild(); }
    void OnValidate(){ if (isActiveAndEnabled) Rebuild(); }

    public void Rebuild()
    {
        if (!_sc) _sc = GetComponent<SplineContainer>();
        if (!_lr) _lr = GetComponent<LineRenderer>();
        if (_sc == null || _sc.Spline == null) return;

        // length & samples
        float length = SplineUtility.CalculateLength(_sc.Spline, _sc.transform.localToWorldMatrix);
        int steps = Mathf.Max(8, Mathf.CeilToInt(length * samplesPerMeter));

        var pts = new Vector3[steps + 1];
        for (int i = 0; i <= steps; i++)
        {
            float t = i / (float)steps; // 0..1
            Vector3 local = (Vector3)SplineUtility.EvaluatePosition(_sc.Spline, t);
            pts[i] = _sc.transform.TransformPoint(local) + Vector3.up * yOffset;
        }

        _lr.positionCount = pts.Length;
        _lr.SetPositions(pts);
        _lr.startWidth = _lr.endWidth = width;
    }
}
