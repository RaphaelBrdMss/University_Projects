using System;
using System.Collections.Generic;

/*
 * Objet cellule qui va permettre d'instancier le labyrinthe dans unity
 * 
 * l'objet contient des coordonée x et y et une list de int qui définit ses cloison (mur)
 */

public class Cell
{


    public int x, y;
    public List<int> wall;
    public Cell(int x,int y, List<int> wall)
    {
        this.x = x;
        this.y = y;
        this.wall = wall;
    }
    public Cell(int x, int y)
    {
        this.x = x;
        this.y = y;
        this.wall = null;
        
    }
    public int getx()
    {
        return x;
    }
    public int gety()
    {
        return y;
    }
    public List<int> getwall()
    {
        return wall;
    }
    public void setwall(List<int> newwall)
    {
        this.wall = newwall;
    }
}
