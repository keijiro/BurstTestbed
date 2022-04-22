using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

[BurstCompile]
sealed class Scrambler : MonoBehaviour
{
    [SerializeField] Texture2D _source = null;
    [SerializeField] RawImage[] _imageViews = null;

    const uint Key = 0xdeafbeef;

    Texture2D _scrambled, _unscrambled;

    void Start()
    {
        _scrambled = new Texture2D(_source.width, _source.height);
        _unscrambled = new Texture2D(_source.width, _source.height);

        _imageViews[0].texture = _source;
        _imageViews[1].texture = _scrambled;
        _imageViews[2].texture = _unscrambled;
    }

    unsafe void Update()
    {
        var buffer = _source.GetPixels32();

        fixed (uint* ptr = MemoryMarshal.Cast<Color32, uint>(buffer))
        {
            // Scramble
            Profiler.BeginSample("Scrambling");
            Scramble_Burst(ptr, buffer.Length, Key);
            Profiler.EndSample();
            _scrambled.SetPixels32(buffer);
            _scrambled.Apply();

            // Unscramble
            Profiler.BeginSample("Unscrambling");
            Scramble_Burst(ptr, buffer.Length, Key);
            Profiler.EndSample();
            _unscrambled.SetPixels32(buffer);
            _unscrambled.Apply();
        }
    }

    [BurstCompile]
    unsafe static void Scramble_Burst(void* ptr, int size, uint key)
      => ImageScrambler.Scramble(new Span<uint>(ptr, size), key);
}
