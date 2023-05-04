using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirstScene : MonoBehaviour
{
    [SerializeField] CHUnitBase testUnit1;
    [SerializeField] CHUnitBase testUnit2;

    [SerializeField, ReadOnly] List<Material> liMaterial = new List<Material>();
    
    
    private void Start()
    {
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        for (int i = 0; i < (int)Defines.EMaterial.Max; ++i)
        {
            CHMMain.Resource.LoadMaterial((Defines.EMaterial)i, (mat) =>
            {
                liMaterial.Add(mat);
            });
        }

        if (liMaterial.Count > 0)
        {
            //testUnit1.meshRenderer.material = liMaterial[(int)Defines.EMaterial.Red];
            //testUnit2.meshRenderer.material = liMaterial[(int)Defines.EMaterial.Yellow];
        }
    }
}
