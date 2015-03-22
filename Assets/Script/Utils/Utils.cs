using UnityEngine;
using System.Collections;

public class Utils {

	public static float indexOrLast(float[] array, int index)
	{
		if(index >= array.Length) return array[array.Length-1];
		return array[index];
	}
}
