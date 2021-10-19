using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
/*
 * petite classe qui permet d'afficher le temps a la fin de la partie si le labyrinthe es réussi
 */

public class DisplayTime : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI text;

    //Fonction qui récupére le temps passé en jeu et qui l'affiche sur l'écran de fin
    void Start()
    {
        text.text = "ton temps est de : " + (int)GridCell.timer+ " seconde";
    }

}
