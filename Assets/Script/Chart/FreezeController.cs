using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FreezeController : MonoBehaviour {

	Arrow parentArrow;
	float distanceFreeze;

	public Transform rootTransform;
	public GameObject baseFreeze;
	Material baseFreezeMaterial;
	public Transform clearZone;
	float clearZoneBaseX = 0f;
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

	EmissionTweener emissionTweener;

	void Awake()
	{
		emissionTweener = GetComponent<EmissionTweener> ();
		baseFreezeMaterial = baseFreeze.GetComponent<Renderer>().material;
		emissionTweener.concernedMaterial = baseFreezeMaterial;
		emissionTweener.speedEmissionDecrease = (float)GameManager.instance.timeBeforeFreezeMiss;
		clearZoneRenderer.enabled = false;
		clearZoneBaseX = clearZone.localScale.x;
		clearZoneMaterial = clearZoneRenderer.material;
	}

	public void init(Arrow parentArrow, float distanceFreeze, double timeEndScheduled, Color colorZone)
	{
		this.parentArrow = parentArrow;
		this.timeEndScheduled = timeEndScheduled;
		this.distanceFreeze = distanceFreeze;
		//Augmenter le rootFreeze
		rootTransform.localScale = Vector3.right + Vector3.forward + (Vector3.up*distanceFreeze);
		//Changer la couleur de la freeze zone
		colorZone.a = clearZoneMaterial.color.a;
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

	private float percentLeft = 0f;
	public void animFreeze(double currentTime)
	{
		percentLeft = (float)((currentTime - parentArrow.scheduledTime) / (timeEndScheduled - parentArrow.scheduledTime));
		//Scale
		rootTransform.localScale = Vector3.right + Vector3.forward 
				+ (Vector3.up*distanceFreeze*(1 - percentLeft));

		clearZone.localScale = (Vector3.right * (clearZoneBaseX * (1 - Mathf.Clamp(percentLeft, 0f, 1f)))) + (Vector3.up * clearZone.localScale.y) + (Vector3.forward * clearZone.localScale.z);
		//Objects
		if (warningObject != null) {
			warningObjectRoot.transform.localPosition = Vector3.up * distanceFreeze * percentLeft;
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
		emissionTweener.pulse ();
		timeLastHit = currentTime;
		clearZoneRenderer.enabled = true;
	}

	public void let()
	{
		emissionTweener.let ();
	}

	public void enableLetInUpdate(bool active)
	{
		emissionTweener.enableLetInUpdate (active);
	}

	public bool isAlreadyValid()
	{
		return timeEndScheduled - timeLastHit <= GameManager.instance.timeBeforeFreezeMiss;
	}

}
