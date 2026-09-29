using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    private Transform rightHand;
    private Transform leftHand;

    private GameObject equippedRightHand;
    private GameObject equippedLeftHand;

    [SerializeField] private GameObject Sword;
    [SerializeField] private GameObject Shield;

    [SerializeField] private Vector3 swordPositionOffset;
    [SerializeField] private Vector3 swordRotationOffset;

    [SerializeField] private Vector3 shieldPositionOffset;
    [SerializeField] private Vector3 shieldRotationOffset;

    [SerializeField] private Vector3 swordScale = Vector3.one;
    [SerializeField] private Vector3 shieldScale = Vector3.one;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _animator = GetComponent<Animator>();
        rightHand = _animator.GetBoneTransform(HumanBodyBones.RightHand);
        leftHand = _animator.GetBoneTransform(HumanBodyBones.LeftHand);

        equippedRightHand = new GameObject("EquippedRightHand");
        equippedLeftHand = new GameObject("EquippedLeftHand");

        equippedRightHand.transform.SetParent(rightHand);
        equippedRightHand.transform.localPosition = Vector3.zero;
        equippedRightHand.transform.localRotation = Quaternion.identity;

        equippedLeftHand.transform.SetParent(leftHand);
        equippedLeftHand.transform.localPosition = Vector3.zero;
        equippedLeftHand.transform.localRotation = Quaternion.identity;


        EquipRightHand(Sword);
        EquipLeftHand(Shield);
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Right Hand: " + rightHand.position);
        Debug.Log("Left Hand: " + leftHand.position);
        Debug.Log("Right Hand rotation: " + rightHand.rotation);
        Debug.Log("Left Hand rotation: " + leftHand.rotation);
    }

    public void EquipRightHand(GameObject equipment)
    {
        if (equippedRightHand != null)
        {
            Destroy(equippedRightHand);
        }

        equippedRightHand = Instantiate(
            equipment,
            rightHand
        );

        equippedRightHand.transform.localPosition = swordPositionOffset;
        equippedRightHand.transform.localEulerAngles = swordRotationOffset;
        equippedRightHand.transform.localScale = swordScale; // Reset scale to 1,1,1
    }

    public void EquipLeftHand(GameObject equipment)
    {
        if (equippedLeftHand != null)
        {
            Destroy(equippedLeftHand);
        }

        equippedLeftHand = Instantiate(
            equipment,
            leftHand
        );

        equippedLeftHand.transform.localPosition = shieldPositionOffset;
        equippedLeftHand.transform.localEulerAngles = shieldRotationOffset;
        equippedLeftHand.transform.localScale = shieldScale; // Reset scale to 1,1,1
    }
}

