using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Misc;
using Characters;


namespace Hexas
{

    public enum HexaType : byte { GROUND, VOID, WALL };
    public enum HexaDirection : byte { UP, UP_RIGHT, DOWN_RIGHT, DOWN, DOWN_LEFT, UP_LEFT };

    public class Hexa
    {
        public static GameObject hexasFolder;
        public static Mesh hexaFilledMesh;
        public static Mesh hexaHollowMesh;
        public static Mesh hexaWallMesh;
        public static GameObject hexaTemplate;

        public static float offsetX = 0;
        public static float offsetY = 0;

        public HexaType type;
        public int x;
        public int y;
        public Character charOn;
        public GameObject go;

        public Hexa(HexaType type, int x, int y)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.charOn = null;
            this.go = GameObject.Instantiate(hexaTemplate, hexasFolder.transform);
            this.go.SetActive(true);
            this.go.transform.position = hexaPosToReal(x, y, 0);
            switch (this.type)
            {
                case HexaType.GROUND: this.go.GetComponent<MeshFilter>().mesh = hexaHollowMesh; break;
                case HexaType.VOID: this.go.GetComponent<MeshFilter>().mesh = hexaFilledMesh; break;
                case HexaType.WALL: this.go.GetComponent<MeshFilter>().mesh = hexaWallMesh; break;
            }
            this.defaultColor();
            this.go.GetComponent<HexaGO>().hexa = this;
        }

        // No GameObject (console mode)
        public Hexa(HexaType type, int x, int y, bool a)
        {
            this.type = type;
            this.x = x;
            this.y = y;
            this.charOn = null;
            this.go = null;
        }

        public void changeType(HexaType newType)
        {
            type = newType;
            switch (this.type)
            {
                case HexaType.GROUND: this.go.GetComponent<MeshFilter>().mesh = hexaHollowMesh; break;
                case HexaType.VOID: this.go.GetComponent<MeshFilter>().mesh = hexaFilledMesh; break;
                case HexaType.WALL: this.go.GetComponent<MeshFilter>().mesh = hexaWallMesh; break;
            }
            this.defaultColor();
        }

        // Console mode
        public void changeType2(HexaType newType)
        {
            type = newType;
        }

        public void changeColor(Color color)
        {
            this.go.GetComponent<Renderer>().material.color = color;
        }

        public void defaultColor()
        {
            switch (this.type)
            {
                case HexaType.GROUND: this.go.GetComponent<Renderer>().material.color = new Color(0.25f, 0.2f, 0.075f, 0.5f); break;
                case HexaType.VOID: this.go.GetComponent<Renderer>().material.color = new Color(0.25f, 0.2f, 0.075f); break;
                case HexaType.WALL: this.go.GetComponent<Renderer>().material.color = new Color(0.25f, 0.2f, 0.075f); break;
            }
        }

