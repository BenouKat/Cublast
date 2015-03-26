using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Utils {

	public static float indexOrLast(float[] array, int index)
	{
		if(index >= array.Length) return array[array.Length-1];
		return array[index];
	}

	public static Precision getWorstPrec(List<double> allPrecs)
	{
		double worst = 0;
		foreach (double prec in allPrecs) {
			if(prec > worst) worst = prec;
		}
		return getPrec (worst);
	}

	public static Precision getPrec(double prec)
	{
		foreach (KeyValuePair<Precision,double> p in GameManager.instance.PrecisionValues) {
			if(p.Key == Precision.MISS) return Precision.MISS;
			if(prec <= p.Value)
			{
				return p.Key;
			}
		}
		return Precision.MISS;
	}

	public static void turnOnLane(Transform t, Lanes lane)
	{
		switch(lane)
		{
		case Lanes.DOWN:
			t.Rotate(Vector3.forward*90f, Space.Self);
			break;
		case Lanes.UP:
			t.Rotate(Vector3.forward*(-90f), Space.Self);
			break;
		case Lanes.RIGHT:
			t.Rotate(Vector3.forward*180f, Space.Self);
			break;
		default:
			break;
		}
	}

	public static double getBPS(double bpmValue){
		return bpmValue/(double)60.0;
	}
}
