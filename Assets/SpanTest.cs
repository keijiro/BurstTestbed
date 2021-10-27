using Unity.Burst;
using UnityEngine;
using System;

sealed class SpanTest : MonoBehaviour
{
    void Start()
      => Debug.Log(SpanTestRunner.Run());
}

[BurstCompile]
static class SpanTestRunner
{
    [BurstCompile]
    public static float Run()
    {
        var span = (Span<float>)(stackalloc float[10]);
        FillSpan(span);
        return span[span.Length - 1];
    }

    static void FillSpan(Span<float> span)
    {
        for (var i = 0; i < span.Length; i++) span[i] = 0.1f * i;
    }
}
