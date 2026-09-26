#ifndef SUPERGENIUS_BRUSHED_METAL_LIGHTING_INCLUDED
#define SUPERGENIUS_BRUSHED_METAL_LIGHTING_INCLUDED

#define SG_PI 3.14159265359

inline float3 SG_FresnelSchlick(float VoH, float3 F0)
{
    float m = 1.0 - saturate(VoH);
    float m2 = m*m;
    return F0 + (1.0 - F0) * (m2*m2*m);
}

inline float SG_D_GGX_Aniso(float3 H, float3 N, float3 T, float3 B, float ax, float ay)
{
    float NoH = max(dot(N,H), 1e-5);
    float ToH = dot(T,H);
    float BoH = dot(B,H);
    float d = (ToH*ToH)/(ax*ax) + (BoH*BoH)/(ay*ay) + NoH*NoH;
    return 1.0 / max(SG_PI * ax * ay * d * d, 1e-6);
}

inline float SG_LambdaGGXAniso(float3 W, float3 N, float3 T, float3 B, float ax, float ay)
{
    float NoW = max(abs(dot(N,W)), 1e-5);
    float ToW = dot(T,W);
    float BoW = dot(B,W);
    float a2tan2 = ((ax*ToW)*(ax*ToW) + (ay*BoW)*(ay*BoW)) / (NoW*NoW);
    return 0.5 * (-1.0 + sqrt(1.0 + a2tan2));
}

inline float SG_G_SmithAniso(float3 V, float3 L, float3 N, float3 T, float3 B, float ax, float ay)
{
    return 1.0 / (1.0 + SG_LambdaGGXAniso(V,N,T,B,ax,ay) + SG_LambdaGGXAniso(L,N,T,B,ax,ay));
}

inline float3 SG_AnisotropicGGX(float3 N, float3 V, float3 L, float3 T, float3 B, float ax, float ay, float3 F0)
{
    float NoL = saturate(dot(N,L));
    float NoV = saturate(dot(N,V));
    if (NoL <= 0 || NoV <= 0) return 0;
    float3 H = normalize(V+L);
    float D = SG_D_GGX_Aniso(H,N,T,B,max(ax,0.02),max(ay,0.02));
    float G = SG_G_SmithAniso(V,L,N,T,B,max(ax,0.02),max(ay,0.02));
    float3 F = SG_FresnelSchlick(dot(V,H), F0);
    return F * D * G / max(4.0 * NoL * NoV, 1e-5);
}

#endif
