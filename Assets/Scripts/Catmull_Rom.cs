using UnityEngine;
using System.Collections.Generic;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class Catmull_Rom : MonoBehaviour
{
	[SerializeField] private List<Transform> points = new List<Transform>();
	[SerializeField] private int segmentFrameCount;

	private Vector3 _p0;
	private Vector3 _m0;
	private Vector3 _p1;
	private Vector3 _m1;

	private LineRenderer lineRenderer;

	private void Awake()
    {
		lineRenderer = GetComponent<LineRenderer>();
    }

	void Update()
	{
		DrawCurve();
	}

	private void DrawCurve()
    {
		for (int i = 0; i < points.Count - 1; i++)
		{
			_p0 = points[i].position;
			_p1 = points[i + 1].position;

			_m0 = i > 0 ? 0.5f * (points[i + 1].position - points[i - 1].position) : points[i + 1].position - points[i].position;

			_m1 = i < points.Count - 2 ? 0.5f * (points[i + 2].position - points[i].position) : points[i + 1].position - points[i].position;

			Vector3 curvePos;
			float _t;
			float uStep = 1.0f / segmentFrameCount;

			if (i == points.Count - 2)
			{
				uStep = 1.0f / (segmentFrameCount - 1.0f);
			}
			for (int j = 0; j < segmentFrameCount; j++)
			{
				_t = j * uStep;
				curvePos = PositionOnCurve(_p0, _m0, _p1, _m1, _t);
				lineRenderer.SetPosition(i * segmentFrameCount + j, curvePos);
			}
		}
		lineRenderer.positionCount = segmentFrameCount * (points.Count - 1);
	}

    private Vector3 PositionOnCurve(Vector3 p0, Vector3 m0, Vector3 p1, Vector3 m1, float t)
    {
		Vector3 position= (2.0f * t * t * t - 3.0f * t * t + 1.0f) * p0
					    + (t * t * t - 2.0f * t * t + t) * m0
					    + (-2.0f * t * t * t + 3.0f * t * t) * p1
					    + (t * t * t - t * t) * m1;
		return position;
	}
}
