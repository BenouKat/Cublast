using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class FreezeController : MonoBehaviour {

	public Transform rootTransform;
	public GameObject baseFreeze;
	Material baseFreezeMaterial;
	float baseEmissionValue;
	float currentEmissionValue;
	public float speedEmissionDecrease = 0.5f;

	public Transform clearZone;
	public MeshRenderer clearZoneRenderer;
	Material clearZoneMaterial;

	public GameObject warningObject;
	public Transform warningObjectRoot;
	public float initWarningSpacing;
	public float warningSpacing;

	Dictionary<float, Transform> warningObjectsPosition;

	public double timeLastHit;
	public double timeEndScheduled;

	void Awake()
	{
		baseFreezeMaterial = baseFreeze.GetComponent<Renderer>().material;
		baseEmissionValue = baseFreezeMaterial.GetFloat("_Emission");
		clearZoneRenderer.enabled = false;
		clearZoneMaterial = clearZoneRenderer.material;
	}

	public void init(float distanceFreeze, double timeEndScheduled, Color colorZone)
	{
		this.timeEndScheduled = timeEndScheduled;
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
				warningInst.SetActive(true);
				currentDistance -= warningSpacing;
			}
		}
	}

	public void hit(double currentTime)
	{
		currentEmissionValue = baseEmissionValue;
		refreshEmission();
		timeLastHit = currentTime;
	}

	public void let()
	{
		currentEmissionValue -= speedEmissionDecrease*Time.deltaTime;
		refreshEmission();
	}

	public void refreshEmission()
	{
		baseFreezeMaterial.SetFloat("_Emission", currentEmissionValue);
	}
}
