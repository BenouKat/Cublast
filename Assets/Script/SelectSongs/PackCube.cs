using UnityEngine;
using System.Collections;

public class PackCube : MonoBehaviour {

	public Renderer objectRenderer;
	public SongPack pack;
	public float rotationValue;
	public GameObject normalOrnement;
	public GameObject selectedOrnement;
	bool selected = false;
	public float zooming = 1.1f;
	Vector3 normalScale;
	Vector3 velocity;

	public void selectPack(bool active)
	{
		normalOrnement.SetActive(!active);
		selectedOrnement.SetActive(active);
		selected = active;
		velocity = Vector3.zero;
	}

	void Start()
	{
		normalScale = transform.localScale;
	}

	void Update()
	{
		if(selected && transform.localScale.x < normalScale.x*zooming)
		{
			transform.localScale = Vector3.SmoothDamp(transform.localScale, normalScale*zooming, ref velocity, 0.1f);
			if(Vector3.Distance(transform.localScale, normalScale*zooming) < 0.01f)
			{
				transform.localScale = normalScale*zooming;
			}
		}else if(!selected && transform.localScale.x > normalScale.x)
		{
			transform.localScale = Vector3.SmoothDamp(transform.localScale, normalScale, ref velocity, 0.1f);
			if(Vector3.Distance(transform.localScale, normalScale) < 0.01f)
			{
				transform.localScale = normalScale;
			}
		}
	}
}
