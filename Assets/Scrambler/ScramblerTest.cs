using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using Stopwatch = System.Diagnostics.Stopwatch;

sealed class ScramblerTest : MonoBehaviour
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
        sw.Start(); Scrambler.ScrambleImage(span, Key); sw.Stop();
        var scrambled = new Texture2D(_source.width, _source.height);
        scrambled.SetPixels32(image);
        scrambled.Apply();

        // Unscramble
        Scrambler.ScrambleImage(span, Key);
        var unscrambled = new Texture2D(_source.width, _source.height);
        unscrambled.SetPixels32(image);
        unscrambled.Apply();

        // Output
        _previews[0].texture = _source;
        _previews[1].texture = scrambled;
        _previews[2].texture = unscrambled;
        _label.text = $"Processing time: {sw.Elapsed.TotalMilliseconds} ms";
    }
}
