using UnityEngine;
using System.IO;
using System.Collections;

public class DynamicTransparancy : MonoBehaviour
{
    public string imageFile;
    public float strength = 2.7f;
    public float zoom = 2.23f;
    public float scale_x = 480.0f;
    public float scale_y = 480.0f;
    public float translate_x = 0;
    public float translate_y = 0;

    public BayerConverter.Rotation rotation;

    private Material mat;
    private RenderTexture background;

    private static int WIDTH = 1024;
    private static int HEIGHT = 1024;
    byte[] data = new byte[WIDTH * HEIGHT];

    BayerConverter converter = null;

    void LoadData()
    {
        var fs = new FileStream(imageFile, FileMode.Open);
        fs.Read(data, 0, WIDTH * HEIGHT);
        fs.Close();
    }


    void Start()
    {
        mat = new Material(Shader.Find("DynamicTransparancy"));

        try
        {
            converter = new BayerConverter(WIDTH, HEIGHT);
            mat.SetTexture("_BackgroundTex", converter.texture);

            LoadData();
        }
        catch (UnityException e)
        {
            print("failed to setup bayer shader: "  + e +
                  " disabling background image generation");
        }
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        if (converter != null)
        {
            converter.Draw(data, strength, zoom, scale_x, scale_y, translate_x, translate_y, rotation);
        }
        mat.SetFloat("_Opacity", OpacityGuage.opacity);
        Graphics.Blit(src, dest, mat);
    }

    void OnDestroy()
    {
        if (converter != null)
        {
            converter.Cleanup();
        }
    }
}
