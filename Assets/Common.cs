using System;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

static class ImageGenerator
{
    public static uint GetPixel(int x, int y, float t)
    {
        var pos = math.float3(x, y, t) * math.float3(0.008f, 0.008f, 0.5f);
        var f32 = noise.snoise(pos) * 0.4f + 0.5f;
        var un8 = (uint)(math.saturate(f32) * 255);
        return un8 | un8 << 8 | un8 << 16 | 0xff000000;
    }

    public static void Fill(Span<uint> buffer, int size, float time)
    {
        var offs = 0;
        for (var y = 0; y < size; y++)
            for (var x = 0; x < size; x++)
                buffer[offs++] = GetPixel(x, y, time);
    }
}

static class ImageScrambler
{
    public static void Scramble(Span<uint> image, uint key)
    {
        var r = new Random(key);
        for (var i = 0; i < image.Length; i++) image[i] ^= r.NextUInt();
    }
}
