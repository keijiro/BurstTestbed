using System;
using System.Runtime.InteropServices;
using Unity.Burst;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;

[BurstCompile]
sealed class Scrambler : MonoBehaviour
{
    [SerializeField] Texture2D _source = null;
    [SerializeField] RawImage[] _imageViews = null;
    [SerializeField] Text _label = null;

    const uint Key = 0xdeafbeef;

    unsafe void Start()
    {
        var buffer = _source.GetPixels32();
        var sw = new Stopwatch();

        fixed (uint* ptr = MemoryMarshal.Cast<Color32, uint>(buffer))
        {
            // Scramble
            var scrambled = new Texture2D(_source.width, _source.height);
            _imageViews[1].texture = scrambled;

            sw.Start(); Scramble_Burst(ptr, buffer.Length, Key); sw.Stop();

            scrambled.SetPixels32(buffer);
            scrambled.Apply();

            // Unscramble
            var unscrambled = new Texture2D(_source.width, _source.height);
            _imageViews[2].texture = unscrambled;

            Scramble_Burst(ptr, buffer.Length, Key);

            unscrambled.SetPixels32(buffer);
            unscrambled.Apply();
        }

        _imageViews[0].texture = _source;
        _label.text = $"Processing time: {sw.Elapsed.TotalMilliseconds} ms";
    }

    [BurstCompile]
    unsafe static void Scramble_Burst(void* ptr, int size, uint key)
      => ImageScrambler.Scramble(new Span<uint>(ptr, size), key);
}
