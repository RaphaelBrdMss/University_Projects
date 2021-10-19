using System;
using UnityEngine;
using UnityEngine.UI;


/* 
 * Petite classe pour assurer le controle de la hauteur de la caméra vu de haut, les est attacher a l'objet CameraTop sur unity
 */
public class CamControl : MonoBehaviour
{

    
    private void Update()
    {

        changeY();
    }

    //Fonction qui permet de changer la valeur Y de la caméra vu de dessu
    public void changeY()
    {
        
        
        
        this.transform.position = new Vector3(this.transform.position.x, GameObject.Find("Slider0").GetComponent<Slider>().value, this.transform.position.z);
    }
}
