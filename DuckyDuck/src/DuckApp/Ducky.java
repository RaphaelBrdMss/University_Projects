package DuckApp;

import java.util.ArrayList;

public class Ducky {

     public Cell pos;
     public Cell posPrev;
     public Cell targetFood;
     public int gridSize;
     public ArrayList<Cell> fov;

     public int estomac = 100; // 100 = plein, 0=dead
     public StateHero m_state;
     private boolean reachedWater = false;
     public boolean inWater = false;


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

         // init target food
         targetFood = new Cell(-1,-1);
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
                reachedWater = false;
                inWater = false;
                break;
            case WALK:
                System.out.println("[STATE] Walk to Water");
                waterTarget();
                decreasedEstomac();
                // if water is reached we change the future state
                if (reachedWater)
                {
                    m_state = StateHero.SWIM;
                }
                break;
            case SWIM:
                System.out.println("[STATE] RSWIM");
                randomSwim();
                decreasedEstomac();
                inWater = true;
                // if valid food cell
                if (validFoodCell())
                {
                    m_state = StateHero.SWIMHUNT;
                }
                break;
            case SWIMHUNT:
                System.out.println("[STATE] SwimAndHunt");
                swimAndHunt();
                decreasedEstomac();
                inWater = true;
                if ((this.pos.x == targetFood.x) && (this.pos.y == targetFood.y))
                {
                    m_state = StateHero.EATING;
                }
                break;
            case HUNT:
                System.out.println("[STATE] Hunt");
                Hunt();
                decreasedEstomac();
                reachedWater = false;
                break;
            case EATING:
                System.out.println("[STATE] Eat");
                Eat();
                break;
            case DEAD:
                System.out.println("[STATE] DEAD !! But Revival");
                // Is dead but we still want to move him
                m_state = StateHero.RANDOM;
                break;
            default:
                decreasedEstomac();
                inWater = false;
                break;
        }

        // DEAD overall
        /*if (estomac <= 0)
        {
            m_state = StateHero.DEAD;
        }*/



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
         walkTowardPosition(xwater, ywater);

        // if we land on target next turn, we need to swim STATE
        reachedWater = ((this.pos.x - xwater) == 0 && (this.pos.y - ywater) == 0);
     }

     //Random swim if there is no fish in lake(s)
     public void randomSwim(){

         int xwater = 0;
         int ywater = 0;
         ArrayList<Cell> waterCells = new ArrayList<Cell>();

         // get to one block of water
         for(Cell c : this.fov) {
             if(c.type == GroundType.WATER){
                 waterCells.add(c);
             }
         }

         // choose one cell among all
         int idx = (int)(Math.random()*waterCells.size());
         xwater = waterCells.get(idx).x;
         ywater = waterCells.get(idx).y;

         System.out.println("Random Block Water : " + xwater +","+ ywater);
         // Walk to this cell
         walkTowardPosition(xwater, ywater);
     }

     // food position  when in Lake
    public void setFoodWater(Cell foodpos)
    {
        targetFood = foodpos;
    }

    private boolean validFoodCell()
    {
        return ! ((targetFood.x == -1) && (targetFood.y == -1));
    }


     //hunt if there is/are fish in the lake
     public void swimAndHunt(){
         // walk toward Food
         walkTowardPosition(targetFood.x, targetFood.y);
     }
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
            //m_state = StateHero.DEAD;
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

     // Walk Toward Target
     void walkTowardPosition(int xtarget, int ytarget)
     {
         // set movement toward cell(xtarget, ytarget)
         float dx = this.pos.x - xtarget;
         float dy = this.pos.y - ytarget;

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
         else if (dy > 0) {
             this.pos.y--;
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
