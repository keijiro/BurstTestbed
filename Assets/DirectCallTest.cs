using Unity.Burst;
using Unity.Collections;
using UnityEngine;
using UnityEngine.UI;

[BurstCompile]
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
                buffer[offs++] = GetPixel(x, y, time);

        _texture.LoadRawTextureData(temp);
        _texture.Apply();
    }

    [BurstCompile]
    static uint GetPixel(int x, int y, float t)
      => ImageGenerator.GetPixel(x, y, t);
}
