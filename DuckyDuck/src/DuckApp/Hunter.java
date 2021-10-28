package DuckApp;

public class Hunter extends  Ducky  {
    public int gridsize;
    private StateHero m_stateHunter;
    int bullets;
    public Boolean shooted = false;



    public Hunter(int gridsize,Grid g,int nbBullets){

        super(gridsize,g);

        fov = g.getFov(this.pos, 3);
        bullets = nbBullets;
        m_stateHunter = StateHero.RANDOM;

    }


    public void Update(Cell duckPos) {

        switch (this.m_stateHunter) {
            case RANDOM:

                for ( Cell c : fov) {

                    if (c.x == duckPos.x && c.y == duckPos.y) {
                        m_stateHunter = StateHero.HUNT;
                    }

                }
                RandomWalk();
                break;

            case WALK:
                walkTowardPosition(duckPos.x, duckPos.y);
                break;

            case HUNT:

                shoot(duckPos);
                //if shoot succed
                if(shooted) {

                    m_stateHunter = StateHero.WALK;

                }

                else {
                    m_stateHunter = StateHero.RANDOM;
                }
                break;




        }
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


        }while(x < 0 || x >=  gridSize || y < 0 || y >=gridSize  );

        Cell newPos = new Cell(x,y);
        setPos(newPos);



    }


    public void shoot(Cell duckPos) {
        if (bullets > 0) {
            for (Cell c : fov) {
                if (c.x == duckPos.x && c.y == duckPos.y) {
                    if (Math.random() > 0) {
                        super.setShooted();
                        shooted = true;
                    }

                }

            }
        }
    }



    public void setPos(Cell newPos){
        pos =  newPos;
    }


}






