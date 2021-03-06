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

        ArrayList<Cell> waterCells = new ArrayList<>();
        for(Cell c : CellsList){
            if(c.type == GroundType.WATER){
                waterCells.add(c);

            }
        }
        // Generation V2 roseau fully functional
        for(int i=0 ; i<lakeSize ; i++){

            if(CellCheck(waterCells.get(i).x-1,waterCells.get(i).y))
                getCell(waterCells.get(i).x - 1, waterCells.get(i).y).setType(GroundType.ROSEAU);

            if(CellCheck(waterCells.get((lakeSize*lakeSize-1)-i).x+1,waterCells.get((lakeSize*lakeSize-1)-i).y))

                getCell(waterCells.get((lakeSize*lakeSize-1)-i).x+1,waterCells.get((lakeSize*lakeSize-1)-i).y).setType(GroundType.ROSEAU);


            for(int j= 0; j<lakeSize; j++){

                if(getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y+1).type == GroundType.GROUND
                        && getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y+1).x ==
                        getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y).x)
                {

                    getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y+1).setType(GroundType.ROSEAU);
                }


                if(getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y-1).type == GroundType.GROUND
                        && getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y-1).x ==
                        getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y).x)
                {

                    getCell(waterCells.get(i*lakeSize+j).x,waterCells.get(i*lakeSize+j).y-1).setType(GroundType.ROSEAU);
                }
            }

        }



    }
    public Cell getCell(int x, int y){
        return CellsList.get(x*this.size+y);


    }

    //check if the cell is on the list
    public Boolean CellCheck(int x,int y){
        return x * this.size + y <= CellsList.size() && x * this.size + y >= 0;
    }

    public ArrayList<Cell> getFov(Cell pos,int fovSize){
        ArrayList<Cell>  fov = new ArrayList<>();
        for (int kx=1-fovSize; kx < fovSize; kx++)
        {
            for (int ky=1-fovSize; ky < fovSize; ky++)
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







}
