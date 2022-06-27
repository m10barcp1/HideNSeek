using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SeekStateManager : MonoBehaviour
{
    #region Variables
    public float viewRadius;
    [Range(0, 360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    [HideInInspector]
    public GameObject hideCharacter;
    [HideInInspector]
    public  GameObject targetObj;

	public List<GameObject> visibleTargets = new List<GameObject>();

	[Header("Draw Field Of View")]
	public float meshResolution;
	public int edgeResolveIterations;
	public float edgeDstThreshold;

	public MeshFilter viewMeshFilter;
	Mesh viewMesh;


	public TextMeshProUGUI PrisonerText;
	public int CharacerInImprison;
    #endregion

    private void Start()
    {
		viewMesh = new Mesh();
		viewMesh.name = "View Mesh";
		viewMeshFilter.mesh = viewMesh;
		CharacerInImprison = 0;
    }
    private void Update()
    {
		// Khi bat dau game
        if (GameManager.instance.StartGame)
        {
			// Check goc quan sat
			FieldOfViewCheck();
            // Lay ra list target de thuc hien hanh dong
			foreach (var i in visibleTargets)
			{
				var HideStateOfCharacter = i.GetComponent<HideStateManager>();
				if (HideStateOfCharacter != null)
				{
					HideStateOfCharacter.Imprison();
				}
				// Tang so luong tu nhan
				IncreaseCharaceterInImprison();
			}
		}
		// Ve ra goc quan sat
		DrawFieldOfView();
	}
	// Reset trang thai
	public void ResetState()
	{
		transform.localRotation = new Quaternion(0, 0, 0, 0);
        if (!gameObject.CompareTag("SeekPlayer"))
        {
			transform.localPosition = Vector3.zero;
		}
	}

	#region Process Number Of Imprison
	// Reset lai tu nhan
	public void ResetCharaceterInImprison()
    {
		CharacerInImprison = 0;
		SetNumberImprisonerForTxt();
	}
	// Tang luong tu nhan
	public void IncreaseCharaceterInImprison()
    {
		CharacerInImprison++;
		SetNumberImprisonerForTxt();
		if ((gameObject.CompareTag("SeekPlayer")) && CharacerInImprison == 6)
        {
			GameManager.instance.WinGameAction();
        }
		
	}
	// Giam luong tu nhan
	public void DecreaseCharaceterInImprison()
    {
		CharacerInImprison--;
		SetNumberImprisonerForTxt();
	}
	public void SetNumberImprisonerForTxt()
    {
		if (gameObject.CompareTag("SeekPlayer"))
			PrisonerText.text = CharacerInImprison.ToString();
	}
    #endregion
    #region Check FOV
    public void FieldOfViewCheck()
    {
		visibleTargets.Clear();
		Collider[] rangeChecks = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

		for (int i = 0; i < rangeChecks.Length; i++)
		{
			Transform target = rangeChecks[i].transform;
			Vector3 dirToTarget = (target.position - transform.position).normalized;
			if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
			{
				float dstToTarget = Vector3.Distance(transform.position, target.position);
				if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
				{
					visibleTargets.Add(target.gameObject);	
				}
			}
		}
    }
    #endregion
    #region DrawView
    void DrawFieldOfView()
	{
		int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
		float stepAngleSize = viewAngle / stepCount;
		List<Vector3> viewPoints = new List<Vector3>();
		ViewCastInfo oldViewCast = new ViewCastInfo();
		for (int i = 0; i <= stepCount; i++)
		{
			float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;
			ViewCastInfo newViewCast = ViewCast(angle);

			if (i > 0)
			{
				bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
				if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
				{
					EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
					if (edge.pointA != Vector3.zero)
					{
						viewPoints.Add(edge.pointA);
					}
					if (edge.pointB != Vector3.zero)
					{
						viewPoints.Add(edge.pointB);
					}
				}

			}


			viewPoints.Add(newViewCast.point);
			oldViewCast = newViewCast;
		}

		int vertexCount = viewPoints.Count + 1;
		Vector3[] vertices = new Vector3[vertexCount];
		int[] triangles = new int[(vertexCount - 2) * 3];

		vertices[0] = Vector3.zero;
		for (int i = 0; i < vertexCount - 1; i++)
		{
			vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

			if (i < vertexCount - 2)
			{
				triangles[i * 3] = 0;
				triangles[i * 3 + 1] = i + 1;
				triangles[i * 3 + 2] = i + 2;
			}
		}

		viewMesh.Clear();

		viewMesh.vertices = vertices;
		viewMesh.triangles = triangles;
		viewMesh.RecalculateNormals();
	}


	EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
	{
		float minAngle = minViewCast.angle;
		float maxAngle = maxViewCast.angle;
		Vector3 minPoint = Vector3.zero;
		Vector3 maxPoint = Vector3.zero;

		for (int i = 0; i < edgeResolveIterations; i++)
		{
			float angle = (minAngle + maxAngle) / 2;
			ViewCastInfo newViewCast = ViewCast(angle);

			bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
			if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
			{
				minAngle = angle;
				minPoint = newViewCast.point;
			}
			else
			{
				maxAngle = angle;
				maxPoint = newViewCast.point;
			}
		}

		return new EdgeInfo(minPoint, maxPoint);
	}


	ViewCastInfo ViewCast(float globalAngle)
	{
		Vector3 dir = DirFromAngle(globalAngle, true);
		RaycastHit hit;

		if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
		{
			return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
		}
		else
		{
			return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
		}
	}

	public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
	{
		if (!angleIsGlobal)
		{
			angleInDegrees += transform.eulerAngles.y;
		}
		return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
	}

	public struct ViewCastInfo
	{
		public bool hit;
		public Vector3 point;
		public float dst;
		public float angle;

		public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle)
		{
			hit = _hit;
			point = _point;
			dst = _dst;
			angle = _angle;
		}
	}

	public struct EdgeInfo
	{
		public Vector3 pointA;
		public Vector3 pointB;

		public EdgeInfo(Vector3 _pointA, Vector3 _pointB)
		{
			pointA = _pointA;
			pointB = _pointB;
		}
	}
    #endregion
}