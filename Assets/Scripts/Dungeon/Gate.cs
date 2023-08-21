using UnityEngine;

[RequireComponent(typeof(Animator))]
[DisallowMultipleComponent]
public class Gate : MonoBehaviour
{
    #region Header OBJECT REFERENCES
    [Space(10)]
    [Header("OBJECT REFERENCES")]
    #endregion

    #region Tooltip
    [Tooltip("Populate this with BoxCollider2D component on the gateCollider gameobject")]
    #endregion

    [SerializeField] private BoxCollider2D gateCollider;

    [HideInInspector] public bool isBossRoomGate = false;

    public SoundEffectSO openDoorEffect;

    private BoxCollider2D gateTrigger;
    private bool isOpen = false;
    private bool isOpened = false;
    private Animator animator;

    private void Awake()
    {
        // Deactivate the Gate Collider by Default
        gateCollider.enabled = false;

        // Load Component
        animator = GetComponent<Animator>();
        gateTrigger = GetComponent<BoxCollider2D>();

    }

    private void OnEnable()
    {
        animator.SetBool(Settings.open, isOpen);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ( collision.tag == Settings.playerTag || collision.tag == Settings.playerWeapon)
        {
            SoundEffectManager.Instance.Play(openDoorEffect);
            GateOpen();
        }
    }
    // Open the Gate
    public void GateOpen()
    {
        if (!isOpen)
        {
            isOpen = true;
            isOpened = true;
            gateCollider.enabled = false;
            gateTrigger.enabled = false;
            animator.SetBool(Settings.open, true);
        }
    }
    
    // Lock the Gate
    public void GateLock()
    {
        isOpen = false;
        gateCollider.enabled = true;
        gateTrigger.enabled = false;
        animator.SetBool(Settings.open, false);
    }
    
    // Unlock the Gate
    public void GateUnlock()
    {
        gateCollider.enabled = false;
        gateTrigger.enabled = true;

        if(isOpened == true)
        {
            isOpen = false;
            GateOpen();
        }
    }

}
