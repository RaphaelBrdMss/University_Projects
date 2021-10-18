using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Misc;
using Hexas;
using Characters;
using AI_Util;
using AI_Class;
using Classifiers;
using Classifiers1;
using Classifiers2;
using Stats;
using UnityEditor;

public class MainGameConsole : MonoBehaviour {
	
	public GameObject textDisplay;
	public GameObject textDisplay2;
	public int nbGames;
	public int currentNbGames;
	public int nbTurns;
	
	public int tileW;
	public int tileH;
	public HexaGrid hexaGrid;
	public Character currentCharControlled;
	public int currentCharControlledID;
	public int winner;
	public StatsGame statsGame;
	public List<ActionAIPos> decisionSequence;
	
	public int t1Wins;
	public int t2Wins;
	
	public int nbMutations;
	public int nbMerges;
	public int nbRemovals;
	
	public int actionMoveErrs;
	public int actionAtkErrs;
	
	public float maxScore;
	public float lastScore;
	
    // Start is called before the first frame update
    void Start(){
		CharsDB.initCharsDB();
        // Init game data if it's not (it should be in the main menu)
		if (MainGame.startGameData == null){
			MainGame.startGameData = new StartGameData();
			MainGame.startGameData.loadSave = false;
			MainGame.startGameData.charsTeam1 = new List<CharClass>();
			MainGame.startGameData.charsTeam1.Add(CharClass.VOLEUR);
			MainGame.startGameData.charsTeam1.Add(CharClass.GUERRIER);
			MainGame.startGameData.charsTeam2 = new List<CharClass>();
			MainGame.startGameData.charsTeam2.Add(CharClass.SOIGNEUR);
			MainGame.startGameData.charsTeam2.Add(CharClass.ARCHER);
			MainGame.startGameData.player1Type = PlayerType.AI_HARD;
			MainGame.startGameData.player2Type = PlayerType.AI_EASY;
			MainGame.startGameData.mapChosen = 1;
			MainGame.startGameData.nbGames = 10;
		}
		
		// Init hexa grid
		hexaGrid = new HexaGrid();
		if (MainGame.startGameData.mapChosen == 0){
			hexaGrid.createGridFromFile2("Data/Map/ruins");
			tileW = hexaGrid.w; tileH = hexaGrid.h;
		}else if (MainGame.startGameData.mapChosen >= 1){
			hexaGrid.createRandomRectGrid2(tileW,tileH);
		}
		// Put characters on the grid
		for (int i=0;i<5;i++){
			if (i < MainGame.startGameData.charsTeam1.Count) hexaGrid.addChar2(MainGame.startGameData.charsTeam1[i],tileW/2-4+2+i,2,0);
			if (i < MainGame.startGameData.charsTeam2.Count) hexaGrid.addChar2(MainGame.startGameData.charsTeam2[i],tileW/2-4+2+i,tileH-2,1);
		}
		foreach (Character c in hexaGrid.charList) hexaGrid.getHexa(c.x,c.y).changeType2(HexaType.GROUND);
		currentCharControlledID = 0;
		currentCharControlled = hexaGrid.charList[currentCharControlledID];
		currentNbGames = 0;
		nbGames = MainGame.startGameData.nbGames;
		
		// Init AI
		decisionSequence = new List<ActionAIPos>();
		AI.hexaGrid = hexaGrid;
		// Init AIHard classifiers
		if (AIHard.rules == null){
			AIHard.rules = new ClassifierSystem<Classifier1>();
			AIHard.rules.loadAllInBinary("Data/Classifiers/classifiersBinary");
			Debug.Log("Loaded " + AIHard.rules.classifiers.Count + " Classifiers.");
		}
		if (AIMedium.rules == null){
			AIMedium.rules = new ClassifierSystem<Classifier1>();
			AIMedium.rules.loadAllInBinary("Data/Classifiers/classifiers2Binary");
			Debug.Log("Loaded " + AIMedium.rules.classifiers.Count + " Classifiers.");
		}
		AIHard.learn = true;
		AIUtil.hexaGrid = hexaGrid;
		statsGame = new StatsGame();
		winner = -1;
		t1Wins = 0;
		t2Wins = 0;
		nbTurns = 0;
		
		nbMutations = 0;
		nbMerges = 0;
		nbRemovals = 0;
		
		actionMoveErrs = 0;
		actionAtkErrs = 0;
		
		maxScore = 0.0f;
		lastScore = 0.0f;
    }

