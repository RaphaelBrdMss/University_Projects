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
using static MenuPrincipal;
using static MenuSlection;

// ##################################################################################################################################################
// MAIN
// ##################################################################################################################################################

public class MainGame : MonoBehaviour {
	// Init in character selection menu
	public static StartGameData startGameData;
	
	// Init in Unity
	public Mesh hexaFilledMesh;
	public Mesh hexaHollowMesh;
	public Mesh hexaWallMesh;
	public GameObject ruinsMap;
	public GameObject arrow;
	public GameObject hexaHighlight;
	public GameObject hexasFolder;
	public GameObject hexaTemplate;
	public GameObject charactersFolder;
	public GameObject characterTemplate;
	public List<GameObject> characterTemplateModels;
	public GameObject particleExplosion;
	public GameObject particleHeal;
	public GameObject damageValueDisplay;
	public GameObject camera_;
	public Transform  cameraPos;
	public Vector3    cameraPosGoal;
	public GameObject UICurrentChar;
	public GameObject UICharTurns;
	public GameObject UICharTurnTemplate;
	public GameObject UIAction;
	public GameObject UIPauseMenu;
	public GameObject UIVictoryScreen;
	public List<Texture> charCards;
	public int tileW;
	public int tileH;
	public bool lockedCamera;
	public Toggle toggleLockedCamera;
	public bool debugMode;
	public bool pauseMenu;
	
	public int frame;
	public bool updateUI;
	public bool updateMouseHover;
	public HexaGrid hexaGrid;
	public Vector3 mousePosOld;
	public Vector3 mousePos;
	public Hexa hexaHoveredOld;
	public Hexa hexaHovered;
	public Character currentCharControlled;
	public int currentCharControlledID;
	public List<GameObject> UICharTurnsList;
	public List<GameObject> pathFinderDisplay;
	public List<GameObject> lineOfSightDisplay;
	public List<GameObject> dangerDisplay;
	public List<Point> pathWalk;
	public Character charHovered;
	public int attackUsed;
	public Point attackUsedTargetHexa;
	public CharsDB.Attack attackUsedAttack;
	public int pathWalkpos;
	public int newTurn;
	public int AIwaitingTime;
	public int winner;
	public StatsGame statsGame;
	
	public Color pathDisplayColor;
	public Color allPathsDisplayColor;
	public Color lineOfSightColor;
	public Color blockedSightColor;
	public Color AoEColor;
	
	public enum ActionType { MOVE,ATK,SPELL,ULT,SKIP };
	public ActionType actionType;
	public List<ActionAIPos> decisionSequence; // AI decisions
		
