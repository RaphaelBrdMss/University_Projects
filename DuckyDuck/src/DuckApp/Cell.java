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

    // create an invalid cell
    public void invalidateCell()
    {
        this.x = -1;
        this.y = -1;
    }

    @Override
    public String toString(){
        return "Coord : " + x + "," + y;
    }

    public boolean isInvalid()
    {
        return (this.x == -1) && (this.y == -1);
    }


}
