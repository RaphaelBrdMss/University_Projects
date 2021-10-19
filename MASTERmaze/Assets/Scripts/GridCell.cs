using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

/*
 * classe coeur du projet 
 * 
 * elle permet d'instancier un labyrinthe et ses cellules sur unity
 * 
 * cellgo est une liste d'objet 3D représentant les cellules dans le jeu elle est rempli grace a unity
 * 
 * elle chosit un labyrithe alléatoirement dans la dificulté que le joeur a choisit puis l'instancie pour créer le plateau de jeu 
 * 
 * 
 */
public class GridCell : MonoBehaviour
{

    public static int diff=0; //variable qui définit la difficulté choisit par le joueur
    [SerializeField]
    public List<GameObject> cellgo; //liste des objet 3D unity représentant les cellules 
    public GameObject fin; //cellule special pour la fin
    public GameObject debut; //cellule special pour le début
    public static int getrand=1; 
    System.Random r = new System.Random();
    public static float timer =0; //variable qui calcule le temps en jeu



    // Use this for initialization
    void Start()
    {
        diff = MainMenu.difficulte;

        
        //génere toute les possibilité de mur et les stock dans une liste
        List<List<int>> allWall = new List<List<int>>();
        for (int i = 0; i < 2; i++)
            for (int j = 0; j < 2; j++)
                for (int k = 0; k < 2; k++)
                    for (int l = 0; l < 2; l++)
                    {
                        List<int> wall = new List<int>();
                        wall.Add(i);
                        wall.Add(j);
                        wall.Add(k);
                        wall.Add(l);
                        allWall.Add(wall);
                    }



        string folder = Path.GetFullPath("laby");

        string[] difDir = Directory.GetDirectories(folder);
        
        string[] facilelaby = Directory.GetFiles(difDir[1]);
        string[] moyenlaby = Directory.GetFiles(difDir[2]);
        string[] durlaby = Directory.GetFiles(difDir[0]);
        int init = r.Next(1, facilelaby.Length - 1);
        
        string file = facilelaby[init];

        while(file.Contains(".DS_Store"))
        {
            init = r.Next(0, facilelaby.Length);
            file = facilelaby[init];

        }
        
        /*-----------------------------------------------------------------
         * -------choix du labyrinthe en fonction de sa difficulté---------
         * ----------------------------------------------------------------*/
        if (Finish.re)
        {
            if (diff == 0)
            {
                    file = facilelaby[Finish.numLaby];
                
            }
            else if (diff == 1)
            {
                    file = moyenlaby[Finish.numLaby];

            }
            else if (diff == 2)
            {
                    file = durlaby[Finish.numLaby];
                
            }
        }
        else
        {
            int rand = r.Next(1, facilelaby.Length);
            
            if (diff == 0)
            {

                file = facilelaby[rand];
                while (file.Contains(".DS_Store"))
                {
                    rand = r.Next(1, facilelaby.Length);
                    file = facilelaby[init];

                }

            }
            else if (diff == 1)
            {

                file = moyenlaby[rand];
                while (file.Contains(".DS_Store"))
                {
                    rand = r.Next(1, facilelaby.Length);
                    file = moyenlaby[init];

                }

            }
            else if (diff == 2)
            {

                file = durlaby[rand];
                while (file.Contains(".DS_Store"))
                {
                    rand = r.Next(1, facilelaby.Length);
                    file = durlaby[init];

                }

            }
            getrand = rand;
        }




        /*--------------------------------------------------------------------------------------
         * -------Création de la liste de cellule qui compose le labyrinthe séléctionné---------
         * -------------------------------------------------------------------------------------*/
        Parser p = new Parser(file);
        List<Cell> listcell = new List<Cell>();
        int cpt = 0;
        for (int i = 0; i < p.listcoord.Count; i += 2)
        {
            Cell c = new Cell(p.listcoord[i], p.listcoord[i + 1]);
            listcell.Add(c);
        }

        for (int i = 0; i < p.wallcell.Count; i += 4)
        {
            List<int> temp = new List<int> { p.wallcell[i], p.wallcell[i + 1], p.wallcell[i + 2], p.wallcell[i + 3] };
            listcell[cpt].setwall(temp);
            cpt++;
        }

        /*---------------------------------------------------------------------------------------------------------
         * -------Instantiation des cellules en fonction de leur coordonnée et de leur mur qui les entours---------
         * --------------------------------------------------------------------------------------------------------*/

        foreach (Cell c in listcell)
        {
            int x = c.x * 2;
            int y = c.y * 2;
            if (c.x == listcell[listcell.Count-1].x && c.y == listcell[listcell.Count - 1].y)
            {
              
                Instantiate(fin, new Vector3(y, 0, x), Quaternion.identity);
                
            }
            if (c.x == listcell[0].x && c.y == listcell[0].y)
            {

                Instantiate(debut, new Vector3(y, 0, x), Quaternion.identity);

            }



            if (c.wall.SequenceEqual(allWall[0]))
                Instantiate(cellgo[0], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[1]))
                Instantiate(cellgo[1], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[2]))
                Instantiate(cellgo[2], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[3]))
                Instantiate(cellgo[3], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[4]))
                Instantiate(cellgo[4], new Vector3(y, 0, x), Quaternion.identity);
   
            else if (c.wall.SequenceEqual(allWall[5]))
                Instantiate(cellgo[5], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[6]))
                Instantiate(cellgo[6], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[7]))
                Instantiate(cellgo[7], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[8]))
                Instantiate(cellgo[8], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[9]))
                Instantiate(cellgo[9], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[10]))
                Instantiate(cellgo[10], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[11]))
                Instantiate(cellgo[11], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[12]))
                Instantiate(cellgo[12], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[13]))
                Instantiate(cellgo[13], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[14]))
                Instantiate(cellgo[14], new Vector3(y, 0, x), Quaternion.identity);

            else if (c.wall.SequenceEqual(allWall[15]))
                Instantiate(cellgo[15], new Vector3(y, 0, x), Quaternion.identity);
                    
        }

    }

        // ici updtate compte le temps passé
        void Update()
        {
            timer += Time.deltaTime;
        
        }

   
}
