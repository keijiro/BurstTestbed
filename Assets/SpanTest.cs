using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using System;

sealed class SpanTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _target = null;

    Texture2D _texture;

    void Start()
      => _target.texture = _texture =
           new Texture2D(_size, _size, TextureFormat.RGBA32, false);

    void Update()
    {
        using var buffer = new NativeArray<uint>(_size * _size, Allocator.Temp);
        TextureGenerator2.Update(buffer, _size, Time.time);
        _texture.LoadRawTextureData(buffer);
        _texture.Apply();
    }
}

[BurstCompile]
static class TextureGenerator2
{
    unsafe public static void Update(NativeArray<uint> buffer, int size, float time)
      => Update_Burst(NativeArrayUnsafeUtility.GetUnsafePtr(buffer), size, time);

    [BurstCompile]
    unsafe public static void Update_Burst(void* pointer, int size, float time)
    {
        var buffer = new Span<uint>(pointer, size * size);
        var offs = 0;
        for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                buffer[offs++] = GetPixel(x, y, time);
    }

    static uint GetPixel(int x, int y, float t)
    {
        var pos = math.float3(x, y, t) * math.float3(0.008f, 0.008f, 0.5f);
        var f32 = noise.snoise(pos) * 0.4f + 0.5f;
        var un8 = (uint)(math.saturate(f32) * 255);
        return un8 | un8 << 8 | un8 << 16 | 0xff000000;
    }
}
