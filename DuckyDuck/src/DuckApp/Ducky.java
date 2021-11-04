package DuckApp;

import java.util.ArrayList;

public class Ducky {

    public Cell pos;
    public Food m_targetFood;
    public int gridSize;
    public ArrayList<Cell> fov;
    public boolean isShooted= false;

    private int m_estomac = 100; // 100 = plein, 0=dead
    private int m_stamina = 100; // 100 = energie , 0 no move allowed
    private int m_happiness = 100; // 100 = full, < 20 impact on stamina
    private StateHero m_state ;
    private boolean reachedWater = false;
    private boolean reachedRoseau = false;

    public boolean inWater = false;
    boolean foundWater = false;

    private int m_pasStamina = 5;
    private int m_addStamina = 10;



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
        fov = g.getFov(pos,2);

        // state
        m_state = StateHero.RANDOM;

        // init target food
        m_targetFood = new Food();
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

        // DEAD overall
        if( isDuckDead()){
            m_state = StateHero.DEAD;
            System.out.println("[STATE] DEAD !! ");
            return;
        }

        // if no energy : rest on cell
        if ( m_stamina < 1)
        {
            m_state = StateHero.REST;
        }

        // Set Desire

        switch (this.m_state)
        {
            case RANDOM:
                //System.out.println("[STATE] RW");
                RandomWalk();
                decreaseStamina();

                // state future state - future BDI
                if (isWaterInFov(this.fov)) {
                    //System.out.println("Water In FOV");
                    m_state = StateHero.WALK;
                }
                reachedWater = false;
                inWater = false;
                break;
            case WALK:
                //System.out.println("[STATE] Walk to Water ");
                waterTarget();
                // if water is reached we change the future state
                if (reachedWater) {
                    m_state = StateHero.SWIM;
                    reachedWater = false;
                } else if (!foundWater) {
                    // if not found water go random Walk
                    m_state = StateHero.RANDOM;
                }
                decreaseStamina();
                break;
            case SWIM:
                //System.out.println("[STATE] RSWIM");
                randomSwim();
                inWater = true;
                // if valid food cell
                if (validFoodCell()) {
                    m_state = StateHero.SWIMHUNT;
                }
                decreaseStamina();
                break;
            case SWIMHUNT:
                //System.out.println("[STATE] SwimAndHunt");
                swimAndHunt();

                inWater = true;
                if ((this.pos.x == m_targetFood.pos.x) && (this.pos.y == m_targetFood.pos.y)) {
                    m_state = StateHero.EATING;
                }
                decreaseStamina();
                break;
            case HUNT:
                //System.out.println("[STATE] Hunt");
                Hunt();
                decreaseStamina();
                break;
            case EATING:
                //System.out.println("[STATE] Eat");
                Eat();
                // future state : go randomWalk
                m_state = StateHero.WALKROS;
                decreaseStamina();
                break;
            case WALKROS:
                //System.out.println("[STATE] Walk To a rest place ");
                roseauxTarget();
                // if roseau is reached we change the future state
                if (reachedRoseau) {
                    m_state = StateHero.REST;
                    reachedRoseau = false;
                }
                decreaseStamina();
                break;
            case REST:
                //System.out.println("[STATE] Rest");
                Rest();
                // Full recharge
                if (m_stamina == 100){
                    m_state = StateHero.RANDOM;
                }
                break;
            case DEAD:
                //System.out.println("[STATE] DEAD !! ");
                // Is dead but we still want to move him
                //m_state = StateHero.RANDOM;
                break;
            default:
                decreaseStamina();
                inWater = false;
                break;
        }

