using System;
using System.Collections.Generic;

namespace Lumos
{
    public class PathFinding
    {
        private class Node
        {
            public int X;
            public int Y;
            public int G;
            public int H;
            public int F;
            public Node Parent;

            public Node(int x, int y)
            {
                X = x;
                Y = y;
            }
        }

        private Tile[,] _mapData;
        private int _mapWidth;
        private int _mapHeight;

        public PathFinding(Tile[,] mapData)
        {
            _mapData = mapData;
            _mapWidth = mapData.GetLength(0);
            _mapHeight = mapData.GetLength(1);
        }

        public List<(int, int)> FindPath((int, int) start, (int, int) end)
        {
            if (_mapData[start.Item1, start.Item2].Collision && _mapData[end.Item1, end.Item2].Collision)
            {
                return null;
            }
            // Create the open and closed lists
            var openList = new List<Node>();
            var closedList = new HashSet<Node>();

            // Create the start and end nodes
            var startNode = new Node(start.Item1, start.Item2);
            var endNode = new Node(end.Item1, end.Item2);

            // Add the start node to the open list
            openList.Add(startNode);
            int cycleLimit = 700;
            int cycles = 0;

            // Loop until the open list is empty
            while (openList.Count > 0 && cycles < cycleLimit)
            {
                cycles++;// Find the node with the lowest F cost in the open list
                var currentNode = openList[0];
                var currentIndex = 0;
                for (var i = 1; i < openList.Count; i++)
                {
                    if (openList[i].F < currentNode.F)
                    {
                        currentNode = openList[i];
                        currentIndex = i;
                    }
                }

                // Remove the current node from the open list and add it to the closed list
                openList.RemoveAt(currentIndex);
                closedList.Add(currentNode);

                // Check if the goal has been reached
                if (currentNode.X == endNode.X && currentNode.Y == endNode.Y)
                {
                    return GeneratePath(currentNode);
                }

                // Generate the neighboring nodes
                var neighbors = GenerateNeighbors(currentNode);

                // Process each neighboring node
                foreach (var neighbor in neighbors)
                {
                    // Skip the neighbor if it is in the closed list
                    if (closedList.Contains(neighbor))
                        continue;

                    // Calculate the G cost (movement cost from start to neighbor)
                    var gCost = currentNode.G + 1;

                    // Check if the neighbor is already in the open list
                    var inOpenList = false;
                    foreach (var openNode in openList)
                    {
                        if (openNode.X == neighbor.X && openNode.Y == neighbor.Y)
                        {
                            inOpenList = true;
                            break;
                        }
                    }

                    // If the neighbor is not in the open list or the new G cost is lower, update the neighbor
                    if (!inOpenList || gCost < neighbor.G)
                    {
                        neighbor.G = gCost;
                        neighbor.H = CalculateHeuristic(neighbor, endNode);
                        neighbor.F = neighbor.G + neighbor.H;
                        neighbor.Parent = currentNode;

                        // Add the neighbor to the open list if it's not already there
                        if (!inOpenList)
                            openList.Add(neighbor);
                    }
                }
            }

            // No path found
            return null;
        }

        private List<(int, int)> GeneratePath(Node endNode)
        {
            var path = new List<(int, int)>();
            var currentNode = endNode;

            while (currentNode != null)
            {
                path.Insert(0, (currentNode.X, currentNode.Y));
                currentNode = currentNode.Parent;
            }

            return path;
        }

        private List<Node> GenerateNeighbors(Node node)
        {
            var neighbors = new List<Node>();

            // Check the eight neighboring tiles (including diagonals)
            CheckNeighbor(node.X - 1, node.Y, neighbors);
            CheckNeighbor(node.X + 1, node.Y, neighbors);
            CheckNeighbor(node.X, node.Y - 1, neighbors);
            CheckNeighbor(node.X, node.Y + 1, neighbors);
            CheckNeighbor(node.X - 1, node.Y - 1, neighbors);
            CheckNeighbor(node.X + 1, node.Y - 1, neighbors);
            CheckNeighbor(node.X - 1, node.Y + 1, neighbors);
            CheckNeighbor(node.X + 1, node.Y + 1, neighbors);

            return neighbors;
        }

        private void CheckNeighbor(int x, int y, List<Node> neighbors)
        {
            // Check if the neighbor is within the map boundaries and is walkable
            if (x >= 0 && x < _mapWidth && y >= 0 && y < _mapHeight && !_mapData[x, y].Collision)
            {
                neighbors.Add(new Node(x, y));
            }
        }

        private int CalculateHeuristic(Node node, Node target)
        {
            // Use Manhattan distance as the heuristic
            return Math.Abs(node.X - target.X) + Math.Abs(node.Y - target.Y);
        }
    }
}