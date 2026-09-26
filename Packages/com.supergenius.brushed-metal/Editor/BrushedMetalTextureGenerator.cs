#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SuperGenius.BrushedMetal.Editor
{
    public static class BrushedMetalTextureGenerator
    {
        public const string Folder="Assets/BrushedMetalGenerated";
        public const string NormalPath=Folder+"/DirectionalBrushNormal.png";
        public const string PackedPath=Folder+"/HistoricalPackedResponse.png";

        [MenuItem("Tools/Super-Genius/Brushed Metal/Generate Historical Textures")]
        public static void GenerateAll(){GeneratePacked(256);GenerateNormal(512,1337);AssetDatabase.Refresh();}

        public static Texture2D GeneratePacked(int size)
        {
            EnsureFolder(Folder); var t=new Texture2D(size,size,TextureFormat.RGBA32,false,true);
            var p=new Color[size*size];
            for(int y=0;y<size;y++){float noH=(y+0.5f)/size;for(int x=0;x<size;x++){float noL=(x+0.5f)/size;p[y*size+x]=new Color(noL,noL,noL,noH);}}
            t.SetPixels(p);t.Apply();File.WriteAllBytes(PackedPath,t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);Import(PackedPath,false,false);return AssetDatabase.LoadAssetAtPath<Texture2D>(PackedPath);
        }

        public static Texture2D GenerateNormal(int size,int seed)
        {
            EnsureFolder(Folder); var rng=new System.Random(seed); float[] h=new float[size];
            float v=0; for(int y=0;y<size;y++){v=0.82f*v+0.18f*((float)rng.NextDouble()*2f-1f);h[y]=v;}
            var t=new Texture2D(size,size,TextureFormat.RGBA32,false,true); var p=new Color[size*size];
            for(int y=0;y<size;y++)
            {
                float dy=h[(y+1)%size]-h[(y-1+size)%size];
                for(int x=0;x<size;x++)
                {
                    float fine=(float)Math.Sin((x*0.17)+(y*0.031))*0.035f; Vector3 n=new Vector3(fine,-dy*5.0f,1).normalized;
                    p[y*size+x]=new Color(n.x*0.5f+0.5f,n.y*0.5f+0.5f,n.z*0.5f+0.5f,1);
                }
            }
            t.SetPixels(p);t.Apply();File.WriteAllBytes(NormalPath,t.EncodeToPNG());UnityEngine.Object.DestroyImmediate(t);Import(NormalPath,false,true);return AssetDatabase.LoadAssetAtPath<Texture2D>(NormalPath);
        }

        static void Import(string path,bool normal,bool repeat)
        {
            AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceUpdate); var imp=AssetImporter.GetAtPath(path) as TextureImporter; if(!imp)return;
            imp.sRGBTexture=false; imp.mipmapEnabled=true; imp.wrapMode=repeat?TextureWrapMode.Repeat:TextureWrapMode.Clamp; imp.filterMode=FilterMode.Bilinear; imp.textureCompression=TextureImporterCompression.Uncompressed;
            if(normal) imp.textureType=TextureImporterType.NormalMap; imp.SaveAndReimport();
        }
        static void EnsureFolder(string folder)
        {
            if(AssetDatabase.IsValidFolder(folder))return; string[] p=folder.Split('/');string cur=p[0];
            for(int i=1;i<p.Length;i++){string next=cur+"/"+p[i];if(!AssetDatabase.IsValidFolder(next))AssetDatabase.CreateFolder(cur,p[i]);cur=next;}
        }
    }
}
#endif
