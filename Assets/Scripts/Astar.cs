﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Astar
{
    /// <summary>
    /// TODO: Implement this function so that it returns a list of Vector2Int positions which describes a path
    /// Note that you will probably need to add some helper functions
    /// from the startPos to the endPos
    /// </summary>
    /// <param name="startPos"></param>
    /// <param name="endPos"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    private List<Node> checkedNodes = new List<Node>();
    private float lowestFScore = 1000;
    
    private Vector2Int[] directions = new []
    {
        new Vector2Int(0,1), //up
        new Vector2Int(1,0), //right
        new Vector2Int(0,-1),//down
        new Vector2Int(-1,0) //left
    };
    
    private Wall[] directionsWall = new []
    {
        Wall.UP,
        Wall.RIGHT,
        Wall.DOWN,
        Wall.LEFT
    };
    
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        checkedNodes.Clear();
        lowestFScore = 1000;

        Node currentNode = new Node(startPos, null, 0, GetHScore(startPos, endPos));
        if (startPos != endPos)
        {
            currentNode.checkedNode = true;
        }
        checkedNodes.Add(currentNode);

        int numberOperations = 0;
        while (true)
        {
            currentNode = CheckClosestNode(currentNode, grid, startPos, endPos);
            
            //Gets the path from end to start when end has been reached
            if (currentNode.position == endPos)
            {
                Debug.Log(currentNode.position + "    FOUND FINAL NODE");
                while (currentNode.position != startPos)
                {
                    path.Add(currentNode.position);
                    
                    currentNode = currentNode.parent;
                }
                break;
            }
            
            //Incase it cant find the path
            if (numberOperations > 200)
            {
                Debug.LogError("finding path took more then 200 operations");
                break;
            }
            numberOperations++;
        }
        
        path.Reverse();
        return path;
    }

    private Node CheckClosestNode(Node currentNode, Cell[,] grid, Vector2Int startPos, Vector2Int endPos)
    {
        for (int i = 0; i < 4; i++)
        {
            if (!grid[currentNode.position.x, currentNode.position.y].HasWall(directionsWall[i]))
            {
                Vector2Int currentDir = new Vector2Int(currentNode.position.x + directions[i].x, currentNode.position.y + directions[i].y);
                bool stop = false;
                foreach (var node in checkedNodes)
                {
                    if (node.position == currentDir && node.checkedNode)
                    {
                        stop = true;
                    }
                }
                if (!stop)
                {
                    Node node = new Node(currentDir, currentNode, GetGScore(currentDir, startPos), GetHScore(currentDir, endPos));
                    checkedNodes.Add(node);
                }
            }
        }
        
        //check which node is closest
        foreach (var node in checkedNodes)
        {
            if (lowestFScore > node.FScore && !node.checkedNode)
            {
                lowestFScore = node.FScore;
            }
        }
        
        //return node with lowest FScore
        foreach (var node in checkedNodes)
        {
            if (lowestFScore == node.FScore && !node.checkedNode && node.position != currentNode.position)
            {
                Debug.Log(node.position + "   closest node");
                node.checkedNode = true;
                return node;
            }
        }
        
        //return node that hasnt been checked yet
        foreach (var node in checkedNodes)
        {
            if (!node.checkedNode)
            {
                lowestFScore = node.FScore;
                node.checkedNode = true;
                Debug.Log(node.position);
                
                return node;
            }
        }

        Debug.LogError("couldnt match a node with the lowestFScore found earlier");
        return null;
    }
    
    private int GetHScore(Vector2Int currentPos, Vector2Int endPos)
    {
        int x = Mathf.Abs(currentPos.x - endPos.x);
        int y = Mathf.Abs(currentPos.y - endPos.y);
        return x + y;
    }

    private int GetGScore(Vector2Int currentPos, Vector2Int startPos)
    {
        int x = Mathf.Abs(currentPos.x - startPos.x);
        int y = Mathf.Abs(currentPos.y - startPos.y);
        return x + y;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node
        public bool checkedNode;

        public float FScore { //GScore + HScore
            get { return GScore + HScore; }
        }
        public float GScore; //Current Travelled Distance
        public float HScore; //Distance estimated based on Heuristic

        public Node() { }
        public Node(Vector2Int position, Node parent, int GScore, int HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
