using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Hexas;

public class DamageValueDisplay : MonoBehaviour
{
	public Transform camera_;
	public int age;
	
	void Update(){
		if (age >= 0){
			if (age == 0){
				GameObject.Destroy(gameObject);
			}else{
				age--;
				gameObject.transform.position = new Vector3(gameObject.transform.position.x,gameObject.transform.position.y+(age+3)*0.0004f,gameObject.transform.position.z);
				gameObject.transform.rotation = camera_.rotation;
				if (age < 15){
					Color c = gameObject.GetComponent<TextMesh>().color;
					gameObject.GetComponent<TextMesh>().color = new Color(c.r,c.g,c.b,age/15.0f);
				}
			}
		}
	}
	
	/** duration is set in frames (60 frames / sec) **/
	public void setValue(int hexaX,int hexaY,string text,Color color,int duration){
		age = duration;
		gameObject.transform.position = Hexa.hexaPosToReal(hexaX,hexaY,1.0f);//new Vector3(hexaX * 0.75f,1.0f,hexaY * -0.86f + (hexaX%2) * 0.43f);
		gameObject.GetComponent<TextMesh>().color = color;
		gameObject.GetComponent<TextMesh>().text  = text;
	}
}
