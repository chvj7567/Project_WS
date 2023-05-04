using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class FirstScene : MonoBehaviour
{
    [SerializeField] Button btnExitReady;
    [SerializeField] List<CHUnitBase> liUnit = new List<CHUnitBase>();

    private void Start()
    {
        btnExitReady.OnClickAsObservable().Subscribe(_ =>
        {
            CHMMain.Particle.OnApplicationQuitHandler();
            CHMMain.Skill.OnApplicationQuitHandler();
        });

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
