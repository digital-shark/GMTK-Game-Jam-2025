using UnityEngine;

public class DrawingCanvas : MonoBehaviour
{
    public Camera drawCamera;
    public RenderTexture drawTexture;
    public Material drawMaterial;

    private Vector3 lastMousePos;
    private bool isDrawing = false;

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isDrawing = true;
            lastMousePos = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDrawing = false;
        }

        if (isDrawing)
        {
            Vector3 mousePos = Input.mousePosition;
            DrawLine(lastMousePos, mousePos);
            lastMousePos = mousePos;
        }
    }

    void DrawLine(Vector3 from, Vector3 to)
    {
        GL.PushMatrix();
        RenderTexture.active = drawTexture;

        drawMaterial.SetPass(0);
        GL.LoadOrtho();
        GL.Begin(GL.LINES);
        GL.Color(Color.black);

        Vector2 start = new Vector2(from.x / Screen.width, from.y / Screen.height);
        Vector2 end = new Vector2(to.x / Screen.width, to.y / Screen.height);
        GL.Vertex(start);
        GL.Vertex(end);

        GL.End();
        GL.PopMatrix();
        RenderTexture.active = null;
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

    public void ClearCanvas()
    {
        RenderTexture.active = drawTexture;
        GL.Clear(true, true, Color.white);
        RenderTexture.active = null;
    }
}
