# Brushed Metal

A Unity 6 / URP reconstruction of Kenneth Hurley's early-2000s **NVEffectsBrowser brushed-metal effect**, followed by modern anisotropic microfacet versions.

Original page: https://www.realistic3d.com/brushed_metal.htm

The surviving page describes an unusually compact pipeline:

- a texture whose **RGB channels encode `N dot L`**,
- whose **alpha channel encodes `N dot H`**,
- a normal map to perturb the surface normal,
- and a lookup that turns those packed lighting terms into the final metal response.

The original source link is:

`https://www.realistic3d.com/Source/BrushedMetal.zip`

The old archive is not redistributed here unless recovered and verified. The `HistoricalPacked` mode is therefore a **documented reconstruction**, not a claim of binary-identical source recovery.

## Included modes

1. **Historical Packed** - generated 2D RGBA texture: `RGB = N dot L`, `A = N dot H`, sampled after a directional normal map.
2. **Isotropic GGX** - modern metallic microfacet baseline.
3. **Anisotropic GGX** - tangent/bitangent roughness split for the stretched highlight expected from brushed metal.
4. **Anisotropic + Scratches** - anisotropic GGX plus a generated directional normal map.

## Unity

Target: Unity 6 + URP 17.

Use:

```text
Tools > Super-Genius > Brushed Metal > Create Comparison Scene
```

The builder generates the historical packed response texture, a deterministic directional brush normal map, four materials, and a side-by-side scene.

## Paper

Working title:

**From Packed Dot Products to Anisotropic GGX: Revisiting an Early Real-Time Brushed Metal Shader**

See `Paper/brushed_metal.tex`.

## License

MIT for the reconstruction code in this repository. Historical files retain their original status.
