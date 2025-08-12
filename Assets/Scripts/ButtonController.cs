using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    public GameObject InventoryPanel;
    // Start is called before the first frame update
    public void Inventory ()
    {
        InventoryPanel.SetActive(true);
    }

    public void InventoryClose()
    {
        InventoryPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
