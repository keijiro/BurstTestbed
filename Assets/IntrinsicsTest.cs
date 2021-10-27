using Unity.Burst;
using Unity.Burst.Intrinsics;
using static Unity.Burst.Intrinsics.Arm.Neon;
using UnityEngine;

[BurstCompile]
sealed class IntrinsicsTest : MonoBehaviour
{
    void Start()
      => Debug.Log(RunTest());

    [BurstCompile]
    static int RunTest()
      => CountBits(new v128(System.Math.PI, System.Math.PI));

    static int CountBits(v128 v)
    {
        if (IsNeonSupported)
            return vaddvq_u8(vcntq_s8(v));
        else
            return CountBits(v.ULong0) + CountBits(v.ULong1);
    }

    static int CountBits(ulong ul)
    {
        var count = 0;
        for (var i = 0; i < 64; i++)
            if (((1ul << i) & ul) != 0) count++;
        return count;
    }
}
