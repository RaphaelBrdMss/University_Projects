package DuckApp;

import java.util.ArrayDeque;
import java.util.Deque;


public class Blackboard {

    private int m_elemSize = 1; // Size of last Seen arrays
    // attributes
    protected Cell m_invalid  = new Cell(-1,-1);
    protected Deque<Cell> m_lastSeenDucks;
    protected Deque<Cell> m_lastSeenFishs;
    protected Deque<Cell> m_lastSeenHunters;
    protected Deque<Cell> m_lastSeenLakes;

    public Blackboard(int elemSize)
    {
        if (elemSize > 0) {
            this.m_elemSize = elemSize;
        }
        else {
            this.m_elemSize = 1;
        }

        m_lastSeenDucks = new ArrayDeque<>();
        m_lastSeenFishs = new ArrayDeque<>();
        m_lastSeenHunters = new ArrayDeque<>();
        m_lastSeenLakes = new ArrayDeque<>();

    }

    // Retrieve the last Cell
    private Cell getLast(Deque<Cell> somearray)
    {
        if (somearray.isEmpty() || somearray == null){
            return m_invalid;
        }

        return somearray.peekLast();
    };

    // Add Element to blackboard
    private void addElement(Deque<Cell> somearray, Cell elemToAdd)
    {
        // Cannot add invalid Cell
        if (elemToAdd.isInvalid()) {
            return;
        }

        // If reached max size , remove first
        somearray.addLast(elemToAdd);
        if (somearray.size() > this.m_elemSize) {
            somearray.removeFirst();
        }
    }

    // Interface to get each last seen array element in array list
    public Cell getLastSeenDuck() { return getLast(m_lastSeenDucks);}
    public Cell getLastSeenFish() { return getLast(m_lastSeenFishs);}
    public Cell getLastSeenHunter() { return getLast(m_lastSeenHunters);}
    public Cell getLastSeenLake() { return getLast(m_lastSeenLakes);}

    // Interface to set last element
    public void addLastSeenDuck(Cell elem)   { addElement(this.m_lastSeenDucks, elem);}
    public void addLastSeenFish(Cell elem)   { addElement(this.m_lastSeenFishs, elem);}
    public void addLastSeenHunter(Cell elem) { addElement(this.m_lastSeenHunters, elem);}
    public void addLastSeenLake(Cell elem)   { addElement(this.m_lastSeenLakes, elem);}

    public void print()
    {
        System.out.println("Ducks :");
        System.out.println(this.m_lastSeenDucks);
        System.out.println("Fish :");
        System.out.println(this.m_lastSeenFishs);
        System.out.println("Hunters :");
        System.out.println(this.m_lastSeenHunters);
        System.out.println("Lakes :");
        System.out.println(this.m_lastSeenLakes);
    }

    /*public static void main(String[] args) {

        Blackboard m_bb = new Blackboard(3);

        m_bb.addLastSeenDuck(new Cell(15,12));
        m_bb.addLastSeenDuck(new Cell(1,0));
        m_bb.addLastSeenDuck(new Cell(3,6));

        m_bb.print();
        m_bb.addLastSeenDuck(new Cell(-1,-1));
        m_bb.addLastSeenDuck(new Cell(0,0));
        m_bb.print();

        System.out.println(m_bb.getLastSeenDuck().toString());


    }*/

}
