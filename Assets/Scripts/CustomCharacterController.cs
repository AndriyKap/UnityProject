using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomCharacterController : MonoBehaviour
{
    public Animator anim;
    public Rigidbody rig;
    public Transform mainCamera;
    public float jumpForce = 3.5f; 
    public float walkingSpeed = 2f;
    public float runningSpeed = 6f;
    public float currentSpeed;
    private float animationInterpolation = 1f;
    public InventoryManager inventoryManager;
    public QuickslotInventory quickslotInventory;
    public AudioSource footspetsSound;


    public Transform aimTarget;
    public Transform hitTarget;
    
    public Vector3 hitTargetOffset;
    // Start is called before the first frame update
    void Start()
    {
        // Перекріплюємо курсор до середини екрану
        Cursor.lockState = CursorLockMode.Locked;
        // і робимо його невидимим
        Cursor.visible = false;
    }
    void Run()
    {
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1.5f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, runningSpeed, Time.deltaTime * 3);
    }
    void Walk()
    {
        // Mathf.Lerp - відповідає за те, щоб кожен кадр число animationInterpolation(в даному випадку) наближалося до 1 зі швидкістю Time.deltaTime * 3.
        // Time.deltaTime - це час між цим кадром та попереднім кадром. Це дозволяє плавно переходити з одного числа до другого НЕЗАЛЕЖНО ВІД КАДРІВ У СЕКУНДУ (FPS)!
        animationInterpolation = Mathf.Lerp(animationInterpolation, 1f, Time.deltaTime * 3);
        anim.SetFloat("x", Input.GetAxis("Horizontal") * animationInterpolation);
        anim.SetFloat("y", Input.GetAxis("Vertical") * animationInterpolation);

        currentSpeed = Mathf.Lerp(currentSpeed, walkingSpeed, Time.deltaTime * 3);
    }

    public void ChangeLayerWeight(float newLayerWeight)
    {
        StartCoroutine(SmoothLayerWeightChange(anim.GetLayerWeight(1), newLayerWeight, 0.3f));
    }

    IEnumerator SmoothLayerWeightChange(float oldWeight, float newWeight, float changeDuration)
    {
        float elapsed = 0f;
        while(elapsed < changeDuration)
        {
            float currentWeight = Mathf.Lerp(oldWeight, newWeight, elapsed / changeDuration);
            anim.SetLayerWeight(1, currentWeight);
            elapsed += Time.deltaTime;
            yield return null;
        }
        anim.SetLayerWeight(1, newWeight);
    }

    private void Update()
    {
   
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if(quickslotInventory.activeSlot != null)
            {
                if (quickslotInventory.activeSlot.item != null)
                {
                    if (quickslotInventory.activeSlot.item.itemName == "Axe")
                    {
                        if (inventoryManager.isOpened == false)
                        {
                            anim.SetBool("Hit", true);
                        }
                    }
                }
            }
        }
        if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            anim.SetBool("Hit", false);
        }
       
        // Встановлюємо поворот персонажа, коли камера повертається 
        transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x,mainCamera.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
        
        // Чи затиснуті кнопки W та Shift?
        if(Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.LeftShift))
        {
            // Чи затиснуті ще кнопки A S D?
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
            {
                // Якщо так, то ми йдемо пішки
                Walk();
            }
            // Якщо ні, тоді біжимо!
            else
            {
                Run();
            }
        }
        // Якщо W & Shift не затиснуті, ми просто йдемо пішки
        else
        {
            Walk();
        }
        //Якщо затиснутий пробіл, то в аніматорі відправляємо повідомлення тригеру, який активує анімацію стрибка
        if (Input.GetKeyDown(KeyCode.Space))
        {
            anim.SetTrigger("Jump");
        }

        Ray desiredTargetRay = mainCamera.GetComponent<Camera>().ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        Vector3 desiredTargetPosition = desiredTargetRay.origin + desiredTargetRay.direction * 1.5f; // changed from 0.7 to 1.5
        aimTarget.position = desiredTargetPosition;
        //hitTarget.position = new Vector3(desiredTargetPosition.x + hitTargetOffset.x, desiredTargetPosition.y + hitTargetOffset.y, desiredTargetPosition.z + hitTargetOffset.z);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Тут ми задаємо рух персонажа в залежності від напрямку в який дивиться камера
        // Зберігаємо напрямок вперед і праворуч від камери
        Vector3 camF = mainCamera.forward;
        Vector3 camR = mainCamera.right;
        // Щоб напрями вперед і вправо не залежали від того, чи дивиться камера вгору або вниз, інакше коли ми дивимося вперед, персонаж буде йти швидше, ніж коли дивиться вгору або вниз
        // Можете самі перевірити, що буде прибравши camF.y = 0 і camR.y = 0 :)
        camF.y = 0;
        camR.y = 0;
        Vector3 movingVector;
        // Тут ми множимо наше натискання на кнопки W&S на напрямок камери вперед і додаємо до натискань на кнопки A&D і множимо на напрямок камери вправо
        movingVector = Vector3.ClampMagnitude(camF.normalized * Input.GetAxis("Vertical") * currentSpeed + camR.normalized * Input.GetAxis("Horizontal") * currentSpeed,currentSpeed);
        // Magnitude – це довжина вектора. я поділяю довжину на currentSpeed так як ми примножуємо цей вектор на currentSpeed на 86 рядку. Я хочу отримати кількість максимум 1.
        anim.SetFloat("magnitude", movingVector.magnitude/currentSpeed);
        // Тут ми рухаємо персонажа! Встановлюємо рух тільки x & z тому що ми не хочемо щоб наш персонаж злітав у повітря
        rig.velocity = new Vector3(movingVector.x, rig.velocity.y,movingVector.z);
       // Виправляємо баг, що персонаж крутився на місці
        rig.angularVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            footspetsSound.enabled = true;
        }
        else
        {
            footspetsSound.enabled = false;
        }
    }
    
    public void Jump()
    {
        // Виконуємо стрибок за командою анімації.
        rig.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
    
    public void Hit()
    {
        foreach(Transform item in quickslotInventory.allWeapons)
        {
            if (item.gameObject.activeSelf)
            {
                item.GetComponent<GatherResources>().GatherResource();
            }
        }
    }
}
