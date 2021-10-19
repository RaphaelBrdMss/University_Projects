using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
/*
 * Classe qui contient les fonctions nécessaires pour assurer le bon fonctionnement de l'écran de victoire
 * 
 * elle contient un booléen pour savoir si le joueur veux recomencer le labyrinthe
 * et elle récupére le labyrinthe qui a été choisi aléatoirement a la création de celui-ci
 */

public class Finish : MonoBehaviour
{
    public static bool re = false;
    public static int numLaby;
    
    //fonction qui permet d'aller a la scene de fin si le joueur trouve la sortie
    private void OnTriggerEnter(Collider other)
    {
           
        SceneManager.LoadScene(2);
    }

    public void retourMenu()
    {
        re = false;
        GridCell.timer = 0;
        SceneManager.LoadScene(0);
        
    }
    //permet de rejouer
    public void refaire()
    {
        re = true;
        numLaby = GridCell.getrand;
        GridCell.timer = 0;

        SceneManager.LoadScene(1);
    }
}
