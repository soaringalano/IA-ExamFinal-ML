using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStar
{
    // Node class representing each cell in the grid
    public class Node
    {
        public int x, y; // Grid coordinates
        public int gCost; // Cost from the start node
        public int hCost; // Heuristic cost to the target node
        public Node parent; // Parent node

        public int fCost => gCost + hCost; // Total cost

        public Node(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }

    // A* pathfinding algorithm
    public static List<Node> FindPath(int[,] grid, int startX, int startY, int targetX, int targetY)
    {
        int width = grid.GetLength(0);
        int height = grid.GetLength(1);

        // Create start and target nodes
        Node startNode = new Node(startX, startY);
        Node targetNode = new Node(targetX, targetY);

        // Initialize open and closed sets
        List<Node> openSet = new List<Node> { startNode };
        HashSet<Node> closedSet = new HashSet<Node>();

        while (openSet.Count > 0)
        {
            // Find the node with the lowest fCost in the open set
            Node currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                {
                    currentNode = openSet[i];
                }
            }

            // Move the current node from open set to closed set
            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            // Check if the target is reached
            if (currentNode.x == targetNode.x && currentNode.y == targetNode.y)
            {
                return RetracePath(startNode, targetNode);
            }

            List<Node> neighbors = GetNeighbors(currentNode, grid, width, height);
            // Explore neighbors
            if(neighbors == null)
            {
                continue;
            }
            foreach (Node neighbor in neighbors)
            {
                if (closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                    {
                        openSet.Add(neighbor);
                    }
                }
            }
        }

        // No path found
        return null;
    }

    // Retrace the path from the start to the target
    public static List<Node> RetracePath(Node startNode, Node endNode)
    {
        List<Node> path = new List<Node>();
        Node currentNode = endNode;

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }

        path.Reverse();
        return path;
    }

    // Get neighboring nodes
    public static List<Node> GetNeighbors(Node node, int[,] grid, int width, int height)
    {
        List<Node> neighbors = new List<Node>();

        for (int xOffset = -1; xOffset <= 1; xOffset++)
        {
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                if (xOffset == 0 && yOffset == 0)
                {
                    continue;
                }
                int checkX = node.x + xOffset;
                int checkY = node.y + yOffset;
                if (checkX >= 0 && checkX < width && checkY >= 0 && checkY < height)
                {
                    if (grid[checkX, checkY] == 0) // Assuming 0 represents walkable path
                    {
                        neighbors.Add(new Node(checkX, checkY));
                    }else if (grid[checkX, checkY] < 0) // Assuming negative values represent obstacles
                    {
                        continue;
                    }
                }
            }
        }

        return neighbors;
    }

    // Get the Manhattan distance between two nodes
    public static int GetDistance(Node nodeA, Node nodeB)
    {
        int dstX = Mathf.Abs(nodeA.x - nodeB.x);
        int dstY = Mathf.Abs(nodeA.y - nodeB.y);

        return dstX + dstY;
    }
}
