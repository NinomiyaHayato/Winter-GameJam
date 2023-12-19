using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 2��Transform��position��y���W�𐅕��ɕ␳�����l��p���āAfrom����to���������������v�Z����B
    /// </summary>
    public static Vector3 NormalizedDirection(this Transform from, Transform to)
    {
        Vector3 a = new(from.transform.position.x, 0, from.transform.position.z);
        Vector3 b = new(to.position.x, 0, to.position.z);
        return (b - a).normalized;
    }
}
