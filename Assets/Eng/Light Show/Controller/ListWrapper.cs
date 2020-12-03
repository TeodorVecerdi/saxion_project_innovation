using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class ListWrapper<T> : IList<T> {
    public List<T> List;
    public ListWrapper(List<T> list) {
        List = list;
    }

    public IEnumerator<T> GetEnumerator() {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator() {
        return GetEnumerator();
    }

    public void Add(T item) {
        List.Add(item);
    }

    public void Clear() {
        List.Clear();
    }

    public bool Contains(T item) {
        return List.Contains(item);
    }

    public void CopyTo(T[] array, int arrayIndex) {
        List.CopyTo(array, arrayIndex);
    }

    public bool Remove(T item) {
        return List.Remove(item);
    }

    public int Count => List.Count;
    public bool IsReadOnly => false;
    public int IndexOf(T item) {
        return List.IndexOf(item);
    }

    public void Insert(int index, T item) {
        List.Insert(index, item);
    }

    public void RemoveAt(int index) {
        List.RemoveAt(index);
    }

    public T this[int index] {
        get => List[index];
        set => List[index] = value;
    }
}

[Serializable]
public class LightShowList : ListWrapper<LightShowController> {
    public LightShowList(List<LightShowController> list) : base(list) { }
}