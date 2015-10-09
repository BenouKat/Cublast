using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;

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
		for(int i=0; i<GameManager.instance.PrecisionValues.Length; i++) {
			if((Precision)i == Precision.MISS) return Precision.MISS;
			if(prec <= GameManager.instance.PrecisionValues[i])
			{
				return (Precision)i;
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

	public static string EncryptScore(double score, string keyIdentifier)
	{
		int demarageSubstring = Mathf.Clamp((keyIdentifier.Length/2) - 10, 0, 1000);
		string identifier = keyIdentifier.Substring(demarageSubstring, Mathf.Clamp(20, 1, keyIdentifier.Length - demarageSubstring));
		string stringscore = score.ToString("000.0000");

		MD5CryptoServiceProvider hashMd5 = new MD5CryptoServiceProvider();
		byte[] passwordHash = hashMd5.ComputeHash(
			UnicodeEncoding.Unicode.GetBytes(identifier));
		
		TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
		des.Key = passwordHash;
		
		des.Mode = CipherMode.ECB;
		
		byte[] buffer = UnicodeEncoding.Unicode.GetBytes(stringscore);
		
		return UnicodeEncoding.Unicode.GetString(
			des.CreateEncryptor().TransformFinalBlock(buffer, 0, buffer.Length));
		
	}

	public static string UppercaseFirst(string text)
	{
		return text.ToUpper()[0] + text.ToLower().Substring(1, text.Length-1);
	}
}
