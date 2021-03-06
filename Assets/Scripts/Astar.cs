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
    
    // private Vector2Int[] directions = new []
    // {
    //     new Vector2Int(0,1), //up
    //     new Vector2Int(1,0), //right
    //     new Vector2Int(0,-1),//down
    //     new Vector2Int(-1,0) //left
    // };
    //
    // private Wall[] directionsWall = new []
    // {
    //     Wall.UP,
    //     Wall.RIGHT,
    //     Wall.DOWN,
    //     Wall.LEFT
    // };
    
    private Vector2Int[] directions = new []
    {
        new Vector2Int(0,1),  //up
        new Vector2Int(1, 1), //top right
        new Vector2Int(1,0),  //right
        new Vector2Int(1, -1),//down right
        new Vector2Int(0,-1), //down
        new Vector2Int(-1,-1),//down left
        new Vector2Int(-1,0), //left
        new Vector2Int(-1,1)  //up left
    };
    
    private Vector2Int[] directionsWall = new []
    {
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,2),
        new Vector2Int(2,2),
        new Vector2Int(2,3),
        new Vector2Int(3,3),
        new Vector2Int(3,0)
    };
    
    private Vector2Int[] directionsWallInverse = new []
    {
        new Vector2Int(2,2),
        new Vector2Int(2,3),
        new Vector2Int(3,3),
        new Vector2Int(3,0),
        new Vector2Int(0,0),
        new Vector2Int(0,1),
        new Vector2Int(1,1),
        new Vector2Int(1,2)
    };
    
    private Wall[] directionsWallEnum = new []
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

        Node currentNode = new Node(startPos, null, 0, GetDistance(startPos, endPos));
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
            if (numberOperations > 700)
            {
                Debug.LogError("finding path took more then 700 operations");
                break;
            }
            numberOperations++;
        }
        
        path.Reverse();
        return path;
    }

    private Node CheckClosestNode(Node currentNode, Cell[,] grid, Vector2Int startPos, Vector2Int endPos)
    {
        //to optimize further add diagonals
        for (int i = 0; i < 8; i++)
        {
            Vector2Int wallDirection = directionsWall[i];
            Vector2Int wallDirectionInverse = directionsWallInverse[i];
            Vector2Int currentDir = new Vector2Int(currentNode.position.x + directions[i].x, currentNode.position.y + directions[i].y);
            
            if (currentDir.x >= 0 && currentDir.x < 10 && currentDir.y >= 0 && currentDir.y < 10)
            {
                if (!grid[currentNode.position.x, currentNode.position.y].HasWall(directionsWallEnum[wallDirection.x]) && !grid[currentNode.position.x, currentNode.position.y].HasWall(directionsWallEnum[wallDirection.y]) &&
                    !grid[currentDir.x, currentDir.y].HasWall(directionsWallEnum[wallDirectionInverse.x]) && !grid[currentDir.x, currentDir.y].HasWall(directionsWallEnum[wallDirectionInverse.y])
                )
                {
                    bool stop = false;
                    foreach (var node in checkedNodes)
                    {
                        if (node.position == currentDir)
                        {
                            stop = true;
                        }
                    }
                    if (!stop)
                    {
                        Node node = new Node(currentDir, currentNode, GetDistance(currentDir, startPos), GetDistance(currentDir, endPos));
                        checkedNodes.Add(node);
                    }
                }
            }
        }

        return lowestFScoreNode();
    }
    
    private Node lowestFScoreNode()
    {
        int numberOfOperations = 0;

        while (numberOfOperations < 50)
        {
            findLowestFScore();
            
            foreach (var node in checkedNodes)
            {
                if (lowestFScore >= node.FScore && !node.checkedNode)
                {
                    Debug.Log(node.position + "    FScore: " + node.FScore + "   closest node    " + node.checkedNode + "     " + checkedNodes.Count);
                    node.checkedNode = true;
                    return node;
                }
            }

            lowestFScore = 1000;
            
            numberOfOperations++;
        }

        Debug.LogError("couldnt match a node with the lowestFScore");
        return null;
    }

    private void findLowestFScore()
    {
        foreach (var node in checkedNodes)
        {
            if (lowestFScore > node.FScore && !node.checkedNode)
            {
                lowestFScore = node.FScore;
            }
        }
    }
    
    private float GetDistance(Vector2Int currentPos, Vector2Int endPos)
    {
        return Vector2Int.Distance(currentPos, endPos);
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
        public Node(Vector2Int position, Node parent, float GScore, float HScore)
        {
            this.position = position;
            this.parent = parent;
            this.GScore = GScore;
            this.HScore = HScore;
        }
    }
}
