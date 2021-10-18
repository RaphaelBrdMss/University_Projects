using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using Misc;
using Hexas;
using Characters;
using AI_Class;
using static MainGame;

namespace AI_Util
{

    public class HexaDamage
    {
        public enum AttackEffect : byte { DAMAGE, HEAL, PA_BUFF };
        public int x;
        public int y;
        public int value;

        public HexaDamage(int x, int y, int value)
        {
            this.x = x;
            this.y = y;
            this.value = value;
        }
    }

    public class AIUtil
    {
        public static HexaGrid hexaGrid;
        public enum AttackEffect : byte { DAMAGE, HEAL, PA_BUFF };

        /** Calculates how much damage can potentially be taken on each hexa.
            Returns a list of w*h that contains the data. */
        public static int[] calculateThreat(int team)
        {
            int w_h = hexaGrid.w * hexaGrid.h;
            int[] finalList = new int[w_h];
            int[] list = new int[w_h];
            for (int i = 0; i < w_h; i++)
            {
                finalList[i] = 0;
                list[i] = 0;
            }

            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != team)
                {


                    int dmg1PA = 0, dmg2PA = 0, dmg3PA = 0, dmg4PA = 0;
                    if (c.getClassData().basicAttack.attackEffect.Equals(CharsDB.AttackEffect.DAMAGE) && c.getClassData().spell.attackEffect.Equals(CharsDB.AttackEffect.DAMAGE) && c.getClassData().ult.attackEffect.Equals(CharsDB.AttackEffect.DAMAGE))
                    {

                        dmg1PA = c.getClassData().basicAttack.effectValue;

                        //cas ou le personnage a 2PA
                        if (c.getClassData().basicAttack.effectValue * 2 < c.getClassData().spell.effectValue)
                        {
                            dmg2PA = c.getClassData().spell.effectValue;
                        }
                        else dmg2PA = c.getClassData().basicAttack.effectValue * 2;

                        //cas ou le personnage a 3PA
                        if (c.getClassData().basicAttack.effectValue * 3 < c.getClassData().basicAttack.effectValue + c.getClassData().spell.effectValue)
                        {
                            dmg3PA = c.getClassData().basicAttack.effectValue + c.getClassData().spell.effectValue;
                        }
                        else dmg3PA = c.getClassData().basicAttack.effectValue * 3;

                        //cas ou le personnage a 4PA a moins d'une modification sur les personnages ce cas la est appliqué que au voleur
                        dmg4PA = c.getClassData().basicAttack.effectValue * 4;

                        //cas si le personnage a son ultime
                        if (c.skillAvailable == true)
                        {

                            dmg1PA = c.getClassData().ult.effectValue;
                            dmg2PA = dmg1PA + c.getClassData().basicAttack.effectValue;
                            dmg3PA = dmg2PA + c.getClassData().basicAttack.effectValue;
                            dmg4PA = dmg3PA + c.getClassData().basicAttack.effectValue;
                        }
                    }
                    else
                    {

                        dmg2PA = c.getClassData().spell.effectValue;
                        dmg3PA = dmg2PA;
                        dmg4PA = dmg2PA * 2;

                    }

                    List<Point> listHexasInSight = hexaGrid.findHexasInSight2(c.x, c.y, c.getClassData().basicAttack.range);
                    int damage = 0;
                    int currentPa = c.PA;
                    for (int i = 0; i < c.PA; i++)
                    {
                        List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                        foreach (Point charpos in listH)
                        {

                            //cas 1PA
                            if (currentPa == 1)
                            {
                                listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().basicAttack.range);
                                if (c.skillAvailable == true) listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().ult.range);
                                damage = dmg1PA;
                            }
                            //cas 2PA
                            else if (currentPa == 2)
                            {
                                if (dmg2PA == c.getClassData().spell.effectValue) listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().spell.range);
                                else if (c.skillAvailable) listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().ult.range);
                                else listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().basicAttack.range);
                                damage = dmg2PA;

                            }
                            //cas 3PA
                            else if (currentPa == 3)
                            {
                                if (dmg3PA == c.getClassData().basicAttack.effectValue + c.getClassData().spell.effectValue) listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().spell.range);
                                else if (c.skillAvailable) listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().ult.range);
                                else listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().basicAttack.range);
                                damage = dmg3PA;

                            }
                            //cas 4PA (uniquement le voleur)
                            else if (currentPa == 4)
                            {
                                listHexasInSight = hexaGrid.findHexasInSight2(charpos.x, charpos.y, c.getClassData().basicAttack.range);
                                damage = dmg4PA;
                            }
                            foreach (Point p in listHexasInSight)
                            {
                                int pos = p.x + p.y * hexaGrid.w;
                                if (list[pos] < damage) list[pos] = damage;

                            }
                        }
                        currentPa--;
                    }

                    // Add to the list

                    for (int i = 0; i < w_h; i++)
                    {
                        finalList[i] += list[i];
                        list[i] = 0;

                    }
                }
            }
            return finalList;
        }


        /** Calculates how much damage can potentially be dealt on each hexa.
            Returns a list of w*h that contains the data. */
        public static int[] calculateDamage(int charID)
        {
            Character currentChar = hexaGrid.charList[charID];
            int w_h = hexaGrid.w * hexaGrid.h;
            int[] list = new int[w_h];
            for (int i = 0; i < w_h; i++)
            {
                list[i] = 0;
            }



            int damage = currentChar.getClassData().basicAttack.effectValue;
            foreach (Character c in hexaGrid.charList)
            {

                if (currentChar.team != c.team)
                {
                    int currentPa = currentChar.PA;

                    for (int i = 0; i < currentChar.PA; i++)
                    {
                        List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * i);


                        foreach (Point charpos in listH)
                        {

                            bool isInUltSight = hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().ult.range);
                            bool isInBasicSight = hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().basicAttack.range);
                            bool isInSpellSight = hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().spell.range);
                            //cas 1PA
                            if (currentPa == 1)
                            {
                                if (isInUltSight)
                                {
                                    damage = currentChar.getClassData().ult.effectValue;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInBasicSight)
                                {
                                    damage = currentChar.getClassData().basicAttack.effectValue;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }

                            }
                            //cas 2PA
                            else if (currentPa == 2)
                            {
                                if (isInUltSight)
                                {
                                    damage = currentChar.getClassData().ult.effectValue + currentChar.getClassData().basicAttack.effectValue;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInBasicSight)
                                {
                                    damage = currentChar.getClassData().basicAttack.effectValue * 2;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInSpellSight)
                                {
                                    damage = currentChar.getClassData().spell.effectValue;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }

                            }
                            //cas 3PA
                            else if (currentPa == 3)
                            {
                                if (isInUltSight)
                                {
                                    damage = currentChar.getClassData().ult.effectValue + (currentChar.getClassData().basicAttack.effectValue * 2);
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInBasicSight)
                                {
                                    damage = currentChar.getClassData().basicAttack.effectValue * 3;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInSpellSight)
                                {
                                    damage = currentChar.getClassData().spell.effectValue + currentChar.getClassData().basicAttack.effectValue;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }

                            }
                            //cas 4PA (uniquement le voleur)
                            else if (currentPa == 4)
                            {
                                if (isInUltSight)
                                {
                                    damage = currentChar.getClassData().ult.effectValue + (currentChar.getClassData().basicAttack.effectValue * 3);
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                                else if (isInBasicSight)
                                {
                                    damage = currentChar.getClassData().basicAttack.effectValue * 4;
                                    if (c.HP <= damage) damage += 100;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < damage) list[pos] = damage;
                                }
                            }

                        }
                        currentPa--;

                    }
                }
            }


            return list;
        }

        /** Calculates how much healing can potentially be done on each hexa
            assuming the character is a healer.
            Returns a list of w*h that contains the data.*/
        public static int[] calculateHealing(int charID)
        {
            Character currentChar = hexaGrid.charList[charID];
            int w_h = hexaGrid.w * hexaGrid.h;
            int[] list = new int[w_h];
            for (int i = 0; i < w_h; i++)
            {
                list[i] = 0;
            }

            int currentPA = currentChar.PA;
            for (int i = 0; i < currentChar.PA; i++)
            {
                List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * i);
                foreach (Point charpos in listH)
                {
                    foreach (Character c in hexaGrid.charList)
                    {
                        bool isInBasicSight = hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().basicAttack.range);
                        bool isInUltSight = hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().ult.range);

                        if (c.team == currentChar.team)
                        {
                            if (currentPA == 3)
                            {
                                if (isInUltSight && currentChar.skillAvailable && c.HPmax < (c.HP * 30) / 100)
                                {
                                    int value = currentChar.getClassData().basicAttack.effectValue * 2 + currentChar.getClassData().ult.effectValue;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;
                                }
                                else if (isInBasicSight)
                                {
                                    int value = currentChar.getClassData().basicAttack.effectValue * 3;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;
                                }
                            }

                            else if (currentPA == 2)
                            {
                                if (isInUltSight && currentChar.skillAvailable && c.HP < (c.HPmax * 30) / 100)
                                {

                                    int value = currentChar.getClassData().basicAttack.effectValue + currentChar.getClassData().ult.effectValue;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;


                                }
                                else if (isInBasicSight)
                                {

                                    int value = currentChar.getClassData().basicAttack.effectValue * 2;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;

                                }
                            }
                            else if (currentPA == 1)
                            {

                                if (isInUltSight && currentChar.skillAvailable && c.HPmax < (c.HP * 30) / 100)
                                {

                                    int value = currentChar.getClassData().ult.effectValue;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;

                                }
                                else if (isInBasicSight)
                                {

                                    int value = currentChar.getClassData().basicAttack.effectValue;
                                    if (c.HP + value > c.HPmax) value = c.HPmax - c.HP;
                                    int pos = charpos.x + charpos.y * hexaGrid.w;
                                    if (list[pos] < value) list[pos] = value;

                                }
                            }


                        }
                    }
                }
                currentPA--;
            }
           
            return list;
        }

        /** Calculates how many PAs can potentially be given on each hexa
            assuming the character is a envouteur.
            Returns a list of w*h that contains the data. moi */
        public static int[] calculateBuff(int charID)
        {
            Character currentChar = hexaGrid.charList[charID];
            int w_h = hexaGrid.w * hexaGrid.h;
            int[] list = new int[w_h];
            for (int i = 0; i < w_h; i++)
            {
                list[i] = 0;
            }

            int healing = currentChar.getClassData().basicAttack.effectValue;
            // 0 PM
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == currentChar.team)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                    {
                        int value;
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER: value = 3; break;
                            case CharClass.VOLEUR: value = 6; break;
                            case CharClass.ARCHER: value = 4; break;
                            case CharClass.MAGE: value = 5; break;
                            case CharClass.SOIGNEUR: value = 2; break;
                            case CharClass.ENVOUTEUR: value = 1; break;
                            default: value = 0; break;
                        }
                        int pos = currentChar.x + currentChar.y * hexaGrid.w;
                        if (list[pos] < value) list[pos] = value;
                    }
                }
            }
            // 1+ PM
            for (int i = 1; i < currentChar.PA; i++)
            {
                List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * i);
                foreach (Point charpos in listH)
                {
                    foreach (Character c in hexaGrid.charList)
                    {
                        if (c.team == currentChar.team)
                        {
                            if (hexaGrid.hexaInSight(charpos.x, charpos.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                            {
                                int value;
                                switch (c.charClass)
                                {
                                    case CharClass.GUERRIER: value = 3; break;
                                    case CharClass.VOLEUR: value = 6; break;
                                    case CharClass.ARCHER: value = 4; break;
                                    case CharClass.MAGE: value = 5; break;
                                    case CharClass.SOIGNEUR: value = 2; break;
                                    case CharClass.ENVOUTEUR: value = 1; break;
                                    default: value = 0; break;
                                }
                                int pos = charpos.x + charpos.y * hexaGrid.w;
                                if (list[pos] < value) list[pos] = value;
                            }
                        }
                    }
                }
            }

            return list;
        }

        /** Returns the position of the hexas where the value is at its maximum in the list of possible hexas.
            Use calculateDamage,... to get v.*/
        public static List<Point> findHexasWhereValueIsMax(List<Point> possibleHexas, int[] v)
        {
            // find best value
            int maxValue = v[possibleHexas[0].x + possibleHexas[0].y * hexaGrid.w];
            foreach (Point p in possibleHexas)
            {
                int vmax = v[p.x + p.y * hexaGrid.w];
                if (vmax > maxValue) maxValue = vmax;
            }
            // find all hexas with best value
            List<Point> bestHexas = new List<Point>();
            foreach (Point p in possibleHexas)
            {
                int vmax = v[p.x + p.y * hexaGrid.w];
                if (vmax == maxValue) bestHexas.Add(p);
            }
            return bestHexas;
        }

        /** Returns the position of the hexas where the least amount of potential damage will be taken in the list of possible hexas. */
        public static List<Point> findSafestHexas(List<Point> possibleHexas, int[] v)
        {
            int minValue = v[possibleHexas[0].x + possibleHexas[0].y * hexaGrid.w];
            foreach (Point p in possibleHexas)
            {
                int vmin = v[p.x + p.y * hexaGrid.w];
                if(vmin<minValue) minValue=vmin;
            }

            List<Point> bestHexas = new List<Point>();
            foreach (Point p in possibleHexas)
            {
                int vmax = v[p.x + p.y * hexaGrid.w];
                if (vmax == minValue) bestHexas.Add(p);
            }
            return bestHexas;

        }

        /** Returns the position of the hexas where the character will be closest to the lowest Enemy in the list of possible hexas. */
        public static List<Point> findHexasClosestToLowestEnemy(int charID, List<Point> possibleHexas)
        {
            Character currentChar = hexaGrid.charList[charID];
            int minDistance = 100000;
            Character cLowest = AIUtil.findLowestEnemy(currentChar.team);
            // find best value
            List<int> possibleHexasValues = new List<int>();
            foreach (Point p in possibleHexas)
            {
                int d = hexaGrid.getWalkingDistance(p.x, p.y, cLowest.x, cLowest.y);
                possibleHexasValues.Add(d);
                if (d != -1) if (d < minDistance) minDistance = d;
            }
            // find all hexas with best value
            List<Point> bestHexas = new List<Point>();
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == minDistance) bestHexas.Add(possibleHexas[i]);
            return bestHexas;
        }

        /** Returns the position of the hexas where the character will be closest to allies in the list of possible hexas. */
        
        public static List<Point> findHexasClosestToAllies(int charID, List<Point> possibleHexas)
        {
            Character currentChar = hexaGrid.charList[charID];
            Point charPos = new Point(currentChar.x, currentChar.y);
            List<Point> bestHexas = new List<Point>();
            int closest = 1000000;
                List<int> possibleHexasValues = new List<int>();
                foreach (Point p in possibleHexas)
                {
                    currentChar.updatePos2(p.x, p.y, hexaGrid);
                    int distance = 0;
                    foreach (Character c in hexaGrid.charList)
                    {
                        if (c.team == currentChar.team && c != currentChar) distance += hexaGrid.getWalkingDistance(p.x, p.y, c.x, c.y);
                    }
                    possibleHexasValues.Add(distance);
                    if (distance < closest) closest = distance;
                }
                for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == closest) bestHexas.Add(possibleHexas[i]);
                currentChar.updatePos2(charPos.x, charPos.y, hexaGrid);
                return bestHexas;
            
        }


        public static List<ActionAIPos> findSequencePathToHexa(int charID, int x, int y)
        {
            Character currentChar = hexaGrid.charList[charID];
            List<ActionAIPos> sequence = new List<ActionAIPos>();
            int nbPA = currentChar.PA;
            if (x == currentChar.x && y == currentChar.y)
            {

            }
            else
            {
                int d = hexaGrid.getWalkingDistance(currentChar.x, currentChar.y, x, y);
                List<Point> shortestPath = hexaGrid.findShortestPath(currentChar.x, currentChar.y, x, y, d);
                for (int i = 0; i < d && nbPA > 0; i += currentChar.PM)
                {
                    Point destination = shortestPath[((i + currentChar.PM) <= d) ? (i + currentChar.PM) : d];
                    sequence.Add(new ActionAIPos(MainGame.ActionType.MOVE, new Point(destination.x, destination.y)));
                    nbPA--;
                }
            }
            return sequence;
        }

        /** Returns the Enemy with the lowest amount of HP. */
        public static Character findLowestEnemy(int myTeam)
        {
            int lowest = 100000;
            Character cLowest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != myTeam && c.HP < lowest)
                {
                    lowest = c.HP;
                    cLowest = c;
                }
            }
            return cLowest;
        }

        /** Returns the ID of the Enemy that either will be killed or be lowest after
            being attacked from the current char pos. */
        public static Character findCharToAttack(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            int lowest = 1000000;
            Character cLowest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != currentChar.team)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                    {
                        if (c.HP < lowest)
                        {
                            lowest = c.HP;
                            cLowest = c;
                        }
                    }
                }
            }
            return cLowest;
        }

        /** Returns the ID of the Enemy that either will be killed or be lowest after
            being attacked with skill from the current char pos. */
        public static Character findCharToAttackSkill(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            int lowest = 100000;
            Character cLowest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != currentChar.team)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().ult.range))
                    {
                        if (c.HP < lowest)
                        {
                            lowest = c.HP;
                            cLowest = c;
                        }
                    }
                }
            }
            return cLowest;
        }

        /** Returns the position of the hexa that either will allow the mage to hit
            the highest amount of Enemies from the current pos. */
        public static Point findWhereToAttackMage(int myCharID)
        {
            int maxTargets = 0;
            Character currentChar = hexaGrid.charList[myCharID];
            List<int> possibleHexasValues = new List<int>();
            List<Point> possibleHexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().basicAttack.range);
            foreach (Point p in possibleHexas)
            {
                List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().basicAttack.rangeAoE, currentChar.getClassData().basicAttack.aoEType);
                int nb = 0;
                // filter allies
                foreach (Character c in lc)
                {
                    if (c.team != currentChar.team) nb++;
                }
                possibleHexasValues.Add(nb);
                if (nb > maxTargets) maxTargets = nb;
            }
            List<Point> bestHexas = new List<Point>();
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == maxTargets) bestHexas.Add(possibleHexas[i]);

            if (bestHexas.Count > 0)
            {
                return bestHexas[0]; // Improve by finding the best one to return
            }
            else
            {
                return null;
            }
        }

        /** Returns the position of the hexa that either will allow the mage to hit
            the highest amount of Enemies from the current pos. */
        public static Point findWhereToAttackMageSkill(int myCharID)
        {
            int maxTargets = 0;
            Character currentChar = hexaGrid.charList[myCharID];
            List<int> possibleHexasValues = new List<int>();
            List<Point> possibleHexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().ult.range);
            foreach (Point p in possibleHexas)
            {
                List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().ult.rangeAoE, currentChar.getClassData().ult.aoEType);
                int nb = 0;
                // filter allies
                foreach (Character c in lc)
                {
                    if (c.team != currentChar.team) nb++;
                }
                possibleHexasValues.Add(nb);
                if (nb > maxTargets) maxTargets = nb;
            }
            List<Point> bestHexas = new List<Point>();
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == maxTargets) bestHexas.Add(possibleHexas[i]);

            if (bestHexas.Count > 0)
            {
                return bestHexas[0]; // Improve by finding the best one to return
            }
            else
            {
                return null;
            }
        }

        /** Returns the position of the hexa that either will allow the soigneur to heal
            the highest amount of allies from the current pos (aoe skill). */
        public static Point findWhereToHealSoigneurSkill(int myCharID)
        {
            int maxTargets = 0;
            Character currentChar = hexaGrid.charList[myCharID];
            List<int> possibleHexasValues = new List<int>();
            List<Point> possibleHexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().ult.range);
            foreach (Point p in possibleHexas)
            {
                List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().ult.rangeAoE, currentChar.getClassData().ult.aoEType);
                int nb = 0;
                // filter Enemies/self
                foreach (Character c in lc)
                {
                    if (c.team == currentChar.team && c != currentChar) nb++;
                }
                possibleHexasValues.Add(nb);
                if (nb > maxTargets) maxTargets = nb;
            }
            List<Point> bestHexas = new List<Point>();
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == maxTargets) bestHexas.Add(possibleHexas[i]);

            if (bestHexas.Count > 0)
            {
                return bestHexas[0]; // Improve by finding the best one to return
            }
            else
            {
                return null;
            }
        }

        /** Returns the ID of the ally that can be healed for the most
            from the current char pos assuming the character is a healer. */
        public static Character findCharToHeal(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            int highest = 0;
            Character cHighest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == currentChar.team && c != currentChar)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                    {
                        if (c.HPmax - c.HP > highest)
                        {
                            highest = c.HP;
                            cHighest = c;
                        }
                    }
                }
            }
            return cHighest;
        }

        /** Returns the ID of the ally that can be buffed for the most
            from the current char pos assuming the character is a envouteur. */
        public static Character findCharToBuff(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            int highest = 0;
            Character cHighest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == currentChar.team && c != currentChar)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                    {
                        int classPrio;
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER: classPrio = 3; break;
                            case CharClass.VOLEUR: classPrio = 6; break;
                            case CharClass.ARCHER: classPrio = 4; break;
                            case CharClass.MAGE: classPrio = 5; break;
                            case CharClass.SOIGNEUR: classPrio = 2; break;
                            case CharClass.ENVOUTEUR: classPrio = 1; break;
                            default: classPrio = 0; break;
                        }
                        if (classPrio > highest)
                        {
                            highest = classPrio;
                            cHighest = c;
                        }
                    }
                }
            }
            return cHighest;
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
        // Functions used for AI HARD
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------

        /** return the maximum amount of targets from the current character's position. */
        public static int getNbMaxTargets(int charID)
        {
            int maxTargets = 0;
            Character currentChar = hexaGrid.charList[charID];
            if (currentChar.skillAvailable)
            {
                List<Point> hexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().ult.range);
                foreach (Point p in hexas)
                {
                    List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().ult.rangeAoE, currentChar.getClassData().ult.aoEType);
                    int nb = 0;
                    // filter allies / Enemies
                    if (currentChar.getClassData().ult.targetsEnemies)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team != currentChar.team) nb++;
                        }
                    }
                    if (currentChar.getClassData().ult.targetsAllies)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team == currentChar.team && c != currentChar) nb++;
                        }
                    }
                    // Soigneur : filter allies with full hp
                    if (currentChar.charClass == CharClass.SOIGNEUR)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team == currentChar.team && c != currentChar && c.HP == c.HPmax) nb--;
                        }
                    }
                    if (nb > maxTargets) maxTargets = nb;
                }
            }
            else
            {
                List<Point> hexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().basicAttack.range);
                foreach (Point p in hexas)
                {
                    List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().basicAttack.rangeAoE, currentChar.getClassData().basicAttack.aoEType);
                    int nb = 0;
                    // filter allies / Enemies
                    if (currentChar.getClassData().basicAttack.targetsEnemies)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team != currentChar.team) nb++;
                        }
                    }
                    if (currentChar.getClassData().basicAttack.targetsAllies)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team == currentChar.team && c != currentChar) nb++;
                        }
                    }
                    // Soigneur : filter allies with full hp
                    if (currentChar.charClass == CharClass.SOIGNEUR)
                    {
                        foreach (Character c in lc)
                        {
                            if (c.team == currentChar.team && c != currentChar && c.HP == c.HPmax) nb--;
                        }
                    }
                    if (nb > maxTargets) maxTargets = nb;
                }
            }
            return maxTargets;
        }

        
        public static List<Point> findHexasClosestToArcher(int charID, List<Point> possibleHexas)
        {
            Character currentChar = hexaGrid.charList[charID];
            Point charPos = new Point(currentChar.x, currentChar.y);
            List<Point> bestHexas = new List<Point>();
            int closest = 100000;
            List<int> possibleHexasValues = new List<int>();
            foreach (Point p in possibleHexas)
            {
                currentChar.updatePos2(p.x, p.y, hexaGrid);
                int distance = 0;
                foreach (Character c in hexaGrid.charList)
                {
                    if (c.team != currentChar.team && c.charClass == CharClass.ARCHER) distance += hexaGrid.getWalkingDistance(p.x, p.y, c.x, c.y);
                }
                possibleHexasValues.Add(distance);
                if (distance < closest) closest = distance;
            }
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == closest) bestHexas.Add(possibleHexas[i]);
            currentChar.updatePos2(charPos.x, charPos.y, hexaGrid);
            return bestHexas;
        }
        public static Character attackArcher(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            Character archer = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != currentChar.team)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().basicAttack.range))
                    {
                        if (c.charClass == CharClass.ARCHER)
                        {
                            archer = c;
                        }

                    }
                }
            }
            return archer;
        }

        public class AIHard
        {
            

            public static ActionAIPos doApproachEnemy(int myCharID)
            {

                Character currentChar = hexaGrid.charList[myCharID];
                switch (currentChar.charClass)
                {
                    case CharClass.GUERRIER:
                        {
                            Character archer = AIUtil.attackArcher(currentChar.team);
                            
                            List<Point> listG = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                            if (listG != null && listG.Count > 0)
                            {
                                if (archer != null)
                                {
                                    List<Point> closesttoArcher = AIUtil.findHexasClosestToArcher(myCharID, listG);
                                    Point archerPoint = closesttoArcher[0];
                                    return new ActionAIPos(MainGame.ActionType.MOVE, archerPoint);
                                }
                                else
                                {
                                    
                                    
                                    // Find hexas where position to lowest Enemy is lowest
                                    List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(myCharID, listG);
                                    Point bestHexa = bestHexas2[0];
                                    if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                                    else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);

                                }

                            }
                            else
                            {
                                return new ActionAIPos(MainGame.ActionType.SKIP, null);
                            }
                        }
                    case CharClass.VOLEUR:
                        {
                            Character cLowest = AIUtil.findLowestEnemy(currentChar.team);
                            List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                            if (listH != null && listH.Count > 0)
                            {
                               
                                
                                // Find hexas where position to lowest Enemy is lowest
                                List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(myCharID, listH);
                                Point bestHexa = bestHexas2[0];
                                if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                                else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);
                            }
                            else
                            {
                                return new ActionAIPos(MainGame.ActionType.SKIP, null);
                            }

                        }
                   
                    case CharClass.MAGE:  
                    case CharClass.ENVOUTEUR:
                    case CharClass.ARCHER:
                        {
                            Character cLowest = AIUtil.findLowestEnemy(currentChar.team);
                            List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                            if (listH != null && listH.Count > 0)
                            {
                             
                               
                                // Find hexas where position to lowest Enemy is lowest
                                List<Point> bestHexas2 = AIUtil.findHexasClosestToLowestEnemy(myCharID, listH);
                                Point bestHexa = bestHexas2[0];
                                if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                                else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);
                            }
                            else
                            {
                                return new ActionAIPos(MainGame.ActionType.SKIP, null);
                            }
                        }
                        case CharClass.SOIGNEUR:
                        {
                            List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                            if (listH != null && listH.Count > 0)
                            {
                                // Find hexas where damage dealt is highest
                                List<Point> bestHexas = AIUtil.findHexasClosestToAllies(myCharID, listH);
                                List<Point> bestHexas2 = AIUtil.findHexasWhereValueIsMax(bestHexas, AIUtil.calculateHealing(myCharID));
                               
                                
                                
                                Point bestHexa = bestHexas2[0];
                                if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                                else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);
                            }
                            else
                            {
                                return new ActionAIPos(MainGame.ActionType.SKIP, null);
                            }
                        }

                }
                return new ActionAIPos(MainGame.ActionType.SKIP, null);
            }


            public static ActionAIPos doApproachAlly(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Character cLowest = AIUtil.findLowestEnemy(currentChar.team);
                List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                if (listH != null && listH.Count > 0)
                {
                    List<Point> bestHexas = listH;
                    List<Point> bestHexas2;
                    int[] threat = AIUtil.calculateThreat(currentChar.team);
                    bestHexas = AIUtil.findSafestHexas(bestHexas, threat);
                    // Find hexas where position is closest to allies
                    if (hexaGrid.charList.Count > 2)
                    {
                       
                        bestHexas2 = AIUtil.findHexasClosestToAllies(myCharID, bestHexas);
                    }else bestHexas2 = bestHexas; 


                    // Find hexas where position to lowest Enemy is lowest
                    //List<Point> bestHexas3 = AIUtil.findHexasClosestToLowestEnemy(myCharID, bestHexas2);
                    Point bestHexa = bestHexas2[0];
                    if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                    else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);
                }
                else
                {
                    return new ActionAIPos(MainGame.ActionType.SKIP, null);
                }
            }


            public static ActionAIPos doFlee(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Character cLowest = AIUtil.findLowestEnemy(currentChar.team);
                List<Point> listH = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM);
                if (listH != null && listH.Count > 0)
                {
                    // Find hexas where threat is lowest
                    int[] threat = AIUtil.calculateThreat(currentChar.team);
                    //for (int i = 0; i < threat.Length; i++) threat[i] = -threat[i];
                    List<Point> bestHexas = AIUtil.findSafestHexas(listH, threat);
                    // Find hexas where position is closest to allies
                    List<Point> bestHexas2 = AIUtil.findHexasClosestToAllies(myCharID, bestHexas);
                    if (hexaGrid.charList.Count == 2)
                    {
                        bestHexas2 = bestHexas;
                    }

                    // Find hexas where position is closest to lowest Enemy
                    //List <Point> bestHexas3 = AIUtil.findHexasClosestToLowestEnemy(myCharID, bestHexas2);
                    Point bestHexa = bestHexas2[0];
                    if (bestHexa.x == currentChar.x && bestHexa.y == currentChar.y) return new ActionAIPos(MainGame.ActionType.SKIP, null);
                    else return new ActionAIPos(MainGame.ActionType.MOVE, bestHexa);
                }
                else
                {
                    return new ActionAIPos(MainGame.ActionType.SKIP, null);
                }
            }

            public static ActionAIPos doAttack(int myCharID, int targetID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Point posAttack = null;
                switch (currentChar.charClass)
                {
                    case CharClass.GUERRIER:
                    case CharClass.VOLEUR:
                    case CharClass.ARCHER:
                        {
                            Character cAttack = AIUtil.findCharToAttack(myCharID);
                            if (cAttack != null) posAttack = new Point(cAttack.x, cAttack.y);
                        }
                        break;
                    case CharClass.MAGE:
                        {
                            posAttack = AIUtil.findWhereToAttackMage(myCharID);
                        }
                        break;
                    case CharClass.SOIGNEUR:
                        {
                            Character cAttack = AIUtil.findCharToHeal(myCharID);
                            if (cAttack != null) posAttack = new Point(cAttack.x, cAttack.y);
                        }
                        break;
                    case CharClass.ENVOUTEUR:
                        {
                            Character cAttack = AIUtil.findCharToBuff(myCharID);
                            if (cAttack != null) posAttack = new Point(cAttack.x, cAttack.y);
                        }
                        break;
                }
                if (posAttack != null)
                {
                    return new ActionAIPos(MainGame.ActionType.ATK, posAttack);
                }
                else
                {
                    return new ActionAIPos(MainGame.ActionType.SKIP, null);
                }
            }
            public static ActionAIPos doSpell(int myCharID, int targetID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Point posAttack = null;
                switch (currentChar.charClass)
                {
                    case CharClass.SOIGNEUR:
                    case CharClass.ENVOUTEUR:
                    case CharClass.VOLEUR:
                    case CharClass.MAGE:
                        {
                            Character cAttack = AIUtil.findCharToAttackSpell(myCharID);
                            if (cAttack != null) posAttack = new Point(cAttack.x, cAttack.y);
                        }
                        break;
                    case CharClass.GUERRIER:
                    case CharClass.ARCHER:
                        {
                            posAttack = AIUtil.findWhereToAttackGVSpell(myCharID);
                        }
                        break;
                }
                if (posAttack != null)
                {
                    currentChar.PA -= 2;
                    return new ActionAIPos(MainGame.ActionType.SPELL, posAttack);

                }
                else
                {
                    return new ActionAIPos(MainGame.ActionType.SKIP, null);
                }
            }
            public static ActionAIPos doSkill(int myCharID, int targetID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Point posAttack = null;
                switch (currentChar.charClass)
                {
                    case CharClass.GUERRIER:
                    case CharClass.VOLEUR:
                    case CharClass.ARCHER:
                        {
                            Character cAttack = AIUtil.findCharToAttackSkill(myCharID);
                            if (cAttack != null) posAttack = new Point(cAttack.x, cAttack.y);
                        }
                        break;
                    case CharClass.MAGE:
                        {
                            posAttack = AIUtil.findWhereToAttackMageSkill(myCharID);
                        }
                        break;
                    case CharClass.SOIGNEUR:
                        {
                            posAttack = AIUtil.findWhereToHealSoigneurSkill(myCharID); // Add skill function
                        }
                        break;
                    case CharClass.ENVOUTEUR:
                        {
                            posAttack = AIUtil.findWhereToHealSoigneurSkill(myCharID); // Add skill function
                        }
                        break;
                }
                if (posAttack != null)
                {
                    return new ActionAIPos(MainGame.ActionType.ULT, posAttack);
                }
                else
                {
                    return new ActionAIPos(MainGame.ActionType.SKIP, null);
                }
            }
        }

        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------
        // 2.0
        // These functions are used for the behavior of the AI of Classifier2 
        // -----------------------------------------------------------------------------------------------------------------------------------------------------------------

        public static bool canTarget(int cX, int cY, int targetX, int targetY, int range, int rangeAoE, CharsDB.AoEType aoEType)
        {
            if (rangeAoE > 0)
            {
                if (hexaGrid.hexaInSight(cX, cY, targetX, targetY, range))
                {
                    return true;
                }
                else if (hexaGrid.getDistance(cX, cY, targetX, targetY) <= range + rangeAoE)
                {
                    List<Point> hexas = hexaGrid.findHexasInSight2(cX, cY, range);
                    foreach (Point h in hexas)
                    {
                        List<Character> chars = hexaGrid.getCharWithinRange(h.x, h.y, cX, cY, rangeAoE, aoEType);
                        foreach (Character c2 in chars)
                        {
                            if (c2.x == targetX && c2.y == targetY) return true;
                        }
                    }
                    return false;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return hexaGrid.hexaInSight(cX, cY, targetX, targetY, range);
            }
        }
        
        public static Character findCharToAttackSpell(int myCharID)
        {
            Character currentChar = hexaGrid.charList[myCharID];
            int lowest = 100000;
            Character cLowest = null;
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team != currentChar.team && c != currentChar)
                {
                    if (hexaGrid.hexaInSight(currentChar.x, currentChar.y, c.x, c.y, currentChar.getClassData().spell.range))
                    {
                        if (c.HP < lowest)
                        {
                            lowest = c.HP;
                            cLowest = c;
                        }
                    }
                }
            }
            return cLowest;
        }
        public static Point findWhereToAttackGVSpell(int myCharID)
        {
            int maxTargets = 0;
            Character currentChar = hexaGrid.charList[myCharID];
            List<int> possibleHexasValues = new List<int>();
            List<Point> possibleHexas = hexaGrid.findHexasInSight2(currentChar.x, currentChar.y, currentChar.getClassData().spell.range);
            foreach (Point p in possibleHexas)
            {
                List<Character> lc = hexaGrid.getCharWithinRange(p.x, p.y, currentChar.x, currentChar.y, currentChar.getClassData().spell.rangeAoE, currentChar.getClassData().spell.aoEType);
                int nb = 0;
                // filter allies
                foreach (Character c in lc)
                {
                    if (c.team != currentChar.team && c != currentChar) nb++;
                }
                possibleHexasValues.Add(nb);
                if (nb > maxTargets) maxTargets = nb;
            }
            List<Point> bestHexas = new List<Point>();
            for (int i = 0; i < possibleHexas.Count; i++) if (possibleHexasValues[i] == maxTargets) bestHexas.Add(possibleHexas[i]);

            if (bestHexas.Count > 0)
            {
                return bestHexas[0]; // Improve by finding the best one to return
            }
            else
            {
                return null;
            }
        }

        public static bool canTargetAttack(Character c, Character target)
        {
            switch (c.charClass)
            {
                case CharClass.GUERRIER:
                case CharClass.VOLEUR:
                case CharClass.ARCHER:
                case CharClass.MAGE:
                    return target.team != c.team && canTarget(c.x, c.y, target.x, target.y, c.getClassData().basicAttack.range, c.getClassData().basicAttack.rangeAoE, c.getClassData().basicAttack.aoEType);
                case CharClass.SOIGNEUR:
                    return target.team == c.team && target != c && target.HP < target.HPmax && canTarget(c.x, c.y, target.x, target.y, c.getClassData().basicAttack.range, c.getClassData().basicAttack.rangeAoE, c.getClassData().basicAttack.aoEType);
                case CharClass.ENVOUTEUR:
                    return target.team == c.team && target != c && target.PA <= target.getClassData().basePA && canTarget(c.x, c.y, target.x, target.y, c.getClassData().basicAttack.range, c.getClassData().basicAttack.rangeAoE, c.getClassData().basicAttack.aoEType);
                default: return false;
            }
        }

        public static bool canTargetSkill(Character c, Character target)
        {
            if (c.skillAvailable)
            {
                switch (c.charClass)
                {
                    case CharClass.GUERRIER:
                    case CharClass.VOLEUR:
                    case CharClass.ARCHER:
                    case CharClass.MAGE:
                        return target.team != c.team && canTarget(c.x, c.y, target.x, target.y, c.getClassData().ult.range, c.getClassData().ult.rangeAoE, c.getClassData().ult.aoEType);
                    case CharClass.SOIGNEUR:
                        return target.team == c.team && target != c && target.HP < target.HPmax && canTarget(c.x, c.y, target.x, target.y, c.getClassData().ult.range, c.getClassData().ult.rangeAoE, c.getClassData().ult.aoEType);
                    case CharClass.ENVOUTEUR:
                        return target.team == c.team && target != c && target.PA <= target.getClassData().basePA && canTarget(c.x, c.y, target.x, target.y, c.getClassData().ult.range, c.getClassData().ult.rangeAoE, c.getClassData().ult.aoEType);
                    default: return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool canTargetSpell(Character c, Character target)
        {
            
            if (c.PA >= 2)
            {
                return target.team != c.team && canTarget(c.x, c.y, target.x, target.y, c.getClassData().ult.range, c.getClassData().ult.rangeAoE, c.getClassData().ult.aoEType);
            }
            else return false;
            
            
        }

        public static bool canTargetWithMovementAttack(Character c, Character target)
        {
            int cX = c.x; int cY = c.y;
            for (int i = 1; i < c.PA; i++)
            {
                List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                foreach (Point charPos in listH)
                {
                    c.updatePos2(charPos.x, charPos.y, hexaGrid);
                    if (canTargetAttack(c, target))
                    {
                        c.updatePos2(cX, cY, hexaGrid);
                        return true;
                    }
                }
            }
            c.updatePos2(cX, cY, hexaGrid);
            return false;
        }

        public static bool canTargetWithMovementSkill(Character c, Character target)
        {
            if (c.skillAvailable)
            {
                int cX = c.x; int cY = c.y;
                for (int i = 1; i < c.PA; i++)
                {
                    List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                    foreach (Point charPos in listH)
                    {
                        c.updatePos2(charPos.x, charPos.y, hexaGrid);
                        if (canTargetSkill(c, target))
                        {
                            c.updatePos2(cX, cY, hexaGrid);
                            return true;
                        }
                    }
                }
                c.updatePos2(cX, cY, hexaGrid);
            }
            return false;
        }

        public static bool canTargetWithMovementSpell(Character c, Character target)
        {
           
                int cX = c.x; int cY = c.y;
                for (int i = 1; i < c.PA; i++)
                {
                    List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                    foreach (Point charPos in listH)
                    {
                        c.updatePos2(charPos.x, charPos.y, hexaGrid);
                        if (canTargetSpell(c, target))
                        {
                            c.updatePos2(cX, cY, hexaGrid);
                            return true;
                        }
                    }
                }
                c.updatePos2(cX, cY, hexaGrid);
            
            return false;
        }

        /** Calculates the threat of a character at hexa (x,y) from currentCharID's turn to theirs (charToCheckID).
            returns a value the indicates an estimation of the amount of HP potentially
            gained until next turn. (negative values indicate damage taken).
            If the value -1000 is returned, the character would die. */
        public static int threatAtHexa(int x, int y, int currentCharID, int toCheckCharID)
        {
            currentCharID = currentCharID % (hexaGrid.charList.Count);
            Character toCheckChar = hexaGrid.charList[toCheckCharID];
            int toCheckCharHP = toCheckChar.HP;
            int toCheckCharX = toCheckChar.x;
            int toCheckCharY = toCheckChar.y;
            toCheckChar.updatePos2(x, y, hexaGrid);

            int threat = 0;
            for (int j = currentCharID; ; j++)
            {
                Character c = hexaGrid.charList[j % (hexaGrid.charList.Count)];
                if (c == toCheckChar) break;

                // damage
                if (c.team != toCheckChar.team && c.charClass != CharClass.SOIGNEUR && c.charClass != CharClass.ENVOUTEUR)
                {
                    int damage = 0;
                    // 0 PM
                    {
                        bool skill = canTargetSkill(c, toCheckChar);
                        bool attack = canTargetAttack(c, toCheckChar);
                        damage = ((skill) ? (c.getClassData().ult.effectValue) : 0) + ((attack) ? (c.getClassData().basicAttack.effectValue * ((skill) ? (c.PA - 1) : (c.PA))) : 0);
                    }

                    // 1+ PM
                    int cX = c.x; int cY = c.y;
                    for (int i = 1; i < c.PA && damage == 0; i++)
                    {
                        List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                        foreach (Point charPos in listH)
                        {
                            c.updatePos2(charPos.x, charPos.y, hexaGrid);
                            bool skill = canTargetSkill(c, toCheckChar);
                            bool attack = canTargetAttack(c, toCheckChar);
                            damage = ((skill) ? (c.getClassData().ult.effectValue) : 0) + ((attack) ? (c.getClassData().basicAttack.effectValue * ((skill) ? (c.PA - i - 1) : (c.PA - i))) : 0);
                            if (damage > 0) break;
                        }
                        c.updatePos2(cX, cY, hexaGrid);
                    }

                    toCheckChar.HP -= damage;
                    if (toCheckChar.HP <= 0) break;
                    // heal
                }
                else if (c.team == toCheckChar.team && c.charClass == CharClass.SOIGNEUR)
                {
                    int heal = 0;
                    // 0 PM
                    {
                        bool skill = canTargetSkill(c, toCheckChar);
                        bool attack = canTargetAttack(c, toCheckChar);
                        heal = ((skill) ? (c.getClassData().ult.effectValue) : 0) + ((attack) ? (c.getClassData().basicAttack.effectValue * ((skill) ? (c.PA - 1) : (c.PA))) : 0);
                    }

                    // 1+ PM
                    int cX = c.x; int cY = c.y;
                    for (int i = 1; i < c.PA && heal == 0; i++)
                    {
                        Point realCharPos = new Point(c.x, c.y);
                        List<Point> listH = hexaGrid.findAllPaths(c.x, c.y, c.PM * i);
                        foreach (Point charPos in listH)
                        {
                            c.updatePos2(charPos.x, charPos.y, hexaGrid);
                            bool skill = canTargetSkill(c, toCheckChar);
                            bool attack = canTargetAttack(c, toCheckChar);
                            heal = ((skill) ? (c.getClassData().ult.effectValue) : 0) + ((attack) ? (c.getClassData().basicAttack.effectValue * ((skill) ? (c.PA - i - 1) : (c.PA - i))) : 0);
                            if (heal > 0) break;
                        }
                        c.updatePos2(cX, cY, hexaGrid);
                    }

                    toCheckChar.HP += heal;
                    if (toCheckChar.HP > toCheckChar.HPmax) toCheckChar.HP = toCheckChar.HPmax;
                }
            }

            toCheckChar.updatePos2(toCheckCharX, toCheckCharY, hexaGrid);
            if (toCheckChar.HP <= 0) threat = -1000;
            else threat = toCheckChar.HP - toCheckCharHP;
            toCheckChar.HP = toCheckCharHP;
            return threat;
        }

        public static List<HexaDamage> threatAtHexas(List<Point> hexas, int currentCharID, int toCheckCharID)
        {
            List<HexaDamage> r = new List<HexaDamage>();
            foreach (Point h in hexas) r.Add(new HexaDamage(h.x, h.y, threatAtHexa(h.x, h.y, currentCharID, toCheckCharID)));
            return r;
        }

        public static List<Point> getHexasWhereThreatIsMin(List<Point> hexas, int currentCharID, int toCheckCharID)
        {
            List<Point> hexas2 = new List<Point>();
            if (hexas.Count > 0)
            {
                List<HexaDamage> hexas2_ = threatAtHexas(hexas, currentCharID, toCheckCharID);
                int best = hexas2_[0].value;
                foreach (HexaDamage hd in hexas2_) if (hd.value > best) best = hd.value;
                foreach (HexaDamage hd in hexas2_) if (hd.value == best) hexas2.Add(new Point(hd.x, hd.y));
            }
            return hexas2;
        }

        /** Can be used to know where an enemy is within range of being attacked.
            Can also be used to know where a SOINGEUR is in range to heal an ally. */
        public static List<Point> getHexasWhereCharIsInRangeAttack(List<Point> hexas, Character c, Character target)
        {
            List<Point> hexas2 = new List<Point>();
            if (hexas.Count > 0)
            {
                int cX = c.x; int cY = c.y;
                foreach (Point p in hexas)
                {
                    c.updatePos2(p.x, p.y, hexaGrid);
                    if (canTargetAttack(c, target)) hexas2.Add(new Point(p.x, p.y));
                }
                c.updatePos2(cX, cY, hexaGrid);
            }
            return hexas2;
        }

        /** Can be used to know where an enemy is within range of being attacked.
            Can also be used to know where a SOINGEUR is in range to heal an ally. */
        public static List<Point> getHexasWhereCharIsInRangeSkill(List<Point> hexas, Character c, Character target)
        {
            List<Point> hexas2 = new List<Point>();
            if (hexas.Count > 0)
            {
                int cX = c.x; int cY = c.y;
                foreach (Point p in hexas)
                {
                    c.updatePos2(p.x, p.y, hexaGrid);
                    if (canTargetSkill(c, target)) hexas2.Add(new Point(p.x, p.y));
                }
                c.updatePos2(cX, cY, hexaGrid);
            }
            return hexas2;
        }

        /** (Lowest PA spent) */
        public static List<Point> getHexasWhereWalkingDistanceIsLowest(List<Point> hexas, int charID, int targetID)
        {
            List<Point> r = new List<Point>();
            if (hexas.Count > 0)
            {
                Character c1 = hexaGrid.charList[charID];
                Character c2 = hexaGrid.charList[targetID];
                Point charPos = new Point(c1.x, c1.y);
                List<int> walkingDistanceTmp = new List<int>();
                c1.updatePos2(hexas[0].x, hexas[0].y, hexaGrid);
                int minWalkingDistanceTmp = hexaGrid.getWalkingDistance(c1.x, c1.y, c2.x, c2.y);
                minWalkingDistanceTmp = (minWalkingDistanceTmp == 0) ? 0 : ((minWalkingDistanceTmp - 1) / c1.PM + 1);
                walkingDistanceTmp.Add(minWalkingDistanceTmp);
                int minWalkingDistance = walkingDistanceTmp[0];
                for (int i = 1; i < hexas.Count; i++)
                {
                    c1.updatePos2(hexas[i].x, hexas[i].y, hexaGrid);
                    int walkingDistance = hexaGrid.getWalkingDistance(c1.x, c1.y, c2.x, c2.y);
                    walkingDistance = (walkingDistance == 0) ? 0 : ((walkingDistance - 1) / c1.PM + 1);
                    if (minWalkingDistance > walkingDistance) minWalkingDistance = walkingDistance;
                    walkingDistanceTmp.Add(walkingDistance);
                }
                for (int i = 0; i < hexas.Count; i++) if (walkingDistanceTmp[i] == minWalkingDistance) r.Add(new Point(hexas[i].x, hexas[i].y));
                c1.updatePos2(charPos.x, charPos.y, hexaGrid);
            }

            return r;
        }

        /** Returns the hexas with the lowest amount of PA to use to get to*/
        public static List<Point> getHexasWhereMovementIsLowest(List<Point> hexas, int charID)
        {
            List<Point> r = new List<Point>();
            if (hexas.Count > 0)
            {
                List<int> values = new List<int>();
                foreach (Point h in hexas) values.Add(findSequencePathToHexa(charID, h.x, h.y).Count);
                int lowest = values[0];
                for (int i = 0; i < values.Count; i++) if (values[i] < lowest) lowest = values[i];
                for (int i = 0; i < values.Count; i++) if (values[i] == lowest) r.Add(new Point(hexas[i].x, hexas[i].y));
            }
            return r;
        }

        public static List<Character> getTargetableCharsInRangeAttack(Character c)
        {
            List<Character> r = new List<Character>();
            foreach (Character c2 in hexaGrid.charList) if (canTargetAttack(c, c2)) r.Add(c2);
            return r;
        }

        public static List<Character> getTargetableCharsInRangeSkill(Character c)
        {
            List<Character> r = new List<Character>();
            foreach (Character c2 in hexaGrid.charList) if (canTargetSkill(c, c2)) r.Add(c2);
            return r;
        }

        /** Finds the target with highest threat within range. (Also works for soigneur) */
        public static Character getMainTargetAttack(int charID)
        {
            Character c = hexaGrid.charList[charID];
            List<Character> l = getTargetableCharsInRangeAttack(c);
            // Target char in range
            if (l.Count > 0)
            {
                List<int> threatTmp = new List<int>();
                threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                int minThreat = threatTmp[0];
                for (int i = 1; i < l.Count; i++)
                {
                    int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                    if (minThreat > threat) minThreat = threat;
                    threatTmp.Add(threat);
                }
                for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                return l[0];
            }
            else
            {
                // Target char in range after movement
                foreach (Character c2 in hexaGrid.charList)
                {
                    switch (c.charClass)
                    {
                        case CharClass.GUERRIER:
                        case CharClass.VOLEUR:
                        case CharClass.ARCHER:
                        case CharClass.MAGE:
                            if (c.team != c2.team && canTargetWithMovementAttack(c, c2)) l.Add(c2); break;

                        case CharClass.SOIGNEUR:
                        case CharClass.ENVOUTEUR:
                            if (c.team == c2.team && c != c2 && canTargetWithMovementAttack(c, c2)) l.Add(c2); break;
                    }
                }
                if (l.Count > 0)
                {
                    List<int> threatTmp = new List<int>();
                    threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                    int minThreat = threatTmp[0];
                    for (int i = 1; i < l.Count; i++)
                    {
                        int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                        if (minThreat > threat) minThreat = threat;
                        threatTmp.Add(threat);
                    }
                    for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                    return l[0];
                    // Get closer to char
                }
                else
                {
                    foreach (Character c2 in hexaGrid.charList)
                    {
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER:
                            case CharClass.VOLEUR:
                            case CharClass.ARCHER:
                            case CharClass.MAGE:
                                if (c.team != c2.team) l.Add(c2); break;

                            case CharClass.SOIGNEUR:
                            case CharClass.ENVOUTEUR:
                                if (c.team == c2.team && c != c2) l.Add(c2); break;
                        }
                    }
                    if (l.Count > 0)
                    {
                        List<int> threatTmp = new List<int>();
                        threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                        int minThreat = threatTmp[0];
                        for (int i = 1; i < l.Count; i++)
                        {
                            int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                            if (minThreat > threat) minThreat = threat;
                            threatTmp.Add(threat);
                        }
                        for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                        return l[0];
                    }
                }
            }
            return null;
        }

        public static Character getMainTargetSkill(int charID)
        {
            Character c = hexaGrid.charList[charID];
            List<Character> l = getTargetableCharsInRangeSkill(c);
            if (l.Count > 0)
            {
                List<int> threatTmp = new List<int>();
                threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                int minThreat = threatTmp[0];
                for (int i = 1; i < l.Count; i++)
                {
                    int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                    if (minThreat > threat) minThreat = threat;
                    threatTmp.Add(threat);
                }
                for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                return l[0];
            }
            else
            {
                foreach (Character c2 in hexaGrid.charList)
                {
                    switch (c.charClass)
                    {
                        case CharClass.GUERRIER:
                        case CharClass.VOLEUR:
                        case CharClass.ARCHER:
                        case CharClass.MAGE:
                            if (c.team != c2.team && canTargetWithMovementSkill(c, c2)) l.Add(c2); break;

                        case CharClass.SOIGNEUR:
                        case CharClass.ENVOUTEUR:
                            if (c.team == c2.team && c != c2 && canTargetWithMovementSkill(c, c2)) l.Add(c2); break;
                    }
                }
                if (l.Count > 0)
                {
                    List<int> threatTmp = new List<int>();
                    threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                    int minThreat = threatTmp[0];
                    for (int i = 1; i < l.Count; i++)
                    {
                        int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                        if (minThreat > threat) minThreat = threat;
                        threatTmp.Add(threat);
                    }
                    for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                    return l[0];
                }
                else
                {
                    foreach (Character c2 in hexaGrid.charList)
                    {
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER:
                            case CharClass.VOLEUR:
                            case CharClass.ARCHER:
                            case CharClass.MAGE:
                                if (c.team != c2.team) l.Add(c2); break;

                            case CharClass.SOIGNEUR:
                            case CharClass.ENVOUTEUR:
                                if (c.team == c2.team && c != c2) l.Add(c2); break;
                        }
                    }
                    if (l.Count > 0)
                    {
                        List<int> threatTmp = new List<int>();
                        threatTmp.Add(threatAtHexa(l[0].x, l[0].y, charID, hexaGrid.getCharID(l[0])));
                        int minThreat = threatTmp[0];
                        for (int i = 1; i < l.Count; i++)
                        {
                            int threat = threatAtHexa(l[i].x, l[i].y, charID, hexaGrid.getCharID(l[i]));
                            if (minThreat > threat) minThreat = threat;
                            threatTmp.Add(threat);
                        }
                        for (int i = 0; i < l.Count; i++) if (threatTmp[i] == minThreat) return l[i];
                        return l[0];
                    }
                }
            }
            return null;
        }

        /** Returns the hexa that should be targeted to attack. Assumes mainTarget is targetable*/
        public static Point getPosToUseAttack(Character c, Character mainTarget)
        {
            if (c.getClassData().basicAttack.rangeAoE > 0)
            {
                List<Point> hexas = hexaGrid.findHexasInSight2(c.x, c.y, c.getClassData().basicAttack.range);
                HexaDamage pos = new HexaDamage(hexas[0].x, hexas[0].y, 0);
                foreach (Point h in hexas)
                {
                    List<Character> chars = hexaGrid.getCharWithinRange(h.x, h.y, c.x, c.y, c.getClassData().basicAttack.rangeAoE, c.getClassData().basicAttack.aoEType);

                    if (!c.getClassData().basicAttack.targetsEnemies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team != c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }
                    if (!c.getClassData().basicAttack.targetsAllies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team == c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }

                    foreach (Character c2 in chars)
                    {
                        if (c2 == mainTarget)
                        {
                            if (chars.Count > pos.value)
                            {
                                pos.x = h.x;
                                pos.y = h.y;
                                pos.value = chars.Count;
                            }
                        }
                    }
                }
                return new Point(pos.x, pos.y);
            }
            else
            {
                return new Point(mainTarget.x, mainTarget.y);
            }
        }

        /** Returns the hexa that should be targeted to attack (skill). Assumes mainTarget is targetable*/
        public static Point getPosToUseSkill(Character c, Character mainTarget)
        {
            if (c.getClassData().ult.rangeAoE > 0)
            {
                List<Point> hexas = hexaGrid.findHexasInSight2(c.x, c.y, c.getClassData().ult.range);
                HexaDamage pos = new HexaDamage(hexas[0].x, hexas[0].y, 0);
                foreach (Point h in hexas)
                {
                    List<Character> chars = hexaGrid.getCharWithinRange(h.x, h.y, c.x, c.y, c.getClassData().ult.rangeAoE, c.getClassData().ult.aoEType);

                    if (!c.getClassData().ult.targetsEnemies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team != c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }
                    if (!c.getClassData().ult.targetsAllies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team == c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }

                    foreach (Character c2 in chars)
                    {
                        if (c2 == mainTarget)
                        {
                            if (chars.Count > pos.value)
                            {
                                pos.x = h.x;
                                pos.y = h.y;
                                pos.value = chars.Count;
                            }
                        }
                    }
                }
                return new Point(pos.x, pos.y);
            }
            else
            {
                return new Point(mainTarget.x, mainTarget.y);
            }
        }

        public static Point getPosToUseSpell(Character c, Character mainTarget)
        {
            if (c.getClassData().ult.rangeAoE > 0)
            {
                List<Point> hexas = hexaGrid.findHexasInSight2(c.x, c.y, c.getClassData().spell.range);
                HexaDamage pos = new HexaDamage(hexas[0].x, hexas[0].y, 0);
                foreach (Point h in hexas)
                {
                    List<Character> chars = hexaGrid.getCharWithinRange(h.x, h.y, c.x, c.y, c.getClassData().spell.rangeAoE, c.getClassData().spell.aoEType);

                    if (!c.getClassData().spell.targetsEnemies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team != c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }
                    if (!c.getClassData().spell.targetsAllies)
                    {
                        for (int i = 0; i < chars.Count; i++)
                        {
                            if (chars[i].team == c.team)
                            {
                                chars.RemoveAt(i); i--;
                            }
                        }
                    }

                    foreach (Character c2 in chars)
                    {
                        if (c2 == mainTarget)
                        {
                            if (chars.Count > pos.value)
                            {
                                pos.x = h.x;
                                pos.y = h.y;
                                pos.value = chars.Count;
                            }
                        }
                    }
                }
                return new Point(pos.x, pos.y);
            }
            else
            {
                return new Point(mainTarget.x, mainTarget.y);
            }
        }

        public static List<Point> getHexasClosestToAllies(List<Point> hexas, int charID)
        {
            Character currentChar = hexaGrid.charList[charID];
            List<Character> alliesList = new List<Character>();
            foreach (Character c in hexaGrid.charList)
            {
                if (c.team == currentChar.team && c != currentChar) alliesList.Add(c);
            }
            if (alliesList.Count > 0 && hexas.Count > 0)
            {
                int charPosX = currentChar.x;
                int charPosY = currentChar.y;
                switch (currentChar.charClass)
                {
                    case CharClass.GUERRIER:
                    case CharClass.VOLEUR:
                    case CharClass.ARCHER:
                    case CharClass.MAGE:
                    case CharClass.SOIGNEUR:
                    case CharClass.ENVOUTEUR:
                        {
                            List<Point> r = new List<Point>();
                            List<int> walkingDistanceList = new List<int>();
                            int walkingDistance = 0;
                            currentChar.updatePos2(hexas[0].x, hexas[0].y, hexaGrid);
                            foreach (Character c in alliesList) walkingDistance += hexaGrid.getWalkingDistance(currentChar.x, currentChar.y, c.x, c.y) * ((c.charClass == CharClass.SOIGNEUR) ? 3 : 1);
                            walkingDistanceList.Add(walkingDistance);
                            int minWalkingDistance = walkingDistanceList[0];

                            for (int i = 1; i < hexas.Count; i++)
                            {
                                walkingDistance = 0;
                                currentChar.updatePos2(hexas[i].x, hexas[i].y, hexaGrid);
                                foreach (Character c in alliesList) walkingDistance += hexaGrid.getWalkingDistance(currentChar.x, currentChar.y, c.x, c.y) * ((c.charClass == CharClass.SOIGNEUR) ? 3 : 1);
                                walkingDistanceList.Add(walkingDistance);
                                if (minWalkingDistance > walkingDistance) minWalkingDistance = walkingDistance;
                            }

                            for (int i = 0; i < hexas.Count; i++)
                            {
                                if (walkingDistanceList[i] == minWalkingDistance) r.Add(hexas[i]);
                            }

                            currentChar.updatePos2(charPosX, charPosY, hexaGrid);
                            return r;
                        }
                }
            }
            return hexas;
        }

        public class AIHard2
        {

            public static Point doFleePos(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                List<Point> hexas1 = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * currentChar.PA);
                List<Point> hexas2 = getHexasWhereThreatIsMin(hexas1, myCharID + 1, myCharID);
                List<Point> hexas3 = getHexasClosestToAllies(hexas2, myCharID);
                List<Point> hexas4 = getHexasWhereMovementIsLowest(hexas3, myCharID);
                return hexas4[0];
            }

            public static List<ActionAIPos> doFlee(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Point hexaPos = doFleePos(myCharID);
                List<ActionAIPos> sequence = new List<ActionAIPos>();

                if (!(hexaPos.x == currentChar.x && hexaPos.y == currentChar.y))
                {
                    sequence = findSequencePathToHexa(myCharID, hexaPos.x, hexaPos.y);
                }

                if (sequence.Count < currentChar.PA)
                {
                    int charposX = currentChar.x;
                    int charposY = currentChar.y;
                    currentChar.updatePos2(hexaPos.x, hexaPos.y, hexaGrid);
                    // Get target
                    List<Character> targetList = getTargetableCharsInRangeAttack(currentChar);
                    // Attack target
                    if (targetList.Count > 0)
                    {
                        Character target = targetList[0];
                        if (canTargetAttack(currentChar, target))
                        {
                            Point targetHexa = getPosToUseAttack(currentChar, target);
                            for (int i = sequence.Count; i < currentChar.PA; i++)
                            {
                                sequence.Add(new ActionAIPos(MainGame.ActionType.ATK, new Point(targetHexa.x, targetHexa.y)));
                            }
                        }
                    }
                    currentChar.updatePos2(charposX, charposY, hexaGrid);
                }

                sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null));
                return sequence;
            }

            public static Point doAttackPos(int myCharID, int targetID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Character target = hexaGrid.charList[targetID];
                List<Point> hexas1 = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * (currentChar.PA - 1));
                List<Point> hexas2 = getHexasWhereCharIsInRangeAttack(hexas1, currentChar, target);
                List<Point> hexas3;
                List<Point> hexas4;
                if (hexas2.Count == 0)
                {
                    hexas1 = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * (currentChar.PA));
                    hexas2 = getHexasWhereThreatIsMin(hexas1, myCharID + 1, myCharID);
                    hexas3 = getHexasWhereWalkingDistanceIsLowest(hexas2, myCharID, targetID);
                    hexas4 = getHexasClosestToAllies(hexas3, myCharID);
                    return hexas4[0];
                }
                else
                {
                    hexas3 = getHexasWhereMovementIsLowest(hexas2, myCharID);
                    hexas4 = getHexasWhereThreatIsMin(hexas3, myCharID + 1, myCharID);
                    List<Point> hexas5 = getHexasClosestToAllies(hexas4, myCharID);
                    return hexas5[0];
                }
            }

            public static List<ActionAIPos> doAttack(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                List<ActionAIPos> sequence = new List<ActionAIPos>();

                // Get target
                Character target = getMainTargetAttack(myCharID); if (target == null) { sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null)); return sequence; }
                // Get destination to target
                Point hexaPos = doAttackPos(myCharID, hexaGrid.getCharID(target));
                // Get path to destination
                if (!(hexaPos.x == currentChar.x && hexaPos.y == currentChar.y))
                {
                    sequence = findSequencePathToHexa(myCharID, hexaPos.x, hexaPos.y);
                }

                int charposX = currentChar.x;
                int charposY = currentChar.y;
                currentChar.updatePos2(hexaPos.x, hexaPos.y, hexaGrid);
                // Attack target
                if (canTargetAttack(currentChar, target))
                {
                    Point targetHexa = getPosToUseAttack(currentChar, target);
                    for (int i = sequence.Count; i < currentChar.PA; i++)
                    {
                        sequence.Add(new ActionAIPos(MainGame.ActionType.ATK, new Point(targetHexa.x, targetHexa.y)));
                    }
                }

                currentChar.updatePos2(charposX, charposY, hexaGrid);
                sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null));
                return sequence;
            }

            public static Point doSkillPos(int myCharID, int targetID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                Character target = hexaGrid.charList[targetID];
                List<Point> hexas1 = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * (currentChar.PA - 1));
                List<Point> hexas2 = getHexasWhereCharIsInRangeSkill(hexas1, currentChar, target);
                List<Point> hexas3;
                List<Point> hexas4;
                if (hexas2.Count == 0)
                {
                    hexas1 = hexaGrid.findAllPaths(currentChar.x, currentChar.y, currentChar.PM * (currentChar.PA));
                    hexas2 = getHexasWhereThreatIsMin(hexas1, myCharID + 1, myCharID);
                    hexas3 = getHexasWhereWalkingDistanceIsLowest(hexas2, myCharID, targetID);
                    hexas4 = getHexasClosestToAllies(hexas3, myCharID);
                    return hexas4[0];
                }
                else
                {
                    hexas3 = getHexasWhereMovementIsLowest(hexas2, myCharID);
                    hexas4 = getHexasWhereThreatIsMin(hexas3, myCharID + 1, myCharID);
                    List<Point> hexas5 = getHexasClosestToAllies(hexas4, myCharID);
                    return hexas5[0];
                }
            }

            public static List<ActionAIPos> doSkill(int myCharID)
            {
                Character currentChar = hexaGrid.charList[myCharID];
                List<ActionAIPos> sequence = new List<ActionAIPos>();

                // Get target
                Character target = getMainTargetSkill(myCharID); if (target == null) { sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null)); return sequence; }
                // Get destination to target
                Point hexaPos = doSkillPos(myCharID, hexaGrid.getCharID(target));
                // Get path to destination
                if (!(hexaPos.x == currentChar.x && hexaPos.y == currentChar.y))
                {
                    sequence = findSequencePathToHexa(myCharID, hexaPos.x, hexaPos.y);
                }

                int charposX = currentChar.x;
                int charposY = currentChar.y;
                currentChar.updatePos2(hexaPos.x, hexaPos.y, hexaGrid);
                // Attack target (skill)
                if (canTargetSkill(currentChar, target))
                {
                    Point targetHexa = getPosToUseSkill(currentChar, target);
                    if (sequence.Count < currentChar.PA) sequence.Add(new ActionAIPos(MainGame.ActionType.ULT, new Point(targetHexa.x, targetHexa.y)));
                }
                // Attack target (basic attack)
                if (canTargetAttack(currentChar, target))
                {
                    Point targetHexa = getPosToUseAttack(currentChar, target);
                    for (int i = sequence.Count; i < currentChar.PA; i++)
                    {
                        sequence.Add(new ActionAIPos(MainGame.ActionType.ATK, new Point(targetHexa.x, targetHexa.y)));
                    }
                }

                currentChar.updatePos2(charposX, charposY, hexaGrid);
                sequence.Add(new ActionAIPos(MainGame.ActionType.SKIP, null));
                return sequence;
            }


            public static int threatIfAttack(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                // Get target
                Character target = getMainTargetAttack(charID);
                if (target == null) return threatAtHexa(currentChar.x, currentChar.y, charID + 1, charID);
                // Get destination to target
                Point hexaPos = doAttackPos(charID, hexaGrid.getCharID(target));
                return threatAtHexa(hexaPos.x, hexaPos.y, charID + 1, charID);
            }

            public static int threatIfSkill(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                // Get target
                Character target = getMainTargetSkill(charID);
                if (target == null) return threatAtHexa(currentChar.x, currentChar.y, charID + 1, charID);
                // Get destination to target
                Point hexaPos = doSkillPos(charID, hexaGrid.getCharID(target));
                return threatAtHexa(hexaPos.x, hexaPos.y, charID + 1, charID);
            }

            public static int threatIfFlee(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                Point hexaPos = doFleePos(charID);
                return threatAtHexa(hexaPos.x, hexaPos.y, charID + 1, charID);
            }


            public static int threatEnemyIfAttack(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                // Get target
                Character target = getMainTargetAttack(charID);
                if (target == null) return 0;
                // Get destination to target
                Point hexaPos = doAttackPos(charID, hexaGrid.getCharID(target));
                int walkingDistanceTmp = hexaGrid.getWalkingDistance(currentChar.x, currentChar.y, hexaPos.x, hexaPos.y);
                walkingDistanceTmp = (walkingDistanceTmp == 0) ? 0 : ((walkingDistanceTmp - 1) / currentChar.PM + 1);

                bool skillAvailable = currentChar.skillAvailable;
                int nbPA = currentChar.PA;
                currentChar.skillAvailable = false;
                currentChar.PA = currentChar.PA - walkingDistanceTmp;
                int threat = threatAtHexa(target.x, target.y, charID, hexaGrid.getCharID(target));
                currentChar.skillAvailable = skillAvailable;
                currentChar.PA = nbPA;
                return threat;
            }

            public static int threatEnemyIfSkill(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                // Get target
                Character target = getMainTargetSkill(charID);
                if (target == null) return 0;
                // Get destination to target
                Point hexaPos = doSkillPos(charID, hexaGrid.getCharID(target));
                int walkingDistanceTmp = hexaGrid.getWalkingDistance(currentChar.x, currentChar.y, hexaPos.x, hexaPos.y);
                walkingDistanceTmp = (walkingDistanceTmp == 0) ? 0 : ((walkingDistanceTmp - 1) / currentChar.PM + 1);

                int nbPA = currentChar.PA;
                currentChar.PA = currentChar.PA - walkingDistanceTmp;
                int threat = threatAtHexa(target.x, target.y, charID, hexaGrid.getCharID(target));
                currentChar.PA = nbPA;
                return threat;
            }

            public static int threatEnemyIfFlee(int charID)
            {
                Character currentChar = hexaGrid.charList[charID];
                // Get target
                Character target = getMainTargetAttack(charID);
                if (target == null) return threatAtHexa(currentChar.x, currentChar.y, charID + 1, charID);
                return threatAtHexa(target.x, target.y, charID + 1, hexaGrid.getCharID(target));
            }
        }
    }

}


