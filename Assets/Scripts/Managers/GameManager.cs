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

        public Transform spawnTransform;
        [HideInInspector]
        public Node spawnNode;
        [HideInInspector]
        public Vector3 spawnPosition;

        Unit curUnit;

        public float editRadius = 6;
        public bool overUIElement;

        UnitManager unitManager;
        UIManager uIManager;

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
            levelRenderer.sprite = Sprite.Create(textureInstance, rect, Vector2.zero);
        }

        void Update()
        {
            overUIElement = EventSystem.current.IsPointerOverGameObject();
            GetMousePosition();
            CheckForUnit();
            uIManager.Tick();
            HandleUnit();
            ClearListOfPixels();
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

                            n.isEmpty = true;
                            textureInstance.SetPixel(t_x, t_y, c);  
                        }
                    }

                    textureInstance.Apply();
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

        List<Node> ClearNodes = new List<Node>(); 

        public void AddCanidateNodes(List<Node> l)
        {
            ClearNodes.AddRange(l);
        }

        public void ClearListOfPixels()
        {
            if (ClearNodes.Count == 0)
                return;

            Color c = Color.white;
            c.a = 0;

            for (int i = 0; i < ClearNodes.Count; i++)
            {
                ClearNodes[i].isEmpty = true;
                textureInstance.SetPixel(ClearNodes[i].x, ClearNodes[i].y, c);
            }

            ClearNodes.Clear();
            textureInstance.Apply();
        }

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
    }
}
