using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour {
    public static UIManager instance {
        get {
            if (m_instance == null) {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance;

    [SerializeField]
    GameObject basicUI, missionClearUI, missionFailUI, attackUI, healthStatus, chestUI, generatorUI, equipmentStatus;


    [SerializeField]
    Image specialAttackModeUI;


    //공격모드를 변경할 때마다 컴포넌트를 다시 참조하지 않기 위해서 미리 Start에서 둘다 참조
    //현재 공격모드에 따라 위의 UI의 투명도를 조절하기 위해 가져오는 Image 컴포넌트
    [SerializeField]
    Image specialAttackModeImage;
    [SerializeField]
    TMP_Text specialAttackModeText;
    [SerializeField]
    GameObject keyGuide;


    //공격 방향 표시 UI
    [SerializeField]
    GameObject centerRotation;
    [SerializeField]
    GameObject attackIndicator;


    [SerializeField]
    TMP_Text magAmmoCount;

    //hp UI
    [SerializeField]
    Transform[] hp;
    [SerializeField]
    Sprite hpSprite;
    [SerializeField]
    Sprite shieldSprite;


    [SerializeField]
    Slider hpSlider;

    //상자 UI

    [SerializeField]
    GameObject chestStorageUI;
    GameObject[] selectedInventorySlotUI;
    GameObject[] selectedChestStorageSlotUI;

    [SerializeField]
    GameObject inventorySlot0, inventorySlot1, inventorySlot2, inventorySlot3;
    [SerializeField]
    GameObject chestStorageSlot0, chestStorageSlot1, chestStorageSlot2, chestStorageSlot3, chestStorageSlot4, chestStorageSlot5, chestStorageSlot6, chestStorageSlot7;

    GameObject selectedSlotUI;

    [SerializeField]
    GameObject arrowDown, arrowUp;

    //전력시스템 UI
    [SerializeField]
    GameObject[] generatorIndicator;
    GameObject selectedGenerator;
    [SerializeField]
    Image[] electricImg;
    [SerializeField]
    Slider[] generators;

    //온도 UI
    [SerializeField]
    private ScriptableRendererFeature fullScreenFreezing;
    [SerializeField]
    private ScriptableRendererFeature fullScreenOverheating;
    [SerializeField]
    private Material freezingMaterial;
    [SerializeField]
    private Material overheatingMaterial;

    private int vignetteIntensity = Shader.PropertyToID("_VignetteIntensity");

    //장비 UI
    [SerializeField]
    Transform[] equipment;
    [SerializeField]
    Sprite armorSprite, lightHelmetSprite, paddedJacketSprite, iceCubeSprite;
    [SerializeField]
    int equipmentIndex = 1;

#if UNITY_EDITOR
    private void OnValidate() {
        hp = healthStatus.GetComponentsInChildren<Transform>();
        equipment = equipmentStatus.GetComponentsInChildren<Transform>();
    }
#endif

    private void Awake() {
        if (instance != this) {
            Destroy(gameObject);
        }
    }
    private void Start() {
        SetSlot();

        if (StageManager.Instance.bIsCold) {
            fullScreenFreezing.SetActive(true);
        }
        if (StageManager.Instance.bIsHot) {
            fullScreenOverheating.SetActive(true);
        }
    }

    //공격 UI
    public void ControlAttack(int currentFirePointIndex) {
        attackUI.SetActive(true);
        centerRotation.SetActive(true);
        attackIndicator.transform.localPosition = new Vector2(5f, 0);
        attackIndicator.transform.eulerAngles = new Vector3(0, 0, 90);
        basicUI.SetActive(false);
    }

    public void StopControlAttack() {
        attackUI.SetActive(false);
        centerRotation.SetActive(false);
        basicUI.SetActive(true);
    }

    public void SwitchAttackDirection(Vector3 rotation) {
        centerRotation.transform.eulerAngles = rotation;
        attackIndicator.transform.localPosition = new Vector2((5 * 3.5f) / Mathf.Sqrt(Mathf.Pow(3.5f * Mathf.Cos(centerRotation.transform.eulerAngles.z * Mathf.Deg2Rad), 2) + Mathf.Pow(5 * Mathf.Sin(centerRotation.transform.eulerAngles.z * Mathf.Deg2Rad), 2)), 0);
    }

    public void AttackModeChange(bool isSpecialAttackModeOn, bool magAmmoReady) {
        if (isSpecialAttackModeOn) {
            specialAttackModeImage.color = new Color(1, 1, 1, 1);
            specialAttackModeUI.color = new Color(1, 1, 1, 1);
            specialAttackModeText.color = new Color(1, 1, 1, 1);
            keyGuide.SetActive(false);
        }
        else {
            specialAttackModeImage.color = new Color(1, 1, 1, 0.6f);
            specialAttackModeUI.color = new Color(1, 1, 1, 0.6f);
            specialAttackModeText.color = new Color(1, 1, 1, 0.6f);
            if (magAmmoReady) {
                keyGuide.SetActive(true);
            }
        }
    }

    public void UpdateMagAmmo(int magAmmo, int magCapacity) {
        magAmmoCount.text = magAmmo + "/" + magCapacity;
    }


    //이동 UI
    public void ControlMovement() {
        basicUI.SetActive(false);
    }
    public void StopControlMovement() {
        basicUI.SetActive(true);
    }


    //Hp UI
    public void UpdateHp(int currentHp, int currentShield) {
        int index = 1;
        for(;index<=currentHp;index++) {
            hp[index].GetComponent<Image>().sprite = hpSprite;
            hp[index].gameObject.SetActive(true);
        }
        for(;index<=currentHp+currentShield;index++) {
            hp[index].GetComponent<Image>().sprite = shieldSprite;
            hp[index].gameObject.SetActive(true);
        }
        for (;index< hp.Length;index++) {
            hp[index].gameObject.SetActive(false);
        }
    }

    public void UpdateShipHp(int currentHp) {
        hpSlider.value = currentHp;
    }

    public void SetShipHp(int maxHp) {
        hpSlider.maxValue = maxHp;
        hpSlider.value = maxHp;
    }

    //Game end
    public void MissionClear() {
        missionClearUI.SetActive(true);
    }

    public void MissionFail() {
        missionFailUI.SetActive(true);
    }

    //Chest
    public void SetSlot() {
        selectedInventorySlotUI = new[] { inventorySlot0, inventorySlot1, inventorySlot2, inventorySlot3 };
        selectedChestStorageSlotUI = new[] { chestStorageSlot0, chestStorageSlot1, chestStorageSlot2, chestStorageSlot3, chestStorageSlot4, chestStorageSlot5, chestStorageSlot6, chestStorageSlot7 };
    }

    public void OpenChest() {
        chestStorageUI.SetActive(true);
        selectedSlotUI = selectedInventorySlotUI[0];
        selectedSlotUI.SetActive(true);
    }
    public void CloseChest() {
        selectedSlotUI.SetActive(false);
        chestStorageUI.SetActive(false);
    }

    public void LeaveSlot(int slotIndex, Chest.ChestMode currentMode) {
        switch (currentMode) {
            case Chest.ChestMode.store:
                selectedInventorySlotUI[slotIndex].SetActive(false);
                break;
            case Chest.ChestMode.take:
                selectedChestStorageSlotUI[slotIndex].SetActive(false);
                break;
        }
    }

    public void ChangeSlot(int slotIndex, Chest.ChestMode currentMode) {
        switch (currentMode) {
            case Chest.ChestMode.store:
                selectedSlotUI = selectedInventorySlotUI[slotIndex];
                selectedSlotUI.SetActive(true);
                break;
            case Chest.ChestMode.take:
                selectedSlotUI = selectedChestStorageSlotUI[slotIndex];
                selectedSlotUI.SetActive(true);
                break;
        }
    }

    public void ChangeChestMode(Chest.ChestMode currentMode) {
        switch (currentMode) {
            case Chest.ChestMode.store:
                selectedSlotUI.SetActive(false);
                selectedSlotUI = selectedInventorySlotUI[0];
                selectedSlotUI.SetActive(true);
                arrowDown.SetActive(false);
                arrowUp.SetActive(true);
                break;
            case Chest.ChestMode.take:
                selectedSlotUI.SetActive(false);
                selectedSlotUI = selectedChestStorageSlotUI[0];
                selectedSlotUI.SetActive(true);
                arrowDown.SetActive(true);
                arrowUp.SetActive(false);
                break;
        }
    }


    //Generator
    public void ContollGenerator() {
        basicUI.SetActive(false);
        generatorUI.SetActive(true);
        selectedGenerator = generatorIndicator[0];
        selectedGenerator.SetActive(true);
    }
    public void StopContollGenerator() {
        selectedGenerator.SetActive(false);
        generatorUI.SetActive(false);
        basicUI.SetActive(true);

    }

    public void LeaveGenerator(int index) {
        selectedGenerator = generatorIndicator[index];
        selectedGenerator.SetActive(false);
    }

    public void SelectGenerator(int index) {
        selectedGenerator = generatorIndicator[index];
        selectedGenerator.SetActive(true);
    }

    public void SetPowerGauge(int index, int value) {
        generators[index].maxValue = value;
    }
    public void UpdatePowerGauge(int index, int value) {
        generators[index].value = value;
        if (value == 0) {
            electricImg[index].color = new Color(.7f, .7f, .7f, 1);
        }
        else {
            electricImg[index].color = new Color(1, 1, 1, 1);
        }
    }

    //Temperature
    public void UpdateTemperature(float temperature) {
        float value;
        if (StageManager.Instance.bIsCold) {
            value = 1.5f - temperature * 0.1f;
        }
        else {
            value = 1.5f + (temperature-40f) * 0.1f;
        }
        if(value < 0.5) {
            value = 0.5f;
        }
        else if (value > 2) {
            value = 2;
        }

        if (StageManager.Instance.bIsCold) {
            freezingMaterial.SetFloat(vignetteIntensity, value);
        }
        else {
            overheatingMaterial.SetFloat(vignetteIntensity, value);
        }
    }
    
    //Equipment
    public void UpdateEquipment(int e) {
        switch(e){
            case 1:
                equipment[equipmentIndex].GetComponent<Image>().sprite = armorSprite;
                equipment[equipmentIndex].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                break;
            case 2:
                equipment[equipmentIndex].GetComponent<Image>().sprite = lightHelmetSprite;
                equipment[equipmentIndex].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                break;
            case 3:
                equipment[equipmentIndex].GetComponent<Image>().sprite = paddedJacketSprite;
                equipment[equipmentIndex].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                break;
            case 4:
                equipment[equipmentIndex].GetComponent<Image>().sprite = iceCubeSprite;
                equipment[equipmentIndex].GetComponent<Image>().color = new Color(1, 1, 1, 1);
                break;
            default:
                break;
        }
        equipmentIndex++;
    }
}