    // Update is called once per frame
    void Update(){
		for (int aaa=0;aaa<5;aaa++){ // 5 actions per frame
			if (winner == -1 && nbTurns < 400){ // Max 400 turns to prevent infinite stalling
				PlayerType currentPlayerType = whoControlsThisChar(currentCharControlled);
				// decide what to do
				if (decisionSequence.Count == 0){
					switch (currentPlayerType){
						case PlayerType.HUMAN :
						case PlayerType.AI_EASY : decisionSequence = AIEasy.decide(currentCharControlledID); break;
						case PlayerType.AI_MEDIUM : decisionSequence = AIMedium.decide(currentCharControlledID,statsGame); break;
						case PlayerType.AI_HARD : decisionSequence = AIHard.decide(currentCharControlledID,statsGame); break;
					}
				// do action after the decision is taken
				}else{
					ActionAIPos actionAIPos = decisionSequence[0]; decisionSequence.RemoveAt(0);
					//Debug.Log(currentPlayerType + " : " + actionAIPos.action + ((actionAIPos.pos != null) ? (" " + actionAIPos.pos.x + " " + actionAIPos.pos.y) : ""));
					switch (actionAIPos.action){
						case MainGame.ActionType.MOVE : actionMove(hexaGrid.getHexa(actionAIPos.pos)); break;
						case MainGame.ActionType.ATK : actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos)); break;
						case MainGame.ActionType.ULT : actionUseAttack(actionAIPos.action, hexaGrid.getHexa(actionAIPos.pos)); break;
						case MainGame.ActionType.SPELL : actionUseAttack(actionAIPos.action,hexaGrid.getHexa(actionAIPos.pos)); break;
						case MainGame.ActionType.SKIP : { currentCharControlled.PA = 0; nextTurn(); } break;
					}
				}
			// end of all the games (go back to main menu by pressing A)
			}else if (winner == 10){
				textDisplay.GetComponent<Text>().text = "VICTOIRE DE L'EQUIPE " + ((t1Wins > t2Wins) ? "ROUGE" : "BLEUE") + " (" + ((winner == 0) ? MainGame.startGameData.player1Type : MainGame.startGameData.player2Type) + ")" + "\nPress A to go quit";
				// A Key : Quit
				if (Input.GetKeyDown(KeyCode.A)){
					AIHard.rules.saveAllInBinary("Data/Classifiers/classifiersBinary");
					AIMedium.rules.saveAllInBinary("Data/Classifiers/classifiers2Binary");
					SceneManager.LoadScene(0);
					Debug.Log("Save in database");
				}
			// next game (reset) (hold A at the end of a game to go back to main menu)
			}else if (winner == 11){
				initGame();
				
				PlayerType classifierType;
				if      (MainGame.startGameData.player1Type == PlayerType.AI_HARD   || MainGame.startGameData.player2Type == PlayerType.AI_HARD  ) classifierType = PlayerType.AI_HARD;
				else if (MainGame.startGameData.player1Type == PlayerType.AI_MEDIUM || MainGame.startGameData.player2Type == PlayerType.AI_MEDIUM) classifierType = PlayerType.AI_MEDIUM;
				else classifierType = PlayerType.HUMAN;
				
				if (classifierType == PlayerType.AI_HARD) algoGenAIHard();
				else if (classifierType == PlayerType.AI_MEDIUM) algoGenAIMedium();
				
				// A Key : Quit (hold)
				if (Input.GetKey(KeyCode.A)){
					SceneManager.LoadScene(0);
				}
				
				// Update display
				string s1 = "EQUIPE ROUGE" + " (" + MainGame.startGameData.player1Type + ") : " + t1Wins + " Wins";
				string s2 = "EQUIPE BLEUE" + " (" + MainGame.startGameData.player2Type + ") : " + t2Wins + " Wins";
				string s3 = "Mutations : " + nbMutations + " | Fusions : " + nbMerges + " | Suppressions : " + nbRemovals + "\nMax Score : " + maxScore + " last : " + lastScore;
				string s4 = "Errors : move : " + actionMoveErrs + " , atk : " + actionAtkErrs;
				textDisplay.GetComponent<Text>().text = currentNbGames + " / " + nbGames;
				textDisplay2.GetComponent<Text>().text = "Nb Classifiers : " + ((classifierType == PlayerType.AI_HARD) ? AIHard.rules.classifiers.Count : AIMedium.rules.classifiers.Count) + "\n" + s1 + "\n" + s2 + "\n" + s3 + "\n" + s4;
			// Game end
			}else{
				if (winner != -1){
					// EVALUATE AI HARD/MEDIUM
					statsGame.endGame(winner,hexaGrid);
					if (MainGame.startGameData.player1Type == PlayerType.AI_HARD   || MainGame.startGameData.player2Type == PlayerType.AI_HARD  ) AIHard.rules.increaseLastUse();
					if (MainGame.startGameData.player1Type == PlayerType.AI_MEDIUM || MainGame.startGameData.player2Type == PlayerType.AI_MEDIUM) AIMedium.rules.increaseLastUse();
					lastScore = statsGame.evaluateGame(maxScore);
					if ((winner == 0 && MainGame.startGameData.player1Type == PlayerType.AI_EASY) || (winner == 1 && MainGame.startGameData.player2Type == PlayerType.AI_EASY)) lastScore = -lastScore;
					maxScore = lastScore*0.1f + maxScore*0.9f;
					if (winner == 0) t1Wins++; else t2Wins++;
				}
				currentNbGames++;
				if (currentNbGames == nbGames) winner = 10;
				else winner = 11;
			}
		}
		
    }
	
	// ##################################################################################################################################################
	// Functions used in main
	// ##################################################################################################################################################
	
	PlayerType whoControlsThisChar(Character c){
		return (c.team == 0) ? MainGame.startGameData.player1Type : MainGame.startGameData.player2Type;
	}
	
	void initGame(){
		statsGame = new StatsGame();
		winner = -1;
		nbTurns = 0;
		foreach (Character c in hexaGrid.charList) hexaGrid.getHexa(c.x,c.y).charOn = null;
		hexaGrid.charList = new List<Character>();
		// Put characters on the grid
		for (int i=0;i<5;i++){
			if (i < MainGame.startGameData.charsTeam1.Count) hexaGrid.addChar2(MainGame.startGameData.charsTeam1[i],tileW/2-4+2+i,2,0);
			if (i < MainGame.startGameData.charsTeam2.Count) hexaGrid.addChar2(MainGame.startGameData.charsTeam2[i],tileW/2-4+2+i,tileH-2,1);
		}
		foreach (Character c in hexaGrid.charList) hexaGrid.getHexa(c.x,c.y).changeType2(HexaType.GROUND);
		currentCharControlledID = 0;
		currentCharControlled = hexaGrid.charList[currentCharControlledID];
		decisionSequence = new List<ActionAIPos>();
	}
	
	void actionMove(Hexa hexaDestination){
		if (hexaDestination != null && hexaDestination.type == HexaType.GROUND){
			List<Point> path = hexaGrid.findShortestPath(currentCharControlled.x,currentCharControlled.y,hexaDestination.x,hexaDestination.y,currentCharControlled.PM);
			if (path != null && path.Count > 1){
				currentCharControlled.updatePos2(hexaDestination.x,hexaDestination.y,hexaGrid);
				nextTurn();
			}else if (whoControlsThisChar(currentCharControlled) != PlayerType.AI_EASY) actionMoveErrs++;
		}else if (whoControlsThisChar(currentCharControlled) != PlayerType.AI_EASY)actionMoveErrs++;
	}
	
	// must trust the AI to choose right
	void actionMoveNoCheck(Hexa hexaDestination){
		currentCharControlled.updatePos2(hexaDestination.x,hexaDestination.y,hexaGrid);
		nextTurn();
	}
	
	void actionUseAttack(MainGame.ActionType attack,Hexa hexaDestination){
		CharsDB.Attack attackUsed_;
		if (attack == MainGame.ActionType.ATK) attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].basicAttack;
		else attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].ult;
		if (hexaDestination != null && hexaGrid.hexaInSight(currentCharControlled.x,currentCharControlled.y,hexaDestination.x,hexaDestination.y,attackUsed_.range)){
			if (attack == MainGame.ActionType.ULT){
				currentCharControlled.skillAvailable = false;
			}
		}else if (whoControlsThisChar(currentCharControlled) != PlayerType.AI_EASY) actionAtkErrs++;
		
		List<Character> hits = hexaGrid.getCharWithinRange(hexaDestination.x,hexaDestination.y,currentCharControlled.x,currentCharControlled.y,attackUsed_.rangeAoE,attackUsed_.aoEType);
		// Filter target(s)
		if (attackUsed_.targetsEnemies == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i].team != currentCharControlled.team){
					hits.RemoveAt(i); i--;
				}
			}
		}
		if (attackUsed_.targetsAllies == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i].team == currentCharControlled.team){
					hits.RemoveAt(i); i--;
				}
			}
		}
		if (attackUsed_.targetsSelf == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i] == currentCharControlled){
					hits.RemoveAt(i); i--;
				}
			}
		}
		foreach (Character c in hits){
			switch (attackUsed_.attackEffect){
				case CharsDB.AttackEffect.DAMAGE  : {
					if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.addToDamageTaken(c,attackUsed_.effectValue);
					if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,attackUsed_.effectValue);
					
					c.HP -= attackUsed_.effectValue;
					// Enemy dies
					if (c.HP <= 0){
						if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.setDead(c,true);
						if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToKills(currentCharControlled,1);
						c.HP = 0;
						hexaGrid.getHexa(c.x,c.y).charOn = null;
						GameObject.Destroy(c.go);
						for (int i=0;i<hexaGrid.charList.Count;i++){
							if (hexaGrid.charList[i] == c){
								hexaGrid.charList.RemoveAt(i);
							}
						}
						// update currentCharControlled ID
						for (int i=0;i<hexaGrid.charList.Count;i++){
							if (hexaGrid.charList[i] == currentCharControlled) currentCharControlledID = i;
						}
						// force AI to make a new decision
						decisionSequence = new List<ActionAIPos>();
						// check if there is a winner
						int nbT1 = 0;
						int nbT2 = 0;
						foreach (Character c2 in hexaGrid.charList){
							if (c2.team == 0) nbT1++;
							else nbT2++;
						}
						if (nbT1 == 0) winner = 1;
						else if (nbT2 == 0) winner = 0;
					}
				} break;
				case CharsDB.AttackEffect.HEAL    : {
					int heal = attackUsed_.effectValue;
					if (heal > c.HPmax - c.HP) heal = c.HPmax - c.HP;
					
					if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,heal);
					
					c.HP += heal;
				} break;
				case CharsDB.AttackEffect.PA_BUFF : {
					if (c.PA == c.getClassData().basePA){
						
						if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,attackUsed_.effectValue);
						
						c.PA += attackUsed_.effectValue;
					}
				} break;
			}
		}
		nextTurn();
	}
	
	void nextTurn(){
		currentCharControlled.PA--;
		// Next char turn
		if (currentCharControlled.PA <= 0){
			nbTurns++;
			currentCharControlled.PA = CharsDB.list[(int)currentCharControlled.charClass].basePA;
			do {
				currentCharControlledID = (currentCharControlledID+1)%hexaGrid.charList.Count;
				currentCharControlled = hexaGrid.charList[currentCharControlledID];
			} while (currentCharControlled.HP <= 0);
			PlayerType currentPlayerType = whoControlsThisChar(currentCharControlled);
			if (currentPlayerType == PlayerType.AI_HARD || currentPlayerType == PlayerType.AI_MEDIUM){
				statsGame.nextTurn(currentCharControlled);
			}
			decisionSequence = new List<ActionAIPos>();
		}
	}
	
	void algoGenAIHard(){

		
		// MUTATIONS
		int maxMutations = (AIHard.rules.classifiers.Count < 500) ? 20 : 15;
		int mutations = 0;
		for (int i=0;i<100;i++){
			if (AIHard.rules.mutateRandomClassifiers(0.3f,5) != null) mutations++;
			if (mutations >= maxMutations) break;
		}
		nbMutations += mutations;
		
		// find average fitness
	
		float avgFitness = 0.0f;
		foreach (Classifier c in AIHard.rules.classifiers) avgFitness += c.fitness;
		avgFitness = avgFitness / (float)AIHard.rules.classifiers.Count;
		
		
		// MERGE
		int maxMerges = 2;
		int merges = 0;
		for (int i=0;i<100;i++){
			if (AIHard.rules.mergeSimilarClassifiers(6,avgFitness+(1.0f-avgFitness)*0.2f) != null) merges++;
			if (merges >= maxMerges) break;
		}
		nbMerges += merges;
		
		// REMOVES
		int maxRemovals = 50;
		int removals = 0;
		for (int i=0;i<100;i++){
			if (AIHard.rules.removeBadClassifier(6,avgFitness*1.1f) != null) removals++;
			if (removals >= maxRemovals) break;
		}
		nbRemovals += removals;
	}
	
	void algoGenAIMedium(){
		// MUTATIONS
		int maxMutations = (AIMedium.rules.classifiers.Count < 500) ? 15 : 10;
		int mutations = 0;
		for (int i=0;i<100;i++){
			if (AIMedium.rules.mutateRandomClassifiers(0.3f,10) != null) mutations++;
			if (mutations >= maxMutations) break;
		}
		nbMutations += mutations;

		float avgFitness = 0.0f;
		foreach (Classifier c in AIMedium.rules.classifiers) avgFitness += c.fitness;
		avgFitness = avgFitness / (float)AIHard.rules.classifiers.Count;


		// MERGE
		int maxMerges = 1;
		int merges = 0;
		for (int i = 0; i < 100; i++)
		{
			if (AIMedium.rules.mergeSimilarClassifiers(6, avgFitness + (1.0f - avgFitness) * 0.2f) != null) merges++;
			if (merges >= maxMerges) break;
		}
		nbMerges += merges;

		// REMOVES
		int maxRemovals = 50;
		int removals = 0;
		for (int i = 0; i < 100; i++)
		{
			if (AIMedium.rules.removeBadClassifier(6, avgFitness * 1.15f) != null) removals++;
			if (removals >= maxRemovals) break;
		}
		nbRemovals += removals;
	}
}
