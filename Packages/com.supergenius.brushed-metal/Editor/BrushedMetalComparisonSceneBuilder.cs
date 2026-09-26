#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace SuperGenius.BrushedMetal.Editor
{
    public static class BrushedMetalComparisonSceneBuilder
    {
        const string SceneFolder="Assets/Scenes", MatFolder="Assets/BrushedMetalGenerated/Materials", ScenePath=SceneFolder+"/BrushedMetalComparison.unity";
        static readonly string[] Names={"Historical Packed","Isotropic GGX","Anisotropic GGX","Anisotropic + Scratches"};

        [MenuItem("Tools/Super-Genius/Brushed Metal/Create Comparison Scene")]
        public static void Build()
        {
            EnsureFolder(SceneFolder);EnsureFolder("Assets/BrushedMetalGenerated");EnsureFolder(MatFolder);BrushedMetalTextureGenerator.GenerateAll();
            Shader shader=Shader.Find("SuperGenius/BrushedMetal");if(!shader){Debug.LogError("BrushedMetal shader not imported yet.");return;}
            var normal=AssetDatabase.LoadAssetAtPath<Texture2D>(BrushedMetalTextureGenerator.NormalPath);var packed=AssetDatabase.LoadAssetAtPath<Texture2D>(BrushedMetalTextureGenerator.PackedPath);
            var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Single);RenderSettings.ambientMode=AmbientMode.Flat;RenderSettings.ambientLight=new Color(0.02f,0.025f,0.03f);
            Camera();Lights();Ground();float spacing=2.4f,start=-3.6f;
            for(int i=0;i<4;i++){var s=GameObject.CreatePrimitive(PrimitiveType.Sphere);s.name=Names[i];s.transform.position=new Vector3(start+i*spacing,0.15f,0);s.transform.localScale=Vector3.one*1.6f;s.GetComponent<Renderer>().sharedMaterial=Mat(shader,normal,packed,i,Names[i]);Label(s.transform.position+new Vector3(0,-1.35f,0),Names[i]);}
            EditorSceneManager.MarkSceneDirty(scene);EditorSceneManager.SaveScene(scene,ScenePath);AssetDatabase.SaveAssets();AssetDatabase.Refresh();
        }
        static Material Mat(Shader sh,Texture n,Texture p,int mode,string name){string path=MatFolder+"/"+name.Replace(" ","_").Replace("+","Plus")+".mat";var m=AssetDatabase.LoadAssetAtPath<Material>(path);if(!m){m=new Material(sh);AssetDatabase.CreateAsset(m,path);}m.shader=sh;m.SetTexture("_NormalMap",n);m.SetTexture("_PackedResponseLUT",p);m.SetFloat("_Mode",mode);m.SetColor("_BaseColor",new Color(0.62f,0.66f,0.70f));m.SetVector("_BrushTiling",new Vector4(5,26,0,0));m.SetFloat("_NormalStrength",0.7f);m.SetFloat("_LegacySpecularPower",72);m.SetFloat("_RoughnessT",0.12f);m.SetFloat("_RoughnessB",0.50f);EditorUtility.SetDirty(m);return m;}
        static void Camera(){var o=new GameObject("Main Camera");var c=o.AddComponent<Camera>();o.tag="MainCamera";o.transform.position=new Vector3(0,1,-10.5f);o.transform.LookAt(Vector3.zero);c.fieldOfView=42;c.clearFlags=CameraClearFlags.SolidColor;c.backgroundColor=new Color(0.045f,0.05f,0.06f);}
        static void Lights(){var a=new GameObject("Key Light");var l=a.AddComponent<Light>();l.type=LightType.Directional;l.intensity=3.0f;l.color=new Color(1,0.92f,0.82f);l.shadows=LightShadows.Soft;a.transform.rotation=Quaternion.Euler(38,-36,0);var b=new GameObject("Rim Light");var r=b.AddComponent<Light>();r.type=LightType.Directional;r.intensity=1.2f;r.color=new Color(0.4f,0.65f,1);b.transform.rotation=Quaternion.Euler(20,145,0);}
        static void Ground(){var g=GameObject.CreatePrimitive(PrimitiveType.Plane);g.name="Ground";g.transform.position=new Vector3(0,-1.55f,0);g.transform.localScale=new Vector3(1.25f,1,1);}
        static void Label(Vector3 pos,string t){var o=new GameObject(t+" Label");o.transform.position=pos;var m=o.AddComponent<TextMesh>();m.text=t;m.anchor=TextAnchor.MiddleCenter;m.alignment=TextAlignment.Center;m.fontSize=48;m.characterSize=0.058f;m.color=Color.white;}
        static void EnsureFolder(string folder){if(AssetDatabase.IsValidFolder(folder))return;string[] p=folder.Split('/');string cur=p[0];for(int i=1;i<p.Length;i++){string next=cur+"/"+p[i];if(!AssetDatabase.IsValidFolder(next))AssetDatabase.CreateFolder(cur,p[i]);cur=next;}}
    }
}
#endif
