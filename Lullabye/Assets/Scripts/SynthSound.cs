using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using AudioHelm;

public class SynthSound : MonoBehaviour
{
//    public HelmController controller;
//    [SerializeField]  HelmPatch patch;
    
    public static SynthSound i;
    private bool loaded;
    void Start()
    {
//        i = this;
//        controller.LoadPatch(patch);
//        controller.UpdateAllParameters();
    }

    public void PlayNote(int i)
    {
//        controller.NoteOn(i);
//        controller.SetParameterPercent(AudioHelm.Param.kVolume, 0);
    }

    void Update()
    {
        if (!loaded)
        {
//            
//            controller.LoadPatch(patch);
//            controller.UpdateAllParameters();
//            loaded = true;
        }
    }
}
