using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

/*
 * classe servant à gerer le menuPrincipal
 */

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    public static int difficulte;
    // Start is called before the first frame update
   
    public void Play()
    {
        SceneManager.LoadScene("Laby");

    }
    public void Quit()
    {
        Application.Quit();
    }

    public void setDiffFacile()
    {
        difficulte = 0;
    }
    public void setDiffMoyen()
    {
        difficulte = 1;
    }
    public void setDiffDur()
    {
        difficulte = 2;
    }

  
}
