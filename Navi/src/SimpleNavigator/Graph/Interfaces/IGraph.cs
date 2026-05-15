using Graph.Models;

namespace Graph.Interfaces;

public interface IGraph
{
    int VertexCount { get; }    

    bool HasEdge(int from, int to);

    int GetWeight(int from, int to);

    IReadOnlyList<int> GetNeighbors(int vertex);

    IReadOnlyList<Edge> GetEdges();

}