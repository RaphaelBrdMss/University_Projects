
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
/* 
 * classe qui va permettre de lire un fichier texte pour le transormer en une série de chiffre qui sera utilisé pour créer les labyrinthes
 */
public class Parser
{

    public List<int> listcoord = new List<int>();
    public List<int> wallcell = new List<int>();
    public List<int> coordSol = new List<int>();
   
    public Parser(string file)
    {


        int rest = 0;


        string[] lines = File.ReadAllLines(file);

        List<string> listcoordS = new List<string>();
        List<string> listwallS = new List<string>();
        List<string> listsol = new List<string>();

        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i] == "solution")
            {
                break;
            }

            if (i % 2 == 0)
            {
                listcoordS.Add(lines[i].Trim('[', ',', ']', ' ').Replace(", ", ","));



            }

            if (i % 2 == 1)
            {
                listwallS.Add(lines[i].Trim('[', ',', ']', ' ').Replace(", ", ","));

            }

            rest += i;

        }
        for (int i = rest + 1; i < lines.Length; i++)
        {
            listsol.Add(lines[i].Trim('[', ',', ']', ' ').Replace(", ", ","));
        }

        for (int i = 0; i < listcoordS.Count; i++)
        {

            foreach (var s in listcoordS[i].Split(','))
            {
                int num;
                if (int.TryParse(s, out num)) this.listcoord.Add(num);


            }


        }

        for (int i = 0; i < listwallS.Count; i++)
        {

            foreach (var s in listwallS[i].Split(','))
            {
                int num;
                if (int.TryParse(s, out num)) this.wallcell.Add(num);


            }
        }
        for (int i = 0; i < listsol.Count; i++)
        {

            foreach (var s in listsol[i].Split(','))
            {
                int num;
                if (int.TryParse(s, out num)) this.coordSol.Add(num);


            }





        }

    }
}
