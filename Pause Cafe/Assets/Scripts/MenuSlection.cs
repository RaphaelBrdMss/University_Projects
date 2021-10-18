
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Characters;
using AI_Class;
using Classifiers;
using Classifiers1;
using Classifiers2;
public class MenuSlection : MonoBehaviour
{
    public List<CharClass> charsTeam1 = new List<CharClass>();
    public List<CharClass> charsTeam2 = new List<CharClass>();
    public GameObject[] selecButtonT1;
    public GameObject[] selecButtonT2;
    public GameObject ddT1;
    public GameObject ddT2;
    bool consoleMode;
    int nbGames;
    public Button ButtonPretT1;
    public Button ButtonPretT2;

    

    /* Methode for 1v1*/
    public void addSorcererT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.MAGE);

    }

    public void addArcherT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.ARCHER);
    }

    public void addBewitchT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.ENVOUTEUR);
    }
    public void addWarriorT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.GUERRIER);
    }
    public void addRobberT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.VOLEUR);
    }

    public void addHealerT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
        if (charsTeam1.Count < 1) charsTeam1.Add(Characters.CharClass.SOIGNEUR);
    }

    public void removeT1()
    {
        if (charsTeam1.Count == 1) charsTeam1.Clear();
    }

    public void addSorcererT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.MAGE);

    }

    public void addArcherT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.ARCHER);
    }

    public void addBewitchT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.ENVOUTEUR);
    }
    public void addWarriorT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.GUERRIER);
    }
    public void addRobberT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.VOLEUR);
    }

    public void addHealerT2()
    {
        if (charsTeam2.Count == 1) charsTeam2.Clear();
        if (charsTeam2.Count < 1) charsTeam2.Add(Characters.CharClass.SOIGNEUR);
    }

    public void removeT2()
    {

        if (charsTeam2.Count == 1) charsTeam2.Clear();
    }

    /* end of methode  for 1v1*/

    /* metode for 5v5*/

    public void DisableButtonSelecTeam1()
    {
        foreach (GameObject g in selecButtonT1)
        {

            if (charsTeam1.Count == 5)
            {

                g.GetComponent<Button>().interactable = false;
                ButtonPretT1.gameObject.SetActive(true);
                
            }
        }
    }
    public void EnableButtonSelecTeam1()
    {


        foreach (GameObject g in selecButtonT1)
        {

            if (charsTeam1.Count < 5)
            {

                g.GetComponent<Button>().interactable = true;
                ButtonPretT1.gameObject.SetActive(false);
            }
        }
    }
    public void enablePretT1(int selec)
    {
        if (selec > 0 )
        {
            ButtonPretT1.interactable = true;
        }
        else ButtonPretT1.interactable = false;
    }

    public void enableScrolingMenuT1()
    {
        if (charsTeam1.Count == 5)
        {

            ddT1.SetActive(true);
        }
    }


    public void DisableScrolingMenuT1()
    {
        if (charsTeam1.Count < 5)
        {

            ddT1.SetActive(false);
        }
    }

    public void addSorcerer5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.MAGE);
        }
    }

    public void addHealer5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.SOIGNEUR);
        }
    }

    public void addWarrior5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.GUERRIER);
        }
    }

    public void addRober5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.VOLEUR);
        }
    }

    public void addBewitch5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.ENVOUTEUR);
        }
    }

    public void addArcher5v5T1()
    {
        if (charsTeam1.Count < 5)
        {
            charsTeam1.Add(CharClass.ARCHER);
        }
    }

    public void removeSorcerTeam1()
    {
        charsTeam1.Remove(CharClass.MAGE);
    }
    public void removeRobberTeam1()
    {
        charsTeam1.Remove(CharClass.VOLEUR);
    }
    public void removeBewitchTeam1()
    {
        charsTeam1.Remove(CharClass.ENVOUTEUR);
    }
    public void removeArcherTeam1()
    {
        charsTeam1.Remove(CharClass.ARCHER);
    }
    public void removeWarriorTeam1()
    {
        charsTeam1.Remove(CharClass.GUERRIER);
    }
    public void removeHealerTeam1()
    {
        charsTeam1.Remove(CharClass.SOIGNEUR);
    }


    public void addSorcerer5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.MAGE);
        }
    }
    public void addRobber5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.VOLEUR);
        }
    }
    public void addHealer5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.SOIGNEUR);
        }
    }
    public void addBewitch5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.ENVOUTEUR);
        }
    }
    public void addWarrior5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.GUERRIER);
        }
    }
    public void addArcher5v5T2()
    {
        if (charsTeam2.Count < 5)
        {
            charsTeam2.Add(CharClass.ARCHER);
        }
    }


    public void removeSorcerTeam2()
    {
        charsTeam2.Remove(CharClass.MAGE);
    }
    public void removeRobberTeam2()
    {
        charsTeam2.Remove(CharClass.VOLEUR);
    }
    public void removeBewitchTeam2()
    {
        charsTeam2.Remove(CharClass.ENVOUTEUR);
    }
    public void removeArcherTeam2()
    {
        charsTeam2.Remove(CharClass.ARCHER);
    }
    public void removeWarriorTeam2()
    {
        charsTeam2.Remove(CharClass.GUERRIER);
    }
    public void removeHealerTeam2()
    {
        charsTeam2.Remove(CharClass.SOIGNEUR);
    }

    public void DisableButtonSelecTeam2()
    {


        foreach (GameObject g in selecButtonT2)
        {

            if (charsTeam2.Count == 5)
            {

                g.GetComponent<Button>().interactable = false;
                ButtonPretT2.gameObject.SetActive(true);
            }
        }
    }
    public void EnableButtonSelecTeam2()
    {


        foreach (GameObject g in selecButtonT2)
        {

            if (charsTeam2.Count < 5)
            {

                g.GetComponent<Button>().interactable = true;
                ButtonPretT2.gameObject.SetActive(false);
            }
        }
    }

    public void enableScrolingMenuT2()
    {
        if (charsTeam2.Count == 5)
        {

            ddT2.SetActive(true);
        }
    }

    public void DisableScrolingMenuT2()
    {
        if (charsTeam2.Count < 5)
        {

            ddT2.SetActive(false);
        }
    }

    public void enablePretT2(int selec)
    {
        if (selec > 0)
        {
            ButtonPretT2.interactable = true;
        }
        else ButtonPretT2.interactable = false;
    }








    public void MenuSelcTeam1(int selc)
    {
        MainGame.startGameData = new StartGameData();

        if (selc == 1)
        {
            MainGame.startGameData.player1Type = PlayerType.AI_EASY;

        }
        else if (selc == 2)
        {
            MainGame.startGameData.player1Type = PlayerType.AI_MEDIUM;

        }
        else if (selc == 3)
        {
            MainGame.startGameData.player1Type = PlayerType.AI_HARD;

        }
        else if (selc == 4)
        {
            MainGame.startGameData.player1Type = PlayerType.HUMAN;

        }



    }
    public void MenuSelcTeam2(int selc)
    {


        if (selc == 1)
        {
            MainGame.startGameData.player2Type = PlayerType.AI_EASY;

        }
        else if (selc == 2)
        {
            MainGame.startGameData.player2Type = PlayerType.AI_MEDIUM;

        }
        else if (selc == 3)
        {
            MainGame.startGameData.player2Type = PlayerType.AI_HARD;

        }
        else if (selc == 4)
        {
            MainGame.startGameData.player2Type = PlayerType.HUMAN;

        }

        




    }

    void saveOptions()
    {
        using (BinaryWriter writer = new BinaryWriter(File.Open("Data/Options/options", FileMode.Create)))
        {
            writer.Write((int)((consoleMode) ? 1 : 0));
            writer.Write(nbGames);
        }
    }

    void loadOptions()
    {
        using (BinaryReader reader = new BinaryReader(File.Open("Data/Options/options", FileMode.Open)))
        {
            consoleMode = (reader.ReadInt32() == 0) ? false : true;
            nbGames = reader.ReadInt32();
        }
    }
    public void startgame()
    {


        MainGame.startGameData.loadSave = false;
        MainGame.startGameData.charsTeam1 = charsTeam1;
        MainGame.startGameData.charsTeam2 = charsTeam2;
        MainGame.startGameData.mapChosen = 0;
        MainGame.startGameData.nbGames = 1;
        SceneManager.LoadScene(1);

        

        if (AIHard.rules == null)
        {
            AIHard.rules = new ClassifierSystem<Classifier1>();
            AIHard.rules.loadAllInBinary("Data/Classifiers/classifiersBinary");
            Debug.Log("Loaded " + AIHard.rules.classifiers.Count + " Classifiers.");
        }
        if (AIMedium.rules == null)
        {
            AIMedium.rules = new ClassifierSystem<Classifier1>();
            AIMedium.rules.loadAllInBinary("Data/Classifiers/classifiers2Binary");
            Debug.Log("Loaded " + AIMedium.rules.classifiers.Count + " Classifiers.");
        }

      
        

        
    }

    public void startgamemodeconsol()
    {


        MainGame.startGameData.loadSave = false;
        MainGame.startGameData.charsTeam1 = charsTeam1;
        MainGame.startGameData.charsTeam2 = charsTeam2;
        MainGame.startGameData.mapChosen = 0;
        MainGame.startGameData.nbGames = 50;
        SceneManager.LoadScene(2);



        if (AIHard.rules == null)
        {
            AIHard.rules = new ClassifierSystem<Classifier1>();
            AIHard.rules.loadAllInBinary("Data/Classifiers/classifiersBinary");
            Debug.Log("Loaded " + AIHard.rules.classifiers.Count + " Classifiers.");
        }
        if (AIMedium.rules == null)
        {
            AIMedium.rules = new ClassifierSystem<Classifier1>();
            AIMedium.rules.loadAllInBinary("Data/Classifiers/classifiers2Binary");
            Debug.Log("Loaded " + AIMedium.rules.classifiers.Count + " Classifiers.");
        }

        consoleMode = true;
        nbGames =10;
        // Read Options from options file
        if (File.Exists("Data/Options/options"))
        {
            loadOptions();
        }
        else
        {
            saveOptions();
        }




    }

}
    


