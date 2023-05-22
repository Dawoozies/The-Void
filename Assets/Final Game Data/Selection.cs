using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum GeometrySelectionMode
{
    None = 0,
    Single = 1,
    Multiple = 2,
}
public class SelectionList<T>
{
    GeometrySelectionMode _selectionMode;
    List<T> _selectedItems;
    public SelectionList()
    {
        _selectionMode = GeometrySelectionMode.None;
        _selectedItems = new List<T>();
    }
    public List<T> selectedItems
    {
        get => _selectedItems;
        set
        {
            _selectedItems = value;
            Update();
        }
    }
    public GeometrySelectionMode selectionMode
    {
        get => _selectionMode;
        set
        {
            _selectionMode = value;
            Update();
        }
    }
    public bool Contains(T item)
    {
        return selectedItems.Contains(item);
    }
    public void Add(T item)
    {
        selectedItems.Add(item);
        Update();
    }
    public void Remove(T item)
    {
        selectedItems.Remove(item);
    }
    void Update()
    {
        if(_selectionMode == GeometrySelectionMode.None)
        {
            if (selectedItems == null || selectedItems.Count == 0)
                return;
            while(selectedItems.Count > 0)
            {
                selectedItems.RemoveAt(0);
            }
        }
        if (_selectionMode == GeometrySelectionMode.Single)
        {
            if (selectedItems == null || selectedItems.Count == 0 || selectedItems.Count == 1)
                return;
            while (selectedItems.Count > 1)
            {
                selectedItems.RemoveAt(0);
            }
        }
    }
}