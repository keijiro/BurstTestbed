using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;

[BurstCompile]
sealed class SpanTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _target = null;
    [SerializeField] Text _label = null;

    Texture2D _texture;

    void Start()
      => _target.texture = _texture =
           new Texture2D(_size, _size, TextureFormat.RGBA32, false);

    unsafe void Update()
    {
        var sw = new Stopwatch();
        using var buffer = new NativeArray<uint>(_size * _size, Allocator.Temp);
        var ptr = NativeArrayUnsafeUtility.GetUnsafePtr(buffer);
        sw.Start(); Fill(ptr, _size, Time.time); sw.Stop();
        _texture.LoadRawTextureData(buffer);
        _texture.Apply();
        _label.text = $"Processing time: {sw.Elapsed.TotalMilliseconds} ms";
    }

    [BurstCompile]
    unsafe static void Fill(void* pointer, int size, float time)
      => ImageGenerator.Fill(new Span<uint>(pointer, size * size), size, time);
}
