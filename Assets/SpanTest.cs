using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[BurstCompile]
sealed class SpanTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _imageView = null;

    Texture2D _texture;
    Color32[] _buffer;

    void Start()
    {
        _imageView.texture = _texture = new Texture2D(_size, _size);
        _buffer = new Color32[_size * _size];
    }

    unsafe void Update()
    {
        Profiler.BeginSample("Texture Generation");

        fixed (uint* ptr = MemoryMarshal.Cast<Color32, uint>(_buffer))
            Fill_Burst(ptr, _size, Time.time);

        Profiler.EndSample();

        _texture.SetPixels32(_buffer);
        _texture.Apply();
    }

    [BurstCompile]
    unsafe static void Fill_Burst(void* pointer, int size, float time)
      => ImageGenerator.Fill(new Span<uint>(pointer, size * size), size, time);
}
