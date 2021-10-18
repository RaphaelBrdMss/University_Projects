using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Classifiers;
using Hexas;
using Characters;
using AI_Util;
using Misc;

namespace Classifiers2 {

public class Classifier2Attributes {
	public enum CharClass : byte {GUERRIER = 1,VOLEUR = 2,ARCHER = 4,MAGE = 8,SOIGNEUR = 16,ENVOUTEUR = 32};
	public enum Threat : byte {HEALED = 1,SAFE = 2,LOW = 4,HIGH = 8,DEATH = 16};
	public enum SkillAvailable : byte {YES = 1,NO = 2};
	
	public static byte getRandomCharClass(){
		return (byte)(1<<Random.Range(0,6));
	}
	public static byte getRandomThreat(){
		return (byte)(1<<Random.Range(0,5));
	}
	public static byte getRandomSkillAvailable(){
		return (byte)(1<<Random.Range(0,1));
	}
	public static Threat intToThreat(int threat){
		if (threat == -1000) return Threat.DEATH;
		else if (threat >=  1) return Threat.HEALED;
		else if (threat >= -2) return Threat.SAFE;
		else if (threat >= -5) return Threat.LOW;
		else return Threat.HIGH;
	}
	public static CharClass charClassToCharClass(Characters.CharClass charClass){
		return (CharClass)(1<<((int)charClass));
	}
}

public class Classifier2 : Classifier{
	public byte charClass;
	
	public byte threatAttack;
	public byte threatSkill;
	public byte threatFlee;
	
	public byte threatEnemyAttack;
	public byte threatEnemySkill;
	public byte threatEnemyFlee;
	
	public byte skillAvailable;
	
	public enum Action : byte {Attack,Skill,Flee};
	public Action action;

