using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WarSimulatorScene : BaseScene
{
    [SerializeField] Button mainMenuBtn;
    [SerializeField] Button warStartBtn;

    [SerializeField, ReadOnly] List<CHTargetTracker> enemyTeamtargetTrackerList = new List<CHTargetTracker>();
    [SerializeField, ReadOnly] List<CHTargetTracker> myTeamtargetTrackerList = new List<CHTargetTracker>();

    private void Start()
    {
        CHMMain.UI.CreateEventSystemObject();
        CHMMain.Resource.InstantiateMajor(Defines.EMajor.GlobalVolume);

        mainMenuBtn.OnClickAsObservable().Subscribe(_ =>
        {
            SceneManager.LoadScene(1);
        });

        warStartBtn.OnClickAsObservable().Subscribe(_ =>
        {
            for (int i = 0; i < enemyTeamtargetTrackerList.Count; i++)
            {
                CHMMain.Unit.SetTargetMask(enemyTeamtargetTrackerList[i], Defines.ELayer.Red);
            }

            for (int i = 0; i < myTeamtargetTrackerList.Count; i++)
            {
                CHMMain.Unit.SetTargetMask(myTeamtargetTrackerList[i], Defines.ELayer.Blue);
            }

            warStartBtn.gameObject.SetActive(false);
        });

        CreateUnit(PlayerPrefs.GetInt("WarSimulator"));
    }

    void CreateUnit(int count)
    {
        float changePosX = 10f;
        float changePosZ = 2.5f;

        if (count == 1)
        {
            CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX, 0, 0), enemyTeamtargetTrackerList);
            CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(-changePosX, 0, 0), myTeamtargetTrackerList);
        }
        else if (count % 2 == 0)
        {
            int repeat = count / 2;
            int repeat2 = 1;

            float posZ = 2.5f;
            if (count > 10)
            {
                repeat = 5;
                repeat2 = count / 10;

                if (count >= 100)
                {
                    changePosX = 1.25f;
                }
                else if (count >= 50)
                {
                    changePosX = 5f;
                }
            }

            for (int k = 0; k < repeat2; ++k)
            {
                for (int i = 0; i < repeat; ++i)
                {
                    CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX + (k * posZ), 0, 1.25f + (i * changePosZ)), enemyTeamtargetTrackerList);
                }

                for (int i = 0; i < repeat; ++i)
                {
                    CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX + (k * posZ), 0, -1.25f - (i * changePosZ)), enemyTeamtargetTrackerList);
                }

                for (int i = 0; i < repeat; ++i)
                {
                    CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Red, Defines.ELayer.Blue, new Vector3(-changePosX - (k * posZ), 0, 1.25f + (i * changePosZ)), myTeamtargetTrackerList);
                }

                for (int i = 0; i < repeat; ++i)
                {
                    CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Red, Defines.ELayer.Blue, new Vector3(-changePosX - (k * posZ), 0, -1.25f - (i * changePosZ)), myTeamtargetTrackerList);
                }
            }
        }
        else if (count % 2 == 1)
        {
            int repeat = count / 2;

            CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX, 0, 0), enemyTeamtargetTrackerList);

            for (int i = 0; i < repeat; ++i)
            {
                CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX, 0, changePosZ + (i * changePosZ)), enemyTeamtargetTrackerList);
            }

            for (int i = 0; i < repeat; ++i)
            {
                CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Blue, Defines.ELayer.Red, new Vector3(changePosX, 0, -changePosZ - (i * changePosZ)), enemyTeamtargetTrackerList);
            }

            CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Red, Defines.ELayer.Blue, new Vector3(-changePosX, 0, 0), myTeamtargetTrackerList);

            for (int i = 0; i < repeat; ++i)
            {
                CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Red, Defines.ELayer.Blue, new Vector3(-changePosX, 0, changePosZ + (i * changePosZ)), myTeamtargetTrackerList);
            }

            for (int i = 0; i < repeat; ++i)
            {
                CHMMain.Unit.CreateRandomUnit(Defines.ELayer.Red, Defines.ELayer.Blue, new Vector3(-changePosX, 0, -changePosZ - (i * changePosZ)), myTeamtargetTrackerList);
            }
        }
    }
}
