using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tag : MonoBehaviour
{

    private TagSystem TS;
    
    [SerializeField]
    public string Name;
    // Start is called before the first frame update
    void Awake()
    {
        TS = Services.Resolve<TagSystem>();
    }


}