	public Classifier2(HexaGrid hexaGrid,int charID){
		Character c = hexaGrid.charList[charID];
		this.charClass = (byte)Classifier2Attributes.charClassToCharClass(c.charClass);
		
		this.threatAttack = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatIfAttack(charID));
		this.threatSkill = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatIfSkill(charID));
		this.threatFlee = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatIfFlee(charID));
		
		this.threatEnemyAttack = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatEnemyIfAttack(charID));
		this.threatEnemySkill = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatEnemyIfSkill(charID));
		this.threatEnemyFlee = (byte)Classifier2Attributes.intToThreat(AIUtil.AIHard2.threatEnemyIfFlee(charID));
		
		this.skillAvailable = (byte)((c.skillAvailable) ? 1 : 2);
		
		action = Action.Attack;
		
		fitness = 0.5f;
		useCount = 0;
		lastUse = 0;
	}
	
	public Classifier2(Classifier2 c2){
		this.charClass = c2.charClass;
		
		this.threatAttack = c2.threatAttack;
		this.threatSkill = c2.threatSkill;
		this.threatFlee = c2.threatFlee;
		
		this.threatEnemyAttack = c2.threatEnemyAttack;
		this.threatEnemySkill = c2.threatEnemySkill;
		this.threatEnemyFlee = c2.threatEnemyFlee;
		
		this.skillAvailable = c2.skillAvailable;
		
		this.action = c2.action;
		this.fitness = c2.fitness;
		this.useCount = c2.useCount;
		this.lastUse = c2.lastUse;
	}
	
	public Classifier2(Classifier2 c1,Classifier2 c2){
		this.charClass = (byte)(c1.charClass | c2.charClass);
		
		this.threatAttack = (byte)(c1.threatAttack | c2.threatAttack);
		this.threatSkill = (byte)(c1.threatSkill | c2.threatSkill);
		this.threatFlee = (byte)(c1.threatFlee | c2.threatFlee);
		
		this.threatEnemyAttack = (byte)(c1.threatEnemyAttack | c2.threatEnemyAttack);
		this.threatEnemySkill = (byte)(c1.threatEnemySkill | c2.threatEnemySkill);
		this.threatEnemyFlee = (byte)(c1.threatEnemyFlee | c2.threatEnemyFlee);
		
		this.skillAvailable = (byte)(c1.skillAvailable | c2.skillAvailable);
		
		this.action = c1.action;
		this.fitness = 0.5f;
		this.useCount = 0;
		this.lastUse = 0;
	}
	
	public bool isValid(){
		if ((this.action == Action.Skill) && ((skillAvailable & (byte)Classifier2Attributes.SkillAvailable.NO) > 0)) return false;
		return true;
	}
	
	public static Action getRandomAction(){
		return (Action)Random.Range(0,3);
	}
	
	override public string getStringInfo(){
		string strDisp = "Class : ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.GUERRIER) > 0) strDisp += "GUERRIER ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.VOLEUR) > 0) strDisp += "VOLEUR ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.ARCHER) > 0) strDisp += "ARCHER ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.MAGE) > 0) strDisp += "MAGE ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.SOIGNEUR) > 0) strDisp += "SOIGNEUR ";
		if ((charClass & (byte)Classifier2Attributes.CharClass.ENVOUTEUR) > 0) strDisp += "ENVOUTEUR ";
		
		strDisp += "| attack : ";
		if ((threatAttack & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatAttack & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatAttack & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatAttack & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatAttack & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "| skill : ";
		if ((threatSkill & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatSkill & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatSkill & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatSkill & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatSkill & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "| flee : ";
		if ((threatFlee & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatFlee & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatFlee & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatFlee & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatFlee & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "\nEnemy : attack : ";
		if ((threatEnemyAttack & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatEnemyAttack & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatEnemyAttack & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatEnemyAttack & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatEnemyAttack & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "| skill : ";
		if ((threatEnemySkill & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatEnemySkill & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatEnemySkill & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatEnemySkill & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatEnemySkill & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "| flee : ";
		if ((threatEnemyFlee & (byte)Classifier2Attributes.Threat.HEALED) > 0) strDisp += "HEALED ";
		if ((threatEnemyFlee & (byte)Classifier2Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
		if ((threatEnemyFlee & (byte)Classifier2Attributes.Threat.LOW) > 0) strDisp += "LOW ";
		if ((threatEnemyFlee & (byte)Classifier2Attributes.Threat.HIGH) > 0) strDisp += "HIGH ";
		if ((threatEnemyFlee & (byte)Classifier2Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";
		
		strDisp += "\nSkill : ";
		if ((skillAvailable & (byte)Classifier2Attributes.SkillAvailable.YES) > 0) strDisp += "YES ";
		if ((skillAvailable & (byte)Classifier2Attributes.SkillAvailable.NO) > 0) strDisp += "NO ";
		
		strDisp += "\nAction : " + action + " | Fitness : " + fitness + " | Use count : " + useCount;
		
		return strDisp;
	}
	
	override public int distance(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		int dist = 0; 
		dist += Misc.MiscBinary.nbDifferentBits(this.charClass,c.charClass);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatAttack,c.threatAttack);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatSkill,c.threatSkill);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatFlee,c.threatFlee);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatEnemyAttack,c.threatEnemyAttack);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatEnemySkill,c.threatEnemySkill);
		dist += Misc.MiscBinary.nbDifferentBits(this.threatEnemyFlee,c.threatEnemyFlee);
		dist += Misc.MiscBinary.nbDifferentBits(this.skillAvailable,c.skillAvailable);
		return dist;
	}
	
	override public bool isSimilar(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return (((this.charClass & c.charClass) > 0) &&
		((this.threatAttack & c.threatAttack) > 0) &&
		((this.threatSkill & c.threatSkill) > 0) &&
		((this.threatFlee & c.threatFlee) > 0) &&
		((this.threatEnemyAttack & c.threatEnemyAttack) > 0) &&
		((this.threatEnemySkill & c.threatEnemySkill) > 0) &&
		((this.threatEnemyFlee & c.threatEnemyFlee) > 0) &&
		((this.skillAvailable & c.skillAvailable) > 0) &&
		(this.action == c.action));
	}
	
	override public bool equals(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return ((this.charClass == c.charClass) &&
		(this.threatAttack == c.threatAttack) &&
		(this.threatSkill == c.threatSkill) &&
		(this.threatFlee == c.threatFlee) &&
		(this.threatEnemyAttack == c.threatEnemyAttack) &&
		(this.threatEnemySkill == c.threatEnemySkill) &&
		(this.threatEnemyFlee == c.threatEnemyFlee) &&
		(this.skillAvailable == c.skillAvailable) &&
		(this.action == c.action));
	}
	
	override public bool equals2(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return ((this.charClass == c.charClass) &&
		(this.threatAttack == c.threatAttack) &&
		(this.threatSkill == c.threatSkill) &&
		(this.threatFlee == c.threatFlee) &&
		(this.threatEnemyAttack == c.threatEnemyAttack) &&
		(this.threatEnemySkill == c.threatEnemySkill) &&
		(this.threatEnemyFlee == c.threatEnemyFlee) &&
		(this.skillAvailable == c.skillAvailable));
	}
	
	override public bool isSimilar2(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return (((this.charClass & c.charClass) > 0) &&
		((this.threatAttack & c.threatAttack) > 0) &&
		((this.threatSkill & c.threatSkill) > 0) &&
		((this.threatFlee & c.threatFlee) > 0) &&
		((this.threatEnemyAttack & c.threatEnemyAttack) > 0) &&
		((this.threatEnemySkill & c.threatEnemySkill) > 0) &&
		((this.threatEnemyFlee & c.threatEnemyFlee) > 0) &&
		((this.skillAvailable & c.skillAvailable) > 0));
	}
	
	override public bool sameAction(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return (this.action == c.action);
	}
	
	override public Classifier copy(){
		return new Classifier2(this);
	}
	
	override public Classifier merge(Classifier c2){
		Classifier2 c = (Classifier2)c2;
		return new Classifier2(this,c);
	}
	
	override public Classifier mutate(float likelihood,int maxAttempts){
		for (int i=0;i<maxAttempts;i++){
			Classifier2 c = new Classifier2(this);
			
			if (fitness > 0.5f){
				if (Random.Range(0.0f,1.0f) <= likelihood) c.charClass |= Classifier2Attributes.getRandomCharClass();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatAttack |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatSkill |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatFlee |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatEnemyAttack |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatEnemySkill |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.threatEnemyFlee |= Classifier2Attributes.getRandomThreat();
				if (Random.Range(0.0f,1.0f) <= likelihood) c.skillAvailable |= Classifier2Attributes.getRandomSkillAvailable();
			}else{
				c.action = getRandomAction();
			}
			
			if (c.isValid()){
				c.fitness = 0.5f;
				c.useCount = 0;
				c.lastUse = 0;
				return c;
			}
		}
		return null;
	}
	
	override public void saveInBinary(BinaryWriter writer){
		writer.Write(charClass);
		
		writer.Write(threatAttack);
		writer.Write(threatSkill);
		writer.Write(threatFlee);
		
		writer.Write(threatEnemyAttack);
		writer.Write(threatEnemySkill);
		writer.Write(threatEnemyFlee);
		
		writer.Write(skillAvailable);
		
		writer.Write((byte)this.action);
		writer.Write(this.fitness);
		writer.Write(this.useCount);
		writer.Write(this.lastUse);
	}
		public override void saveInDB() { }
		override public void loadInBinary(BinaryReader reader){
		this.charClass = reader.ReadByte();
		
		this.threatAttack = reader.ReadByte();
		this.threatSkill = reader.ReadByte();
		this.threatFlee = reader.ReadByte();
		
		this.threatEnemyAttack = reader.ReadByte();
		this.threatEnemySkill = reader.ReadByte();
		this.threatEnemyFlee = reader.ReadByte();
		
		this.skillAvailable = reader.ReadByte();
		
		this.action = (Action)reader.ReadByte();
		this.fitness = reader.ReadSingle();
		this.useCount = reader.ReadInt32();
		this.lastUse = reader.ReadInt32();
	}
}

}
