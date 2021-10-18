using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Hexas;
using Misc;
using Characters;
using AI_Util;
using Classifiers;
using Classifiers1;
using Classifiers2;
using Stats;
using static MainGame;


namespace AI_Class {
	
	
public class ActionAIPos{
	public MainGame.ActionType action;
	public Point pos;
	
	public ActionAIPos(MainGame.ActionType action,Point pos){
		this.action = action;
		this.pos = pos;
	}
}

public class AI{
	public static HexaGrid hexaGrid;
	
	public static List<ActionAIPos> decide(int charID){
		List<ActionAIPos> sequence = new List<ActionAIPos>();
		sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
		return sequence;
	}
}

public class AIEasy : AI {
	new public static List<ActionAIPos> decide(int charID){
		Character currentChar = hexaGrid.charList[charID];
		List<ActionAIPos> sequence = new List<ActionAIPos>();
		
		switch (currentChar.charClass){
			case CharClass.GUERRIER :
			case CharClass.VOLEUR :
			case CharClass.ARCHER :
			case CharClass.MAGE : {
				int[] damage = AIUtil.calculateDamage(charID);
				List<Point> listH = hexaGrid.findAllPaths(currentChar.x,currentChar.y,currentChar.PM*currentChar.PA);
				if (listH != null && listH.Count > 0){
					// Find hexas where damage dealt is highest
					List<Point> bestHexas  = AIUtil.findHexasWhereValueIsMax(listH,damage);
					// Find hexas where position to lowest Enemy is lowest
					List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(charID,bestHexas);
					Point bestHexa = bestHexas2[0];
					// find path to hexa
					sequence = AIUtil.findSequencePathToHexa(charID,bestHexa.x,bestHexa.y);
					// Attack the Enemy
					int nbPA = currentChar.PA - sequence.Count;
					for (int i=0;i<nbPA;i++){
						Character cAttack = AIUtil.findCharToAttack(charID);
						if (cAttack != null){
							sequence.Add(new ActionAIPos(MainGame.ActionType.ATK,new Point(cAttack.x,cAttack.y)));
						}else if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y){
							sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
						}
					}
				}else{
					sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
				}
			} break;
			case CharClass.SOIGNEUR : {
				int[] healing = AIUtil.calculateHealing(charID);
				List<Point> listH = hexaGrid.findAllPaths(currentChar.x,currentChar.y,currentChar.PM*currentChar.PA);
				if (listH != null && listH.Count > 0){
					// Find hexas where healing done is highest
					List<Point> bestHexas  = AIUtil.findHexasWhereValueIsMax(listH,healing);
					// Find hexas where position to lowest Enemy is lowest
					List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(charID,bestHexas);
					Point bestHexa = bestHexas2[0];
					// find path to hexa
					sequence = AIUtil.findSequencePathToHexa(charID,bestHexa.x,bestHexa.y);
					int nbPA = currentChar.PA - sequence.Count;
					// Heal allies
					for (int i=0;i<nbPA;i++){
						Character cHeal = AIUtil.findCharToHeal(charID);
						if (cHeal != null){
							sequence.Add(new ActionAIPos(MainGame.ActionType.ATK,new Point(cHeal.x,cHeal.y)));
						}else if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y){
							sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
						}
					}
				}else{
					sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
				}
			} break;
			// TO DO
			case CharClass.ENVOUTEUR : {
				int[] healing = AIUtil.calculateBuff(charID);
				List<Point> listH = hexaGrid.findAllPaths(currentChar.x,currentChar.y,currentChar.PM*currentChar.PA);
				if (listH != null && listH.Count > 0){
					// Find hexas where healing done is highest
					List<Point> bestHexas  = AIUtil.findHexasWhereValueIsMax(listH,healing);
					// Find hexas where position to lowest Enemy is lowest
					List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(charID,bestHexas);
					Point bestHexa = bestHexas2[0];
					// find path to hexa
					sequence = AIUtil.findSequencePathToHexa(charID,bestHexa.x,bestHexa.y);
					int nbPA = currentChar.PA - sequence.Count;
					// Buff allies
					for (int i=0;i<nbPA;i++){
						Character cBuff = AIUtil.findCharToBuff(charID);
						if (cBuff != null){
							sequence.Add(new ActionAIPos(MainGame.ActionType.ATK,new Point(cBuff.x,cBuff.y)));
						}else if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y){
							sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
						}
					}
				}else{
					sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null));
				}
			} break;
		}
		
		return sequence;
	}
}


