using Graph.Exceptions;

namespace Graph.Validation;

public static class GraphValidator
{
    public static void Validate(int[,] matrix)
    {
        ValidateNull(matrix);

        ValidateNotEmpty(matrix);

        ValidateSquare(matrix);

        ValidateWeights(matrix);

        ValidateConnected(matrix);
    }

    private static void ValidateNull(int[,] matrix)
    {
        if (matrix is null)
        {
            throw new InvalidGraphException("Adjacency matrix is null.");
        }
    }

    private static void ValidateNotEmpty(int[,] matrix)
    {
        if (matrix.Length == 0)
        {
            throw new InvalidGraphException(
                "Adjacency matrix is empty.");
        }

        if (matrix.GetLength(0) == 0 ||
            matrix.GetLength(1) == 0)
        {
            throw new InvalidGraphException("Adjacency matrix dimensions must be greater than zero.");
        }
    }

    private static void ValidateSquare(int[,] matrix)
    {
        int rows = matrix.GetLength(0);
        int columns = matrix.GetLength(1);

        if (rows != columns)
        {
            throw new InvalidGraphException("Adjacency matrix must be square.");
        }
    }

    private static void ValidateWeights(int[,] matrix)
    {
        int size = matrix.GetLength(0);

        for (int row = 0; row < size; row++)
        {
            for (int column = 0; column < size; column++)
            {
                ValidateWeight(matrix[row, column], row, column);
            }
        }
    }

    private static void ValidateWeight(
        int weight,
        int row,
        int column)
    {
        if (weight < 0)
        {
            throw new InvalidGraphException($"Negative edge weight detected at [{row}, {column}].");
        }
    }

    private static void ValidateConnected(int[,] matrix)
    {
        int size = matrix.GetLength(0);

        bool[] visited = new bool[size];

        DepthFirstSearch(matrix, 0, visited);

        for (int vertex = 0; vertex < size; vertex++)
        {
            if (!visited[vertex])
            {
                throw new InvalidGraphException("Adjacency matrix is disconnected.");
            }
        }
    }

    private static void DepthFirstSearch(
            int[,] matrix,
            int vertex,
            bool[] visited)
    {
        visited[vertex] = true;

        int size = matrix.GetLength(0);

        for (int neighbor = 0; neighbor < size; neighbor++)
        {
            bool hasConnection =
                matrix[vertex, neighbor] > 0 ||
                matrix[neighbor, vertex] > 0;

            if (hasConnection && !visited[neighbor])
            {
                DepthFirstSearch(
                    matrix,
                    neighbor,
                    visited);
            }
        }
    }
}