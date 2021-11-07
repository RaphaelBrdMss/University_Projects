package DuckApp;

import java.util.ArrayDeque;
import java.util.Deque;
import java.util.ArrayList;
import java.util.Iterator;


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

    // getter all list
    public ArrayList<Cell> getAllElements(Deque<Cell> somearray)
    {
        ArrayList<Cell> toArrayL = new ArrayList<Cell>();
        Iterator<Cell> itCell = somearray.iterator();
        while(itCell.hasNext()){
            toArrayL.add(itCell.next());
        }

        return toArrayL;
    }

    // get Deque element by index
    private Cell get(Deque<Cell> somearray, int index)
    {
        Iterator<Cell> itCell = somearray.iterator();
        Cell c = new Cell(-1,-1);

        if (index >= somearray.size()) {
            return c;
        }

        for (int k = 0; (k < index+1) && itCell.hasNext(); k++){
            c = itCell.next();
        }

        return c;
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

    // Interface to get all element as an arrayList
    public ArrayList<Cell> getAllSeenDucks() { return getAllElements(m_lastSeenDucks);}
    public ArrayList<Cell> getAllSeenFishs() { return getAllElements(m_lastSeenFishs);}
    public ArrayList<Cell> getAllSeenHunters() { return getAllElements(m_lastSeenHunters);}
    public ArrayList<Cell> getAllSeenLakes() { return getAllElements(m_lastSeenLakes);}

    // Interface to get Element
    public Cell getSeenDuck(int index) { return get(m_lastSeenDucks, index);}
    public Cell getSeenFish(int index) { return get(m_lastSeenFishs, index);}
    public Cell getSeenHunter(int index) { return get(m_lastSeenHunters, index);}
    public Cell getSeenLake(int index) { return get(m_lastSeenLakes, index);}

    // Get Elements size
    public int getMaxSize() { return this.m_elemSize;}
    public int getSeenDucksSize() { return this.m_lastSeenDucks.size(); }
    public int getSeenFishsSize() { return this.m_lastSeenFishs.size(); }
    public int getSeenHuntersSize() { return this.m_lastSeenHunters.size(); }
    public int getSeenLakesSize() { return this.m_lastSeenLakes.size(); }

    public void printContent()
    {
        System.out.print("Ducks : ");
        System.out.println(this.m_lastSeenDucks);
        System.out.print("Fish : ");
        System.out.println(this.m_lastSeenFishs);
        System.out.print("Hunters : ");
        System.out.println(this.m_lastSeenHunters);
        System.out.print("Lakes : ");
        System.out.println(this.m_lastSeenLakes);
    }

    public static void main(String[] args) {

        Blackboard m_bb = new Blackboard(3);

        m_bb.addLastSeenDuck(new Cell(15,12));
        m_bb.addLastSeenDuck(new Cell(1,0));
        m_bb.addLastSeenDuck(new Cell(3,6));

        m_bb.printContent();
        m_bb.addLastSeenDuck(new Cell(-1,-1));
        m_bb.addLastSeenDuck(new Cell(0,0));
        m_bb.printContent();

        System.out.println(m_bb.getLastSeenDuck().toString());

        ArrayList<Cell> test = m_bb.getAllSeenDucks();
        System.out.print("All Seen Ducks:\n---------\n");
        for (Cell c : test) {
            System.out.print(c.toString()+ "\n");
        }

        Cell secondDuck = m_bb.getSeenDuck(1);
        System.out.print("2nd Duck:\n------\n");
        System.out.print(secondDuck.toString()+ "\n");

        System.out.print("get Elem Size:\n--------------\n");
        System.out.print("Number of Seen Ducks : " + m_bb.getSeenDucksSize() + "\n");
        System.out.print("Number of Seen Fishs : " + m_bb.getSeenFishsSize()+ "\n");
        System.out.print("Number of Seen Hunters : " + m_bb.getSeenHuntersSize()+ "\n");
        System.out.print("Number of Seen Lakes : " + m_bb.getSeenLakesSize()+ "\n");


    }

}
