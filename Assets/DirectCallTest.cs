using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

sealed class DirectCallTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _target = null;

    Texture2D _texture;

    void Start()
      => _target.texture = _texture =
           new Texture2D(_size, _size, TextureFormat.RGBA32, false);

    void Update()
    {
        using var temp = new NativeArray<uint>(_size * _size, Allocator.Temp);
        var buffer = new NativeSlice<uint>(temp);

        var time = Time.time;
        var offs = 0;

        for (var y = 0; y < _size; y++)
            for (var x = 0; x < _size; x++)
                buffer[offs++] = TextureGenerator.GetPixel(x, y, time);

        _texture.LoadRawTextureData(temp);
        _texture.Apply();
    }
}

[BurstCompile]
static class TextureGenerator
{
    [BurstCompile]
    public static uint GetPixel(int x, int y, float t)
    {
        var pos = math.float3(x, y, t) * math.float3(0.008f, 0.008f, 0.5f);
        var f32 = noise.snoise(pos) * 0.4f + 0.5f;
        var un8 = (uint)(math.saturate(f32) * 255);
        return un8 | un8 << 8 | un8 << 16 | 0xff000000;
    }
}
