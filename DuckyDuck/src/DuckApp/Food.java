package DuckApp;

import java.util.ArrayList;

public class Food {

    public Cell pos;
    public int gridSize;

    public Food(int sizeGrid, Grid g ) {
        this.gridSize = g.size;


        int randX = (int) (Math.random() * sizeGrid);
        int randY = (int) (Math.random() * sizeGrid);

        while (g.getCell(randX, randY).type != GroundType.WATER) {
            randX = (int) (Math.random() * sizeGrid);
            randY = (int) (Math.random() * sizeGrid);

        }

        this.pos = g.getCell(randX, randY);

    }
}
