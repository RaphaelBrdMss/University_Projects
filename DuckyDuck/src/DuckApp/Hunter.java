package DuckApp;

import java.util.ArrayList;

public class Hunter extends  Ducky  {
    public int gridsize;

    int bullets;



    public Hunter(int gridsize,Grid g,int nbBullets){

        super(gridsize,g);
        fov = g.getFov(this.pos, 3);
        bullets = nbBullets;

    }


    public void Update(Cell duckPos) {

        switch (this.m_state) {
            case RANDOM:
                for ( Cell c : fov) {
                    System.out.println( "cell fov :" + c.toString() +"  duck pos : "+duckPos.toString());
                    if (c.x == duckPos.x && c.y == duckPos.y) {
                        System.out.println("je veux le cannard");
                        m_state = StateHero.HUNT;
                    }
                }

                RandomWalk();
                // state future state - future BDI



                break;

            case WALK:
                //walk to the dead duck :'( sad life
                break;

            case HUNT:
                System.out.println("[STATE] Hunt");
                shoot();
                //if shoot succed
                // m_state = StateHero.walk
                break;
            case EATING:
                System.out.println("[STATE] Eat");
                Eat();
                // go randomWalk
                m_state = StateHero.WALKROS;
                break;

        }
    }


    public void shoot(){

        if(this.fov.contains(super.getPos())&& bullets>0)
            bullets -=1;


    }





}
