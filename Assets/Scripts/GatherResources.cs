using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatherResources : MonoBehaviour
{
    public Camera mainCamera;
    public LayerMask layerMask;
    public InventoryManager inventoryManager;
    public ItemScriptableObject resource;
    public int resourceAmount;
    public GameObject hitFX;
    public AudioSource axeHitSound;

    public void GatherResource()
    {
        RaycastHit hit;
        Ray ray = mainCamera.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        if (Physics.Raycast(ray, out hit, 1.5f, layerMask))
        {
            if(hit.collider.GetComponent<TreeHealth>().health >= 1)
            {
                axeHitSound.Play();
                Instantiate(hitFX, hit.point, Quaternion.LookRotation(hit.normal));
                inventoryManager.AddItem(resource, resourceAmount);
                hit.collider.GetComponent<TreeHealth>().health--;

                if (hit.collider.GetComponent<TreeHealth>().health <= 0)
                {
                    hit.collider.GetComponent<TreeHealth>().TreeFall();

                    // Використовуємо hit.normal як напрямок падіння дерева
                    hit.collider.GetComponent<Rigidbody>().AddForce(hit.normal * 10, ForceMode.Impulse);
                }
            }
        }
    }
}
