using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class GameManager : MonoBehaviour
    {
        public Texture2D levelTexture;
        public SpriteRenderer levelRenderer;

        // posOffset est égal à 1 / pixelPerUnit du niveau
        public float posOffset = 0.01f;
        int maxX;
        int maxY;
        Node[,] grid;

        Vector3 mousePos;
        Node curNode;
        Node prevNode;

        void Start()
        {
            CreateLevel();
        }

        void CreateLevel()
        {
            maxX = levelTexture.width;
            maxY = levelTexture.height;
            grid = new Node[maxX, maxY];

            for (int x = 0; x < maxX; x++)
            {
                for (int y = 0; y < maxY; y++)
                {
                    Node n = new Node();
                    n.x = x;
                    n.y = y;

                    Color c = levelTexture.GetPixel(x, y);
                    n.isEmpty = (c.a == 0);

                    grid[x, y] = n;
                }
            }
        }

        private void Update()
        {
            GetMousePosition();
            HandleMouseInput();
        }

        void HandleMouseInput()
        {
            if (curNode == null)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if(curNode != prevNode)
                {
                    prevNode = curNode;

                    Color c = Color.white;
                    c.a = 0;

                    for (int x = -2; x < 2; x++)
                    {
                        for (int y = -2; y < 2; y++)
                        {
                            int t_x = x + curNode.x;
                            int t_y = y + curNode.y;

                            levelTexture.SetPixel(t_x, t_y, c);
                        }
                    }

                    levelTexture.Apply();
                }
            }
        }

        void GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = ray.GetPoint(5);
            curNode = GetNodeFromWorldPos(mousePos);
        }

        Node GetNodeFromWorldPos(Vector3 worldPosition)
        {
            int t_x = Mathf.RoundToInt(worldPosition.x / posOffset);
            int t_y = Mathf.RoundToInt(worldPosition.y / posOffset);

            return GetNode(t_x, t_y);
        }

        Node GetNode(int x, int y)
        {
            if (x < 0 || y < 0 || x > maxX - 1 || y > maxY - 1)
                return null;
            return grid[x, y];
        }
    }


    public class Node
    {
        public int x;
        public int y;
        public bool isEmpty;
    }
}
