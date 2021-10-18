using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Hexas;
using Characters;
using AI_Util;
using System.Data;
using Mono.Data.Sqlite;
using AI_Class;
using Classifiers;

namespace Classifiers1
{

    public class Classifier1Attributes
    {
        public enum CharClass : byte { GUERRIER = 1, VOLEUR = 2, ARCHER = 4, MAGE = 8, SOIGNEUR = 16, ENVOUTEUR = 32 };
        public enum HP_ : byte { BETWEEN_100_75 = 1, BETWEEN_74_40 = 2, BETWEEN_39_0 = 4 };
        public enum PA_ : byte { ONE = 1, TWO_OR_MORE = 2 };
        public enum SkillAvailable : byte { YES = 1, NO = 2 };
        public enum Threat : byte { SAFE = 1, DANGER = 2, DEATH = 4 };
        public enum MaxTargets : byte { NONE = 1, ONE = 2, TWO = 4, THREE_OR_MORE = 8 };

        public enum ClassType : byte { SOIGNEUR = 1, ENVOUTEUR = 2, OTHER = 4, NONE = 8 };
        public enum Distance : byte { ATTACK = 1, SKILL = 2, SPELL = 4, ATTACK_SPELL_AND_SKILL = 8, MOVEMENT = 16 };

        public static CharClass getRandomCharClassValue()
        {
            var values = CharClass.GetValues(typeof(CharClass));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            CharClass randomVal = (CharClass)values.GetValue(nb);
            return randomVal;
        }
        public static HP_ getRandomHPValue()
        {
            var values = HP_.GetValues(typeof(HP_));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            HP_ randomVal = (HP_)values.GetValue(nb);
            return randomVal;
        }
        public static PA_ getRandomPAValue()
        {
            var values = PA_.GetValues(typeof(PA_));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            PA_ randomVal = (PA_)values.GetValue(nb);
            return randomVal;
        }

        public static SkillAvailable getRandomSkillValue()
        {
            var values = SkillAvailable.GetValues(typeof(SkillAvailable));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            SkillAvailable randomVal = (SkillAvailable)values.GetValue(nb);
            return randomVal;
        }

        public static Threat getRandomThreatValue()
        {
            var values = Threat.GetValues(typeof(Threat));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            Threat randomVal = (Threat)values.GetValue(nb);
            return randomVal;
        }

        /**mutate ally & Enemy*/
        public static ClassType getRandomCharTypeValue(byte OldValue)
        {
            var values = ClassType.GetValues(typeof(ClassType));
            System.Random random = new System.Random();

            ClassType randomVal;
            int nb;
            do
            {
                nb = random.Next(0, values.Length);
                randomVal = (ClassType)values.GetValue(nb);
            } while (((byte)randomVal) == OldValue);

            return randomVal;
        }
    }

    public class Classifier1 : Classifier
    {
        public int id;
        public bool modified;
        public byte charClass;
        public byte HP;
        public byte PA;
        public byte skillAvailable;
        public byte threat;
        public byte maxTargets;

        public class InfoChars
        {
            public byte classType; // soigneur / envouteur / autres
            public byte HP;
            public byte threat;
            public byte distance;
            

