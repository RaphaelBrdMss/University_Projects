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

public enum PlayerType : byte {HUMAN,AI_EASY,AI_MEDIUM,AI_HARD};

// Data structure used to send game info when switching scenes
public class StartGameData{
	public bool loadSave;
	public List<CharClass> charsTeam1 = new List<CharClass>();
	public List<CharClass> charsTeam2 = new List<CharClass>();
	public PlayerType player1Type;
	public PlayerType player2Type;
	public int mapChosen; // 0 = Ruins, 1 = Random
	public int nbGames;
	public StartGameData(){}
}
