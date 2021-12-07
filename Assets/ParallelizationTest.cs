using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.UI;

sealed class ParallelizationTest : MonoBehaviour
{
    [SerializeField] int _size = 256;
    [SerializeField] RawImage _target = null;

    Texture2D _texture;

    void Start()
      => _target.texture = _texture =
           new Texture2D(_size, _size, TextureFormat.RGBA32, false);

    void Update()
    {
        using var temp =
          new NativeArray<uint>(_size * _size, Allocator.TempJob);

        var job = new GeneratorJob
          { XMask = _size - 1,
            YShift = Mathf.RoundToInt(Mathf.Log(_size, 2)),
            Time = Time.time,
            Buffer = temp };

        job.Schedule(_size * _size, 64).Complete();

        _texture.LoadRawTextureData(temp);
        _texture.Apply();
    }

    [BurstCompile(CompileSynchronously = true)]
    struct GeneratorJob : IJobParallelFor
    {
        public int XMask;
        public int YShift;
        public float Time;

        [WriteOnly] public NativeArray<uint> Buffer;

        public void Execute(int i)
          => Buffer[i] = ImageGenerator.GetPixel(i & XMask, i >> YShift, Time);
    }
}
