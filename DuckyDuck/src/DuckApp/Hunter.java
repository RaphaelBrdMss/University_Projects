package DuckApp;

import java.util.ArrayList;

public class Hunter extends  Ducky  {
    public int gridsize;
    private StateHero m_stateHunter;
    int bullets;
    public Boolean shootSucced = false;
    public Ducky target;



    public Hunter(int gridsize,Grid g,int nbBullets){

        super(gridsize,g);

        fov = g.getFov(this.pos, 3);
        bullets = nbBullets;
        m_stateHunter = StateHero.RANDOM;

    }




    public void Update(ArrayList<Ducky> ducks) {


        switch (this.m_stateHunter) {
            case RANDOM:

                for ( Cell c : fov) {

                    for(Ducky d : ducks) {
                        if (c.x == d.pos.x && c.y == d.pos.y && bullets > 0) {

                            target = d;

                            shoot(target, c);
                            if (shootSucced) {
                                m_stateHunter = StateHero.WALK;
                            } else {
                                m_stateHunter = StateHero.REST;
                            }
                            break;


                        }
                    }
                }
                RandomWalk();
                break;


            case WALK:
                if(pos.x !=target.pos.x || pos.y != target.pos.y) {
                    System.out.println("Hunter WALK");
                    walkTowardPosition(target.pos.x, target.pos.y);

                }else{
                    m_stateHunter = StateHero.REST;
                }
                break;

            case REST :

                walkTowardPosition(0,0);
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

    //hunter can shoot the duck with this fonction (make sure to set chance of shooting for a good sim)
    public void shoot(Ducky target,Cell targetCell) {

        if (bullets > 0 ) {
            bullets -=1;

                if ( targetCell.type !=GroundType.WATER && targetCell.type != GroundType.ROSEAU) {
                    if (Math.random() > 0) {

                        shootSucced = true;
                    }

                }


        }else{
            shootSucced = false;
        }


    }



    public void setPos(Cell newPos){
        pos =  newPos;
    }


}






