namespace Graph.Models;

public readonly record struct Edge(
    int From,
    int To,
    int Weight);