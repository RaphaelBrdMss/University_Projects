package DuckApp;

import java.util.ArrayList;

public class Food {

    public Cell pos;
    public int gridSize;
    public boolean available;
    public int m_id;

    public Food(int sizeGrid, Grid g ) {
        this.gridSize = g.size;
        this.available = true;


        int randX = (int) (Math.random() * sizeGrid);
        int randY = (int) (Math.random() * sizeGrid);

        while (g.getCell(randX, randY).type != GroundType.WATER) {
            randX = (int) (Math.random() * sizeGrid);
            randY = (int) (Math.random() * sizeGrid);

        }

        this.pos = g.getCell(randX, randY);
        this.m_id = randX*sizeGrid + randY;

    }

    // Empty constructor
    public Food()
    {
        this.gridSize = 0;
        this.available = false;
        this.pos = new Cell(-1,-1);
        this.m_id = -1;
    }


    // Food is being eat -> disappear
    public void isEaten()
    {
        // invalid cell
        this.pos.invalidateCell();
        this.available = false;
        this.m_id = -1;
    }

    // recreate a new position
    public void regenerate(int sizeGrid, Grid g )
    {
        this.gridSize = g.size;
        this.available = true;


        int randX = (int) (Math.random() * sizeGrid);
        int randY = (int) (Math.random() * sizeGrid);

        while (g.getCell(randX, randY).type != GroundType.WATER) {
            randX = (int) (Math.random() * sizeGrid);
            randY = (int) (Math.random() * sizeGrid);

        }

        this.pos = g.getCell(randX, randY);
        this.m_id = randX*sizeGrid + randY;
    }

    public boolean isId(int id) { return id == m_id;}
    public void setId(int id){m_id = id;}
    public int getId() { return m_id;}

}
