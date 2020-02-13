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
    public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos, Cell[,] grid)
    {
        List<Node> nodeGrid = new List<Node>();

        Vector2Int gridSize = new Vector2Int(grid.GetLength(0), grid.GetLength(1));
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int pos = new Vector2Int(x, y);
                nodeGrid.Add(new Node(pos, null, int.MaxValue, CalculateDistanceCost(pos, endPos)));
            }
        }

        Node startNode = nodeGrid.Find(cell => cell.position == startPos);
        startNode.GScore = 0;

        List<Node> openList = new List<Node>();
        List<Node> closedList = new List<Node>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            Node current = GetLowestFScoreNode(openList);
            if (current.position == endPos)
            {
                Debug.Log("met endpos");
                break;
            }

            openList.Remove(current);

            closedList.Add(current);
            List<Node> neighbours = GetNeighbours(current, grid, nodeGrid, closedList);
            Debug.Log(neighbours.Count);

            foreach (Node neighbour in neighbours)
            {
                int tentativeGScore = (int)current.GScore + CalculateDistanceCost(current.position, neighbour.position);
                if (tentativeGScore < neighbour.GScore)
                {
                    neighbour.parent = current;
                    neighbour.GScore = tentativeGScore;

                    if (!openList.Contains(neighbour))
                    {
                        openList.Add(neighbour);
                    }
                }
            }
        }

        Node endNode = nodeGrid.Find(cell => cell.position == endPos);
        if (endNode.parent == null)
        {
            return null;
        }

        return CalculatePath(startNode, endNode);        
    }

    private List<Vector2Int> CalculatePath(Node startNode, Node endNode)
    {
        if (endNode.parent == null)
        {
            return null;
        }

        List<Vector2Int> path = new List<Vector2Int>();

        Node cNode = endNode;

        while (cNode.parent != null)
        {
            path.Add(cNode.position);
            cNode = cNode.parent;
        }

        path.Reverse();
        return path;
    }

    private Node GetLowestFScoreNode(List<Node> openList)
    {
        Node lowestFNode = openList[0];

        for (int i = 1; i < openList.Count; i++)
        {
            Node cNode = openList[i];
            if (cNode.FScore < lowestFNode.FScore)
            {
                lowestFNode = cNode;
            }
        }

        return lowestFNode;
    }

    private bool CanWalkToCell(Cell fromCell, Cell toCell)
    {
        if (fromCell.gridPosition.x > toCell.gridPosition.x)
        {
            return !(fromCell.HasWall(Wall.LEFT) && toCell.HasWall(Wall.RIGHT));
        } else if(fromCell.gridPosition.x < toCell.gridPosition.x)
        {
            return !(fromCell.HasWall(Wall.RIGHT) && toCell.HasWall(Wall.LEFT));
        } else if(fromCell.gridPosition.y > toCell.gridPosition.y)
        {
            return !(fromCell.HasWall(Wall.DOWN) && toCell.HasWall(Wall.UP));
        } else
        {
            return !(fromCell.HasWall(Wall.UP) && toCell.HasWall(Wall.DOWN));
        }
    }

    private int CalculateDistanceCost(Vector2Int cPos, Vector2Int endPos)
    {
        int xDistance = Mathf.Abs(cPos.x - endPos.x);
        int yDistance = Mathf.Abs(cPos.y - endPos.y);

        return xDistance + yDistance;
    }

    private List<Node> GetNeighbours(Node cell, Cell[,] grid, List<Node> nodeGrid, List<Node> closedList)
    {
        List<Node> result = new List<Node>();
        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                int cellX = cell.position.x + x;
                int cellY = cell.position.y + y;
                if (cellX < 0 || cellX >= grid.GetLength(0) || cellY < 0 || cellY >= grid.GetLength(1) || Mathf.Abs(x) == Mathf.Abs(y))
                {
                    continue;
                }
                if (!CanWalkToCell(grid[cell.position.x, cell.position.y], grid[cellX, cellY]))
                {
                    continue;
                }
                Vector2Int canditatePos = new Vector2Int(cellX, cellY);
                Node candidateNode = nodeGrid.Find(node => node.position == canditatePos);
                if (candidateNode != null && !closedList.Contains(candidateNode))
                {
                    result.Add(candidateNode);
                }
            }
        }

        return result;
    }

    /// <summary>
    /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
    /// </summary>
    public class Node
    {
        public Vector2Int position; //Position on the grid
        public Node parent; //Parent Node of this node

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
