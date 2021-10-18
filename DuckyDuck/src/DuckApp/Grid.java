package DuckApp;

import java.util.ArrayList;

public class Grid {
    public ArrayList<Cell> CellsList;
    public int size;
    public int lakeSize;

    public  Grid(int size, int lakeSize){
        this.size = size;
        this.lakeSize = lakeSize;
        CellsList = new ArrayList<Cell>();
        int randX = (int) (Math.random()*((size -lakeSize)+1));
        int randY = (int) (Math.random()*((size -lakeSize)+1));


        for(int i = 0 ; i< size; i++){
            for(int j = 0 ; j< size; j++){
                Cell c = new Cell(i,j,GroundType.GROUND);
                CellsList.add(i*size+j,c);
            }
        }

        for(int i =0 ; i<lakeSize ; i++){
            for(int j =0 ; j<lakeSize ; j++){
                CellsList.get((randX+i)*size+(randY+j)).setType(GroundType.WATER);
            }
        }
    }
    public Cell getCell(int x, int y){
        return CellsList.get(x*this.size+y);


    }

    public ArrayList<Cell> getFov(Cell pos){
        ArrayList<Cell>  fov = new ArrayList<>();
        for (int kx=-1; kx < 2; kx++)
        {
            for (int ky=-1; ky < 2; ky++)
            {
                int xk = pos.x + kx;
                int yk = pos.y + ky;

                // pos.x pos.y not included
                if ((kx == 0 && ky ==0) || xk <0 || yk <0 || xk >=size || yk >=size ) continue;

                fov.add(getCell(xk, yk));
            }

        }
        return fov;
    }




    /*public static void main(String[] args){
        DuckApp.Grid c = new DuckApp.Grid(10,3);
        for(int i = 0; i< c.size* c.size;i++){
            System.out.println( c.CellsList.get(i).x +","+ c.CellsList.get(i).y +" : Type :"+ c.CellsList.get(i).type );

        }
    }*/


}
