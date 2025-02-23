using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerControl : MonoBehaviour
{
    private SpriteRenderer mySprite;

    private Vector2 target;
    public GameObject doorObj;
    public GameObject itemObj;
    public GameObject UseObj;

    public Canvas UICanvas;
    public GameObject ItemSlotParent;
    public Transform[] ItemSlots;    //itemslot positions in the inventory (an empty gameobject sits where they are)
    public Transform emptyItemSlot;

    private GameObject prefabItem;   //an empty prefab to select them from the switch statement
    public GameObject CircleItem;   //make these images for the inventory screen buttons so you can click and use them
    public GameObject CircleItem2;   
    private string storedUseItemRef = "";

    public TextMeshProUGUI InspectText;

    public bool itemlock;
    public bool itemUseBool;
    public bool movingToObject;

    private float movementSpeed;
    float horizontal;
    private Rigidbody2D myRigidbody;

    private Animator anim;

    private void Start()
    {
        myRigidbody = GetComponent<Rigidbody2D>();
        movementSpeed = 5;
        horizontal = 0;
        target = new Vector2(transform.position.x, transform.position.y);
        itemlock = false;
        itemUseBool = false;
        movingToObject = false;
        InspectText.enabled = false;

        ItemSlots = ItemSlotParent.GetComponentsInChildren<Transform>();

        mySprite = GetComponentInChildren<SpriteRenderer>();
        anim = GetComponentInChildren<Animator>();
    }

    IEnumerator Pickup()
    {
        anim.SetBool("isPickup", true);             //had a pickup animation rigged and working, but it had to be deleted for the new sprite sheet that only had a walk animation
        yield return new WaitForSeconds(0.5f);

        switch (itemObj.name)   //switch statement to find the prefab of the item's namesake
        {
            case "Circle":
                prefabItem = CircleItem;
                break;
            case "Circle2":
                prefabItem = CircleItem2;
                break;
        }

        //copy item to inventory
        for (int i = 0; i < ItemSlots.Length;)
        {
            if (ItemSlots[i].transform.childCount <= 0)
            {
                emptyItemSlot = ItemSlots[i];
                break;
            }
            else
                i++;
        }
        var pickupItem = Instantiate(prefabItem, emptyItemSlot.position, Quaternion.identity);   //need code to determine free item slots
        pickupItem.transform.SetParent(emptyItemSlot, false);
        pickupItem.transform.position = emptyItemSlot.position;

        //destroy item from world
        Destroy(itemObj);
        itemObj = null;
        itemlock = false;
        anim.SetBool("isPickup", false);
    }

    public void WhichUseItem(string s)
    {
        storedUseItemRef = s;
        itemUseBool = true;
    }

    IEnumerator UseTextEnum()   //turns the text bubble on above the character's head when they use an item on another item.
    {
        InspectText.enabled = true;

        yield return new WaitForSeconds(3.0f);

        InspectText.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "light")
        {
            Debug.Log("In Light");
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "light")
        {
            Debug.Log("Exiting Light");
        }
    }

    private void HandleMovement(float horizontal)
    {
        myRigidbody.velocity = new Vector2(horizontal * movementSpeed, myRigidbody.velocity.y);
    }

    IEnumerator ItemCatalogue()
    {
        anim.SetBool("isUsing", true);                      //same thing for the animation that played when you used an item on an object. The new sprite from the artists only had a walk animation.
        switch (UseObj.name)   //switch statement to find the prefab of the item's namesake
        {
            case "Green Interactible":
                if (storedUseItemRef == "Circle")
                {
                    InspectText.text = "Now that's unlocked.";
                    StartCoroutine(UseTextEnum());
                    //Destroy(UseObj); //to destroy the item clicked on
                    Destroy(GameObject.Find("CirclePrefab(Clone)"));    //storing the object would be more elegant
                    Destroy(UseObj);
                    itemUseBool = false;
                    itemlock = false;
                    break;
                }
                else
                {
                    InspectText.text = "That doesn't work with that.";
                    StartCoroutine(UseTextEnum());
                    itemUseBool = false;
                    itemlock = false;
                    break;
                }
        }

        yield return new WaitForSeconds(0.2f);
        for (int i = 0; i < ItemSlots.Length - 1; i++)
        {
            if (ItemSlots[i].transform.childCount <= 0 && ItemSlots[i + 1].transform.childCount > 0)
            {
                ItemSlots[i + 1].transform.SetParent(ItemSlots[i], false);
                ItemSlots[i + 1].transform.position = ItemSlots[i].position;
            }
        }
        yield return new WaitForSeconds(0.5f);
        anim.SetBool("isUsing", false);
    }

    void Update()
    {
        //float horizontal = (Input.GetKey(KeyCode.A)) ? -1f : (Input.GetKey(KeyCode.D)) ? 1f : 0f;
        if (Input.GetKey(KeyCode.A) || (Input.GetKey(KeyCode.D)))
        {
            if (Input.GetKey(KeyCode.A))
            {
                horizontal = -1f;
                mySprite.flipX = true;
                anim.SetBool("isWalking", true);
            }
            if (Input.GetKey(KeyCode.D))
            {
                horizontal = 1f;
                mySprite.flipX = false;
                anim.SetBool("isWalking", true);
            }
        }
        else
        {
            horizontal = 0f;
            anim.SetBool("isWalking", false);       //this would turn on the idle animation, but there isn't one.
        }
        HandleMovement(horizontal);

        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction, Mathf.Infinity);
            if (hit)
            {
                if (itemlock == false)
                {
                    if (hit.collider.gameObject.tag == "item")
                    {
                        itemObj = hit.collider.gameObject;
                        if (Vector2.Distance(itemObj.transform.position, transform.position) < 4.0f)
                        {
                            //pick it up
                            StartCoroutine(Pickup());
                            itemlock = true;
                        }
                    }
                    if (hit.collider.gameObject.tag == "Interactable")
                    {
                        UseObj = hit.collider.gameObject;   //store the selected world object to use an item on
                    }
                }
            }
        }

        if (itemUseBool)        //you click the use button on an item
        {
            if (Input.GetMouseButtonDown(0))    //you click somewhere in the scene
            {
                //Getting the coordinates of the mouseposition in game
                Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                //store the location
                target = new Vector2(mousePos.x, transform.position.y);

                if (UseObj != null)     //A few lines above it detects if you clicked an interactible object and stores that in useObj. Here it goes into that.
                {
                    if (itemlock == false)  //lock it from repeatedly entering
                    {
                        movingToObject = true;
                    }
                }
                else
                    itemUseBool = false;
            }
        }
        if (movingToObject)
        {
            anim.SetBool("isWalking", true);
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(UseObj.transform.position.x, transform.position.y), Time.deltaTime * 5f);

            if (Vector2.Distance(UseObj.transform.position, transform.position) < 4.0f)    //do the thing only when close to the object.
            {
                movingToObject = false;
                StartCoroutine(ItemCatalogue());
            }
        }
    }
}
