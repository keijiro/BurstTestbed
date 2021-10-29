using System;
using Unity.Burst;
using Random = Unity.Mathematics.Random;

[BurstCompile]
public static class Scrambler
{
    [BurstCompile]
    unsafe static void ScrambleImage(void* ptr, int size, uint key)
    {
        var span = new Span<uint>(ptr, size);
        var r = new Random(key);
        for (var i = 0; i < span.Length; i++) span[i] ^= r.NextUInt();
    }

    unsafe public static void ScrambleImage(Span<uint> image, uint key)
    {
        fixed (uint* ptr = image) ScrambleImage(ptr, image.Length, key);
    }
}
