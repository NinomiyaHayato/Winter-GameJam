using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorExtensions
{
    /// <summary>
    /// 2つのTransformのpositionのy座標を水平に補正した値を用いて、fromからtoを向いた方向を計算する。
    /// </summary>
    public static Vector3 NormalizedDirection(this Transform from, Transform to)
    {
        Vector3 a = new(from.transform.position.x, 0, from.transform.position.z);
        Vector3 b = new(to.position.x, 0, to.position.z);
        return (b - a).normalized;
    }
}