        public void hoveredColor()
        {
            switch (this.type)
            {
                case HexaType.GROUND: this.go.GetComponent<Renderer>().material.color = new Color(1, 1, 1); break;
                case HexaType.VOID: this.go.GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f); break;
                case HexaType.WALL: this.go.GetComponent<Renderer>().material.color = new Color(0.25f, 0.25f, 0.25f); break;
            }
        }

        public string getName()
        {
            switch (this.type)
            {
                case HexaType.GROUND: return "Ground";
                case HexaType.VOID: return "Void";
                case HexaType.WALL: return "Wall";
                default: return "None";
            }
        }

        public static Vector3 hexaPosToReal(int x, int y, float height)
        {
            return new Vector3(x * 0.75f + offsetX, height, y * -0.86f + (x % 2) * 0.43f + offsetY);
        }
    }



    public class HexaGrid
    {
        public List<Hexa> hexaList;
        public List<Character> charList;
        public int w;
        public int h;

        public HexaGrid()
        {
            hexaList = new List<Hexa>();
            charList = new List<Character>();
            w = 0;
            h = 0;
        }

        public void createGridFromFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    w = reader.ReadInt32();
                    h = reader.ReadInt32();
                    Hexa.offsetX = -((w - 1) * 0.75f) / 2;
                    Hexa.offsetY = -((h - 1) * -0.86f + ((w - 1) % 2) * 0.43f) / 2;
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            hexaList.Add(new Hexa((HexaType)reader.ReadByte(), i, j));
                        }
                    }
                }
            }
            else
            {
                createRectGrid(34, 30);
            }
        }

        // Console mode
        public void createGridFromFile2(string filePath)
        {
            if (File.Exists(filePath))
            {
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
                {
                    w = reader.ReadInt32();
                    h = reader.ReadInt32();
                    Hexa.offsetX = -((w - 1) * 0.75f) / 2;
                    Hexa.offsetY = -((h - 1) * -0.86f + ((w - 1) % 2) * 0.43f) / 2;
                    for (int j = 0; j < h; j++)
                    {
                        for (int i = 0; i < w; i++)
                        {
                            hexaList.Add(new Hexa((HexaType)reader.ReadByte(), i, j, true));
                        }
                    }
                }
            }
            else
            {
                createRectGrid(34, 30);
            }
        }

        public void createRectGrid(int w, int h)
        {
            Hexa.offsetX = -((w - 1) * 0.75f) / 2;
            Hexa.offsetY = -((h - 1) * -0.86f + ((w - 1) % 2) * 0.43f) / 2;
            this.w = w;
            this.h = h;
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    hexaList.Add(new Hexa(HexaType.GROUND, i, j));
                }
            }
        }

        public void createRandomRectGrid(int w, int h)
        {
            Hexa.offsetX = -((w - 1) * 0.75f) / 2;
            Hexa.offsetY = -((h - 1) * -0.86f + ((w - 1) % 2) * 0.43f) / 2;
            this.w = w;
            this.h = h;
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    hexaList.Add(new Hexa((((int)Random.Range(0, 8)) == 0) ? HexaType.WALL : HexaType.GROUND, i, j));
                }
            }
        }

        // Console mode
        public void createRandomRectGrid2(int w, int h)
        {
            Hexa.offsetX = -((w - 1) * 0.75f) / 2;
            Hexa.offsetY = -((h - 1) * -0.86f + ((w - 1) % 2) * 0.43f) / 2;
            this.w = w;
            this.h = h;
            for (int j = 0; j < h; j++)
            {
                for (int i = 0; i < w; i++)
                {
                    hexaList.Add(new Hexa((((int)Random.Range(0, 8)) == 0) ? HexaType.WALL : HexaType.GROUND, i, j, true));
                }
            }
        }

        public void saveGridInFile(string filePath)
        {
            using (BinaryWriter writer = new BinaryWriter(File.Open(filePath, FileMode.Create)))
            {
                writer.Write(w);
                writer.Write(h);
                int k = 0;
                for (int j = 0; j < h; j++)
                {
                    for (int i = 0; i < w; i++)
                    {
                        writer.Write((byte)(hexaList[k].type));
                        k++;
                    }
                }
            }
        }

        public void addChar(CharClass charClass, int x, int y, int team)
        {
            Hexa hexa = getHexa(x, y);
            if (hexa != null && hexa.charOn == null)
            {
                Character c = new Character(charClass, x, y, team);
                hexa.charOn = c;
                charList.Add(c);
            }
        }

        // Console mode
        public void addChar2(CharClass charClass, int x, int y, int team)
        {
            Hexa hexa = getHexa(x, y);
            if (hexa != null && hexa.charOn == null)
            {
                Character c = new Character(charClass, x, y, team, true);
                hexa.charOn = c;
                charList.Add(c);
            }
        }

        public void addChar(Character c)
        {
            Hexa hexa = getHexa(c.x, c.y);
            if (hexa != null && hexa.charOn == null)
            {
                hexa.charOn = c;
                charList.Add(c);
            }
        }

        /** Finds the position of an adjacent hexa **/
        public static Point findPos(int x, int y, HexaDirection direction)
        {
            switch (direction)
            {
                case HexaDirection.UP: return new Point(x, y - 1);
                case HexaDirection.UP_RIGHT: return new Point(x + 1, (x % 2 == 0) ? y : y - 1);
                case HexaDirection.DOWN_RIGHT: return new Point(x + 1, (x % 2 == 0) ? y + 1 : y);
                case HexaDirection.DOWN: return new Point(x, y + 1);
                case HexaDirection.DOWN_LEFT: return new Point(x - 1, (x % 2 == 0) ? y + 1 : y);
                case HexaDirection.UP_LEFT: return new Point(x - 1, (x % 2 == 0) ? y : y - 1);
                default: return null;
            }
        }

        public Hexa getHexa(int x, int y)
        {
            if (x >= 0 && x < w && y >= 0 && y < h)
            {
                return hexaList[x + y * w];
            }
            else
            {
                return null;
            }
        }

        public Hexa getHexa(Point p)
        {
            if (p != null && p.x >= 0 && p.x < w && p.y >= 0 && p.y < h)
            {
                return hexaList[p.x + p.y * w];
            }
            else
            {
                return null;
            }
        }

        public int getCharID(Character c)
        {
            for (int i = 0; i < charList.Count; i++)
            {
                if (c == charList[i]) return i;
            }
            return -1;
        }

        public class HexaTemp
        {
            public int x, y, nbSteps;
            public HexaTemp(int x, int y, int nbSteps)
            {
                this.x = x; this.y = y; this.nbSteps = nbSteps;
            }
        }

        /** Finds the shortest path between point (x1,y1) and (x2,y2) within maxSteps steps.
            Returns either null if there is no path or the list of hexas to take from (x2,y2) to (x1,y1). **/
        public List<Point> findShortestPath(int x1, int y1, int x2, int y2, int maxSteps)
        {
            if (getHexa(x1, y1).type == HexaType.WALL || getHexa(x2, y2).type == HexaType.WALL) return null;
            List<HexaTemp> hexaList2 = new List<HexaTemp>();
            foreach (Hexa hexa in hexaList)
            {
                hexaList2.Add(new HexaTemp(hexa.x, hexa.y, maxSteps + 1));
            }
            List<HexaTemp> toCheck = new List<HexaTemp>();
            toCheck.Add(new HexaTemp(x1, y1, 0));
            hexaList2[x1 + y1 * w].nbSteps = 0;
            int minSteps = maxSteps + 1;

            //int a = 0;

            while (toCheck.Count > 0)
            {
                HexaTemp p = toCheck[0];
                toCheck.RemoveAt(0);
                if (p.nbSteps < maxSteps && p.nbSteps < minSteps)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        HexaDirection hexaDirectionI = (HexaDirection)i;
                        Point p2 = findPos(p.x, p.y, hexaDirectionI);
                        Hexa h = getHexa(p2);
                        if (h != null && h.type == HexaType.GROUND && h.charOn == null && hexaList2[p2.x + p2.y * w].nbSteps > p.nbSteps + 1)
                        {
                            hexaList2[p2.x + p2.y * w].nbSteps = p.nbSteps + 1;
                            if (p2.x == x2 && p2.y == y2) minSteps = p.nbSteps + 1;
                            toCheck.Add(new HexaTemp(p2.x, p2.y, p.nbSteps + 1));
                            //a++;
                        }
                    }
                }
            }

            //Debug.Log(a);

            if (hexaList2[x2 + y2 * w].nbSteps != maxSteps + 1)
            {
                List<Point> l = new List<Point>();
                int nbSteps = hexaList2[x2 + y2 * w].nbSteps - 1;
                Point p = new Point(x2, y2);
                l.Add(p);
                while (!(p.x == x1 && p.y == y1))
                {
                    for (int i = 0; i < 6; i++)
                    {
                        HexaDirection hexaDirectionI = (HexaDirection)i;
                        Point p2 = findPos(p.x, p.y, hexaDirectionI);
                        if (p2 != null && p2.x >= 0 && p2.x < w && p2.y >= 0 && p2.y < h && hexaList2[p2.x + p2.y * w].nbSteps == nbSteps)
                        {
                            i = 6;
                            p = p2;
                            l.Add(p);
                            nbSteps--;
                        }
                    }
                }
                // Flip the list
                List<Point> l2 = new List<Point>();
                for (int i = l.Count - 1; i >= 0; i--) l2.Add(l[i]);
                return l2;
            }
            else
            {
                return null;
            }
        }

        /** Finds all the possible paths from (x,y).
            Returns the list of hexas. **/
        public List<Point> findAllPaths(int x, int y, int maxSteps)
        {
            if (getHexa(x, y).type == HexaType.WALL) return new List<Point>();
            List<HexaTemp> hexaList2 = new List<HexaTemp>();
            foreach (Hexa hexa in hexaList)
            {
                hexaList2.Add(new HexaTemp(hexa.x, hexa.y, maxSteps + 1));
            }
            List<HexaTemp> toCheck = new List<HexaTemp>();
            toCheck.Add(new HexaTemp(x, y, 0));
            hexaList2[x + y * w].nbSteps = 0;

            while (toCheck.Count > 0)
            {
                HexaTemp p = toCheck[0];
                toCheck.RemoveAt(0);
                if (p.nbSteps < maxSteps)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        HexaDirection hexaDirectionI = (HexaDirection)i;
                        Point p2 = findPos(p.x, p.y, hexaDirectionI);
                        Hexa h = getHexa(p2);
                        if (h != null && h.type == HexaType.GROUND && h.charOn == null && hexaList2[p2.x + p2.y * w].nbSteps > p.nbSteps + 1)
                        {
                            hexaList2[p2.x + p2.y * w].nbSteps = p.nbSteps + 1;
                            toCheck.Add(new HexaTemp(p2.x, p2.y, p.nbSteps + 1));
                        }
                    }
                }
            }

            List<Point> pList = new List<Point>();
            foreach (HexaTemp ht in hexaList2)
            {
                if (ht.nbSteps <= maxSteps) pList.Add(new Point(ht.x, ht.y));
            }
            return pList;
        }

        /** (private) Used by findHexasInSight */
        private bool isSightBlockedByHexa(int x1, int y1, int x2, int y2, int hexaX, int hexaY)
        {
            return (Geometry.line_intersects_line(x1, y1, x2, y2, hexaX - 1, hexaY - 2, hexaX + 1, hexaY - 2) ||
                Geometry.line_intersects_line(x1, y1, x2, y2, hexaX + 1, hexaY - 2, hexaX + 2, hexaY) ||
                Geometry.line_intersects_line(x1, y1, x2, y2, hexaX + 2, hexaY, hexaX + 1, hexaY + 2) ||
                Geometry.line_intersects_line(x1, y1, x2, y2, hexaX + 1, hexaY + 2, hexaX - 1, hexaY + 2) ||
                Geometry.line_intersects_line(x1, y1, x2, y2, hexaX - 1, hexaY + 2, hexaX - 2, hexaY) ||
                Geometry.line_intersects_line(x1, y1, x2, y2, hexaX - 2, hexaY, hexaX - 1, hexaY - 2));
        }

        /** Finds all hexas in sight from the position (x,y) within maxRange.
            Returns the list of ground-type hexa positions that are in sight.
            The list of hexas blocked is returned in out blocked. **/
        public List<Point> findHexasInSight(int x, int y, int maxRange, out List<Point> blocked)
        {
            if (x >= 0 && x < w && y >= 0 && y < h && hexaList[x + y * w].type != HexaType.WALL)
            {
                int x2 = x;
                int y2 = y;
                List<Point> toCheck = new List<Point>();
                List<Point> wallList = new List<Point>();
                List<Character> charList_ = new List<Character>();
                List<Point> charWallList = new List<Point>();
                List<Point> inSight = new List<Point>();
                blocked = new List<Point>();

                for (int j = 0; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        toCheck.Add(new Point(x2, y2 + i));
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2++;
                }

                x2 = x - 1;
                for (int j = 1; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        toCheck.Add(new Point(x2, y2 + i));
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2--;
                }

                foreach (Point p in wallList)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    p.x = cx;
                    p.y = cy;
                }

                foreach (Character p in charList_)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    charWallList.Add(new Point(cx, cy));
                }

                int myPosx = x * 3;
                int myPosy = y * -4 + (x % 2) * 2;
                foreach (Point p in toCheck)
                {
                    int px = p.x * 3;
                    int py = p.y * -4 + (p.x % 2) * 2;
                    bool hexaInSight = true;
                    for (int i = 0; i < wallList.Count; i++)
                    {
                        if (isSightBlockedByHexa(myPosx, myPosy, px, py, wallList[i].x, wallList[i].y))
                        {
                            i = wallList.Count;
                            hexaInSight = false;
                        }
                    }
                    if (hexaInSight)
                    {
                        Character chara = getHexa(p).charOn;
                        for (int i = 0; i < charWallList.Count; i++)
                        {
                            if (charList_[i] != chara)
                            {
                                if (isSightBlockedByHexa(myPosx, myPosy, px, py, charWallList[i].x, charWallList[i].y))
                                {
                                    i = charWallList.Count;
                                    hexaInSight = false;
                                }
                            }
                        }
                    }
                    if (hexaInSight) inSight.Add(p);
                    else blocked.Add(p);
                }
                return inSight;
            }
            else
            {
                blocked = new List<Point>();
                return new List<Point>();
            }
        }

        /** Finds all hexas in sight from the position (x,y) within maxRange.
            Returns the list of ground-type hexa positions that are in sight. **/
        public List<Point> findHexasInSight2(int x, int y, int maxRange)
        {
            if (x >= 0 && x < w && y >= 0 && y < h && hexaList[x + y * w].type != HexaType.WALL)
            {
                int x2 = x;
                int y2 = y;
                List<Point> toCheck = new List<Point>();
                List<Point> wallList = new List<Point>();
                List<Character> charList_ = new List<Character>();
                List<Point> charWallList = new List<Point>();
                List<Point> inSight = new List<Point>();

                for (int j = 0; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        toCheck.Add(new Point(x2, y2 + i));
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2++;
                }

                x2 = x - 1;
                for (int j = 1; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        toCheck.Add(new Point(x2, y2 + i));
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2--;
                }

                foreach (Point p in wallList)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    p.x = cx;
                    p.y = cy;
                }

                foreach (Character p in charList_)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    charWallList.Add(new Point(cx, cy));
                }

                int myPosx = x * 3;
                int myPosy = y * -4 + (x % 2) * 2;
                foreach (Point p in toCheck)
                {
                    int px = p.x * 3;
                    int py = p.y * -4 + (p.x % 2) * 2;
                    bool hexaInSight = true;
                    for (int i = 0; i < wallList.Count; i++)
                    {
                        if (isSightBlockedByHexa(myPosx, myPosy, px, py, wallList[i].x, wallList[i].y))
                        {
                            i = wallList.Count;
                            hexaInSight = false;
                        }
                    }
                    if (hexaInSight)
                    {
                        Character chara = getHexa(p).charOn;
                        for (int i = 0; i < charWallList.Count; i++)
                        {
                            if (charList_[i] != chara)
                            {
                                if (isSightBlockedByHexa(myPosx, myPosy, px, py, charWallList[i].x, charWallList[i].y))
                                {
                                    i = charWallList.Count;
                                    hexaInSight = false;
                                }
                            }
                        }
                    }
                    if (hexaInSight) inSight.Add(p);
                }
                return inSight;
            }
            else
            {
                return new List<Point>();
            }
        }

        /** Returns true if the hexa at position (hexaX,hexaY) is in sight from (x,y). **/
        public bool hexaInSight(int x, int y, int hexaX, int hexaY, int maxRange)
        {
            if (getDistance(x, y, hexaX, hexaY) <= maxRange && x >= 0 && x < w && y >= 0 && y < h && hexaList[x + y * w].type != HexaType.WALL && hexaX >= 0 && hexaX < w && hexaY >= 0 && hexaY < h && hexaList[hexaX + hexaY * w].type == HexaType.GROUND)
            {
                int x2 = x;
                int y2 = y;
                List<Point> wallList = new List<Point>();
                List<Character> charList_ = new List<Character>();
                List<Point> charWallList = new List<Point>();

                bool found = false;
                for (int j = 0; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn);
                                        if (!found && x2 == hexaX && y2 + i == hexaY) found = true; break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2++;
                }

                x2 = x - 1;
                for (int j = 1; j <= maxRange; j++)
                {
                    int iMin = -maxRange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = maxRange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND:
                                        if ((x2 != x || y2 + i != y) && hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn);
                                        if (!found && x2 == hexaX && y2 + i == hexaY) found = true; break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: wallList.Add(new Point(x2, y2 + i)); break;
                                }
                            }
                        }
                    }
                    x2--;
                }
                if (!found) return false;

                foreach (Point p in wallList)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    p.x = cx;
                    p.y = cy;
                }

                foreach (Character p in charList_)
                {
                    int cx = p.x * 3;
                    int cy = p.y * -4 + (p.x % 2) * 2;
                    charWallList.Add(new Point(cx, cy));
                }

                int myPosx = x * 3;
                int myPosy = y * -4 + (x % 2) * 2;
                int px = hexaX * 3;
                int py = hexaY * -4 + (hexaX % 2) * 2;

                for (int i = 0; i < wallList.Count; i++)
                {
                    if (isSightBlockedByHexa(myPosx, myPosy, px, py, wallList[i].x, wallList[i].y))
                    {
                        return false;
                    }
                }
                Character chara = getHexa(hexaX, hexaY).charOn;
                for (int i = 0; i < charWallList.Count; i++)
                {
                    if (charList_[i] != chara)
                    {
                        if (isSightBlockedByHexa(myPosx, myPosy, px, py, charWallList[i].x, charWallList[i].y))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        /** Returns the list of hexas pos within AoERange of position (x,y). **/
        public List<Point> getHexasWithinRange(int x, int y, int charx, int chary, int AoERange, CharsDB.AoEType aoEType)
        {
            int x2 = x;
            int y2 = y;
            List<Point> pList = new List<Point>();
            if (aoEType == CharsDB.AoEType.GLOBAL)
            {
                for (int j = 0; j <= AoERange; j++)
                {
                    int iMin = -AoERange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = AoERange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {//x2+....
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 + i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                    x2++;
                }

                x2 = x - 1;
                for (int j = 1; j <= AoERange; j++)
                {
                    int iMin = -AoERange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = AoERange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            { //x2 + ....
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 + i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                    x2--;

                }
            }


            if (aoEType == CharsDB.AoEType.STRG)
            {
                x2 = x;
                y2 = y;
                if (charx == x2 && chary == y2) pList.Add(new Point(x2, y2));

                else if (chary < y2)
                {
                    
                    for (int j = 0; j <= AoERange; j++)
                    {
                        int iMin = 0;
                        int iMax = AoERange;

                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            { //x2 + ....
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 + i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }

                    }


                }
                else if (chary == y2 && charx % 2 == 0)
                {
                    
                    for (int j = 0; j <= AoERange; j++)
                    {
                        int iMin = 0;
                        int iMax = AoERange;

                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 - i >= 0 && y2 - i < h)
                            { //x2 + ....
                                switch (hexaList[x2 + (y2 - i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 - i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                }

                else if (chary == y2 && charx % 2 == 1)
                {
                    for (int j = 0; j <= AoERange; j++)
                    {
                        int iMin = 0;
                        int iMax = AoERange;

                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            { //x2 + ....
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 + i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                }


                else if (chary > y2)
                {
                  
                    for (int j = 0; j <= AoERange; j++)
                    {
                        int iMin = 0;
                        int iMax = AoERange;

                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 - i >= 0 && y2 - i < h)
                            { //x2 + ....
                                switch (hexaList[x2 + (y2 - i) * w].type)
                                {
                                    case HexaType.GROUND: pList.Add(new Point(x2, y2 - i)); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }


                    }
                }
            }

            return pList;
        }


        /** Returns the list of characters within AoERange of position (x,y). **/
        public List<Character> getCharWithinRange(int x, int y, int charx, int chary, int AoERange, CharsDB.AoEType aoEType)
        {
            int x2 = x;
            int y2 = y;
            List<Character> charList_ = new List<Character>();

            //pour les aoe global
            if (aoEType == CharsDB.AoEType.GLOBAL)
            {
                for (int j = 0; j <= AoERange; j++)
                {
                    int iMin = -AoERange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = AoERange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: if (hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                    x2++;
                }

                x2 = x - 1;
                for (int j = 1; j <= AoERange; j++)
                {
                    int iMin = -AoERange + ((j + ((x + 1) % 2)) / 2);
                    int iMax = AoERange - ((j + (x % 2)) / 2);
                    if (x2 >= 0 && x2 < w)
                    {
                        for (int i = iMin; i <= iMax; i++)
                        {
                            if (y2 + i >= 0 && y2 + i < h)
                            {
                                switch (hexaList[x2 + (y2 + i) * w].type)
                                {
                                    case HexaType.GROUND: if (hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                    case HexaType.VOID: break;
                                    case HexaType.WALL: break;
                                }
                            }
                        }
                    }
                    x2--;
                }
            }

            //pour les aoe en ligne
            else if (aoEType == CharsDB.AoEType.STRG)
            {
                x2 = x;
                y2 = y;


                if (chary < y2)
                {
                    int iMin = 0;
                    int iMax = AoERange;

                    for (int i = iMin; i <= iMax; i++)
                    {
                        if (y2 + i >= 0 && y2 + i < h)
                        { //x2 + ....
                            switch (hexaList[x2 + (y2 + i) * w].type)
                            {
                                case HexaType.GROUND: if (hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                case HexaType.VOID: break;
                                case HexaType.WALL: break;
                            }
                        }
                    }





                }
                else if (chary == y2 && charx % 2 == 0)
                {


                    int iMin = 0;
                    int iMax = AoERange;

                    for (int i = iMin; i <= iMax; i++)
                    {
                        if (y2 - i >= 0 && y2 - i < h)
                        { //x2 + ....
                            switch (hexaList[x2 + (y2 - i) * w].type)
                            {
                                case HexaType.GROUND: if (hexaList[x2 + (y2 - i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 - i) * w].charOn); break;
                                case HexaType.VOID: break;
                                case HexaType.WALL: break;
                            }
                        }
                    }


                }

                else if (chary == y2 && charx % 2 == 1)
                {


                    int iMin = 0;
                    int iMax = AoERange;

                    for (int i = iMin; i <= iMax; i++)
                    {
                        if (y2 + i >= 0 && y2 + i < h)
                        { //x2 + ....
                            switch (hexaList[x2 + (y2 + i) * w].type)
                            {
                                case HexaType.GROUND: if (hexaList[x2 + (y2 + i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 + i) * w].charOn); break;
                                case HexaType.VOID: break;
                                case HexaType.WALL: break;
                            }
                        }
                    }


                }


                else if (chary > y2)
                {

                    int iMin = 0;
                    int iMax = AoERange;

                    for (int i = iMin; i <= iMax; i++)
                    {
                        if (y2 - i >= 0 && y2 - i < h)
                        { //x2 + ....
                            switch (hexaList[x2 + (y2 - i) * w].type)
                            {
                                case HexaType.GROUND: if (hexaList[x2 + (y2 - i) * w].charOn != null) charList_.Add(hexaList[x2 + (y2 - i) * w].charOn); break;
                                case HexaType.VOID: break;
                                case HexaType.WALL: break;
                            }
                        }
                    }




                }
               
            }

            else
            {
                switch (hexaList[x + y * w].type)
                {
                    case HexaType.GROUND: if (hexaList[x + y * w].charOn != null) charList_.Add(hexaList[x + y * w].charOn); break;
                    case HexaType.VOID: break;
                    case HexaType.WALL: break;
                }
            }

            return charList_;
        }

        /** Returns the distance between two hexas. */
        public int getDistance(int x1, int y1, int x2, int y2)
        {
            int distance = 0;
            while (x1 != x2)
            {
                if (y1 > y2)
                {
                    if (x1 > x2)
                    {
                        if (x1 % 2 == 1) y1--;
                        x1--;
                    }
                    else
                    {
                        if (x1 % 2 == 1) y1--;
                        x1++;
                    }
                }
                else if (y1 < y2)
                {
                    if (x1 > x2)
                    {
                        if (x1 % 2 == 0) y1++;
                        x1--;
                    }
                    else
                    {
                        if (x1 % 2 == 0) y1++;
                        x1++;
                    }
                }
                else
                {
                    return distance + ((x1 > x2) ? (x1 - x2) : (x2 - x1));
                }
                distance++;
            }
            return distance + ((y1 > y2) ? (y1 - y2) : (y2 - y1));
        }

        /** Returns the number of steps between two hexas. */
        public int getWalkingDistance(int x1, int y1, int x2, int y2)
        {
            int maxSteps = 300;
            if (this.getHexa(x1, y1).type == HexaType.WALL || this.getHexa(x2, y2).type == HexaType.WALL) return maxSteps;
            List<HexaTemp> hexaList2 = new List<HexaTemp>();
            foreach (Hexa hexa in this.hexaList)
            {
                hexaList2.Add(new HexaGrid.HexaTemp(hexa.x, hexa.y, maxSteps + 1));
            }
            List<HexaTemp> toCheck = new List<HexaTemp>();
            toCheck.Add(new HexaTemp(x1, y1, 0));
            hexaList2[x1 + y1 * this.w].nbSteps = 0;
            int minSteps = maxSteps + 1;

            while (toCheck.Count > 0)
            {
                HexaTemp p = toCheck[0];
                toCheck.RemoveAt(0);
                if (p.nbSteps < maxSteps && p.nbSteps < minSteps)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        HexaDirection hexaDirectionI = (HexaDirection)i;
                        Point p2 = findPos(p.x, p.y, hexaDirectionI);
                        Hexa h = this.getHexa(p2);
                        if (h != null && ((h.x == x2 && h.y == y2) || (h.type == HexaType.GROUND && h.charOn == null)) && hexaList2[p2.x + p2.y * this.w].nbSteps > p.nbSteps + 1)
                        {
                            hexaList2[p2.x + p2.y * this.w].nbSteps = p.nbSteps + 1;
                            if (p2.x == x2 && p2.y == y2) minSteps = p.nbSteps + 1;
                            toCheck.Add(new HexaTemp(p2.x, p2.y, p.nbSteps + 1));
                        }
                    }
                }
            }

            return (hexaList2[x2 + y2 * this.w].nbSteps == maxSteps) ? maxSteps : hexaList2[x2 + y2 * this.w].nbSteps;
        }
    }

}