public class AIMedium : AI{
		public static ClassifierSystem<Classifiers1.Classifier1> rules;
		public static bool learn = true;

		public static List<ActionAIPos> decide(int charID, StatsGame statsGame)
		{
			Character currentChar = hexaGrid.charList[charID];
			List<ActionAIPos> sequence = new List<ActionAIPos>();
			Classifier1 rule = null;


			// Get the current situation
			Classifier1 currentSituation = new Classifier1(hexaGrid, charID);
			// Find matching classifiers to the current situation in the database 
			List<Classifier1> matchingRules = rules.findMatchingClassifiers(currentSituation);
			// Add the classifier to the database if no match is found
			if (matchingRules.Count == 0)
			{
				// Create a random classifier (forces attacking)
				bool att = currentSituation.isInRangeToUseAttack();
				bool skl = currentSituation.isInRangeToUseSkill();
				bool spell = currentSituation.isInRangeToUseSpell();
				if (att || skl || spell)
				{
					currentSituation.action = Classifier1.Action.Attack;
					if (skl) if (Random.Range(0, 2) == 0) currentSituation.action = Classifier1.Action.Skill;
				}
				else
				{
					int act = Random.Range(0, 3);
					if (act == 0)
					{
						currentSituation.action = Classifier1.Action.ApproachEnemy;
					}
					else if (act == 1)
					{
						if (hexaGrid.charList.Count == 2)
						{
							currentSituation.action = Classifier1.Action.ApproachEnemy;
						}
						else
							currentSituation.action = Classifier1.Action.ApproachAlly;
					}
					else
					{
						currentSituation.action = Classifier1.Action.ApproachEnemy;
					}
				}


				matchingRules.Add(currentSituation);
				foreach (Classifier1 c in matchingRules) rules.Add(c);
				//Debug.Log("Pas trouvé : on ajoute " + matchingRules.Count + " règle(s)");
			}
			else
			{
				//Debug.Log("Trouvé " + matchingRules.Count + " règle(s)");
			}

			List<Classifier1> validRules = new List<Classifier1>();
			// Check matching classifiers validity
			foreach (Classifier1 c in matchingRules)
			{
				if (c.isValid(charID, hexaGrid))
				{
					validRules.Add(c);
				}
			}

			// Get one classifier from matching/valid ones :
			if (validRules.Count == 0)
			{
				Debug.LogWarning("No valid classifier found !!");

				var situation = currentSituation;
				situation.action = Classifier1.Action.ApproachEnemy;
				validRules.Add(situation);
				situation.action = Classifier1.Action.ApproachAlly;
				validRules.Add(situation);
				situation.action = Classifier1.Action.Flee;
				validRules.Add(situation);
				situation.action = Classifier1.Action.Attack;

				validRules.Add(situation);
				situation.action = Classifier1.Action.Spell;
				validRules.Add(situation);
				rule = ClassifierList<Classifier1>.getClassifierRouletteWheel(validRules);



			}
			else
			{
				if (learn) rule = ClassifierList<Classifier1>.getClassifierRouletteWheel(validRules);
				else rule = ClassifierList<Classifier1>.getClassifierElististSelection(validRules);
				statsGame.addToRules(currentChar, rule);
				Debug.Log("Hard AI action : " + rule.action);

			}

			// Convert the action from the classifier to an action that can be executed in game
			switch (rule.action)
			{
				case Classifier1.Action.ApproachEnemy: sequence.Add(AIUtil.AIHard.doApproachEnemy(charID)); break;
				case Classifier1.Action.ApproachAlly: sequence.Add(AIUtil.AIHard.doApproachAlly(charID)); break;
				case Classifier1.Action.Flee: sequence.Add(AIUtil.AIHard.doFlee(charID)); break;
				case Classifier1.Action.Attack: sequence.Add(AIUtil.AIHard.doAttack(charID, 0)); break;
				case Classifier1.Action.Skill: sequence.Add(AIUtil.AIHard.doSkill(charID, 0)); break;
				default: sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null)); break;
			}

			return sequence;
		}
	}