    // Start is called before the first frame update
    void Start(){

		// Init Hexa/Char global variables
		Hexa.hexasFolder    = hexasFolder;
		Hexa.hexaFilledMesh = hexaFilledMesh;
		Hexa.hexaHollowMesh = hexaHollowMesh;
		Hexa.hexaWallMesh   = hexaWallMesh;
		Hexa.hexaTemplate   = hexaTemplate;
		Character.characterTemplate = characterTemplate;
		Character.characterTemplateModels = characterTemplateModels;
		Character.charactersFolder  = charactersFolder;
		CharsDB.initCharsDB();
		Hexa.offsetX = -((tileW-1) * 0.75f) / 2;
		Hexa.offsetY = -((tileH-1) * -0.86f + ((tileW-1)%2) * 0.43f) / 2;
		
		hexaHighlight.GetComponent<Renderer>().material.color = new Color(1,1,1,0.25f);
		frame = 0;
		updateUI = true;
		updateMouseHover = true;
		pauseMenu = false;
		
		// Init game data if it's not (it should be in the main menu)
		if (startGameData == null){
			startGameData = new StartGameData();
			startGameData.loadSave = false;
			startGameData.charsTeam1 = new List<CharClass>();
			startGameData.charsTeam1.Add(CharClass.VOLEUR);
			startGameData.charsTeam1.Add(CharClass.GUERRIER);
			startGameData.charsTeam2 = new List<CharClass>();
			startGameData.charsTeam2.Add(CharClass.SOIGNEUR);
			startGameData.charsTeam2.Add(CharClass.ARCHER);
			startGameData.player1Type = PlayerType.AI_HARD;
			startGameData.player2Type = PlayerType.AI_EASY;
			startGameData.mapChosen = 1;
		}
		
		if (startGameData.loadSave){
			loadGame("Data/Save/gameSave");
		}else{
			// Init hexa grid
			hexaGrid = new HexaGrid();
			if (startGameData.mapChosen == 0){
				hexaGrid.createGridFromFile("Data/Map/ruins");
				tileW = hexaGrid.w; tileH = hexaGrid.h;
				ruinsMap.SetActive(true);
				foreach (Hexa hexa in hexaGrid.hexaList){
					if (hexa.type != HexaType.GROUND){
						hexa.go.GetComponent<Renderer>().enabled = false;
					}
				}
			}else if (startGameData.mapChosen >= 1){
				hexaGrid.createRandomRectGrid(tileW,tileH);
			}
			// Put characters on the grid
			for (int i=0;i<5;i++){
				if (i < startGameData.charsTeam1.Count) hexaGrid.addChar(startGameData.charsTeam1[i],tileW/2-4+2+i,2,0);
				if (i < startGameData.charsTeam2.Count) hexaGrid.addChar(startGameData.charsTeam2[i],tileW/2-4+2+i,tileH-2,1);
			}
			foreach (Character c in hexaGrid.charList) hexaGrid.getHexa(c.x,c.y).changeType(HexaType.GROUND);
			currentCharControlledID = 0;
			currentCharControlled = hexaGrid.charList[currentCharControlledID];
		}
			
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
		AIHard.learn = false;
		AIUtil.hexaGrid = hexaGrid;
		
		mousePos = Input.mousePosition;
		hexaHovered = null;
		hexaHoveredOld = null;
		charHovered = null;
		
		pathFinderDisplay = new List<GameObject>();
		lineOfSightDisplay = new List<GameObject>();
		pathWalk = null;
		attackUsed = 0;
		pathWalkpos = 0;
		newTurn = 0;
		winner = -1;
		statsGame = new StatsGame();
		actionType = ActionType.MOVE;
		UICharTurns.SetActive(true);
		UIAction.SetActive(true);
		{ // Init character turn list UI
			int i = 0;
			foreach (Character c in hexaGrid.charList){
				GameObject go = GameObject.Instantiate(UICharTurnTemplate,UICharTurns.transform);
				go.transform.localPosition = new Vector3(200+i,0,0);
				go.transform.GetChild(0).GetComponent<Image>().color = (c.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
				go.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)c.charClass];
				UICharTurnsList.Add(go);
				i += 80;
			}
		}
		// Init Camera
		cameraPosGoal = cameraPos.position;
		lockedCamera = (startGameData.player1Type != PlayerType.HUMAN && startGameData.player2Type != PlayerType.HUMAN);
		toggleLockedCamera.isOn = lockedCamera;
		AIwaitingTime = lockedCamera ? 0 : 20;
    }

    // Update is called once per frame
    void Update(){
		frame++;
		// Update mouse position
		mousePosOld = mousePos;
		mousePos    = Input.mousePosition;
		
		// PAUSE MENU
		if (pauseMenu){
			if (Input.GetMouseButtonDown(0)){
				int menuPosX = Screen.width/2 - 80;
				int menuPosY = Screen.height/2 + 100 - 100;
				// Save the current game
				if (mousePos.x >= menuPosX && mousePos.x < menuPosX+160 && mousePos.y >= menuPosY-30 && mousePos.y < menuPosY-30+20){
					saveGame("Data/Save/gameSave");
				// Quit
				}else if (mousePos.x >= menuPosX && mousePos.x < menuPosX+160 && mousePos.y >= menuPosY-60 && mousePos.y < menuPosY-60+20){
					SceneManager.LoadScene(0);
					AIHard.rules.saveAllInBinary("Data/Classifiers/classifiersBinary");
					AIMedium.rules.saveAllInBinary("Data/Classifiers/classifiers2Binary");
					//AIHard.rules.saveAllInDB();
					Debug.Log("Save in database");
					// back
				}
				else if (mousePos.x >= Screen.width/2 - 80 && mousePos.x < Screen.width/2 + 80 && mousePos.y >= Screen.height-30 && mousePos.y < Screen.height){
					pauseMenu = false;
					UIPauseMenu.SetActive(false);
				}
				lockedCamera = toggleLockedCamera.isOn;
			}
		}else if (winner == -1){ 
			// ZOOM (MOUSEWHEEL)
			if (Input.GetAxis("Mouse ScrollWheel") != 0.0f ){
				cameraPosGoal = new Vector3(cameraPosGoal.x,cameraPosGoal.y-Input.GetAxis("Mouse ScrollWheel")*2.0f,cameraPosGoal.z+Input.GetAxis("Mouse ScrollWheel")*2.0f); 
			}
			// MOVE CAMERA (MIDDLE CLICK OR ZQSD (AZERTY keyboard))
			if (Input.GetMouseButton(2) && !Input.GetMouseButtonDown(2)){
				cameraPosGoal = new Vector3(cameraPosGoal.x + (mousePosOld.x-mousePos.x)*0.003f*cameraPosGoal.y,cameraPosGoal.y,cameraPosGoal.z + (mousePosOld.y-mousePos.y)*0.003f*cameraPosGoal.y);
			}
			if (Input.GetKey(KeyCode.Z)){
				cameraPosGoal = new Vector3(cameraPosGoal.x,cameraPosGoal.y,cameraPosGoal.z+1);
			}
			if (Input.GetKey(KeyCode.Q)){
				cameraPosGoal = new Vector3(cameraPosGoal.x-1,cameraPosGoal.y,cameraPosGoal.z);
			}
			if (Input.GetKey(KeyCode.S)){
				cameraPosGoal = new Vector3(cameraPosGoal.x,cameraPosGoal.y,cameraPosGoal.z-1);
			}
			if (Input.GetKey(KeyCode.D)){
				cameraPosGoal = new Vector3(cameraPosGoal.x+1,cameraPosGoal.y,cameraPosGoal.z);
			}
			// OPEN PAUSE MENU
			if (Input.GetMouseButtonDown(0)){
				if (mousePos.x >= Screen.width/2 - 80 && mousePos.x < Screen.width/2 + 80 && mousePos.y >= Screen.height-30 && mousePos.y < Screen.height){
					pauseMenu = true;
					UIPauseMenu.SetActive(true);
				}
			}
		}
		
		// SMOOTH CAMERA
		cameraPos.position = new Vector3(cameraPos.position.x*0.85f+cameraPosGoal.x*0.15f,cameraPos.position.y*0.85f+cameraPosGoal.y*0.15f,cameraPos.position.z*0.85f+cameraPosGoal.z*0.15f);
		
		
		// MAIN GAME LOOP
		if (winner == -1){
			if (pathWalk != null){
				// Walking animation when going from an hexa to another
				walkingAnimation();
			}else if (attackUsed > 0){
				// Attack animation
				if (attackUsed == 1){
					useAttack();
				}
				attackUsed--;
			// Interaction with the game
			}else if (!pauseMenu){
				PlayerType currentPlayerType = whoControlsThisChar(currentCharControlled);
				// ACTIONS FOR HUMAN PLAYERS
				if (currentPlayerType == PlayerType.HUMAN){
					// HOVER DETECTION : hovered hexa is stored in hexaHovered
					getHoveredHexa();
					
					// Right Click changes from wall to ground and vice versa (debug) probleme pas seulement en mode debug
					if (debugMode){
						if (Input.GetMouseButtonDown(1) && hexaHovered != null){
							switch (hexaHovered.type){
								case HexaType.GROUND : hexaHovered.changeType(HexaType.VOID); break;
								case HexaType.VOID   : hexaHovered.changeType(HexaType.WALL); break;
								case HexaType.WALL   : hexaHovered.changeType(HexaType.GROUND);   break;
							}
							updateMouseHover = true; updateUI = true;
						}
					}
					
					// TEST Classifier (debug)
					if (Input.GetKeyDown(KeyCode.T)){
						Classifier1 c = new Classifier1(hexaGrid,currentCharControlledID);
						Debug.Log(c.getStringInfo());
					}
					
					// A Key : action (Move / Attack) (cycle through them)
					if (Input.GetKeyDown(KeyCode.A)){
						switch (actionType){
							case ActionType.MOVE : actionType = ActionType.ATK; break;
							case ActionType.ATK : actionType = (currentCharControlled.skillAvailable) ? ActionType.ULT : ActionType.MOVE;  break;
							case ActionType.ULT : actionType = ActionType.MOVE; break;
						}
						updateMouseHover = true; updateUI = true;
					}
					
					// Left Click : action (Move / Attack) or change action Type (UI) or skip turn ici ce n'est pas des button réelement....
					if (Input.GetMouseButtonDown(0)){
						if (mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-275 && mousePos.y < Screen.height-275+20){
							actionType = ActionType.MOVE;
						}else if(mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-300 && mousePos.y < Screen.height-300+20){
							actionType = ActionType.ATK;
						}else if (mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-325 && mousePos.y < Screen.height-325+20){
							if(currentCharControlled.PA>=2) actionType=ActionType.SPELL;
						}else if (mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-350 && mousePos.y < Screen.height-350+20){
							if (currentCharControlled.skillAvailable) actionType = ActionType.ULT;
						}else if (mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-375 && mousePos.y < Screen.height-375+20){
							currentCharControlled.PA = 0;
							nextTurn();
						}else{
							switch (actionType){
								case ActionType.MOVE : actionMove(hexaHovered); break;
								case ActionType.ATK : case ActionType.SPELL : case ActionType.ULT : actionUseAttack(actionType,hexaHovered); break;
								case ActionType.SKIP : break;
							}

							
						}
					}
				// ACTIONS FOR AI PLAYERS
				}else{
					if (newTurn > AIwaitingTime){
						if (decisionSequence.Count == 0){
							switch (currentPlayerType){
								case PlayerType.HUMAN : // failsafe
								case PlayerType.AI_EASY : decisionSequence = AIEasy.decide(currentCharControlledID); break;
								case PlayerType.AI_MEDIUM :	decisionSequence = AIMedium.decide(currentCharControlledID,statsGame); break;
								case PlayerType.AI_HARD : decisionSequence = AIHard.decide(currentCharControlledID,statsGame); break;
							}
						}else{
							ActionAIPos actionAIPos = decisionSequence[0]; decisionSequence.RemoveAt(0);
							//Debug.Log(currentPlayerType + " : " + actionAIPos.action + ((actionAIPos.pos != null) ? (" " + actionAIPos.pos.x + " " + actionAIPos.pos.y) : ""));
							Classifier1 c = new Classifier1(hexaGrid, currentCharControlledID);
							Debug.Log(c.getStringInfo()); 
							switch (actionAIPos.action){
								case ActionType.MOVE: actionMove(hexaGrid.getHexa(actionAIPos.pos)); break;
								case ActionType.ATK : 	case ActionType.SPELL : case ActionType.ULT : actionUseAttack(actionAIPos.action,hexaGrid.getHexa(actionAIPos.pos)); break;
								case ActionType.SKIP : { currentCharControlled.PA = 0; nextTurn(); } break;
							}
						}
					}
				}
			}
		// DISPLAY WINNER
		}else{
			UIPauseMenu.SetActive(false);
			UICharTurns.SetActive(false);
			UIAction.SetActive(false);
			UIVictoryScreen.SetActive(true);
			UIVictoryScreen.transform.GetChild(0).GetComponent<Text>().text = "VICTOIRE DE L'EQUIPE " + ((winner == 0) ? "ROUGE" : "BLEUE");
			UIVictoryScreen.transform.GetChild(0).GetComponent<Text>().color = ((winner == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR);
			
			// Back to menu
			if (Input.GetMouseButtonDown(0)){
				if (mousePos.x >= Screen.width/2 - 100 && mousePos.x < Screen.width/2 + 100 && mousePos.y >= Screen.height/2-90 && mousePos.y < Screen.height/2-90+40){
					// EVALUATE AI HARD
					statsGame.endGame(winner,hexaGrid);
					if (AIHard.learn) statsGame.evaluateGame();
					statsGame.evaluateGame();
					SceneManager.LoadScene(0);
					AIHard.rules.saveAllInBinary("Data/Classifiers/classifiersBinary");
					AIMedium.rules.saveAllInBinary("Data/Classifiers/classifiers2Binary");
					//AIHard.rules.saveAllInDB();
					Debug.Log("Save in database");
				}
			}
		}
		
		// - DISPLAY -------------------------------------------------------------------------------
		
		// CENTER CHARACTER MODEL
		foreach (Character c in hexaGrid.charList){
			if (c.go.transform.GetChild(1))	c.go.transform.GetChild(1).transform.position = c.go.transform.position;
		}
		if (winner == -1){
			// Display arrow above the current character controlled
			{
				float currentHeight = (((newTurn%60) < 30) ? (newTurn%60) : (60-(newTurn%60))) / 60.0f * 0.2f;
				arrow.transform.position = new Vector3(currentCharControlled.go.transform.position.x,currentHeight+1.5f,currentCharControlled.go.transform.position.z);
				if (newTurn == 0){
					hexaHighlight.GetComponent<MeshFilter>().mesh = hexaHollowMesh;
				}else if (newTurn < 10){
					hexaHighlight.transform.localScale = new Vector3(1+(10-newTurn)*0.3f,1+(10-newTurn)*0.3f,1);
				}else if (newTurn == 10){
					hexaHighlight.transform.localScale = new Vector3(1,1,1);
					hexaHighlight.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
				}
				hexaHighlight.transform.position = new Vector3(currentCharControlled.go.transform.position.x,-0.013f,currentCharControlled.go.transform.position.z);
				newTurn++;
			}
			
			if (updateMouseHover){
				// Clear previous hexas displayed
				foreach (GameObject go in pathFinderDisplay) GameObject.Destroy(go);
				pathFinderDisplay = new List<GameObject>();
				foreach (GameObject go in lineOfSightDisplay) GameObject.Destroy(go);
				lineOfSightDisplay = new List<GameObject>(); 
				// Display hovered hexa
				displayHoveredHexa();
			}
			
			// Display path in green / line of sight in blue / AoE in red
			if (pathWalk == null && updateMouseHover && whoControlsThisChar(currentCharControlled) == PlayerType.HUMAN){
				switch (actionType){
					case ActionType.MOVE : displayPossiblePaths(); displaySortestPath(); break;
					case ActionType.ATK : case ActionType.SPELL : case ActionType.ULT : displayLineOfSight(); break;
					case ActionType.SKIP : break;
				}
			}
			
			// Display Danger
			/*if (newTurn == 1){
				foreach (GameObject go in dangerDisplay) GameObject.Destroy(go);
				dangerDisplay = new List<GameObject>();
				List<HexaDamage> dangerList = AIUtil.threatAtHexas(hexaGrid.findAllPaths(currentCharControlled.x,currentCharControlled.y,currentCharControlled.PM*currentCharControlled.PA),currentCharControlledID+1,currentCharControlledID);

				foreach (HexaDamage p in dangerList){
					GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
					go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
					go.GetComponent<MeshCollider>().enabled = false;
					go.transform.position = Hexa.hexaPosToReal(p.x,p.y,-0.017f);
					if (p.value < 0) go.GetComponent<Renderer>().material.color = new Color(-p.value*0.05f,0,0);
					else  go.GetComponent<Renderer>().material.color = new Color(0,p.value*0.05f,0);
					dangerDisplay.Add(go);
				}
			}*/
			
			if (Input.GetKeyDown(KeyCode.W)){
				foreach (GameObject go in dangerDisplay) GameObject.Destroy(go);
				dangerDisplay = new List<GameObject>();
				
				Character target = AIUtil.getMainTargetAttack(currentCharControlledID);
				if (target != null){
					Point p = AIUtil.AIHard2.doAttackPos(currentCharControlledID,hexaGrid.getCharID(target));
					GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
					go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
					go.GetComponent<MeshCollider>().enabled = false;
					go.transform.position = Hexa.hexaPosToReal(p.x,p.y,-0.017f);
					//if (p.value < 0) go.GetComponent<Renderer>().material.color = new Color(-p.value*0.05f,0,0);
					//else  go.GetComponent<Renderer>().material.color = new Color(0,p.value*0.05f,0);
					dangerDisplay.Add(go);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.X)){
				foreach (GameObject go in dangerDisplay) GameObject.Destroy(go);
				dangerDisplay = new List<GameObject>();
				
				{
					Point p = AIUtil.AIHard2.doAttackPos(currentCharControlledID,hexaGrid.getCharID(AIUtil.getMainTargetAttack(currentCharControlledID)));
					GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
					go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
					go.GetComponent<MeshCollider>().enabled = false;
					go.transform.position = Hexa.hexaPosToReal(p.x,p.y,-0.017f);
					//if (p.value < 0) go.GetComponent<Renderer>().material.color = new Color(-p.value*0.05f,0,0);
					//else  go.GetComponent<Renderer>().material.color = new Color(0,p.value*0.05f,0);
					dangerDisplay.Add(go);
				}
			}
			
			if (Input.GetKeyDown(KeyCode.C)){
				Classifier2 c = new Classifier2(hexaGrid,currentCharControlledID);
				
				Debug.Log(c.getStringInfo());
			}
			
			// Display UI
			if (updateUI){
				displayCharTurnList();
				displayActionButtons();
			}
		// Display Winner
		}else{
			//..
		}
		
		updateMouseHover = false; updateUI = false;
    }
	
	// ##################################################################################################################################################
	// Functions used in main
	// ##################################################################################################################################################
	
	PlayerType whoControlsThisChar(Character c){
		return (c.team == 0) ? startGameData.player1Type : startGameData.player2Type;
	}
	
	public void getHoveredHexa(){
		if ((mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-275 && mousePos.y < Screen.height-275+20) ||
		(mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-300 && mousePos.y < Screen.height-300+20) ||
		(mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-325 && mousePos.y < Screen.height-325+20) ||
		(mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-350 && mousePos.y < Screen.height-350+20) ||
		(mousePos.x >= 20 && mousePos.x < 20+120 && mousePos.y >= Screen.height-375 && mousePos.y < Screen.height-375+20) ||
		(mousePos.x >= Screen.width/2 - 80 && mousePos.x < Screen.width/2 + 80 && mousePos.y >= Screen.height-30 && mousePos.y < Screen.height)){
			if (hexaHovered != null){
				hexaHoveredOld = hexaHovered;
				hexaHovered = null;
				charHovered = null;
			}
			updateMouseHover = true;
			updateUI = true;
		}else{
			RaycastHit raycastHit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			Debug.DrawRay(ray.origin,ray.direction*100,Color.red);
			bool success = false;
			if (Physics.Raycast(ray.origin,ray.direction,out raycastHit,100)) success = (raycastHit.transform.gameObject.tag == "Hexa");
			if (success){
				Hexa hexaHit = raycastHit.transform.gameObject.GetComponent<HexaGO>().hexa;
				if (hexaHit != hexaHovered){
					hexaHoveredOld = hexaHovered;
					hexaHovered = hexaHit;
					charHovered = hexaHovered.charOn;
					updateMouseHover = true;
					updateUI = true;
				}
			}else{
				if (hexaHovered != null){
					hexaHoveredOld = hexaHovered;
					hexaHovered = null;
					charHovered = null;
				}
				updateMouseHover = true;
				updateUI = true;
			}
		}
	}
	void actionMove(Hexa hexaDestination){
		if (hexaDestination != null && hexaDestination.type == HexaType.GROUND){
			List<Point> path = hexaGrid.findShortestPath(currentCharControlled.x,currentCharControlled.y,hexaDestination.x,hexaDestination.y,currentCharControlled.PM*currentCharControlled.PA);
			if (path != null && path.Count > 1){
				pathWalk = path;
				pathWalkpos = 0;
			}else Debug.LogWarning("ActionMove : error");
		}else Debug.LogWarning("ActionMove : error");
	}
	
	void actionUseAttack(ActionType attack,Hexa hexaDestination){
		CharsDB.Attack attackUsed_;
		if (attack == ActionType.ATK) attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].basicAttack;
		else if(attack == ActionType.SPELL) attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].spell;
		else attackUsed_ = CharsDB.list[(int)currentCharControlled.charClass].ult;
		if (hexaDestination != null && hexaGrid.hexaInSight(currentCharControlled.x,currentCharControlled.y,hexaDestination.x,hexaDestination.y,attackUsed_.range)){
			if (attack == ActionType.ULT){
				currentCharControlled.skillAvailable = false;
				actionType = ActionType.ATK;
			}
			// Attack animation
			Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
			if (animator){
				animator.SetTrigger("Attack1Trigger");
			}
			attackUsedAttack = attackUsed_;
			attackUsedTargetHexa = new Point(hexaDestination.x,hexaDestination.y);
			attackUsed = 30; // Delay attack
			
			// Particles for mage / soigneur
			if (currentCharControlled.charClass == CharClass.SOIGNEUR){
				GameObject go = GameObject.Instantiate(particleHeal);
				go.transform.position = Hexa.hexaPosToReal(hexaDestination.x,hexaDestination.y,0);
				go.transform.localScale *= 0.1f;
				Destroy(go, 5);
			}else if (currentCharControlled.charClass == CharClass.MAGE){
				GameObject go = GameObject.Instantiate(particleExplosion);
				go.transform.position = Hexa.hexaPosToReal(hexaDestination.x,hexaDestination.y,0);
				go.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
				Destroy(go, 5);
			}
			
			// calculate the angle the character will be facing
			Vector3 v1 = Hexa.hexaPosToReal(hexaDestination.x,hexaDestination.y,0);
			Vector3 v2 = Hexa.hexaPosToReal(currentCharControlled.x,currentCharControlled.y,0);
			float x = v1.x-v2.x;
			float y = v1.z-v2.z;
			float d = Mathf.Sqrt(x*x + y*y);
			float cos_ = (x == 0) ? 0 : x/d;
			float angle = Mathf.Acos(cos_);
			if (y < 0) angle = -angle;
			int angleDegrees = (int)((angle*180)/(Mathf.PI));
			if (angleDegrees<0) angleDegrees = 360 + angleDegrees;
			angleDegrees = 360-angleDegrees+90;
			angleDegrees = (angleDegrees+5)/10*10;
			//Debug.Log(x + " " + y + " " + cos_ + " " + " " + angle + " " + angleDegrees);
			Transform charModel = currentCharControlled.go.transform.GetChild(1);
			if (charModel) charModel.eulerAngles = new Vector3(0,angleDegrees,0);
		}
		updateMouseHover = true;
		updateUI = true;
	}
	
	void useAttack(){
		List<Character> hits = hexaGrid.getCharWithinRange(attackUsedTargetHexa.x,attackUsedTargetHexa.y,currentCharControlled.x,currentCharControlled.y,attackUsedAttack.rangeAoE,attackUsedAttack.aoEType);
		// Filter target(s)
		if (attackUsedAttack.targetsEnemies == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i].team != currentCharControlled.team){
					hits.RemoveAt(i); i--;
				}
			}
		}
		if (attackUsedAttack.targetsAllies == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i].team == currentCharControlled.team){
					hits.RemoveAt(i); i--;
				}
			}
		}
		if (attackUsedAttack.targetsSelf == false){
			for (int i=0;i<hits.Count;i++){
				if (hits[i] == currentCharControlled){
					hits.RemoveAt(i); i--;
				}
			}
		}
		foreach (Character c in hits){
			switch (attackUsedAttack.attackEffect){
				case CharsDB.AttackEffect.DAMAGE  : {
					// Create object that shows damage dealt
					GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
					dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
					dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x,c.y,"-"+attackUsedAttack.effectValue,Color.red,60);
					
					if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.addToDamageTaken(c,attackUsedAttack.effectValue);
					if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,attackUsedAttack.effectValue);
					
					c.HP -= attackUsedAttack.effectValue;
					// Enemy dies
					if (c.HP <= 0){
						if (whoControlsThisChar(c) == PlayerType.AI_HARD) statsGame.setDead(c,true);
						if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToKills(currentCharControlled,1);
						c.HP = 0;
						hexaGrid.getHexa(c.x,c.y).charOn = null;
						GameObject.Destroy(c.go);
						for (int i=0;i<hexaGrid.charList.Count;i++){
							if (hexaGrid.charList[i] == c){
								GameObject.Destroy(UICharTurnsList[i]);
								UICharTurnsList.RemoveAt(i);
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
					// Create object that shows heal
					GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
					dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
					int heal = attackUsedAttack.effectValue;
					if (heal > c.HPmax - c.HP) heal = c.HPmax - c.HP;
					dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x,c.y,"+"+heal,Color.green,60);
					
					if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,heal);
					
					c.HP += heal;
				} break;
				case CharsDB.AttackEffect.PA_BUFF : {
					if (c.PA == c.getClassData().basePA){
						// Create object that shows pa buff
						GameObject dmgValueDisp = GameObject.Instantiate(damageValueDisplay);
						dmgValueDisp.GetComponent<DamageValueDisplay>().camera_ = cameraPos;
						dmgValueDisp.GetComponent<DamageValueDisplay>().setValue(c.x,c.y,"+"+attackUsedAttack.effectValue,Color.blue,60);
						
						if (whoControlsThisChar(currentCharControlled) == PlayerType.AI_HARD) statsGame.addToDamageDealt(currentCharControlled,attackUsedAttack.effectValue);
						
						c.PA += attackUsedAttack.effectValue;
					}
				} break;
			}
		}
		nextTurn();
	}
	
	void walkingAnimation(){
		int speed = 6;
		if (pathWalkpos == 0){
			Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
			if (animator){
				animator.SetBool ("Moving", true);
				animator.SetBool ("Running", true);
			}
		}
		if (pathWalkpos < (pathWalk.Count-1)*speed){
			//currentCharControlled.updatePos(pathWalk[pathWalkpos/speed].x,pathWalk[pathWalkpos/speed].y,hexaGrid);
			
			for (int i=0;i<6;i++){
				Point p = HexaGrid.findPos(pathWalk[pathWalkpos/speed].x,pathWalk[pathWalkpos/speed].y,(HexaDirection)i);
				if (p.x == pathWalk[pathWalkpos/speed+1].x && p.y == pathWalk[pathWalkpos/speed+1].y){
					currentCharControlled.setDirection((HexaDirection)i);
					i = 6;
				}
			}
			
			float multiplier = (pathWalkpos%speed)/(float)speed;
			
			float x1 = pathWalk[pathWalkpos/speed].x*0.75f;
			float x2 = pathWalk[pathWalkpos/speed+1].x*0.75f;
			float x  = x1 * (1.0f-multiplier) + x2 * multiplier;
			
			float y1 = pathWalk[pathWalkpos/speed].y * -0.86f + (pathWalk[pathWalkpos/speed].x%2) * 0.43f;
			float y2 = pathWalk[pathWalkpos/speed+1].y * -0.86f + (pathWalk[pathWalkpos/speed+1].x%2) * 0.43f;
			float y  = y1 * (1.0f-multiplier) + y2 * multiplier;
			currentCharControlled.go.transform.position = new Vector3(x+Hexa.offsetX,0,y+Hexa.offsetY);
			pathWalkpos++;
		}else{
			currentCharControlled.updatePos(pathWalk[pathWalk.Count-1].x,pathWalk[pathWalk.Count-1].y,hexaGrid);
			Animator animator = currentCharControlled.go.transform.GetChild(1).GetComponent<Animator>();
			if (animator){
				animator.SetBool ("Moving", false);
				animator.SetBool ("Running", false);
			}
			nextTurn();
		}
	}
	
	void nextTurn(){
		
		if( actionType==ActionType.SPELL){
			currentCharControlled.PA = currentCharControlled.PA- currentCharControlled.getClassData().spell.coutPA;
		}
		else if (actionType == ActionType.ULT){
			currentCharControlled.PA=currentCharControlled.PA- currentCharControlled.getClassData().ult.coutPA ;
		}else if(actionType == ActionType.ATK){
			currentCharControlled.PA=currentCharControlled.PA- currentCharControlled.getClassData().basicAttack.coutPA;
		}else{
			currentCharControlled.PA--;
		}
		// Next char turn
		if (currentCharControlled.PA <= 0){
			currentCharControlled.PA = CharsDB.list[(int)currentCharControlled.charClass].basePA;
			do {
				currentCharControlledID = (currentCharControlledID+1)%hexaGrid.charList.Count;
				currentCharControlled = hexaGrid.charList[currentCharControlledID];
			} while (currentCharControlled.HP <= 0);
			PlayerType currentPlayerType = (currentCharControlled.team == 0) ? startGameData.player1Type : startGameData.player2Type;
			if (currentPlayerType == PlayerType.AI_HARD || currentPlayerType == PlayerType.AI_MEDIUM){
				statsGame.nextTurn(currentCharControlled);
			}
			actionType = ActionType.MOVE;
			newTurn = 0;
			decisionSequence = new List<ActionAIPos>();
			if(!lockedCamera) cameraPosGoal = new Vector3(currentCharControlled.go.transform.position.x,cameraPosGoal.y,((hexaGrid.h*-0.43f)*0.0f+currentCharControlled.go.transform.position.z*1.0f) - cameraPosGoal.y*0.75f);
		}
		updateUI = true;
		updateMouseHover = true;
		pathWalk = null;
	}
	
	void loadGame(string filePath){
		if (File.Exists(filePath)){
			using (BinaryReader reader = new BinaryReader(File.Open(filePath,FileMode.Open))){
				// Read map chosen
				startGameData.mapChosen = (int)reader.ReadByte();
				hexaGrid = new HexaGrid();
				if (startGameData.mapChosen == 0){
					hexaGrid.createGridFromFile("Data/Map/ruins");
					tileW = hexaGrid.w; tileH = hexaGrid.h;
					ruinsMap.SetActive(true);
					foreach (Hexa hexa in hexaGrid.hexaList){
						if (hexa.type != HexaType.GROUND){
							hexa.go.GetComponent<Renderer>().enabled = false;
						}
					}
				}else{
					hexaGrid.w = reader.ReadInt32();
					hexaGrid.h = reader.ReadInt32();
					Hexa.offsetX = -((hexaGrid.w-1) * 0.75f) / 2;
					Hexa.offsetY = -((hexaGrid.h-1) * -0.86f + ((hexaGrid.w-1)%2) * 0.43f) / 2;
					for (int j=0;j<hexaGrid.h;j++){
						for (int i=0;i<hexaGrid.w;i++){
							hexaGrid.hexaList.Add(new Hexa((HexaType)reader.ReadByte(),i,j));
						}
					}
				}
				// Read Characters info
				int nbChar = reader.ReadInt32();
				for (int i=0;i<nbChar;i++){
					
					CharClass cCharClass = (CharClass)reader.ReadByte();
					int cTeam = (int)reader.ReadByte();
					int cX = reader.ReadInt32();
					int cY = reader.ReadInt32();
					Character c = new Character(cCharClass,cX,cY,cTeam);
					c.PA = (int)reader.ReadByte();
					c.HP = (int)reader.ReadByte();
					c.skillAvailable = (reader.ReadByte() == 0) ? false : true;
					c.directionFacing = (HexaDirection)reader.ReadByte();
					hexaGrid.addChar(c);
				}
				// Read Players info
				startGameData.player1Type = (PlayerType)reader.ReadByte();
				startGameData.player2Type = (PlayerType)reader.ReadByte();
				currentCharControlledID = reader.ReadInt32();
				currentCharControlled = hexaGrid.charList[currentCharControlledID];
			}
		}
	}
	
	void saveGame(string filePath){
		using (BinaryWriter writer = new BinaryWriter(File.Open(filePath,FileMode.Create))){
			// Write map chosen
			writer.Write((byte)startGameData.mapChosen);
			if (startGameData.mapChosen == 1){ // If we chose a random map, write it.
				writer.Write(hexaGrid.w);
				writer.Write(hexaGrid.h);
				int k = 0;
				for (int j=0;j<hexaGrid.h;j++){
					for (int i=0;i<hexaGrid.w;i++){
						writer.Write((byte)(hexaGrid.hexaList[k].type));
						k++;
					}
				}
			}
			// Write Characters info
			writer.Write(hexaGrid.charList.Count);
			foreach (Character c in hexaGrid.charList){
				writer.Write((byte)c.charClass);
				writer.Write((byte)c.team);
				writer.Write(c.x);
				writer.Write(c.y);
				writer.Write((byte)c.PA);
				writer.Write((byte)c.HP);
				writer.Write((byte)(c.skillAvailable ? 0 : 1));
				writer.Write((byte)c.directionFacing);
			}
			// Write Players info
			writer.Write((byte)startGameData.player1Type);
			writer.Write((byte)startGameData.player2Type);
			writer.Write(currentCharControlledID);
        }
	}
	
	// ##################################################################################################################################################
	// Display Functions used in main
	// ##################################################################################################################################################

	void displayCharTurnList(){
		for (int i=0;i<hexaGrid.charList.Count;i++){
			Character c = hexaGrid.charList[i];
			GameObject go = UICharTurnsList[i];
			if (c == currentCharControlled){
				go.transform.position = new Vector3(10000,0,10000);
				UICurrentChar.transform.GetChild(3).GetComponent<Text>().text = c.PA+"" ;
				UICurrentChar.transform.GetChild(6).GetComponent<Text>().text = c.HP + "/" + c.HPmax;
				UICurrentChar.transform.GetChild(1).GetComponent<RawImage>().texture = charCards[(int)c.charClass];
				if (c == charHovered){
					UICurrentChar.transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1);
				}else{
					UICurrentChar.transform.GetChild(0).GetComponent<Image>().color = (c.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
				}
			}else{
				int distanceTurn = (i > currentCharControlledID) ? (i - currentCharControlledID) : i + (hexaGrid.charList.Count-currentCharControlledID);
				go.transform.position = new Vector3(distanceTurn*80+85,Screen.height,0);
				go.transform.GetChild(3).GetComponent<Text>().text = c.PA+"" ;
				go.transform.GetChild(6).GetComponent<Text>().text = c.HP + "/" + c.HPmax;
				if (c == charHovered){
					go.transform.GetChild(0).GetComponent<Image>().color = new Color(1,1,1);
				}else{
					go.transform.GetChild(0).GetComponent<Image>().color = (c.team == 0) ? Character.TEAM_1_COLOR : Character.TEAM_2_COLOR;
				}
			}
		}
	}
	
	void displayActionButtons(){
		for (int i=0;i<4;i++){
			if (((int)actionType) == i) UIAction.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().fontStyle = FontStyle.Bold;
			else UIAction.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Text>().fontStyle = FontStyle.Normal;
		}
		if(currentCharControlled.PA<2) UIAction.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().color = new Color(0.33f,0.33f,0.33f);
		else UIAction.transform.GetChild(2).transform.GetChild(0).GetComponent<Text>().color = new Color(1,1,1);
		if (currentCharControlled.skillAvailable == false) UIAction.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().color = new Color(0.33f,0.33f,0.33f);
		else UIAction.transform.GetChild(3).transform.GetChild(0).GetComponent<Text>().color = new Color(1,1,1);
	}
	
	void displayHoveredHexa(){
		if (hexaHovered != null) hexaHovered.hoveredColor();
		if (hexaHoveredOld != null) hexaHoveredOld.defaultColor();
	}
	
	void displayPossiblePaths(){
		List<Point> path = hexaGrid.findAllPaths(currentCharControlled.x,currentCharControlled.y,currentCharControlled.PM);
		foreach (Point p in path){
			GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
			go.SetActive(true);
			go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x,p.y,-0.015f);
			go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
			go.GetComponent<Renderer>().material.color = allPathsDisplayColor;
			go.GetComponent<Collider>().enabled = false;
			pathFinderDisplay.Add(go);
		}
	}
	
	void displaySortestPath(){
		// Display path (create the green hexas)
		if (hexaHovered != null && hexaHovered.type == HexaType.GROUND){
			List<Point> path = hexaGrid.findShortestPath(currentCharControlled.x,currentCharControlled.y,hexaHovered.x,hexaHovered.y,currentCharControlled.PM);
			if (path != null){
				path.RemoveAt(0);
				foreach (Point p in path){
					GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
					go.SetActive(true);
					go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x,p.y,-0.014f);
					go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
					go.GetComponent<Renderer>().material.color = pathDisplayColor;
					go.GetComponent<Collider>().enabled = false;
					pathFinderDisplay.Add(go);
				}
			}
		}
	}
	
	void displayLineOfSight(){
		List<Point> hexasBlocked = null;
		int maxRange;
		if(actionType == ActionType.ATK){
			maxRange = currentCharControlled.getClassData().basicAttack.range;
		}else if(actionType == ActionType.SPELL){
			maxRange = currentCharControlled.getClassData().spell.range;
		}else{
			maxRange = currentCharControlled.getClassData().ult.range;
		}
		List<Point> pointList = hexaGrid.findHexasInSight(currentCharControlled.x,currentCharControlled.y,maxRange,out hexasBlocked);
		
		bool hexaHoveredTargetable = false;
		// Display line of sight (Blue hexas)
		foreach (Point p in pointList){
			if (hexaHovered != null && p.x == hexaHovered.x && p.y == hexaHovered.y) hexaHoveredTargetable = true;
			GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
			go.SetActive(true);
			go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x,p.y,-0.015f);
			go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
			go.GetComponent<Renderer>().material.color = lineOfSightColor;
			go.GetComponent<Collider>().enabled = false;
			lineOfSightDisplay.Add(go);
		}
		// Display blocked hexas (transparent blue hexas)
		foreach (Point p in hexasBlocked){
			GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
			go.SetActive(true);
			go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x,p.y,-0.015f);
			go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
			go.GetComponent<Renderer>().material.color = blockedSightColor;
			go.GetComponent<Collider>().enabled = false;
			lineOfSightDisplay.Add(go);
		}
		
		if (hexaHoveredTargetable){
			int rangeAoE;
			CharsDB.AoEType aoEType;
			if(actionType == ActionType.ATK){
			rangeAoE = currentCharControlled.getClassData().basicAttack.rangeAoE;
			aoEType = currentCharControlled.getClassData().basicAttack.aoEType;
		}else if(actionType == ActionType.SPELL){
			rangeAoE = currentCharControlled.getClassData().spell.rangeAoE;
			aoEType= currentCharControlled.getClassData().spell.aoEType;
		}else{
			rangeAoE = currentCharControlled.getClassData().ult.rangeAoE;
			aoEType = currentCharControlled.getClassData().ult.aoEType;
		}
			List<Point> hexaPos = hexaGrid.getHexasWithinRange(hexaHovered.x,hexaHovered.y,currentCharControlled.x,currentCharControlled.y,rangeAoE,aoEType);
			
			// Display AoE (red hexas)
			foreach (Point p in hexaPos){
				GameObject go = GameObject.Instantiate(hexaTemplate,hexasFolder.transform);
				go.SetActive(true);
				go.GetComponent<Transform>().position = Hexa.hexaPosToReal(p.x,p.y,-0.014f);
				go.GetComponent<MeshFilter>().mesh = hexaFilledMesh;
				go.GetComponent<Renderer>().material.color = AoEColor;
				go.GetComponent<Collider>().enabled = false;
				lineOfSightDisplay.Add(go);
			}
		}
	}

	public int getHexaHoveredx(){
		return hexaHovered.x;
	}
	public int getHexaHoveredy(){
		return hexaHovered.y;
	}
	
}













