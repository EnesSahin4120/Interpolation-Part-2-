using UnityEngine;

[ExecuteInEditMode, RequireComponent(typeof(LineRenderer))]
public class Kochanek_Bartels : MonoBehaviour
{
	[SerializeField] private CurveParameterInfo[] curveParameterInfos;
	[SerializeField] private int segmentFrameCount;

	private Transform[] points;
	private float[] tension;
	private float[] bias;
	private float[] continuity;

	private Vector3 _p_1;
	private Vector3 _p0;
	private Vector3 _p1;
	private Vector3 _p2;
	private Vector3 _m0;
	private Vector3 _m1;

	private LineRenderer lineRenderer;

	private void Awake()
    {
		lineRenderer = GetComponent<LineRenderer>();
		GetCurveParameters();
    }

	void Update()
	{
		DrawCurve();
	}

	private void GetCurveParameters()
    {
		int pointCount = curveParameterInfos.Length;

		points = new Transform[pointCount];
		tension = new float[pointCount];
		bias = new float[pointCount];
		continuity = new float[pointCount];

		for(int i = 0; i < curveParameterInfos.Length; i++)
        {
			points[i] = curveParameterInfos[i].point;
			tension[i] = curveParameterInfos[i].tension;
			bias[i] = curveParameterInfos[i].bias;
			continuity[i] = curveParameterInfos[i].continuity;
		}
	}

	private void DrawCurve()
    {
		for (int i = 0; i < points.Length - 1; i++)
		{
			_p_1 = i == 0 ? points[i].position : points[i - 1].position;
			_p0 = points[i].position;
			_p1 = points[i + 1].position;
			_p2 = i + 2 >= points.Length ? points[i + 1].position : points[i + 2].position;

			float cur_t = tension[i];
			float cur_b = bias[i];
			float cur_c = continuity[i];
			_m0 = 0.5f * (1 - cur_t) * ((1 + cur_b) * (1 - cur_c) * (_p0 - _p_1) + (1 - cur_b) * (1 + cur_c) * (_p1 - _p0));
			cur_t = tension[i + 1];
			cur_b = bias[i + 1];
			cur_c = continuity[i + 1];
			_m1 = 0.5f * (1 - cur_t) * ((1 + cur_b) * (1 + cur_c) * (_p1 - _p0) + (1 - cur_b) * (1 - cur_c) * (_p2 - _p1));

			Vector3 curvePos;
			float _t;
			float uStep = 1.0f / segmentFrameCount;

			if (i == points.Length - 2)
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
		lineRenderer.positionCount = segmentFrameCount * (points.Length - 1);
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