public class AIHard : AI{
	public static ClassifierSystem<Classifiers1.Classifier1> rules;
	public static bool learn = true;
	
	public static List<ActionAIPos> decide(int charID,StatsGame statsGame){
		Character currentChar = hexaGrid.charList[charID];
		List<ActionAIPos> sequence = new List<ActionAIPos>();
		Classifier1 rule = null;
		
		
		// Get the current situation
		Classifier1 currentSituation = new Classifier1(hexaGrid,charID);
		// Find matching classifiers to the current situation in the database 
		List<Classifier1> matchingRules = rules.findMatchingClassifiers(currentSituation);
		// Add the classifier to the database if no match is found
		if (matchingRules.Count == 0){
			// Create a random classifier (forces attacking)
			bool att = currentSituation.isInRangeToUseAttack();
			bool skl = currentSituation.isInRangeToUseSkill();
			bool spell = currentSituation.isInRangeToUseSpell();
			if (att || skl||spell){
				currentSituation.action = Classifier1.Action.Attack;
				if (skl) if (Random.Range(0,2) == 0) currentSituation.action = Classifier1.Action.Skill;
			}else{
				int act = Random.Range(0,3);
				if (act == 0){
					currentSituation.action = Classifier1.Action.ApproachEnemy;
				}else if (act == 1){
					if(hexaGrid.charList.Count==2) {
							currentSituation.action = Classifier1.Action.ApproachEnemy;
					}else
						currentSituation.action = Classifier1.Action.ApproachAlly;
				}else{
					currentSituation.action = Classifier1.Action.ApproachEnemy;
				}
			}
			
			
			matchingRules.Add(currentSituation);
			foreach (Classifier1 c in matchingRules) rules.Add(c);
			//Debug.Log("Pas trouvé : on ajoute " + matchingRules.Count + " règle(s)");
		}else{
			//Debug.Log("Trouvé " + matchingRules.Count + " règle(s)");
		}
		
		List<Classifier1> validRules = new List<Classifier1>();
		// Check matching classifiers validity
		foreach (Classifier1 c in matchingRules){
			if (c.isValid(charID,hexaGrid)){
				validRules.Add(c);
			}
		}
		
		// Get one classifier from matching/valid ones :
		if (validRules.Count == 0){
				Debug.LogWarning("No valid classifier found !!");
				
					var situation = currentSituation;
					situation.action = Classifier1.Action.ApproachEnemy;
					validRules.Add(situation);
					situation.action = Classifier1.Action.ApproachAlly;
					validRules.Add(situation);
					situation.action = Classifier1.Action.Flee;
					validRules.Add(situation);
					situation.action = Classifier1.Action.Attack;
					
					validRules.Add(situation);
					situation.action = Classifier1.Action.Spell;
					validRules.Add(situation);
					rule = ClassifierList<Classifier1>.getClassifierRouletteWheel(validRules);

				
				
		}else{
			if (learn) rule = ClassifierList<Classifier1>.getClassifierRouletteWheel(validRules);
			else rule = ClassifierList<Classifier1>.getClassifierElististSelection(validRules);
			statsGame.addToRules(currentChar,rule);
			Debug.Log("Hard AI action : " + rule.action);
			
		}
		
		// Convert the action from the classifier to an action that can be executed in game
		switch (rule.action){
			case Classifier1.Action.ApproachEnemy : sequence.Add(AIUtil.AIHard.doApproachEnemy(charID)); break;
			case Classifier1.Action.ApproachAlly   : sequence.Add(AIUtil.AIHard.doApproachAlly(charID)) ; break;
			case Classifier1.Action.Flee           : sequence.Add(AIUtil.AIHard.doFlee(charID)); break;
			case Classifier1.Action.Attack         : sequence.Add(AIUtil.AIHard.doAttack(charID,0)); break;
			case Classifier1.Action.Skill          : sequence.Add(AIUtil.AIHard.doSkill(charID,0)); break;
			case Classifier1.Action.Spell		   : sequence.Add(AIUtil.AIHard.doSpell(charID, 0)); break;
			default : sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP,null)); break;
		}
		
		return sequence;
	}


}

}
