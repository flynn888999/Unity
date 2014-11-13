using UnityEngine;
using System.Collections;

public class AssetCacheInfo
{
    public byte[] binary;

    private AssetBundle bundle;
    public UnityEngine.AssetBundle Bundle
    {
        get { return bundle; }
        set
        {
            if (bundle == null)
                bundle = value;
        }
    }

    public void UnLoad()
    {
        binary = null;
        bundle.Unload(false);
    }
}
