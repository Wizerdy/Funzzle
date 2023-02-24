using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IReferenceSetter<T> where T : class {
    void SetInstance(T newInstance);
}
