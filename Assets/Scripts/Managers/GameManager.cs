using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace SA
{
    public class GameManager : MonoBehaviour
    {
        public Texture2D levelTexture;
        Texture2D textureInstance;
        public SpriteRenderer levelRenderer;

        public float posOffset = 0.01f;
        int maxX;
        int maxY;
        Node[,] grid;

        Vector3 mousePos;
        Node curNode;
        Node prevNode;

        public Transform fillDebugObj;
        public bool addFill;
        public int pixelsOut;
        public int maxPixels;
        float f_t;
        float p_t;

        public Transform spawnTransform;
        [HideInInspector]
        public Node spawnNode;
        [HideInInspector]
        public Vector3 spawnPosition;

        Unit curUnit;

        public Color buildColor = Color.blue;
        public Color fillColor = Color.cyan;
        public float editRadius = 6;
        public bool overUIElement;

        UnitManager unitManager;
        UIManager uIManager;
        bool applyTexture;

        public static GameManager singleton;

        void Awake()
        {
            singleton = this;
        }

        void Start()
        {
            unitManager = UnitManager.singleton;
            uIManager = UIManager.singleton;
            CreateLevel();
            spawnNode = GetNodeFromWorldPos(spawnTransform.position);
            spawnPosition = GetWorldPosFromNode(spawnNode);
        }

        void CreateLevel()
        {
            maxX = levelTexture.width;
            maxY = levelTexture.height;
            grid = new Node[maxX, maxY];
            textureInstance = new Texture2D(maxX, maxY);
            textureInstance.filterMode = FilterMode.Point;

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Node n = new Node();
                    n.x = x;
                    n.y = y;

                    Color c = levelTexture.GetPixel(x, y);
                    textureInstance.SetPixel(x, y, c);
                    n.isEmpty = (c.a == 0);

                    grid[x, y] = n;
                }
            }

            textureInstance.Apply();
            Rect rect = new Rect(0, 0, maxX, maxY);
            levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero, 100, 0, SpriteMeshType.FullRect);
        }

        void Update()
        {
            overUIElement = EventSystem.current.IsPointerOverGameObject();
            GetMousePosition();
            CheckForUnit();
            uIManager.Tick();
            HandleUnit();

            if (addFill)
            {
                DebugFill();
            }

            HandleFillNodes();
            ClearListOfPixels();

            BuildListOfNodes();            

            if (applyTexture)
                textureInstance.Apply();

            //HandleMouseInput();
        }

        void HandleUnit()
        {
            if (curUnit == null)
                return;

            if (Input.GetMouseButtonUp(0))
            {
                if (uIManager.targetAbility == Ability.walker)
                    return;

                if(curUnit.curAbility == Ability.walker)
                    curUnit.ChangeAbility(uIManager.targetAbility);
            }
        }

        void HandleMouseInput()
        {
            if (curNode == null)
                return;

            if (Input.GetMouseButton(0))
            {
                if (curNode != prevNode)
                {
                    prevNode = curNode;

                    Color c = Color.white;
                    c.a = 0;

                    Vector3 center = GetWorldPosFromNode(curNode);
                    float radius = editRadius * posOffset;

                    for (int x = -6; x < 6; x++)
                    {
                        for (int y = -6; y < 6; y++)
                        {
                            int t_x = x + curNode.x;
                            int t_y = y + curNode.y;

                            float d = Vector3.Distance(center, GetWorldPosFromNode(t_x, t_y));
                            if (d > radius)
                                continue;

                            Node n = GetNode(t_x, t_y);
                            if (n == null)
                                continue;

                            //n.isEmpty = true;
                            textureInstance.SetPixel(t_x, t_y, buildColor);  
                        }
                    }

                    applyTexture = true;
                }
            }
        }

        void CheckForUnit()
        {
            mousePos.z = 0;

            curUnit = unitManager.GetClosest(mousePos);
            if (curUnit == null)
            {
                uIManager.overUnit = false;
                return;
            }
            uIManager.overUnit = true;
        }

        void GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = ray.GetPoint(5);
            curNode = GetNodeFromWorldPos(mousePos);
        }


        // Edit functions
        List<Node> clearNodes = new List<Node>();
        List<Node> buildNodes = new List<Node>();
        List<FillNode> fillNodes = new List<FillNode>();

        public void AddCanidateNodesToClear(List<Node> l)
        {
            clearNodes.AddRange(l);
        }

        public void ClearListOfPixels()
        {
            if (clearNodes.Count == 0)
                return;

            Color c = Color.white;
            c.a = 0;

            for (int i = 0; i < clearNodes.Count; i++)
            {
                clearNodes[i].isEmpty = true;
                clearNodes[i].isFiller = false;
                textureInstance.SetPixel(clearNodes[i].x, clearNodes[i].y, c);
            }

            clearNodes.Clear();
            applyTexture = true;
        }

        public void AddCanidatesNodesToBuild(List<Node> l)
        {
            buildNodes.AddRange(l);
        }

        void BuildListOfNodes()
        {
            if (buildNodes.Count == 0)
                return;

            for (int i = 0; i < buildNodes.Count; i++)
            {
                buildNodes[i].isEmpty = false;
                textureInstance.SetPixel(buildNodes[i].x, buildNodes[i].y, buildColor);
            }

            buildNodes.Clear();
            applyTexture = true;
        }

        void DebugFill()
        {
            if(pixelsOut > maxPixels)
            {
                addFill = false;
                return;
            }

            p_t += Time.deltaTime;

            if (p_t > 0.05f)
            {
                pixelsOut++;
                p_t = 0;
            }
            else
            {
                return;
            }

            Node n = GetNodeFromWorldPos(fillDebugObj.position);
            FillNode f = new FillNode();
            f.x = n.x;
            f.y = n.y;
            fillNodes.Add(f);
            applyTexture = true;
        }

        public void AddFillNodes(FillNode f)
        {
            fillNodes.Add(f);
        }

        void HandleFillNodes()
        {
            f_t += Time.deltaTime;

            if(f_t > 0.05f)
            {
                f_t = 0;
            }
            else
            {
                return;
            }

            if (fillNodes.Count == 0)
                return;

            for (int i = 0; i < fillNodes.Count; i++)
            {
                FillNode f = fillNodes[i];
                Node cn = GetNode(f.x, f.y);
                cn.isFiller = true;

                int _y = f.y;
                _y -= 1;

                Node d = GetNode(f.x, _y);
                if (d == null)
                {
                    fillNodes.Remove(f);
                    continue;
                }

                if (d.isEmpty)
                {
                    d.isEmpty = false;
                   // d.isFiller = true;
                    textureInstance.SetPixel(d.x, d.y, fillColor);
                    f.y = _y;
                    clearNodes.Add(cn);
                }
                else
                {
                    Node df = GetNode(f.x - 1, _y);
                    if (df.isEmpty)
                    {
                        textureInstance.SetPixel(df.x, df.y, fillColor);
                        f.y = _y;
                        f.x -= 1;
                        df.isEmpty = false;
                    //    df.isFiller = true;
                        clearNodes.Add(cn);
                    }
                    else
                    {
                        Node bf = GetNode(f.x + 1, _y);

                        if (bf.isEmpty)
                        {
                            bf.isEmpty = false;
                          //  bf.isFiller = true;
                            textureInstance.SetPixel(bf.x, bf.y, fillColor);
                            f.y = _y;
                            f.x += 1;
                            clearNodes.Add(cn);
                        }
                        else
                        {
                            f.t++;
                            if (f.t > 15)
                            {
                                Node _cn = GetNode(f.x, f.y);
                                _cn.isFiller = false;
                                fillNodes.Remove(f);
                            }
                        }
                    }

                    /*int _x1 = (f.movingLeft) ? -1 : 1;
                    int _x2 = (f.movingLeft) ? 1 : -1;

                    Node df = GetNode(f.x + _x1, _y);
                    if (df.isEmpty)
                    {
                        df.isEmpty = false;
                        textureInstance.SetPixel(df.x, df.y, fillColor);
                        f.y = _y;
                        f.x += _x1;
                        clearNodes.Add(cn);
                    }
                    else
                    {
                        Node db = GetNode(f.x + _x2, _y);
                        if (db.isEmpty)
                        {
                            db.isEmpty = false;
                            textureInstance.SetPixel(db.x, db.y, fillColor);
                            f.y = _y;
                            f.x += _x2;
                            clearNodes.Add(db);
                        }
                        else
                        {
                            f.t++;
                            if(f.t > 5)
                            {
                                fillNodes.Remove(f);
                            }
                        }
                    }*/
                }
            }
        }

        //Node functions
        public Node GetNodeFromWorldPos(Vector3 wp)
        {
            int t_x = Mathf.RoundToInt(wp.x / posOffset);
            int t_y = Mathf.RoundToInt(wp.y / posOffset);

            return GetNode(t_x, t_y);
        }

        public Node GetNode(int x, int y)
        {
            if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
                return null;
            return grid[x, y];
        }

        public Vector3 GetWorldPosFromNode(int x, int y)
        {
            Vector3 r = Vector3.zero;
            r.x = x * posOffset;
            r.y = y * posOffset;
            return r;
        }

        public Vector3 GetWorldPosFromNode(Node n)
        {
            if (n == null)
                return -Vector3.one;

            Vector3 r = Vector3.zero;
            r.x = n.x * posOffset;
            r.y = n.y * posOffset;
            return r;
        }
    }

    public class Node
    {
        public int x;
        public int y;
        public bool isEmpty;
        public bool isStoped;
        public bool isFiller;
    }

    public class FillNode
    {
        public int x;
        public int y;
        public int t;
        public bool movingLeft;

    }
}
