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
    [SerializeField] RawImage[] _previews = null;
    [SerializeField] Text _label = null;

    const uint Key = 0xdeafbeef;

    void Start()
    {
        var image = _source.GetPixels32();
        var span = MemoryMarshal.Cast<Color32, uint>(new Span<Color32>(image));
        var sw = new Stopwatch();

        // Scramble
        sw.Start(); Scramble(span, Key); sw.Stop();
        var scrambled = new Texture2D(_source.width, _source.height);
        scrambled.SetPixels32(image);
        scrambled.Apply();

        // Unscramble
        Scramble(span, Key);
        var unscrambled = new Texture2D(_source.width, _source.height);
        unscrambled.SetPixels32(image);
        unscrambled.Apply();

        // Output
        _previews[0].texture = _source;
        _previews[1].texture = scrambled;
        _previews[2].texture = unscrambled;
        _label.text = $"Processing time: {sw.Elapsed.TotalMilliseconds} ms";
    }

    unsafe public static void Scramble(Span<uint> image, uint key)
    {
        fixed (uint* ptr = image) Scramble_Burst(ptr, image.Length, key);
    }

    [BurstCompile]
    unsafe static void Scramble_Burst(void* ptr, int size, uint key)
      => ImageScrambler.Scramble(new Span<uint>(ptr, size), key);
}