            public InfoChars(HexaGrid hexaGrid, Character c, bool isWithinAttackRange, bool isWithinSkillRange)
            {
                int charID = 0;
                for (int i = 0; i < hexaGrid.charList.Count; i++)
                {
                    if (hexaGrid.charList[i] == c)
                    {
                        charID = i;
                        break;
                    }
                }
                // ClassType
                switch (c.charClass)
                {
                    case CharClass.GUERRIER:
                    case CharClass.VOLEUR:
                    case CharClass.ARCHER:
                    case CharClass.MAGE: classType = (byte)Classifier1Attributes.ClassType.OTHER; break;
                    case CharClass.SOIGNEUR: classType = (byte)Classifier1Attributes.ClassType.SOIGNEUR; break;
                    case CharClass.ENVOUTEUR: classType = (byte)Classifier1Attributes.ClassType.ENVOUTEUR; break;
                }

                // HP
                float HPprc = ((float)c.HP) / c.HPmax;
                if (HPprc >= 0.75)
                {
                    HP = (byte)Classifier1Attributes.HP_.BETWEEN_100_75;
                }
                else if (HPprc >= 0.4)
                {
                    HP = (byte)Classifier1Attributes.HP_.BETWEEN_74_40;
                }
                else
                {
                    HP = (byte)Classifier1Attributes.HP_.BETWEEN_39_0;
                }

                // Threat
                int threat_int = AIUtil.threatAtHexa(c.x, c.y, charID + 1, charID);
                if (threat_int == -1000)
                {
                    threat = (byte)Classifier1Attributes.Threat.DEATH;
                }
                else if (threat_int >= -3)
                {
                    threat = (byte)Classifier1Attributes.Threat.SAFE;
                }
                else
                {
                    threat = (byte)Classifier1Attributes.Threat.DANGER;
                }

                // Distance
                if (isWithinAttackRange)
                {
                    if (isWithinSkillRange)
                    {
                        distance = (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL;
                    }
                    else
                    {
                        distance = (byte)Classifier1Attributes.Distance.ATTACK;
                    }
                }
                else if (isWithinSkillRange)
                {
                    distance = (byte)Classifier1Attributes.Distance.SKILL;
                }
                else
                {
                    distance = (byte)Classifier1Attributes.Distance.MOVEMENT;
                }

                

            }


            public InfoChars()
            {
                classType = (byte)Classifier1Attributes.ClassType.NONE;
                HP = 1 + 2 + 4;
                threat = 1 + 2 + 4;
                distance = 1 + 2 + 4 + 8;
            }

            public InfoChars(InfoChars i)
            {
                this.classType = i.classType;
                this.HP = i.HP;
                this.threat = i.threat;
                this.distance = i.distance;
            }

            public InfoChars(InfoChars i1, InfoChars i2)
            {
                this.classType = (byte)(i1.classType | i1.classType);
                this.HP = (byte)(i1.HP | i1.HP);
                this.threat = (byte)(i1.threat | i1.threat);
                this.distance = (byte)(i1.distance | i1.distance);
            }

            public bool equals(InfoChars i)
            {
                return ((classType == i.classType) && (HP == i.HP) && (threat == i.threat) && (distance == i.distance));
            }

            public bool isSimilar(InfoChars i)
            {
                return (((classType & i.classType) > 0) && ((HP & i.HP) > 0) && ((threat & i.threat) > 0) && ((distance & i.distance) > 0));
            }

            public int distance_(InfoChars i)
            {
                int dist = 0;
                if ((classType & i.classType) == 0) dist++;
                if ((HP & i.HP) == 0) dist++;
                if ((threat & i.threat) == 0) dist++;
                if ((distance & i.distance) == 0) dist++;
                return dist;
            }
        }

        public List<InfoChars> infoAllies;
        public List<InfoChars> infoEnemies;

        public enum Action : byte { ApproachEnemy, ApproachAlly, Flee, Attack, Skill, Spell };
        public Action action;
        public int merges;
        private object hexaGrid;

