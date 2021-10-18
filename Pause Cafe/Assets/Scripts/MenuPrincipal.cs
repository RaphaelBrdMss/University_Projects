using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using AI_Class;
using Classifiers;
using System.IO;

public class MenuPrincipal : MonoBehaviour
{






    public void loadSave()
    {


        
            if (File.Exists("Data/Save/gameSave"))
            {
                MainGame.startGameData = new StartGameData();
                MainGame.startGameData.loadSave = true;
                SceneManager.LoadScene(1);
            }
            
        
    }
   

	
    public void QuitGame()
    {
        
        Application.Quit();
        
    }

}
