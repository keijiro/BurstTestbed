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

    unsafe void Start()
    {
        var buffer = _source.GetPixels32();

        fixed (uint* ptr = MemoryMarshal.Cast<Color32, uint>(buffer))
        {
            // Scramble
            var scrambled = new Texture2D(_source.width, _source.height);
            _imageViews[1].texture = scrambled;

            Profiler.BeginSample("Scrambling");
            Scramble_Burst(ptr, buffer.Length, Key);
            Profiler.EndSample();

            scrambled.SetPixels32(buffer);
            scrambled.Apply();

            // Unscramble
            var unscrambled = new Texture2D(_source.width, _source.height);
            _imageViews[2].texture = unscrambled;

            Profiler.BeginSample("Unscrambling");
            Scramble_Burst(ptr, buffer.Length, Key);
            Profiler.EndSample();

            unscrambled.SetPixels32(buffer);
            unscrambled.Apply();
        }

        _imageViews[0].texture = _source;
    }

    [BurstCompile]
    unsafe static void Scramble_Burst(void* ptr, int size, uint key)
      => ImageScrambler.Scramble(new Span<uint>(ptr, size), key);
}
