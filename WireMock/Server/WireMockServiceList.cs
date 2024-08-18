using System.Collections;
using JetBrains.Annotations;

namespace WireMock.Server;

/// <summary>
/// Represents a list of WireMock services.
/// Provides events to adding and removing of items to the list
/// </summary>
public class WireMockServiceList : IList<WireMockService>
{
    private readonly IList<WireMockService> _list = new List<WireMockService>();

    public WireMockService this[int index]
    {
        get => _list[index];
        set => _list[index] = value;
    }

    /// <summary>
    /// Gets the number of elements in the WireMockServiceList.
    /// </summary>
    /// <returns>The number of elements in the WireMockServiceList.</returns>
    public int Count => _list.Count;

    /// <summary>
    /// Gets a value indicating whether the WireMockServiceList is read-only.
    /// </summary>
    /// <value>
    /// <c>true</c> if the WireMockServiceList is read-only; otherwise, <c>false</c>.
    /// </value>
    public bool IsReadOnly => _list.IsReadOnly;


    public event EventHandler<ChangedMappingsEventArgs>? MappingRemoved;


    /// <summary>
    /// Adds a WireMockService to the WireMockServiceList.
    /// </summary>
    /// <param name="item">The WireMockService to add.</param>
    public void Add(WireMockService item)
    {
        item.MappingsAdded += OnMappingAdded;
        item.MappingsRemoved += OnMappingRemoved;
        _list.Add(item);
    }

    /// <summary>
    /// Removes a WireMockService from the WireMockServiceList.
    /// </summary>
    /// <param name="item">The WireMockService to remove.</param>
    /// <returns>Returns a boolean value indicating whether the WireMockService was successfully removed or not.</returns>
    public bool Remove(WireMockService item)
    {
        item.MappingsAdded -= OnMappingAdded;
        item.MappingsRemoved -= OnMappingRemoved;
        return _list.Remove(item);
    }

    /// <summary>
    /// Removes all WireMockServices from the WireMockServiceList.
    /// </summary>
    public void Clear()
    {
        foreach (var service in _list)
            Remove(service);

        _list.Clear();
    }


    public event EventHandler<ChangedMappingsEventArgs>? MappingAdded;

    private void OnMappingAdded(object? sender, ChangedMappingsEventArgs args)
        => MappingAdded?.Invoke(sender ?? this, args);

    private void OnMappingRemoved(object? sender, ChangedMappingsEventArgs args)
        => MappingRemoved?.Invoke(sender ?? this, args);

    public bool Contains(WireMockService item)
        => _list.Contains(item);


    public void CopyTo(WireMockService[] array, int arrayIndex)
        => _list.CopyTo(array, arrayIndex);

    [MustDisposeResource]
    public IEnumerator<WireMockService> GetEnumerator()
        => _list.GetEnumerator();


    public int IndexOf(WireMockService item)
        => _list.IndexOf(item);


    public void Insert(int index, WireMockService item)
        => _list.Insert(index, item);


    public void RemoveAt(int index)
        => _list.RemoveAt(index);

    [MustDisposeResource]
    IEnumerator IEnumerable.GetEnumerator()
        => _list.GetEnumerator();
}