using UnityEngine;
using System.Collections;

public class BayerConverter {
    public enum Rotation { _0, _90, _180, _270 };

    private static string SHADER_NAME = "shaders/bayer";

    private static uint nextKernel = 1;

    public int width;
    public int height;

    public RenderTexture texture;

    ComputeShader shader;
    int kernelIdx;
    ComputeBuffer buffer;

    private int threadsX, threadsY;


    private string GetKernelName()
    {
        if (nextKernel > 2)
        {
            throw new System.Exception("max 2 instances of BayerConverter supported");
        }
        return string.Format("bayer{0}", nextKernel++);
    }

    public BayerConverter(int width, int height)
    {
        this.width = width;
        this.height = height;

        /* set-up target texture */
        texture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);
        texture.enableRandomWrite = true;
        texture.Create();

        /* load shader */
        shader = Resources.Load(SHADER_NAME) as ComputeShader;
        if (shader == null)
        {
            throw new System.Exception("can't load shader '" + SHADER_NAME + "'");
        }

        /* set up target texture and input buffer */
        kernelIdx = shader.FindKernel(GetKernelName());
        shader.SetTexture(kernelIdx, "Result", texture);

        buffer = new ComputeBuffer(width * height, 4);
        shader.SetBuffer(kernelIdx, "raw", buffer);

        shader.SetFloat("width", width);
        shader.SetFloat("height", height);

        uint tx, ty, tz;
        shader.GetKernelThreadGroupSizes(kernelIdx, out tx, out ty, out tz);
        if (tz != 1)
        {
            throw new System.Exception("unexpected number of threads in" +
                "Z dimension, expected 1, got " + tz);
        }
        threadsX = (int)tx;
        threadsY = (int)ty;
    }

    public void Draw(byte[] data, float strength, float zoom, float scale_x, float scale_y, float translate_x, float translate_y, Rotation rotation)
    {
        buffer.SetData(data);
        shader.SetFloat("strength", strength);
        shader.SetFloat("zoom", zoom);
        shader.SetFloat("scale_x", scale_x);
        shader.SetFloat("scale_y", scale_y);
        shader.SetFloat("translate_x", translate_x);
        shader.SetFloat("translate_y", translate_y);

        float cos_val = 1.0f;
        float sin_val = 0.0f;

        switch (rotation)
        {
            case Rotation._0:
                cos_val = 1.0f;
                sin_val = 0.0f;
                break;
            case Rotation._90:
                cos_val = 0.0f;
                sin_val = 1.0f;
                break;
            case Rotation._180:
                cos_val = -1.0f;
                sin_val = 0.0f;
                break;
            case Rotation._270:
                cos_val = 0.0f;
                sin_val = -1.0f;
                break;
        }

        shader.SetFloat("rot_cos", cos_val);
        shader.SetFloat("rot_sin", sin_val);
        shader.Dispatch(kernelIdx, width / threadsX, height / threadsY, 1);
    }

    public void Cleanup()
    {
        buffer.Release();
    }
}
