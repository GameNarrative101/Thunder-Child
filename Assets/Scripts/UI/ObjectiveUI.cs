using UnityEngine;
using System;

public class ObjectiveUI : MonoBehaviour
{
    [SerializeField] GameObject secureCoreObjective;
    [SerializeField] GameObject extractCoreObjective;



    void Awake()
    {
        PrometheusCore.OnPrometheusCoreCollected += PrometheusCore_OnPrometheusCoreCollected;
    }
    void Start()
    {
        secureCoreObjective.SetActive(true);
        extractCoreObjective.SetActive(false);
    }



    private void PrometheusCore_OnPrometheusCoreCollected(object sender, EventArgs e)
    {
        secureCoreObjective.SetActive(false);
        extractCoreObjective.SetActive(true);
    }
}
