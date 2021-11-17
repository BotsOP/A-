using System.Collections;
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

            if (currentNode.position == endPos)
            {
                Debug.Log(currentNode.position + "    FOUND FINAL NODE");
                int numberOperations2 = 0;
                while (numberOperations2 < 100)
                {
                    path.Add(currentNode.position);
                    
                    if (currentNode.position == startPos)
                    {
                        break;
                    }
                    numberOperations2++;
                    currentNode = currentNode.parent;
                }
                break;
            }
            
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
        Vector2Int currentCellPos = new Vector2Int(currentNode.position.x, currentNode.position.y);
        //Debug.Log(currentCellPos);

        //make a for a loop and assign 0-3 to vectors
        if (!grid[currentCellPos.x, currentCellPos.y].HasWall(Wall.LEFT))
        {
            Vector2Int left = new Vector2Int(currentCellPos.x - 1, currentCellPos.y);
            bool stop = false;
            foreach (var node in checkedNodes)
            {
                if (node.position == left && node.checkedNode)
                {
                    //Debug.Log("stopped");
                    stop = true;
                }
            }
            if (!stop)
            {
                Node leftNode = new Node(left, currentNode, GetGScore(left, startPos), GetHScore(left, endPos));
                checkedNodes.Add(leftNode);
            }
        }
        
        if (!grid[currentCellPos.x, currentCellPos.y].HasWall(Wall.RIGHT))
        {
            Vector2Int right = new Vector2Int(currentCellPos.x + 1, currentCellPos.y);
            bool stop = false;
            foreach (var node in checkedNodes)
            {
                if (node.position == right && node.checkedNode)
                {
                    //Debug.Log("stopped");
                    stop = true;
                }
            }
            if (!stop)
            {
                Node rightNode = new Node(right, currentNode, GetGScore(right, startPos), GetHScore(right, endPos));
                checkedNodes.Add(rightNode);
            }
        }
        
        if (!grid[currentCellPos.x, currentCellPos.y].HasWall(Wall.UP))
        {
            Vector2Int up = new Vector2Int(currentCellPos.x, currentCellPos.y + 1);
            bool stop = false;
            foreach (var node in checkedNodes)
            {
                if (node.position == up && node.checkedNode)
                {
                    //Debug.Log("stopped");
                    stop = true;
                }
            }
            if (!stop)
            {
                Node upNode = new Node(up, currentNode, GetGScore(up, startPos), GetHScore(up, endPos));
                checkedNodes.Add(upNode);
            }
        }

        if (!grid[currentCellPos.x, currentCellPos.y].HasWall(Wall.DOWN))
        {
            Vector2Int down = new Vector2Int(currentCellPos.x, currentCellPos.y - 1);
            bool stop = false;
            foreach (var node in checkedNodes)
            {
                if (node.position == down && node.checkedNode)
                {
                    //Debug.Log("stopped");
                    stop = true;
                }
            }
            if (!stop)
            {
                Node downNode = new Node(down, currentNode, GetGScore(down, startPos), GetHScore(down, endPos));
                checkedNodes.Add(downNode);
            }
        }
        
        
        foreach (var node in checkedNodes)
        {
            if (lowestFScore > node.FScore && !node.checkedNode)
            {
                //Debug.Log(node.position + " lowest fscore");
                lowestFScore = node.FScore;
            }
            
        }
        
        foreach (var node in checkedNodes)
        {
            //Debug.Log(lowestFScore + "   " + node.FScore + "   " + node.checkedNode);
            if (lowestFScore == node.FScore && !node.checkedNode && node.position != currentNode.position)
            {
                //Debug.Log(node.position + "   closest node");
                node.checkedNode = true;
                return node;
            }
        }

        foreach (var node in checkedNodes)
        {
            if (!node.checkedNode && node.position.x >= 0)
            {
                lowestFScore = node.FScore;
                node.checkedNode = true;
                //Debug.Log(node.position);
                
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