        /** Init the Classifier1 from a character's situation. */
        public Classifier1(HexaGrid hexaGrid, int charID)
        {
            Character c = hexaGrid.charList[charID];

            // ClassType
            switch (c.charClass)
            {
                case CharClass.GUERRIER: charClass = (byte)Classifier1Attributes.CharClass.GUERRIER; break;
                case CharClass.VOLEUR: charClass = (byte)Classifier1Attributes.CharClass.VOLEUR; break;
                case CharClass.ARCHER: charClass = (byte)Classifier1Attributes.CharClass.ARCHER; break;
                case CharClass.MAGE: charClass = (byte)Classifier1Attributes.CharClass.MAGE; break;
                case CharClass.SOIGNEUR: charClass = (byte)Classifier1Attributes.CharClass.SOIGNEUR; break;
                case CharClass.ENVOUTEUR: charClass = (byte)Classifier1Attributes.CharClass.ENVOUTEUR; break;
            }

            // HP
            float HPprc = ((float)c.HP) / c.HPmax;
            if (HPprc >= 0.75)
            {
                HP = (byte)Classifier1Attributes.HP_.BETWEEN_100_75;
            }
            else if (HPprc >= 0.4)
            {
                HP = (byte)Classifier1Attributes.HP_.BETWEEN_74_40;
            }
            else
            {
                HP = (byte)Classifier1Attributes.HP_.BETWEEN_39_0;
            }

            // PA
            PA = (c.PA == 1) ? (byte)Classifier1Attributes.PA_.ONE : (byte)Classifier1Attributes.PA_.TWO_OR_MORE;

            // SkillAvailable
            skillAvailable = (c.skillAvailable) ? (byte)Classifier1Attributes.SkillAvailable.YES : (byte)Classifier1Attributes.SkillAvailable.NO;

            // Threat
            int threat_int = AIUtil.threatAtHexa(c.x, c.y, charID + 1, charID);
            if (threat_int == -1000)
            {
                threat = (byte)Classifier1Attributes.Threat.DEATH;
            }
            else if (threat_int >= -3)
            {
                threat = (byte)Classifier1Attributes.Threat.SAFE;
            }
            else
            {
                threat = (byte)Classifier1Attributes.Threat.DANGER;
            }

            // MaxTargets (0-1 for everyone, 0-3+ for mage)
            int nb = AIUtil.getNbMaxTargets(charID);
            if (nb == 0)
            {
                maxTargets = (byte)Classifier1Attributes.MaxTargets.NONE;
            }
            else if (nb == 1)
            {
                maxTargets = (byte)Classifier1Attributes.MaxTargets.ONE;
            }
            else if (nb == 2)
            {
                maxTargets = (byte)Classifier1Attributes.MaxTargets.TWO;
            }
            else
            {
                maxTargets = (byte)Classifier1Attributes.MaxTargets.THREE_OR_MORE;
            }


            infoAllies = new List<InfoChars>();
            infoEnemies = new List<InfoChars>();

            // infoAllies / infoEnemies
            foreach (Character c2 in hexaGrid.charList)
            {
                if (c2 != c)
                {
                    if (c2.team == c.team)
                    { // Ally
                      // I am GUERRIER / VOLEUR / ARCHER / MAGE : 
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER:
                            case CharClass.VOLEUR:
                            case CharClass.ARCHER:
                            case CharClass.MAGE:
                                {
                                    switch (c2.charClass)
                                    {
                                        // ally is GUERRIER / VOLEUR / ARCHER / MAGE : Is Ally able to attack the Enemy that I want to attack ?
                                        case CharClass.GUERRIER:
                                        case CharClass.VOLEUR:
                                        case CharClass.ARCHER:
                                        case CharClass.MAGE:
                                            {
                                                if (maxTargets != (byte)Classifier1Attributes.MaxTargets.NONE)
                                                {
                                                    Character cAttack = AIUtil.findCharToAttack(charID);
                                                    if (cAttack != null)
                                                    {
                                                        bool isWithinAttackRange = AIUtil.canTargetAttack(c2, cAttack);
                                                        bool isWithinSkillRange = AIUtil.canTargetSkill(c2, cAttack);
                                                        bool isWithinSpellRange = AIUtil.canTargetSpell(c2, cAttack);
                                                        if (isWithinAttackRange || isWithinSkillRange || isWithinSpellRange)
                                                        {
                                                            infoAllies.Add(new InfoChars(hexaGrid, c2, isWithinAttackRange, isWithinSkillRange));
                                                        }
                                                        else
                                                        {
                                                            if (AIUtil.canTargetWithMovementAttack(c2, cAttack) || AIUtil.canTargetWithMovementSkill(c2, cAttack) || AIUtil.canTargetWithMovementSpell(c2, cAttack))
                                                            {
                                                                infoAllies.Add(new InfoChars(hexaGrid, c2, false, false));
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            break;
                                        // ally is SOIGNEUR / ENVOUTEUR : Is Ally able to heal/buff me ?
                                        case CharClass.SOIGNEUR:
                                        case CharClass.ENVOUTEUR:
                                            {
                                                bool isWithinAttackRange = AIUtil.canTargetAttack(c2, c);
                                                bool isWithinSkillRange = AIUtil.canTargetSkill(c2, c);
                                                if (isWithinAttackRange || isWithinSkillRange)
                                                {
                                                    infoAllies.Add(new InfoChars(hexaGrid, c2, isWithinAttackRange, isWithinSkillRange));
                                                }
                                                else
                                                {
                                                    if (AIUtil.canTargetWithMovementAttack(c2, c) || AIUtil.canTargetWithMovementSkill(c2, c))
                                                    {
                                                        infoAllies.Add(new InfoChars(hexaGrid, c2, false, false));
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                                break;
                            // I am SOIGNEUR / ENVOUTEUR : Can I heal/buff my ally ?
                            case CharClass.SOIGNEUR:
                            case CharClass.ENVOUTEUR:
                                {
                                    if (c.charClass == CharClass.SOIGNEUR && c2.HP == c2.HPmax)
                                    {
                                    }
                                    else
                                    {
                                        bool isWithinAttackRange = AIUtil.canTargetAttack(c, c2);
                                        bool isWithinSkillRange = AIUtil.canTargetSkill(c, c2);
                                        if (isWithinAttackRange || isWithinSkillRange)
                                        {
                                            infoAllies.Add(new InfoChars(hexaGrid, c2, isWithinAttackRange, isWithinSkillRange));
                                        }
                                        else
                                        {
                                            if (AIUtil.canTargetWithMovementAttack(c, c2) || AIUtil.canTargetWithMovementSkill(c, c2))
                                            {
                                                infoAllies.Add(new InfoChars(hexaGrid, c2, false, false));
                                            }
                                        }
                                    }
                                }
                                break;
                        }

                    }
                    else
                    { // Enemy : Can I reach the Enemy (directly or with movement)
                        switch (c.charClass)
                        {
                            case CharClass.GUERRIER:
                            case CharClass.VOLEUR:
                            case CharClass.ARCHER:
                            case CharClass.MAGE:
                                {
                                    bool isWithinAttackRange = hexaGrid.hexaInSight(c.x, c.y, c2.x, c2.y, c.getClassData().basicAttack.range);
                                    bool isWithinSkillRange = c.skillAvailable && hexaGrid.hexaInSight(c.x, c.y, c2.x, c2.y, c.getClassData().ult.range);
                                    bool isWithinSpellRange = hexaGrid.hexaInSight(c.x, c.y, c2.x, c2.y, c.getClassData().spell.range);

                                    if (isWithinAttackRange || isWithinSkillRange || isWithinSpellRange)
                                    {
                                        infoEnemies.Add(new InfoChars(hexaGrid, c2, isWithinAttackRange, isWithinSkillRange));
                                    }
                                    else
                                    {
                                        if (AIUtil.canTargetWithMovementAttack(c, c2) || AIUtil.canTargetWithMovementAttack(c, c2))
                                        {
                                            infoEnemies.Add(new InfoChars(hexaGrid, c2, false, false));
                                        }
                                    }
                                }
                                break;
                            case CharClass.SOIGNEUR:
                            case CharClass.ENVOUTEUR:
                                {
                                    // don't add anything to the list because it doesn't matter.
                                }
                                break;
                        }
                    }
                }
            }

            // fill allies/Enemies with NONE
            /*for (int i=infoAllies.Count;i<5;i++){
                infoAllies.Add(new InfoChars());
            }
            for (int i=infoEnemies.Count;i<5;i++){
                infoEnemies.Add(new InfoChars());
            }*/

            action = Action.ApproachEnemy;
            useCount = 0;
            fitness = 0.5f;
            id = 0;
            modified = true;
        }

        /** Copies the given Classifier1 c. */
        public Classifier1(Classifier1 c)
        {
            this.charClass = c.charClass;
            this.HP = c.HP;
            this.PA = c.PA;
            this.skillAvailable = c.skillAvailable;
            this.threat = c.threat;
            this.maxTargets = c.maxTargets;

            this.infoAllies = new List<InfoChars>();
            this.infoEnemies = new List<InfoChars>();
            foreach (InfoChars i in c.infoAllies) this.infoAllies.Add(new InfoChars(i));
            foreach (InfoChars i in c.infoEnemies) this.infoEnemies.Add(new InfoChars(i));

            this.action = c.action;
            this.fitness = c.fitness;
            this.useCount = c.useCount;
            this.id = 0;
            this.modified = true;
        }

        /** Creates a Classifier1 by merging two */
        public Classifier1(Classifier1 c1, Classifier1 c2)
        {
            this.charClass = (byte)(c1.charClass | c2.charClass);
            this.HP = (byte)(c1.HP | c2.HP);
            this.PA = (byte)(c1.PA | c2.PA);
            this.skillAvailable = (byte)(c1.skillAvailable | c2.skillAvailable);
            this.threat = (byte)(c1.threat | c2.threat);
            this.maxTargets = (byte)(c1.maxTargets | c2.maxTargets);

            this.infoAllies = new List<InfoChars>();
            this.infoEnemies = new List<InfoChars>();
            for (int i = 0; i < c1.infoAllies.Count; i++)
            {
                this.infoAllies.Add(new InfoChars(c1.infoAllies[i], c2.infoAllies[i]));
            }
            for (int i = 0; i < c1.infoEnemies.Count; i++)
            {
                this.infoEnemies.Add(new InfoChars(c1.infoEnemies[i], c2.infoEnemies[i]));
            }

            this.action = c1.action;
            this.fitness = 0.5f;
            this.useCount = 0;
            this.id = 0;
            this.modified = true;
            this.merges = c1.merges + c2.merges + 1;
        }

        /** Loads a Classifier1 from a file. */
        public Classifier1(BinaryReader reader)
        {
            this.charClass = reader.ReadByte();
            this.HP = reader.ReadByte();
            this.PA = reader.ReadByte();
            this.skillAvailable = reader.ReadByte();
            this.threat = reader.ReadByte();
            this.maxTargets = reader.ReadByte();
            int nbAllies = reader.ReadByte();
            this.infoAllies = new List<InfoChars>();
            for (int i = 0; i < nbAllies; i++)
            {
                InfoChars temp = new InfoChars();
                temp.classType = reader.ReadByte();
                temp.HP = reader.ReadByte();
                temp.threat = reader.ReadByte();
                temp.distance = reader.ReadByte();
                this.infoAllies.Add(temp);
            }
            int nbEnemies = reader.ReadByte();
            this.infoEnemies = new List<InfoChars>();
            for (int i = 0; i < nbEnemies; i++)
            {
                InfoChars temp = new InfoChars();
                temp.classType = reader.ReadByte();
                temp.HP = reader.ReadByte();
                temp.threat = reader.ReadByte();
                temp.distance = reader.ReadByte();
                this.infoEnemies.Add(temp);
            }
            this.action = (Action)reader.ReadByte();
            this.fitness = reader.ReadSingle();
            this.useCount = reader.ReadInt32();
        }

        /** Checks if the Classifier1 is valid in the current situation. */
        public bool isValid(int charID, HexaGrid hexaGrid)
        {
            Character currentChar = hexaGrid.charList[charID];

            if (this.action == Classifier1.Action.Skill)
            {
                if (this.skillAvailable == (byte)Classifier1Attributes.SkillAvailable.NO) return false;
                else if (!this.isInRangeToUseSkill()) return false;
            }
            else if (this.action == Classifier1.Action.Attack)
            {
                if (!this.isInRangeToUseAttack()) return false;
            }
            else if (this.action == Classifier1.Action.Spell)
            {
                if (!this.isInRangeToUseSpell()) return false;
            }
            else if (this.action == Classifier1.Action.ApproachAlly)
            {

                int nbAllies = 0;
                foreach (Character c in hexaGrid.charList) if (c.team == currentChar.team) nbAllies++;
                if (nbAllies <= 1) return false;
                ActionAIPos temp = AIUtil.AIHard.doApproachAlly(charID);
                if (temp.pos == null) return false;
                //if (temp.pos.x == currentChar.x && temp.pos.y == currentChar.y) return false;
            }
            else if (this.action == Classifier1.Action.ApproachEnemy)
            {
                ActionAIPos temp = AIUtil.AIHard.doApproachEnemy(charID);
                if (temp.pos == null) return false;
                //if (temp.pos.x == currentChar.x && temp.pos.y == currentChar.y) return false;
            }
            else if (this.action == Classifier1.Action.Flee)
            {
                ActionAIPos temp = AIUtil.AIHard.doFlee(charID);
                if (temp.pos == null) return false;
                //if (temp.pos.x == currentChar.x && temp.pos.y == currentChar.y) return false;
            }
            return true;
        }

        /** Checks if the Classifier1 is valid in a general situation. */
        public bool isValid()
        {
            if (this.action == Classifier1.Action.Skill)
            {
                if (this.skillAvailable == (byte)Classifier1Attributes.SkillAvailable.NO) return false;
                else if (!this.isInRangeToUseSkill()) return false;
            }
            else if (this.action == Classifier1.Action.Attack)
            {
                if (!this.isInRangeToUseAttack()) return false;
            }
            else if (this.action == Classifier1.Action.Spell)
            {

                if (!this.isInRangeToUseSpell()) return false;
            }
            else if (this.action == Classifier1.Action.ApproachAlly)
            {
            }
            else if (this.action == Classifier1.Action.ApproachEnemy)
            {
            }
            else if (this.action == Classifier1.Action.Flee)
            {
            }
            return true;
        }

        public bool isInRangeToUseAttack()
        {
            if ((charClass & (byte)Classifier1Attributes.CharClass.GUERRIER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            if ((charClass & (byte)Classifier1Attributes.CharClass.VOLEUR) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            if ((charClass & (byte)Classifier1Attributes.CharClass.ARCHER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            if ((charClass & (byte)Classifier1Attributes.CharClass.MAGE) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            if ((charClass & (byte)Classifier1Attributes.CharClass.SOIGNEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            if ((charClass & (byte)Classifier1Attributes.CharClass.ENVOUTEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.ATTACK || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            return false;
        }

        public bool isInRangeToUseSkill()
        {
            if (skillAvailable == (byte)Classifier1Attributes.SkillAvailable.YES)
            {
                if ((charClass & (byte)Classifier1Attributes.CharClass.GUERRIER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.VOLEUR) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.ARCHER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.MAGE) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.SOIGNEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.ENVOUTEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SKILL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            }
            return false;
        }
        public bool isInRangeToUseSpell()

        {

            if (PA > (byte)Classifier1Attributes.PA_.ONE)
            {
                if ((charClass & (byte)Classifier1Attributes.CharClass.GUERRIER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.VOLEUR) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.ARCHER) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.MAGE) > 0) foreach (InfoChars ic in infoEnemies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.SOIGNEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
                if ((charClass & (byte)Classifier1Attributes.CharClass.ENVOUTEUR) > 0) foreach (InfoChars ic in infoAllies) if (ic.classType != (byte)Classifier1Attributes.ClassType.NONE && (ic.distance == (byte)Classifier1Attributes.Distance.SPELL || ic.distance == (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL)) return true;
            }
            return false;

        }

        public bool alliesEquals(Classifier1 c)
        {
            if (infoAllies.Count == c.infoAllies.Count)
            {
                for (int i = 0; i < infoAllies.Count; i++) if (!(c.infoAllies[i].equals(infoAllies[i]))) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EnemiesEquals(Classifier1 c)
        {
            if (infoEnemies.Count == c.infoEnemies.Count)
            {
                for (int i = 0; i < infoEnemies.Count; i++) if (!(c.infoEnemies[i].equals(infoEnemies[i]))) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool alliesSimilar(Classifier1 c)
        {
            if (infoAllies.Count == c.infoAllies.Count)
            {
                for (int i = 0; i < infoAllies.Count; i++) if (!(c.infoAllies[i].isSimilar(infoAllies[i]))) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool EnemiesSimilar(Classifier1 c)
        {
            if (infoEnemies.Count == c.infoEnemies.Count)
            {
                for (int i = 0; i < infoEnemies.Count; i++) if (!(c.infoEnemies[i].isSimilar(infoEnemies[i]))) return false;
                return true;
            }
            else
            {
                return false;
            }
        }

        public int alliesDistance(Classifier1 c)
        {
            int dist = 0;
            for (int i = 0; i < infoAllies.Count; i++)
            {
                dist += c.infoAllies[i].distance_(infoAllies[i]);
            }
            return dist;
        }

        public int EnemiesDistance(Classifier1 c)
        {
            int dist = 0;
            for (int i = 0; i < infoEnemies.Count; i++)
            {
                dist += c.infoEnemies[i].distance_(infoEnemies[i]);
            }
            return dist;
        }

        public override bool equals(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return ((charClass == c.charClass) &&
            (HP == c.HP) &&
            (PA == c.PA) &&
            (PA == c.PA) &&
            (skillAvailable == c.skillAvailable) &&
            (threat == c.threat) &&
            (maxTargets == c.maxTargets) &&
            (alliesEquals(c)) &&
            (EnemiesEquals(c)) &&
            (action == c.action));
        }

        public override bool isSimilar(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return (((charClass & c.charClass) > 0) &&
            ((HP & c.HP) > 0) &&
            ((PA & c.PA) > 0) &&
            ((PA & c.PA) > 0) &&
            ((skillAvailable & c.skillAvailable) > 0) &&
            ((threat & c.threat) > 0) &&
            ((maxTargets & c.maxTargets) > 0) &&
            (alliesSimilar(c)) &&
            (EnemiesSimilar(c)) &&
            (action == c.action));
        }

        // These don't check for action match (Used to matching situations)
        public override bool equals2(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return ((charClass == c.charClass) &&
            (HP == c.HP) &&
            (PA == c.PA) &&
            (PA == c.PA) &&
            (skillAvailable == c.skillAvailable) &&
            (threat == c.threat) &&
            (maxTargets == c.maxTargets) &&
            (alliesEquals(c)) &&
            (EnemiesEquals(c)));
        }

        public override bool isSimilar2(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return (((charClass & c.charClass) > 0) &&
            ((HP & c.HP) > 0) &&
            ((PA & c.PA) > 0) &&
            ((PA & c.PA) > 0) &&
            ((skillAvailable & c.skillAvailable) > 0) &&
            ((threat & c.threat) > 0) &&
            ((maxTargets & c.maxTargets) > 0) &&
            (alliesSimilar(c)) &&
            (EnemiesSimilar(c)));
        }

        public override bool sameAction(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return (this.action == c.action);
        }

        public override int distance(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            if (this.infoAllies.Count != c.infoAllies.Count) return 10000;
            if (this.infoEnemies.Count != c.infoEnemies.Count) return 10000;

            int dist = 0;
            // !!!! use -> Misc.MiscBinary.nbDifferentBits() !!!!! (TO DO)
            if ((charClass & c.charClass) == 0) dist++;
            if ((HP & c.HP) == 0) dist++;
            if ((PA & c.PA) == 0) dist++;
            if ((PA & c.PA) == 0) dist++;
            if ((skillAvailable & c.skillAvailable) == 0) dist++;
            if ((threat & c.threat) == 0) dist++;
            if ((maxTargets & c.maxTargets) == 0) dist++;
            dist += alliesDistance(c);
            dist += EnemiesDistance(c);
            return dist;
        }

        /** Writes the Classifier1 in binary */
        public override void saveInBinary(BinaryWriter writer)
        {
            writer.Write(this.charClass);
            writer.Write(this.HP);
            writer.Write(this.PA);
            writer.Write(this.skillAvailable);
            writer.Write(this.threat);
            writer.Write(this.maxTargets);
            writer.Write((byte)this.infoAllies.Count);
            foreach (Classifier1.InfoChars ic in this.infoAllies)
            {
                writer.Write(ic.classType);
                writer.Write(ic.HP);
                writer.Write(ic.threat);
                writer.Write(ic.distance);
            }
            writer.Write((byte)this.infoEnemies.Count);
            foreach (Classifier1.InfoChars ic in this.infoEnemies)
            {
                writer.Write(ic.classType);
                writer.Write(ic.HP);
                writer.Write(ic.threat);
                writer.Write(ic.distance);
            }
            writer.Write((byte)this.action);
            writer.Write(this.fitness);
            writer.Write(this.useCount);
        }

        public override void saveInDB()
        {
            if (File.Exists(Application.dataPath + "/Data/ClassifierHardIA.s3db"))
            {
                string conn = "Data Source=" + Application.dataPath + "/Data/ClassifierHardIA.s3db ; Version=3;"; //Path to database.
                IDbConnection dbconn;
                dbconn = (IDbConnection)new SqliteConnection(conn);
                dbconn.Open(); //Open connection to the database.
                string situation = this.charClass + "/" + this.HP + "/" + this.PA + "/" + this.skillAvailable + "/" + this.threat + "/" + this.maxTargets;
                string AI = "";
                foreach (Classifier1.InfoChars ic in this.infoAllies)
                {
                    AI += ic.classType + "/" + ic.HP + "/" + ic.threat + "/" + ic.distance + "/";
                }
                string EI = "";
                foreach (Classifier1.InfoChars ic in this.infoEnemies)
                {
                    EI += ic.classType + "/" + ic.HP + "/" + ic.threat + "/" + ic.distance + "/";
                }
                string sqlQuery = "INSERT INTO Rules (SITUATION, AI, EI, ACTION, FITNESS, USES) VALUES ('" + situation + "', '" + AI + "', '" + EI + "','" + (byte)this.action + "'," + (int)(this.fitness*10000) + "," + this.useCount + ")";
                IDbCommand dbcmd = dbconn.CreateCommand();
                dbcmd.CommandText = sqlQuery;
                dbcmd.ExecuteNonQuery();
                dbconn.Close();
                dbconn = null;
            }
        }


        public override void loadInBinary(BinaryReader reader)
        {
            this.charClass = reader.ReadByte();
            this.HP = reader.ReadByte();
            this.PA = reader.ReadByte();
            this.skillAvailable = reader.ReadByte();
            this.threat = reader.ReadByte();
            this.maxTargets = reader.ReadByte();
            int nbAllies = reader.ReadByte();
            this.infoAllies = new List<InfoChars>();
            for (int i = 0; i < nbAllies; i++)
            {
                InfoChars temp = new InfoChars();
                temp.classType = reader.ReadByte();
                temp.HP = reader.ReadByte();
                temp.threat = reader.ReadByte();
                temp.distance = reader.ReadByte();
                this.infoAllies.Add(temp);
            }
            int nbEnemies = reader.ReadByte();
            this.infoEnemies = new List<InfoChars>();
            for (int i = 0; i < nbEnemies; i++)
            {
                InfoChars temp = new InfoChars();
                temp.classType = reader.ReadByte();
                temp.HP = reader.ReadByte();
                temp.threat = reader.ReadByte();
                temp.distance = reader.ReadByte();
                this.infoEnemies.Add(temp);
            }
            this.action = (Action)reader.ReadByte();
            this.fitness = reader.ReadSingle();
            this.useCount = reader.ReadInt32();
        }

        public override string getStringInfo()
        {
            string strDisp = "Char class : ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.GUERRIER) > 0) strDisp += "GUERRIER ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.VOLEUR) > 0) strDisp += "VOLEUR ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.ARCHER) > 0) strDisp += "ARCHER ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.MAGE) > 0) strDisp += "MAGE ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.SOIGNEUR) > 0) strDisp += "SOIGNEUR ";
            if ((charClass & (byte)Classifier1Attributes.CharClass.ENVOUTEUR) > 0) strDisp += "ENVOUTEUR ";

            strDisp += "| HP : ";
            if ((HP & (byte)Classifier1Attributes.HP_.BETWEEN_100_75) > 0) strDisp += "100-75% ";
            if ((HP & (byte)Classifier1Attributes.HP_.BETWEEN_74_40) > 0) strDisp += "74-40% ";
            if ((HP & (byte)Classifier1Attributes.HP_.BETWEEN_39_0) > 0) strDisp += "39-0% ";

            strDisp += "| PA : ";
            if ((PA & (byte)Classifier1Attributes.PA_.ONE) > 0) strDisp += "ONE ";
            if ((PA & (byte)Classifier1Attributes.PA_.TWO_OR_MORE) > 0) strDisp += "TWO+ ";

            strDisp += "| Skill : ";
            if ((skillAvailable & (byte)Classifier1Attributes.SkillAvailable.YES) > 0) strDisp += "YES ";
            if ((skillAvailable & (byte)Classifier1Attributes.SkillAvailable.NO) > 0) strDisp += "NO ";

            strDisp += "| Threat : ";
            if ((threat & (byte)Classifier1Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
            if ((threat & (byte)Classifier1Attributes.Threat.DANGER) > 0) strDisp += "DANGER ";
            if ((threat & (byte)Classifier1Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";

            strDisp += "| Max Targets : ";
            if ((maxTargets & (byte)Classifier1Attributes.MaxTargets.NONE) > 0) strDisp += "NONE ";
            if ((maxTargets & (byte)Classifier1Attributes.MaxTargets.ONE) > 0) strDisp += "ONE ";
            if ((maxTargets & (byte)Classifier1Attributes.MaxTargets.TWO) > 0) strDisp += "TWO ";
            if ((maxTargets & (byte)Classifier1Attributes.MaxTargets.THREE_OR_MORE) > 0) strDisp += "THREE+ ";

            if (infoAllies.Count > 0) strDisp += "\nAllies :";
            foreach (InfoChars i in infoAllies)
            {
                strDisp += "\nClass Type : ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.SOIGNEUR) > 0) strDisp += "SOIGNEUR ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.ENVOUTEUR) > 0) strDisp += "ENVOUTEUR ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.OTHER) > 0) strDisp += "OTHER ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.NONE) > 0) strDisp += "NONE ";
                strDisp += "| HP : ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_100_75) > 0) strDisp += "100-75% ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_74_40) > 0) strDisp += "74-40% ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_39_0) > 0) strDisp += "39-0% ";

                strDisp += "| Threat : ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.DANGER) > 0) strDisp += "DANGER ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";

                strDisp += "| Distance : ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.ATTACK) > 0) strDisp += "ATTACK ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.SKILL) > 0) strDisp += "SKILL ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL) > 0) strDisp += "ATTACK/SKILL ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.MOVEMENT) > 0) strDisp += "MOVEMENT ";
            }
            if (infoEnemies.Count > 0) strDisp += "\nEnemies :";
            foreach (InfoChars i in infoEnemies)
            {
                strDisp += "\nClass Type : ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.SOIGNEUR) > 0) strDisp += "SOIGNEUR ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.ENVOUTEUR) > 0) strDisp += "ENVOUTEUR ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.OTHER) > 0) strDisp += "(GUERRIER/VOLEUR/ARCHER/MAGE) ";
                if ((i.classType & (byte)Classifier1Attributes.ClassType.NONE) > 0) strDisp += "NONE ";
                strDisp += "| HP : ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_100_75) > 0) strDisp += "100-75% ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_74_40) > 0) strDisp += "74-40% ";
                if ((i.HP & (byte)Classifier1Attributes.HP_.BETWEEN_39_0) > 0) strDisp += "39-0% ";

                strDisp += "| Threat : ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.SAFE) > 0) strDisp += "SAFE ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.DANGER) > 0) strDisp += "DANGER ";
                if ((i.threat & (byte)Classifier1Attributes.Threat.DEATH) > 0) strDisp += "DEATH ";

                strDisp += "| Distance : ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.ATTACK) > 0) strDisp += "ATTACK ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.SKILL) > 0) strDisp += "SKILL ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.ATTACK_SPELL_AND_SKILL) > 0) strDisp += "ATTACK/SKILL ";
                if ((i.distance & (byte)Classifier1Attributes.Distance.MOVEMENT) > 0) strDisp += "MOVEMENT ";
            }

            strDisp += "\nAction : " + action + " | Fitness : " + fitness + " | Use count : " + useCount;

            return strDisp;
        }

        public override Classifier copy()
        {
            return new Classifier1(this);
        }

        public override Classifier merge(Classifier c1)
        {
            Classifier1 c = (Classifier1)c1;
            return new Classifier1(this, c);
        }

        /** used for mutations  */
        public static Action getRandomAction()
        {
            var values = Action.GetValues(typeof(Action));
            System.Random random = new System.Random();
            int nb = random.Next(0, values.Length);
            Action randomVal = (Action)values.GetValue(nb);
            return randomVal;
        }

        public override Classifier mutate(float likelihood, int maxAttempts)
        {
            for (int i = 0; i < maxAttempts; i++)
            {
                Classifier1 c = new Classifier1(this);
                if (Random.Range(0.0f, 1.0f) <= likelihood) c.charClass = (byte)Classifier1Attributes.getRandomCharClassValue();
                if (Random.Range(0.0f, 1.0f) <= likelihood) c.HP = (byte)Classifier1Attributes.getRandomHPValue();
                if (Random.Range(0.0f, 1.0f) <= likelihood) c.PA = (byte)Classifier1Attributes.getRandomPAValue();
                if (Random.Range(0.0f, 1.0f) <= likelihood) c.action = Classifier1.getRandomAction();
                if (Random.Range(0.0f, 1.0f) <= likelihood) c.threat = (byte)Classifier1Attributes.getRandomThreatValue();

                if (c.isValid())
                {
                    c.fitness = 0.5f;
                    c.useCount = 0;
                    return c;
                }
            }
            return null;
        }

    }
}