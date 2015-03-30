using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FreezeController : MonoBehaviour {

	Arrow parentArrow;
	float distanceFreeze;

	public Transform rootTransform;
	public GameObject baseFreeze;
	Material baseFreezeMaterial;
	Color baseEmissionColor;
	Color currentEmissionColor;
	float currentEmissionPower = 1f;
	public float speedEmissionDecrease = 0.5f;

	public Transform clearZone;
	public MeshRenderer clearZoneRenderer;
	Material clearZoneMaterial;

	public GameObject warningObject;
	public Transform warningObjectRoot;
	public float initWarningSpacing;
	public float warningSpacing;

	//For animation
	List<Transform> warningObjectsPosition = new List<Transform> ();
	Transform[] warningObjectsPositionArray;
	int indexWarningObject;

	[HideInInspector] public double timeLastHit;
	[HideInInspector] public double timeEndScheduled;

	bool letInUpdate;

	void Awake()
	{
		baseFreezeMaterial = baseFreeze.GetComponent<Renderer>().material;
		baseEmissionColor = baseFreezeMaterial.GetColor("_EmissionColor");
		clearZoneRenderer.enabled = false;
		clearZoneMaterial = clearZoneRenderer.material;
	}

	void Update()
	{
		if (letInUpdate) {
			let ();
		}
	}

	public void init(Arrow parentArrow, float distanceFreeze, double timeEndScheduled, Color colorZone)
	{
		this.parentArrow = parentArrow;
		this.timeEndScheduled = timeEndScheduled;
		this.distanceFreeze = distanceFreeze;
		//Augmenter le rootFreeze
		rootTransform.localScale = Vector3.right + Vector3.forward + (Vector3.up*distanceFreeze);
		//Changer la couleur de la freeze zone
		clearZoneMaterial.color = colorZone;
		//Instantier les warning;
		if(warningObject != null)
		{
			float currentDistance = distanceFreeze - initWarningSpacing;
			while(currentDistance > 0f)
			{
				GameObject warningInst = Instantiate(warningObject, warningObject.transform.position, warningObject.transform.rotation) as GameObject;
				warningInst.transform.SetParent(warningObjectRoot);
				warningInst.transform.localPosition = -Vector3.up*(distanceFreeze - currentDistance);
				warningObjectsPosition.Add(warningInst.transform);
				warningInst.SetActive(true);
				currentDistance -= warningSpacing;
			}
			warningObjectsPositionArray = warningObjectsPosition.ToArray();
			indexWarningObject = 0;
			warningObjectRoot.gameObject.SetActive(true);
		}
	}

	public void animFreeze(double currentTime)
	{
		//Scale
		rootTransform.localScale = Vector3.right + Vector3.forward 
			+ (Vector3.up*distanceFreeze*(float)((1 - ((currentTime - parentArrow.scheduledTime) / (timeEndScheduled - parentArrow.scheduledTime)))));

		//Objects
		if (warningObject != null) {
			warningObjectRoot.transform.localPosition = Vector3.up * distanceFreeze * (float)(((currentTime - parentArrow.scheduledTime) / (timeEndScheduled - parentArrow.scheduledTime)));
			if (indexWarningObject < warningObjectsPositionArray.Length && 
			    warningObjectRoot.transform.localPosition.y >= -warningObjectsPositionArray [indexWarningObject].localPosition.y) 
			{
				warningObjectsPositionArray [indexWarningObject].gameObject.SetActive(false);
				indexWarningObject++;
			}
		}

	}

	public void hit(double currentTime)
	{
		currentEmissionPower = 1f;
		refreshEmission();
		timeLastHit = currentTime;
	}

	public void enableLetInUpdate(bool active)
	{
		letInUpdate = active;
	}

	public void let()
	{
		currentEmissionPower -= speedEmissionDecrease * Time.deltaTime;
		refreshEmission();
	}

	public void refreshEmission()
	{
		currentEmissionColor = currentEmissionPower*baseEmissionColor;
		baseFreezeMaterial.SetColor("_EmissionColor", currentEmissionColor);
	}
}
