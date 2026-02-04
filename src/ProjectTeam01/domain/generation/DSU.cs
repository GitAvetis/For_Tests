namespace ProjectTeam01.domain.generation;

/// Система непересекающихся множеств (DSU) для построения минимального остовного дерева
public class DSU
{
    private int[] _parent;
    private int[] _rank;

    public DSU(int size)
    {
        _parent = new int[size];
        _rank = new int[size];
        MakeSets();
    }

    /// Инициализирует DSU - каждая комната в своем множестве
    private void MakeSets()
    {
        for (int i = 0; i < _parent.Length; i++)
        {
            _parent[i] = i;
            _rank[i] = 0;
        }
    }

    /// Находит корень множества для комнаты
    public int FindSet(int v)
    {
        if (v == _parent[v])
            return v;
        return _parent[v] = FindSet(_parent[v]);
    }

    /// Объединяет два множества
    public bool UnionSets(int v, int u)
    {
        v = FindSet(v);
        u = FindSet(u);

        if (u == v)
            return false;

        if (_rank[u] >= _rank[v])
        {
            _parent[v] = u;
            if (_rank[u] == _rank[v])
                _rank[u]++;
        }
        else
        {
            _parent[u] = v;
        }

        return true;
    }

    /// Проверить, что все элементы находятся в одном множестве (граф связный)
    public bool IsConnected()
    {
        if (_parent.Length == 0)
            return true;

        int root = FindSet(0);
        for (int i = 1; i < _parent.Length; i++)
        {
            if (FindSet(i) != root)
                return false;
        }
        return true;
    }

    /// Получить количество компонент связности
    public int GetConnectedComponentsCount()
    {
        var roots = new HashSet<int>();
        for (int i = 0; i < _parent.Length; i++)
        {
            roots.Add(FindSet(i));
        }
        return roots.Count;
    }
}

