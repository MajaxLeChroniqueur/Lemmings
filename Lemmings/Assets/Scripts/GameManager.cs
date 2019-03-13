﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SA
{
    public class GameManager : MonoBehaviour
    {
        public Texture2D levelTexture;
        Texture2D textureInstance;
        public SpriteRenderer levelRenderer;

         

        // posOffset est égal à 1 / pixelPerUnit du niveau
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

        public Unit unit;

        public static GameManager singleton;
        private void Awake()
        {
            singleton = this;
        }

        void Start()
        {
            CreateLevel();
            spawnNode = GetNodeFromWorldPos(spawnTransform.position);
            spawnPosition = GetWorldPosFromNode(spawnNode);
            unit.Init(this);
        }

        void CreateLevel()
        {
            maxX = levelTexture.width;
            maxY = levelTexture.height;
            grid = new Node[maxX, maxY];
            textureInstance = levelTexture;
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

                            Node n = GetNode(t_x, t_y);

                            if(n == null)
                            {
                                continue;
                            }

                            n.isEmpty = true;
                            textureInstance.SetPixel(t_x, t_y, c);
                        }
                    }

                    textureInstance.Apply();
                }
            }
        }

        void GetMousePosition()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            mousePos = ray.GetPoint(5);
            curNode = GetNodeFromWorldPos(mousePos);
        }

        public Node GetNodeFromWorldPos(Vector3 worldPosition)
        {
            int t_x = Mathf.RoundToInt(worldPosition.x / posOffset);
            int t_y = Mathf.RoundToInt(worldPosition.y / posOffset);

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
            if ( n== null)
            {
                return -Vector3.one;
            }
            else
            {
                Vector3 r = Vector3.zero;
                r.x = n.x * posOffset;
                r.y = n.y * posOffset;
                return r;
            }
        }
    }


    public class Node
    {
        public int x;
        public int y;
        public bool isEmpty;
    }
}
