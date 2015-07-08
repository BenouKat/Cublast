using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MatchCanvasGroupAlpha : MonoBehaviour {

	List<CanvasGroup> canvasGroup = new List<CanvasGroup>();
	float lastAlpha = -1f;
	public string colorName;
	Renderer objectRenderer;
	// Use this for initialization
	void Start () {
		findCanvasGroup (transform);
		objectRenderer = GetComponent<Renderer> ();
	}

	public void findCanvasGroup(Transform child)
	{
		CanvasGroup cg = child.GetComponent<CanvasGroup> ();
		if (cg != null)
			canvasGroup.Add (cg);
		if (child.parent != null) {
			findCanvasGroup(child.parent);
		}
	}
	
	// Update is called once per frame
	Color poolColor = new Color(0f, 0f, 0f, 0f);
	float currentMixedAlpha = 1f;
	void Update () {

		currentMixedAlpha = 1f;
		foreach (CanvasGroup cg in canvasGroup) {
			currentMixedAlpha *= cg.alpha;
		}

		if (canvasGroup != null && lastAlpha != currentMixedAlpha) {
			poolColor = objectRenderer.material.GetColor(colorName);
			poolColor.a = currentMixedAlpha;
			objectRenderer.material.SetColor(colorName, poolColor);
			lastAlpha = currentMixedAlpha;
		}
	}
}
