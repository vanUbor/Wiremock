using System.Collections;
using WireMock.Server;

public class WireMockServiceList : IList<WireMockService>
{
    private readonly IList<WireMockService> _list = new List<WireMockService>();

    public WireMockService this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    public int Count => _list.Count;

    public bool IsReadOnly => _list.IsReadOnly;

    public EventHandler<ChangedMappingsArgs>? MappingAdded;
    public EventHandler<ChangedMappingsArgs>? MappingRemoved;

    public void Add(WireMockService item)
    {
        item.MappingsAdded += MappingAdded;
        item.MappingsRemoved += MappingRemoved;
        _list.Add(item);
    }

    public bool Remove(WireMockService item)
    {
        item.MappingsAdded -= MappingAdded;
        item.MappingsRemoved -= MappingRemoved;
        return _list.Remove(item);
    }

    public void Clear()
    {
        _list.Clear();
    }

    public bool Contains(WireMockService item)
    {
        return _list.Contains(item);
    }

    public void CopyTo(WireMockService[] array, int arrayIndex)
    {
        _list.CopyTo(array, arrayIndex);
    }

    public IEnumerator<WireMockService> GetEnumerator()
    {
        return _list.GetEnumerator();
    }

    public int IndexOf(WireMockService item)
    {
        return _list.IndexOf(item);
    }

    public void Insert(int index, WireMockService item)
    {
        _list.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        _list.RemoveAt(index);
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _list.GetEnumerator();
    }
}