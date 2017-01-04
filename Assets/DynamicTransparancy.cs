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

    BayerConverter converter;

    void LoadData()
    {
        var fs = new FileStream(imageFile, FileMode.Open);
        fs.Read(data, 0, WIDTH * HEIGHT);
        fs.Close();
    }


    void Start()
    {
        converter = new BayerConverter(WIDTH, HEIGHT);

        mat = new Material(Shader.Find("DynamicTransparancy"));
        mat.SetTexture("_BackgroundTex", converter.texture);

        LoadData();
    }

    void OnRenderImage(RenderTexture src, RenderTexture dest)
    {
        converter.Draw(data, strength, zoom, scale_x, scale_y, translate_x, translate_y, rotation);
        mat.SetFloat("_Opacity", OpacityGuage.opacity);
        Graphics.Blit(src, dest, mat);
    }

    void OnDestroy()
    {
        converter.Cleanup();
    }
}
