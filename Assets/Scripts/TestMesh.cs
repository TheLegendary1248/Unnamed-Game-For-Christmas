using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TestMesh : MonoBehaviour
{
    public float maskRes = 8;
    float lastRes = 8;
    public float seeThrough = 1f;
    public bool useNormal;
    Mesh m;
    Texture2D tex;
    Sprite s;

    Vector2[] bakedNormals; //represents the ray directions already calc'd ahead of time in Start
    ushort[] bakedTris;
    public float dist = 10f;
    // Start is called before the first frame update
    void Start()
    {
        SpriteMask mask = gameObject.AddComponent<SpriteMask>();
        s = Sprite.Create(tex = Texture2D.whiteTexture, new Rect(0, 0, tex.width, tex.height), Vector2.one / 2f);
        mask.sprite = s;
        RebakeGeo();
    }
    void RebakeGeo()
    {
        bakedNormals = new Vector2[(int)maskRes];
        for (int i = 0; i < bakedNormals.Length; i++)
        {
            bakedNormals[i] = new Vector2(Mathf.Sin((i * Mathf.PI * 2f) / maskRes), Mathf.Cos((i * Mathf.PI * 2f) / maskRes));
        }
        bakedTris = new ushort[(int)maskRes * 3];
        for (int i = 0; i < maskRes - 1; i++)
        {
            int index = i * 3;
            bakedTris[index + 1] = (ushort)(i + 1);
            bakedTris[index + 2] = (ushort)(i + 2);
        }
        bakedTris[bakedTris.Length - 2] = (ushort)maskRes;
        bakedTris[bakedTris.Length - 1] = 1;
    }
    private void FixedUpdate()
    {
        if (lastRes != maskRes) RebakeGeo();
        lastRes = maskRes;
        Vector2[] hits = new Vector2[bakedNormals.Length + 1];
        for (int i = 0; i < bakedNormals.Length; i++)
        {
            RaycastHit2D rh = Physics2D.Raycast(Player.pos, bakedNormals[i], dist);
            hits[i + 1] = (rh.collider ? ((rh.point - Player.pos) / dist) + (useNormal ? -rh.normal : bakedNormals[i]) / dist * seeThrough: bakedNormals[i]) + (Vector2.one * 2);
        }
        hits[0] = Vector2.one * 2;
        s.OverrideGeometry(hits, bakedTris);
        transform.position = Player.pos;
        transform.localScale = Vector2.one * dist * 100f;
    }
    private void OnDestroy()
    {
        Destroy(m);
        Destroy(s);
    }
}
