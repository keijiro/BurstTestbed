BurstTestbed
------------

![photo](https://i.imgur.com/IztZ3dml.jpg)

This repository contains some small samples for the [Unity Burst compiler].

[Unity Burst compiler]:
  https://docs.unity3d.com/Packages/com.unity.burst@latest

At the moment, the following samples are included:

- **DirectCallTest** shows how to use the [Burst direct call feature].
- **IntrinsicsTest** shows use of Arm Neon intrinsics within a Burst function.
- **Scrambler** is a sample encrypting/decrypting an image using a simple
  algorithm.
- **SpanTest** shows use of `System.Span<T>` within a Burst function.

[Burst direct call feature]:
  https://docs.unity3d.com/Packages/com.unity.burst@1.6/manual/docs/CSharpLanguageSupport_Lang.html#directly-calling-burst-compiled-code
