using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Misc;
using Characters;
using Classifiers;
using Hexas;

namespace Stats
{

	public class StatsGame
	{
		public List<StatsTurn> statsTurn;
		public int winner;
		public List<Character> survivors; // at the end of the game (to evaluate how close the game was)
		int kill = 0;
		int death = 0;
		int dmgdealt = 0;
		int received = 0;

		public StatsGame()
		{
			statsTurn = new List<StatsTurn>();
			winner = -1;
		}

		public void nextTurn(Character currentCharTurn)
		{
			statsTurn.Add(new StatsTurn(currentCharTurn));
		}

		public void endGame(int winner, HexaGrid hexagrid)
		{
			this.winner = winner;
			survivors = new List<Character>();
			foreach (Character c in hexagrid.charList)
			{
				survivors.Add(c);
			}
		}

		public StatsTurn getStatsTurn(Character c)
		{
			for (int i = statsTurn.Count - 1; i >= 0; i--) if (statsTurn[i].character == c) return statsTurn[i];
			return null;
		}

		public void addToDamageTaken(Character c, int n)
		{
			StatsTurn st = getStatsTurn(c);
			if (st != null)
			{
				st.damageTaken += n;
				st.HP -= n;
				received += n;
				if (st.HP < 0)
					st.HP = 0;
				st.HPBefore = 0;
			}
		}

		public void addToDamageDealt(Character c, int n)
		{
			StatsTurn st = getStatsTurn(c);
			if (st != null)
			{
				st.damageDealt += n;
				dmgdealt += n;
			}
		}

		public void addToKills(Character c, int n)
		{
			StatsTurn st = getStatsTurn(c);
			if (st != null)
			{
				kill += n;
				st.kills += n;
			}
		}

		public void setDead(Character c, bool dead)
		{
			StatsTurn st = getStatsTurn(c);
			if (st != null)
			{
				death += 1;
				st.dead = dead;
			}
		}

		public void addToRules(Character c, Classifier classifier)
		{
			StatsTurn st = getStatsTurn(c);
			if (st != null) st.rulesUsed.Add(classifier);
		}

		public string disp()
		{
			string str = "";
			foreach (StatsTurn st in statsTurn)
			{
				str += st.disp() + "\n";
			}
			return str;
		}

		public float calculatereward(float damageDealtValue, float damageTakenValue, float killsValue, float deathValue)
		{
			//foreach (StatsTurn st in statsTurn) st.evaluateTurn();
			//for (int i=0;i<statsTurn.Count;i++) evaluateTurn(i,1);

			// The reward/punishment for winning/losing the game is increased for every character that has survived and how much HP they have left.
			//foreach (Character c in survivors) if (c.team == winner) reward += 0.02f + (((float)c.HP / (float)c.HPmax) * 0.03f);
			float reward = -0.005f + dmgdealt * damageDealtValue - received * damageTakenValue + kill * killsValue - death * deathValue;
			Debug.Log("Dealt : " + dmgdealt + "  Received : " + received + "   Kill : " + kill + "   Death : " + death + "   Reward :" + reward + "\n");
			return reward;
		}
		public void evaluateGame()
		{

			float reward = calculatereward(0.000011f, 0.00001f, 0.005f, 0.006f);

			foreach (StatsTurn st in statsTurn)
			{
				foreach (Classifier c in st.rulesUsed)
				{
					//Debug.Log("Fitness actuelle : "+c.fitness+" Reward : "+reward+"\n");
					c.addToFitness(reward);
					//Debug.Log("Fitness apres le add : " + c.fitness + "\n");
					c.useCount++;
					c.lastUse = 0;
				}
			}
		}

		/** returns the score of the game. */
		public float evaluateGame(float expectedScore)
		{
			//foreach (StatsTurn st in statsTurn) st.evaluateTurn();
			//for (int i=0;i<statsTurn.Count;i++) evaluateTurn(i,1);

			// The reward/punishment for winning/losing the game is increased for every character that has survived and how much HP they have left.
			float reward = calculatereward(0.000011f, 0.00001f, 0.005f, 0.006f);
			//foreach (Character c in survivors) if (c.team == winner) reward += 0.02f + (((float)c.HP / (float)c.HPmax) * 0.03f);
			//reward *= 5.0f;

			foreach (StatsTurn st in statsTurn)
			{
				foreach (Classifier c in st.rulesUsed)
				{
					c.addToFitness((st.character.team == winner) ? reward - expectedScore : (-reward - expectedScore));
					//Debug.Log(calculatereward(0.02f, 0.01f, 0.1f, 0.1f));
					c.useCount++;
					c.lastUse = 0;
				}
			}

			return reward;
		}

		/** Evaluates every turn with the outcome of the following turns*/
		public void evaluateTurn(int turnID, int nbTurns)
		{
			float goodness = 0;
			int nbTurnsReal = 0;
			for (int i = turnID; i < statsTurn.Count && nbTurns > nbTurnsReal; i++)
			{
				StatsTurn st = statsTurn[i];
				goodness += st.calculate(1.0f, 1.0f, 10.0f, 10.0f);
				nbTurnsReal++;
			}
			float g = goodness / (float)(nbTurnsReal);
			//if (g < 0) g /= 2.0f; // remove later

			if (g != 0.0f)
			{
				foreach (Classifier c in statsTurn[turnID].rulesUsed)
				{
					c.addToFitness(g);
					Debug.Log("Numero 3\n");
				}
			}
		}
	}

	public class StatsTurn
	{
		public Character character;
		public int HP;
		public int HPBefore;
		public int damageDealt;
		public int damageTaken; // Damage taken AFTER their turn (between the end of their turn and their next turn)
		public int kills;
		public bool dead;
		public List<Classifier> rulesUsed;

		public StatsTurn(Character c)
		{
			character = c;
			HP = c.HP;
			HPBefore = c.HP;
			damageDealt = 0;
			damageTaken = 0;
			kills = 0;
			dead = false;
			rulesUsed = new List<Classifier>();
		}

		public string disp()
		{
			return character.charClass + " " + character.team + " : " + HP + " HP, " + damageDealt + " dealt, " + damageTaken + " taken, " + kills + " kills, dead : " + dead + " nb Rules : " + rulesUsed.Count;
		}

		public float calculate(float damageDealtValue, float damageTakenValue, float killsValue, float deathValue)
		{
			return damageDealt * damageDealtValue - damageTaken * damageTakenValue + kills * killsValue - ((dead) ? deathValue : 0);
		}

		public void evaluateTurn()
		{
			float goodness = this.calculate(1.0f, 1.0f, 10.0f, 10.0f);

			if (goodness != 0)
			{
				foreach (Classifier c in rulesUsed)
				{
					c.addToFitness(goodness);
					Debug.Log("Numero 4\n");
					//c.modified = true;
				}
			}
		}
	}

}
