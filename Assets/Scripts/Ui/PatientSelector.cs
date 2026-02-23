using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DermapathologieVR.Ui;

[System.Serializable]
public class Patient
{
    public string patientName;
    public string imageName; // e.g. "patient1.png"
}

public class PatientSelector : MonoBehaviour
{
    [Header("Patient Data")]
    public List<Patient> patients = new List<Patient>();

    [Header("UI References")]
    public TMP_Dropdown patientDropdown;
    public RawImage imageViewer;

    private void Start()
    {
        PopulateDropdown();
        patientDropdown.onValueChanged.AddListener(OnPatientSelected);
        if (patients.Count > 0)
        {
            OnPatientSelected(0);
        }
    }

    private void PopulateDropdown()
    {
        patientDropdown.ClearOptions();
        List<string> names = new List<string>();
        foreach (var patient in patients)
        {
            names.Add(patient.patientName);
        }
        patientDropdown.AddOptions(names);
    }

    private void OnPatientSelected(int index)
    {
        if (index < 0 || index >= patients.Count) return;
        // Images must be in Assets/Resources/Skin Images/ and imported as Default (Texture2D)
        string imagePath = "Skin Images/" + System.IO.Path.GetFileNameWithoutExtension(patients[index].imageName);
        Texture2D tex = Resources.Load<Texture2D>(imagePath);
        if (tex != null && imageViewer != null)
        {
            imageViewer.texture = tex;
            
            // Reset the image view (zoom, pan, rotation) when loading a new image
            VRImageManipulator manipulator = imageViewer.GetComponent<VRImageManipulator>();
            if (manipulator != null)
            {
                manipulator.ResetImageTransform();
            }
        }
        else if (imageViewer != null)
        {
            imageViewer.texture = null;
        }
    }
}
