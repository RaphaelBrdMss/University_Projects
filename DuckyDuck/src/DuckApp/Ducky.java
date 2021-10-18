package DuckApp;

import java.util.ArrayList;

public class Ducky {

     public Cell pos;
     public Cell posPrev;
     public int gridSize;
     public ArrayList<Cell> fov;

     public int estomac = 100; // 100 = plein, 0=dead
     public StateHero m_state;


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

         // state
         m_state = StateHero.RANDOM;


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


    // update Method
    public void Update(){

        switch (this.m_state)
        {
            case RANDOM:
                System.out.println("[STATE] RW");
                RandomWalk();
                decreasedEstomac();
                // state future state
                if(isWaterInFov(this.fov))
                {
                    System.out.println("je veux l'eau svp");
                    m_state = StateHero.WALK;
                }
                break;
            case WALK:
                System.out.println("[STATE] Walk to Water");
                waterTarget();
                decreasedEstomac();
                break;
            case SWIM:
                System.out.println("[STATE] RSWIM");
                randomSwim();
                decreasedEstomac();
                break;
            case SWIMHUNT:
                System.out.println("[STATE] SwimAndHunt");
                swimAndHunt();
                decreasedEstomac();
                break;
            case HUNT:
                System.out.println("[STATE] Hunt");
                Hunt();
                decreasedEstomac();
                break;
            case EATING:
                System.out.println("[STATE] Eat");
                Eat();
                break;
            case DEAD:
                System.out.println("[STATE] DEAD !! ");
                // Is dead but we still want to move him
                m_state = StateHero.RANDOM;
                break;
            default:
                decreasedEstomac();
                break;
        }



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

     }

    // Walk to Water
     public void waterTarget(){

         int xwater = 0;
         int ywater = 0;
         int dxy = Integer.MAX_VALUE;
         // get nearest water
         for(Cell c : this.fov) {
             if(c.type == GroundType.WATER){
                 if ((Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y)) < dxy )
                 {
                     xwater = c.x;
                     ywater = c.y;
                     dxy = Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y);
                 }
             }

         }

         System.out.println("Nearest Water : " + xwater +","+ ywater);
         System.out.println("Ducky Pos  " + pos.x +","+ pos.y);

         // set movement toward cell
         float dx = this.pos.x - xwater;
         float dy = this.pos.y - ywater;

         if (dx < 0 ){
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

         // if we land on target next turn, we need to swim STATE
        if ((this.pos.x - xwater) == 0 && (this.pos.y - ywater) == 0)
        {
            this.m_state = StateHero.SWIM;
        }


     }

     //Random swim if there is no fish in lake(s)
     public void randomSwim(){}
     //hunt if there is/are fish in the lake
     public void swimAndHunt(){}
    // hunt on ground
    public void Hunt(){}

    public void decreasedEstomac()
    {
        // if not eating
        if (estomac > 0)
        {
            estomac--;
        }
        else
        {
            m_state = StateHero.DEAD;
        }
    }
    // eat : +50
    public void Eat(){
         if (estomac > 50)
         {
             estomac = 100;
         }
         else
         {
             estomac+=50;
         }

    }

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
