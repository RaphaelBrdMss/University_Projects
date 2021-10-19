using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
 * classe qui permet de gérer toute la partie interface utilisateur un fois en jeu
 * 
 * c'est grace au slider de cette classe qu'on pourra choisir la hauteur de la caméra en vu de dessu
 */


public class GameUtile : MonoBehaviour
{
    

    [SerializeField] GameObject panel;
    [SerializeField] public  Slider slider;
    public static int valycam;
    public bool ispause = false;
    

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (!ispause) Pause();
            else UnPause();

        }
        

       
    }
    
    

    public void Pause()
    {
        Time.timeScale =0;
        panel.SetActive(true);
        ispause = true;
    }

    public void UnPause()
    {
        
        Time.timeScale = 1;
        panel.SetActive(false);
        ispause = false;
    }

    public void ToMenu()
    {
        ispause = false;
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
        
    }
    public void setispause()
    {
        ispause = false;
    }

}
