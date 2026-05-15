using Graph.Exceptions;
using Graph.Interfaces;
using Graph.Validation;

namespace Graph.Models;

public sealed class Graph : IGraph
{
    private readonly int[,] _matrix;

    public int VertexCount => _matrix.GetLength(0);

    public Graph(int[,] adjacencyMatrix)
    {
        GraphValidator.Validate(adjacencyMatrix);

        _matrix = CloneMatrix(adjacencyMatrix);
    }

    public int GetWeight(int from, int to)
    {
        ValidateVertex(from);

        ValidateVertex(to);

        return _matrix[from - 1, to - 1];
    }

    public bool HasEdge(int from, int to)
    {
        return GetWeight(from, to) > 0;
    }

    public IReadOnlyList<int> GetNeighbors(int vertex)
    {
        ValidateVertex(vertex);

        List<int> neighbors = [];

        int row = vertex - 1;

        for (int col = 0; col < VertexCount; col++)
        {
            if (_matrix[row, col] > 0)
            {
                neighbors.Add(col + 1);
            }
        }

        return neighbors;
    }
    public IReadOnlyList<Edge> GetEdges()
    {
        List<Edge> edges = [];

        for (int row = 0; row < VertexCount; row++)
        {
            for (int column = 0; column < VertexCount; column++)
            {
                int weight = _matrix[row, column];

                if (weight > 0)
                {
                    edges.Add(
                        new Edge(
                            row + 1,
                            column + 1,
                            weight));
                }
            }
        }

        return edges;
    }

    private void ValidateVertex(int vertex)
    {
        if (vertex < 1 || vertex > VertexCount)
        {
            throw new InvalidGraphException("Vertex is not vald: " + vertex);
        }
    }

    private static int[,] CloneMatrix(int[,] source)
    {
        int rows = source.GetLength(0);
        int cols = source.GetLength(1);

        int[,] clone = new int[rows, cols];

        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                clone[row, col] = source[row, col];
            }
        }

        return clone;
    }
}