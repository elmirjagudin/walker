using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Net;

class AssetBundleLoadError : SystemException
{
    public AssetBundleLoadError(string message) : base(message) {}
}

public class AssetLoader : MonoBehaviour
{
    string DownloadAssetBundle(string bundleURL)
    {
        string AssetBundlePath = Path.Combine(Application.persistentDataPath, "import");


        WebClient Client = new WebClient();
        Debug.Log("download asset bundle from " + bundleURL);
        Client.DownloadFile(bundleURL, AssetBundlePath);
        Debug.Log("download complete");

        return AssetBundlePath;
    }

    GameObject LoadAssBundle(string AssetBundlePath)
    {
        var ass_bundle = AssetBundle.LoadFromFile(AssetBundlePath);
        if (ass_bundle == null)
        {
            throw new AssetBundleLoadError ("error loading " + AssetBundlePath);
        }

        var prefab = ass_bundle.LoadAsset<GameObject>("ifc");
        var gobj = Instantiate(prefab);

        ass_bundle.Unload(false);

        return gobj;
    }

	void Start()
    {
        LoadAssBundle(DownloadAssetBundle("http://localhost/assets/linux/import"));
	}
}
