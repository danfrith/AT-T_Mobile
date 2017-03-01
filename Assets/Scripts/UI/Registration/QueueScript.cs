using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class QueueScript : MonoBehaviour {

    public Transform QueueItemContainer;
    public Transform CurrentPlayerContainer;

    public Transform QueueItemPrefab;
    public Transform CurrentItemPrefab;
    

    public RegistrationForm RegistrationManager;

    void OnEnable()
    {
        if (QueueItemContainer == null)
            Logger.LogError("QueueItemContainer is missing in QueueScript" + name);

        if (QueueItemPrefab == null)
            Logger.LogError("QueueItemPrefab is missing in QueueScript" + name);

        if (CurrentItemPrefab == null)
            Logger.LogError("CurrentItemPrefab is missing in QueueScript" + name);

        if (CurrentPlayerContainer == null)
            Logger.LogError("CurrentPlayerContainer is missing in QueueScript" + name);

        if (RegistrationManager == null)
            Logger.LogError("RegistrationManager is missing in QueueScript " + name);
        else
        {
            RegistrationManager.RegistrantsUpdatedHandler += new RegistrationForm.RegistrantsUpdated(UpdateScores);
            RegistrationManager.CurrentPlayerUpdatedHandler += new RegistrationForm.CurrentPlayerUpdated(SetCurrentPlayer);
        }

        ClearCurrentPlayer();
    }

    void ClearItems()
    {
        Logger.Log("Clearing Queue items");

        // TODO possibly breaks if the indexes are updated during the loop
        for (int i = 0; i < QueueItemContainer.childCount; i++)
        {
            Destroy(QueueItemContainer.GetChild(i).gameObject);
        }

    }

   
    void ClearCurrentPlayer()
    {
        for (int i = 0; i < CurrentPlayerContainer.childCount; i++)
        {
            GameObject.Destroy(CurrentPlayerContainer.GetChild(i).gameObject);
        }
    }

    void SetCurrentPlayer()
    {
        Logger.LogDebug("Queue:SetCurrentPlayer: called");

        ClearCurrentPlayer();

        if (RegistrationManager.HasCurrentPlayer())
            InstantiateCurrentPlayerItem(RegistrationManager.GetCurrentPlayer());

    }

    private Transform InstantiateCurrentPlayerItem(ScoreData _item)
    {
        Transform t = (Transform)Instantiate(CurrentItemPrefab);
        t.transform.SetParent(CurrentPlayerContainer, false);

        CurrentPlayer sb = t.gameObject.GetComponent<CurrentPlayer>();

        if (sb == null)
        {
            Logger.LogError("CurrentPlayer prefab item spawned without a QueueItem CurrentPlayer " + _item.FirstName);
            return t;
        }
        else
            sb.Init(_item);

        return t;

    }

    private bool bUpdateScores = true;

    private void PopulateList()
    {
        List<ScoreData> currentScores = RegistrationManager.GetRegistrants();

        int count = 0;
        foreach (ScoreData a in currentScores)
        {
            count++;
            //Logger.Log("Adding item" + a.FirstName);
            InstantiateQueueItem(count, a);
        }

    }

    private Transform InstantiateQueueItem(int _index, ScoreData _item)
    {
        Transform t = (Transform)Instantiate(QueueItemPrefab);
        t.transform.SetParent(QueueItemContainer, false);

        QueueItem sb = t.gameObject.GetComponent<QueueItem>();
        if (sb == null)
        {
            Logger.LogError("Queueitem prefab item spawned without a QueueItem component " + _item.FirstName);
            return t;
        }
        else
            sb.Init(_item);

        return t;

    }

    public void UpdateScores()
    {
        ClearItems();
        PopulateList();
    }

}
