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
			if(prec > worst) prec = worst;
		}
		return getPrec (worst);
	}

	public static Precision getPrec(double prec)
	{
		foreach (KeyValuePair<Precision,double> p in GameManager.instance.PrecisionValues) {
			if(prec <= p.Value)
			{
				return p.Key;
			}
		}
	}
}
