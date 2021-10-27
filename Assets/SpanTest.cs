using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using System;

[BurstCompile]
sealed class SpanTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _target = null;

    Texture2D _texture;

    void Start()
      => _target.texture = _texture =
           new Texture2D(_size, _size, TextureFormat.RGBA32, false);

    unsafe void Update()
    {
        using var buffer = new NativeArray<uint>(_size * _size, Allocator.Temp);
        var ptr = NativeArrayUnsafeUtility.GetUnsafePtr(buffer);
        Fill(ptr, _size, Time.time);
        _texture.LoadRawTextureData(buffer);
        _texture.Apply();
    }

    [BurstCompile]
    unsafe static void Fill(void* pointer, int size, float time)
      => ImageGenerator.Fill(new Span<uint>(pointer, size * size), size, time);
}
