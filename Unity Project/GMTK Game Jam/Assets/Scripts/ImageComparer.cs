using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ImageComparer : MonoBehaviour
{
    public Texture2D referenceTexture;
    public RenderTexture drawTexture;

    [Range(0.1f, 1f)] public float threshold = 0.5f;
    public float score;

    public void Compare()
    {
        Texture2D userTex = GetDrawnTexture();

        if (userTex.width != referenceTexture.width || userTex.height != referenceTexture.height)
        {
            Debug.LogWarning("Textures must be same size!");
            return;
        }

        int match = 0, union = 0;

        for (int y = 0; y < userTex.height; y++)
        {
            for (int x = 0; x < userTex.width; x++)
            {
                bool user = userTex.GetPixel(x, y).grayscale > threshold;
                bool reference = referenceTexture.GetPixel(x, y).grayscale > threshold;

                if (user || reference) union++;
                if (user && reference) match++;
            }
        }

        score = (union == 0) ? 0f : (float)match / union * 100f;
        Debug.Log("Match Score: " + Mathf.RoundToInt(score) + "%" + " Match: " + match + " Union: " + union);
    }

    public Texture2D GetDrawnTexture()
    {
        Texture2D tex = new Texture2D(drawTexture.width, drawTexture.height, TextureFormat.RGBA32, false);
        RenderTexture.active = drawTexture;
        tex.ReadPixels(new Rect(0, 0, drawTexture.width, drawTexture.height), 0, 0);
        tex.Apply();
        RenderTexture.active = null;
        return tex;
    }

    //float timer = 0;

    private void Update()
    {
        //timer += Time.deltaTime;
        //if (timer >= 5.0f)
        //{
        //    timer = 0;
        //    Compare();
        //}
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ImageComparer))]
public class ImageComparerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        ImageComparer comparer = (ImageComparer)target;

        if (GUILayout.Button("Compare"))
        {
            comparer.Compare();
        }
    }
}
#endif
