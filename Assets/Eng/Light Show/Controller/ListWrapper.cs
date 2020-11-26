using System;
using System.Collections.Generic;

[Serializable]
public class ListWrapper<T> {
    public List<T> List;
    public ListWrapper(List<T> list) {
        List = list;
    }
}

[Serializable]
public class LightShowList : ListWrapper<LightShowController> {
    public LightShowList(List<LightShowController> list) : base(list) { }
}