using UnityEngine;
using Cinemachine;

[DisallowMultipleComponent]
public class Minimap : MonoBehaviour
{
    #region Tooltip
    [Tooltip("Populate it with the child minimapPlayer gameobject")]
    #endregion
    [SerializeField] private GameObject minimapPlayer;

    private Transform playerTrans;

    private void Start()
    {
        playerTrans = GameManager.Instance.GetPlayer().transform;
        // Populate player as the target for the fcinemachine camera
        CinemachineVirtualCamera cmVirtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
        cmVirtualCamera.Follow = playerTrans;

        //  Set the player icon for the minimap display
        SpriteRenderer spriteRenderer = minimapPlayer.GetComponent<SpriteRenderer>();
        if(spriteRenderer != null)
        {
            spriteRenderer.sprite = GameManager.Instance.GetMinimapIcon();
        }
    }

    private void Update()
    {
        // Update the position of the miniplayer player which follows player movement
        if(playerTrans != null && minimapPlayer != null)
        {
            minimapPlayer.transform.position = playerTrans.position;
        }
    }
}
