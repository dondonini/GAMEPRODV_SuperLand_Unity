using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMapMove
{
    Vector3 GetRootPosition();

    bool IsIgnored();

    void SegmentEnabled(bool _value);

    Vector3 GetAbsoluteScale();

    Vector3 GetAbsolutePosition();

    Bounds GetSegmentBounds();
}
