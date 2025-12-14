using UnityEngine;

public class Act1CarrasquetaController : MonoBehaviour
{
    private RewardManager rewardManager;

    void Start()
    {
        if (!GameManager.Check("Act1_IntroDone"))
            return;

        if (GameManager.Check("Act1_ReachedCarrasqueta"))
            return;

        rewardManager = FindObjectOfType<RewardManager>();
        if (rewardManager != null)
        {
            rewardManager.giveReward("ACT1_REACHED_CARRASQUETA");
        }
        else
        {
            Debug.LogWarning("Act1CarrasquetaController: no se encontró RewardManager en la escena.");
        }
    }
}
