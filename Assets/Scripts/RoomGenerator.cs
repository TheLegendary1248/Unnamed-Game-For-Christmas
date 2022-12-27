using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RoomGenerator : MonoBehaviour
{
    public LineRenderer lineRend;
    public EdgeCollider2D lineCol;
    public Vector2 originPt;
    public int wallCount;
    public int range;
    Edge[] x_edge;
    Edge[] y_edge;
    public GameObject refer;
    GameObject[] circles = new GameObject[] { };
    enum roomShape
    {
        Compact,
        Roundabout,
        Streets,
        Corridor,
        PerfectRectangle,
        PerfectSquare,
        PerfectCircle,
        PerfectCorner,
        Mirrored,
        QuadMirrored,
        Snake,
        PerfectWeb,
        AlignedWeb,
        VariedWeb,
    }
    enum roomType
    {
        OutsideCity,
        Office,
        Dungeon,
        Cavern,
        Bedroom,
        Kitchen,
    }
    void Start()
    {
        Vector3[] vec = new Vector3[wallCount];
        y_edge = x_edge = new Edge[wallCount / 2];
        vec[0] = Vector2.zero;
        /*
        for (int i = 1; i < vec.Length; i++)
        {
            Vector2 s = Vector2.zero;
            s[i % 2] = vec[i - 1][i % 2]; //Grab the x or y to get perpendicular walls
            int r = Random.Range(-range, range);
            r += (int)Mathf.Sign(r);
            s[(i + 1) % 2] = vec[i - 1][(i + 1) % 2] + r;
            vec[i] = s;
            (i % 2 == 1 ? y_edge : x_edge)[i / 2] = new Edge((int)s[i % 2], (int)s[i % 2], (int)vec[i][i % 2]);
        }
        SetPositions(vec);
        
        x_edge.OrderBy(x => x.startVal);
        y_edge.OrderBy(x => x.startVal);*/
        foreach (GameObject o in circles)
        {
            Destroy(o);
        }
        switch (Random.Range(0, 6))
        {
            case 0:
                PerfectRectangle();
                break;
            case 1:
                PerfectSquare();
                break;
            case 2:
                PerfectCircle();
                break;
            case 3:
                Corridor();
                break;
            case 4:
                PerfectCorner();
                break;
            case 5:
                JaggedPerpendiculars();
                break;
        }
        Pill();
        Debug.Log(originPt);
        Player.single.transform.position = originPt;
        
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) Start();
        if (Input.GetKeyDown(KeyCode.M))
        {
            RaycastHit2D r = Physics2D.Raycast(Player.pos, Random.insideUnitCircle.normalized);
            Debug.DrawLine(Player.pos, r.point, Color.red, 3f);
            for (int i = 0; i < 20; i++)
            {
                Vector2 last = r.point;
                Vector2 randDir;
                do { randDir = Random.insideUnitCircle.normalized; } while (Vector2.Dot(r.normal, randDir) < 0f);
                r = Physics2D.Raycast(r.point + (r.normal / 100), randDir);
                Debug.DrawLine(last, r.point, new Color(1,0,0,0.4f), 3f);
            }
            Debug.DrawLine(Player.pos, r.point, Color.green, 3f);
        }
    }
    #region RoomGenerationFuncs
    void PerfectRectangle() //Creates a rectangular room
    {
        float w = Random.Range(2.5f, 5);
        float h = Random.Range(2.5f, 9);
        SetPositions(new Vector3[] { new Vector3(-w, h), new Vector3(w, h), new Vector3(w, -h), new Vector3(-w, -h), new Vector3(-w, h) });
    }
    void PerfectSquare() //Creates a square room
    {
        float l = Random.Range(2.5f, 8);
        SetPositions(new Vector3[] { new Vector3(l, -l), new Vector3(l, l), new Vector3(-l, l), new Vector3(-l, -l), new Vector3(l, -l) });
    }
    void PerfectCircle() //Creates a circular room
    {
        float range = Random.Range(2f, 10);
        int res = Random.Range(5, 20);
        float offset = Random.Range(0, Mathf.PI * 2);
        Vector3[] cirPos = new Vector3[res + 1];
        for (int i = 0; i < res; i++)
        {
            float angle = ((i + offset) * Mathf.PI * 2f) / res;
            cirPos[i] = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle)) * range;
        }
        cirPos[res] = cirPos[0];
        SetPositions(cirPos);
    }
    void Corridor() //Basically PerfectRectangle, but stretched
    {
        bool longerSide = Random.value > 0.5f;
        float w = longerSide ? Random.Range(5f, 18) : Random.Range(1f, 2.5f);
        float h = longerSide ? Random.Range(1f, 2.5f) : Random.Range(5f, 18);
        SetPositions(new Vector3[] { new Vector3(-w,h), new Vector3(w,h), new Vector3(w, -h), new Vector3(-w, -h), new Vector3(-w, h) });
    }
    void Pill()
    {
        bool longerSide = Random.value > 0.5f;
        float w = longerSide ? Random.Range(5f, 18) : Random.Range(1f, 2.5f);
        float h = longerSide ? Random.Range(1f, 2.5f) : Random.Range(5f, 18);
        List<Vector3> v = new List<Vector3>();
        v.AddRange(new Vector3[] { new Vector3(-w,h), new Vector3(w,h), new Vector3(w,-h), new Vector3(-w,-h) });
    }
    void HalfPill()
    {

    }
    void JaggedPerpendiculars()
    {
        byte quadrantCheck = 0;
        Vector2[] pts = new Vector2[Random.Range(6, 27)]; //Create reference point array
        circles = new GameObject[pts.Length];
        retry:
        for (int i = 0; i < pts.Length; i++)
        {
            pts[i] = Random.insideUnitCircle.normalized * Random.Range(4f, 24); //Create a bunch of random points
            quadrantCheck |= (byte)(1 << ((pts[i].x > 0 ? 1 : 0) + (pts[i].y > 0 ? 2 : 0)));
        }
        Debug.Log(quadrantCheck);
        if(quadrantCheck != 15) { Debug.LogWarning("Failed to create around origin room"); goto retry; }
        pts = pts.OrderBy(v => Mathf.Atan2(v.y , v.x)).ToArray(); //Sort as to reduce overlaps
        Vector3[] wallPts = new Vector3[(pts.Length * 2) + 1]; 
        bool det = Random.value > 0.5f;
        for (int i = 0; i < pts.Length; i++)
        {
            wallPts[i * 2] = pts[i];
            circles[i] = Instantiate(refer, pts[i], Quaternion.identity);
            wallPts[(i * 2) + 1] = new Vector3(det ? pts[i].x : pts[(i + 1) % pts.Length].x, !det ? pts[i].y : pts[(i + 1) % pts.Length].y);
        }
        wallPts[wallPts.Length - 1] = wallPts[0];
        SetPositions(wallPts);
        originPt = Vector2.zero;
    }
    void PerfectCorner()
    {
        float w = Random.Range(3.5f, 8);
        float h = Random.Range(3.5f, 8);
        Vector3 c = new Vector3(Random.Range(-w + 2, w - 2), Random.Range(-h + 2, h - 2));
        List<Vector3> pos = new List<Vector3>(new Vector3[] { new Vector3(-w, h), new Vector3(w, h), new Vector3(w, -h), new Vector3(-w, -h), new Vector3(-w, h) });
        int quad = Random.Range(0, 4);
        bool flip = quad % 2 == 1;
        pos.InsertRange(quad, new Vector3[] {
            new Vector3((flip ? c : pos[(quad + 3) % 4]).x, (flip ? pos[(quad + 3) % 4] : c).y) , 
            c , 
            new Vector3((flip ? pos[quad] : c).x,(flip ? c : pos[quad]).y)});
        pos.RemoveAt((quad + 3) % 7);
        originPt = Vector2.Lerp(pos[quad + 1], pos[(quad + 4) % 7], 0.5f);
        SetPositions(pos.ToArray());

    }
    void Roundabout()
    {
         
    }
    #endregion
    void SetPositions(Vector3[] m)
    {
        lineRend.positionCount = m.Length - 1;
        lineRend.SetPositions(m);
        lineCol.points = System.Array.ConvertAll(m, x => (Vector2)x);
    }
    void SetPoint(Vector2 m, int index)
    {
        lineRend.SetPosition(index, m);
        lineCol.points[index] = m;
    }
    public struct Edge
    {
        public int offset; //The offset of the edge on the other axis
        public int startVal; //The start point of the edge on this axis
        public int endVal; //The end point of the edge on this axis
        public Edge(int offs, int start, int end)
        {
            offset = offs;
            startVal = start;
            endVal = end;
        }
    }
}
