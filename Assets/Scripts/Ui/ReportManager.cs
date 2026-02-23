using UnityEngine;

public class ReportManager : MonoBehaviour
{
    public GameObject reportUi;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (reportUi != null)
        {
            reportUi.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetOn()
    {
        if (reportUi != null)
        {
            reportUi.SetActive(true);
        }
    }
    
    public void SetOff()
    {
        if (reportUi != null)
        {
            reportUi.SetActive(false);
        }
    }
}
