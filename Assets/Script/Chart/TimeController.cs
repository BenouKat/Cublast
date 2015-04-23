using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TimeController : MonoBehaviour {

	public static TimeController instance;
	void Awake()
	{
		if (instance == null)
			instance = this;
	}

	public Image timeBar;

	float timeMin = 10000f;
	float timeMax = 10000f;

	public void init(Vector2 timeRange)
	{
		timeMin = timeRange.x;
		timeMax = timeRange.y;

		InvokeRepeating ("UpdateTime", 0f, 0.1f);
	}

	public void stopUpdate()
	{
		CancelInvoke ("UpdateTime");
	}

	void UpdateTime()
	{
		timeBar.fillAmount = Mathf.Clamp (((float)ChartManager.instance.currentTime - timeMin) / (timeMax - timeMin), 0f, 1f);
	}

}
