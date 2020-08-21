using UnityEngine;

public class DecalHoldersManager : Singleton<DecalHoldersManager>
{
    public Transform staticDecalsHolder;                    // Parent of inactive decals
    public Transform staticReflectedDecalsHolder;           // Parent of inactive reflected decals

    public Transform dynamicDecalsHolder;                   // Active decal`s parent
    public Transform dynamicReflectedDecalsHolder;          // Active reflected decal`s parent
}
