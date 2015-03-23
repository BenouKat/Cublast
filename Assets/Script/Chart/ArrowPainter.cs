using UnityEngine;
using System.Collections;

public class ArrowPainter : MonoBehaviour {


	public Color beat4;
	public Color beat8;
	public Color beat12;
	public Color beat16;
	public Color beat24;
	public Color beat32;
	public Color beat48;
	public Color beat64;
	public Color beat192;
	public Color defaultColor;
		
		
	public Color getMesureColor(int mesure, int posMesure)
	{
		switch(mesure){
		case 4:	
			return beat4;
		
			
		case 8:
			if(posmesure%2 == 0){
				return beat8;
			}
			return beat4;
			
			
		case 12:
			if((posmesure-1)%3 == 0){
				return beat4;
			}
			return beat12;
			
		case 16:
			if((posmesure-1)%4 == 0){
				return beat4;
			}else if((posmesure+1)%4 == 0){
				return beat8;
			}
			return beat16;
		
			
		case 24:
			if((posmesure+2)%6 == 0){
				return beat8;
			}else if((posmesure-1)%6 == 0){
				return beat4;
			}
			return beat24;
			
			
		case 32:
			if((posmesure-1)%8 == 0){
				return beat4;
			}else if((posmesure+3)%8 == 0){
				return beat8;
			}else if((posmesure+1)%4 == 0){
				return beat16;
			}
			return beat32;
			
		case 48:
			if((posmesure+2)%6 == 0){
				return beat16;
			}else if((posmesure-1)%12 == 0){
				return beat4;
			}else if((posmesure+5)%12 == 0){
				return beat8;
			}
			return beat48;
		
			
		case 64:
			if((posmesure-1)%16 == 0){
				return beat4;
			}else if((posmesure+7)%16 == 0){
				return beat8;
			}else if((posmesure+3)%8 == 0){
				return beat16;
			}else if((posmesure+1)%4 == 0){
				return beat32;
			}
			return beat64;
			
		case 192:
			if((posmesure-1)%48 == 0){
				return beat4;
			}else if((posmesure+23)%48 == 0){
				return beat8;
			}else if((posmesure+11)%24 == 0){
				return beat16;
			}else if((posmesure-1)%4 == 0){
				return beat24;
			}else if((posmesure+5)%12 == 0){
				return beat32;
			}
			return beat192;
			
		default:
			return defaultColor;
			
		}
	}
}
