using Mono.Cecil;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class FirstScene : MonoBehaviour
{
    [SerializeField] List<CHUnitBase> liUnit = new List<CHUnitBase>();

    private void Start()
    {
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        foreach (var unit in liUnit)
        {
            int randomTeam = Random.Range(0, 2);

            if (randomTeam == 0)
            {
                unit.gameObject.layer = (int)Defines.ETeamLayer.Red;
                var targetTracker = unit.GetComponent<CHTargetTracker>();
                if (targetTracker != null)
                {
                    targetTracker.targetMask = 1 << (int)Defines.ETeamLayer.Blue;
                }
            }
            else
            {
                unit.gameObject.layer = (int)Defines.ETeamLayer.Blue;
                var targetTracker = unit.GetComponent<CHTargetTracker>();
                if (targetTracker != null)
                {
                    targetTracker.targetMask = 1 << (int)Defines.ETeamLayer.Red;
                }
            }

            int randomColor = Random.Range(0, (int)Defines.EMaterial.Max);

            unit.GetComponent<MeshRenderer>().material = CHMMain.Unit.GetUnitMaterial((Defines.EMaterial)randomColor);
        }
    }
}