        // increase Hunger each 3 move
        updateHunger(m_pasStamina);

    }

    public boolean isDuckDead(){

        if(m_estomac  <=0){
            return true;
        }
        else return isShooted;
    }

     public void setisShooted(){
        isShooted = true;
     }

    // REST and Recharge
    public void Rest()
    {
        // Wait and Recharge
        System.out.println("REST");
        // Update Stamina
        m_stamina += m_addStamina;
        if (m_stamina > 100){
            m_stamina = 100;
        }

    }

    // Walk to Water
    public void waterTarget(){

        int xwater = 0;
        int ywater = 0;
        int dxy = Integer.MAX_VALUE;
        foundWater = false;
        // get nearest water
        for(Cell c : this.fov) {
            if(c.type == GroundType.WATER){
                if ((Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y)) < dxy )
                {
                    xwater = c.x;
                    ywater = c.y;
                    dxy = Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y);
                    foundWater = true;
                }
            }
        }

        if (foundWater)
        {
            System.out.println("Nearest Water : " + xwater +","+ ywater);
            System.out.println("Ducky Pos  " + pos.x +","+ pos.y);
            // set movement toward cell
            walkTowardPosition(xwater, ywater);
        }
        else // if water not found we need to random walk
        {
            System.out.println("Set to center : " + gridSize/ 2  +","+ gridSize/ 2);
            System.out.println("Ducky Pos  " + pos.x +","+ pos.y);
            // set movement toward center
            walkTowardPosition(gridSize/ 2, gridSize/ 2);
        }

        // if we land on target next turn, we need to swim STATE
        reachedWater = ((this.pos.x - xwater) == 0 && (this.pos.y - ywater) == 0);
    }

    // Walk to Roseaux
    public void roseauxTarget(){

        int xtarget = 0;
        int ytarget = 0;
        int dxy = Integer.MAX_VALUE;
        int xwat = 0;
        int ywat = 0;
        int dxywater = Integer.MIN_VALUE;
        boolean foundRoseau = false;
        // get roseaux
        for(Cell c : this.fov) {
            if(c.type == GroundType.ROSEAU){
                if ((Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y)) < dxy )
                {
                    xtarget = c.x;
                    ytarget = c.y;
                    dxy = Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y);
                    foundRoseau = true;
                }
            } else if (c.type == GroundType.WATER)
            {
                if ((Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y)) > dxywater )
                {
                    xwat = c.x;
                    ywat = c.y;
                    dxywater = Math.abs(this.pos.x - c.x) + Math.abs(this.pos.y - c.y);
                }
            }
        }

        if (foundRoseau)
        {
            // set movement toward cell
            walkTowardPosition(xtarget, ytarget);
            System.out.println("Nearest Roseau : " + xtarget +","+ ytarget);
        }
        else
        {
            // walk to farthest water
            walkTowardPosition(xwat, ywat);
            System.out.println("Nearest Water : " + xwat +","+ ywat);
        }

        System.out.println("Ducky Pos  " + pos.x +","+ pos.y);

        // if we land on target next turn, we need to swim STATE
        reachedRoseau = ((this.pos.x - xtarget) == 0 && (this.pos.y - ytarget) == 0);
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
    public void setFoodWater(Cell snack, int id)
    {
        m_targetFood.pos.x = snack.x;
        m_targetFood.pos.y = snack.y;
        m_targetFood.setId(id);
    }

    public int getEatenId() { return m_targetFood.getId(); }

    private boolean validFoodCell()
    {
        return ! ((m_targetFood.pos.x == -1) && (m_targetFood.pos.y == -1));
    }


    //hunt if there is/are fish in the lake
    public void swimAndHunt(){
        // walk toward Food
        walkTowardPosition(m_targetFood.pos.x, m_targetFood.pos.y);
    }
    // hunt on ground
    public void Hunt(){}

    // eat : +50
    public void Eat(){
        if (m_estomac > 50)
        {
            m_estomac = 100;
        }
        else
        {
            m_estomac+=50;
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

    // Increased hunger according to stamina
    private void updateHunger(int pas)
    {
        if ((m_stamina % pas) == 0)
        {
            m_estomac--;
        }
    }

    // Stamina -= 1
    // force resting if not enough stamina
    private void decreaseStamina()
    {
        if (m_stamina > 1)
        {
            m_stamina--;
        }
        else // force resting on cell
        {
            m_state = StateHero.REST;
        }
    }


    // Getters
    // -------

    public int getEstomac(){
        return m_estomac;
    }

    public int getM_stamina(){
        return m_stamina;
    }

    public StateHero getM_state(){
        return m_state;
    }



}
