# Original Technique Reconstruction

## Surviving primary source

The Realistic3D page describes an effect created in NVIDIA's NVEffectsBrowser. The surviving explanation says the technique used a texture with `N dot L` encoded in RGB and `N dot H` encoded in alpha, combined with a normal map to obtain the final color.

Original page: https://www.realistic3d.com/brushed_metal.htm

Original source link: https://www.realistic3d.com/Source/BrushedMetal.zip

## What is certain

The page clearly establishes three design choices:

1. diffuse and specular angular terms were packed into one RGBA texture;
2. a normal map supplied high-frequency surface direction changes;
3. a dependent lookup/combine stage produced the final color.

This is a textbook example of early programmable-GPU thinking: spend texture bandwidth and precomputation to save arithmetic and instructions.

## What is reconstructed

Until the old source archive is recovered, this repository does not claim the exact original texture-combiner sequence. The historical mode preserves the documented channel semantics in an explicit 2D response texture:

```text
RGB = saturate(N dot L)
A   = saturate(N dot H)
```

The sampled RGB drives the body term and alpha drives a Blinn-style specular exponent. A directional normal map supplies the brushed microstructure.

## Modern comparison

Modern renderers model the stretched highlight directly through an anisotropic BRDF. The comparison modes therefore move from the packed lookup to isotropic GGX, anisotropic GGX, and finally anisotropic GGX plus visible brush scratches.
