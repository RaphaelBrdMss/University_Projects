package DuckApp;

public class Cell {
    public int x,y;
    GroundType type;

    public Cell(int x, int y, GroundType type){
        this.type = type;
        this.x = x;
        this.y = y;
    }
    public Cell(int x , int y){
        this.x = x;
        this.y =y;
    }

    public void setType(GroundType type) {
        this.type = type;
    }

    @Override
    public String toString(){
        return "Coord : " + x + "," + y;
    }
}
