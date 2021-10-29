using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;

[BurstCompile]
sealed class SpanTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _imageView = null;
    [SerializeField] Text _label = null;

    Texture2D _texture;
    Color32[] _buffer;

    void Start()
    {
        _imageView.texture = _texture = new Texture2D(_size, _size);
        _buffer = new Color32[_size * _size];
    }

    unsafe void Update()
    {
        var sw = new Stopwatch();

        fixed (uint* ptr = MemoryMarshal.Cast<Color32, uint>(_buffer))
        {
            sw.Start(); Fill_Burst(ptr, _size, Time.time); sw.Stop();
        }

        _texture.SetPixels32(_buffer);
        _texture.Apply();

        _label.text = $"Processing time: {sw.Elapsed.TotalMilliseconds} ms";
    }

    [BurstCompile]
    unsafe static void Fill_Burst(void* pointer, int size, float time)
      => ImageGenerator.Fill(new Span<uint>(pointer, size * size), size, time);
}
