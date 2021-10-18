package DuckApp;

import java.util.ArrayList;

public class Ducky {

     public Cell pos;
     public Cell posPrev;
     public int gridSize;
     public ArrayList<Cell> fov;

     public int estomac = 100; // 100 = plein, 0=dead
     public boolean alive = true;


     public Ducky(int sizeGrid, Grid g ) {
         this.gridSize = g.size;
         fov = new ArrayList<Cell>();

         int randX = (int) (Math.random() * sizeGrid);
         int randY = (int) (Math.random() * sizeGrid);

         while (g.getCell(randX, randY).type != GroundType.GROUND) {
             randX = (int) (Math.random() * sizeGrid);
             randY = (int) (Math.random() * sizeGrid);

         }

         //spawn
         this.pos = g.getCell(randX, randY);

         // def du fov 3x3
         fov = g.getFov(pos);


     }


     public void RandomWalk(){

         int x,y;
         x= pos.x;
         y= pos.y;
         int c = 0;
         do{

             int num = (int)(Math.random()*4);


                if(num ==0){

                    x++;
                }
                else if (num ==1){
                    x--;
                }
                else if (num ==2){
                    y--;

                }
                else if (num ==3){
                    y++;

                }
                c++;
                if(c>10){
                    x= pos.x;
                    y= pos.y;
                    break;
                }


            }while(x < 0 || x >=  gridSize || y < 0 || y >=gridSize);
         pos.x = x;
         pos.y = y;


     }


     public void move(){
         //on ne met jamais a jour la pos.type donc il voit pas d'eau ou dans certain cas pas encore trouvés.
         if(pos.type == GroundType.WATER){
             System.out.println("plifplouf");

         }

         else if(isWaterInFov(this.fov)){
             System.out.println("je veux l'eau svp");
             waterTarget();
         }

         else{
             System.out.println("rw");
             RandomWalk();
         }
         // if not eating
         if (estomac > 0)
            estomac--;
     }

    // Walk to Water
     public void waterTarget(){

         int xwater = 0;
         int ywater = 0;
         // get nearest water
         for(Cell c : this.fov) {
             if(c.type == GroundType.WATER){
                 xwater = c.x;
                 ywater = c.y;
                 System.out.println("Nearest Water : " + xwater +","+ ywater);
                 break;
             }

         }
         System.out.println("Ducky Pos  " + pos.x +","+ pos.y);
         // set best path
         float dx = this.pos.x - xwater;
         float dy = this.pos.y - ywater;

         if (dx < 0 )
         {
             this.pos.x++;
         }
         else if (dx > 0)
         {
             this.pos.x--;
         }

         if (dy < 0 )
         {
             this.pos.y++;
         }
         else if (dy > 0)
         {
             this.pos.y--;
         }




     }

     //Random swim if there is no fish in lake(s)
     public void randomSwim(){}
     //hunt if there is/are fish in the lake
     public void swimAndHunt(){}

     void setFov(ArrayList<Cell> fov){
         this.fov =fov;
     }

     public Boolean isWaterInFov(ArrayList<Cell> fov){

         for(Cell c : fov){
             if(c.type == GroundType.WATER){
                 return true;
             }
         }
         return false;
     }

}
