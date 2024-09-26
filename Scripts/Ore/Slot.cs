using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 슬롯
public class Slot : MonoBehaviour
{
    [SerializeField]
    public Image image;

    [SerializeField]
    public TMP_Text textCount;

    private OreData _oreData;
    public OreData oreData
    {
        get { return _oreData; }
        set
        {
            _oreData = value;
            UpdateSlotUI();
            //아래 코드를 위 함수로 변경
            //if (_oreData != null)
            //{
            //    image.sprite = _oreData.minedOreSprite;
            //    image.color = new Color(1, 1, 1, 1);
            //}
            //else
            //{
            //    image.sprite = null;
            //    image.color = new Color(1, 1, 1, 0);
            //}
        }
    }

    private int _count;
    public int count
    {
        get { return _count; }
        set
        {
            _count = value;
            if (oreData == null && oreId != -1)  // oreData가 null일 경우 oreId를 통해 초기화
            {
                oreData = FindOreDataById(oreId);
            }

            if (oreData != null)
            {
                image.sprite = oreData.minedOreSprite;
                textCount.text = _count > 0 ? _count.ToString() : "";
            }
            else
            {
                image.sprite = null;
                textCount.text = "";
            }

            if (_count <= 0)
            {
                oreData = null;
                image.color = new Color(1, 1, 1, 0);
                textCount.text = "";
            }
        }
    }

    public int maxCapacity;
    public int oreId;  // oreId를 저장하기 위한 변수

    private void Awake()
    {
        textCount.text = "";
    }
    private void UpdateSlotUI() {
        if (oreData != null) {
            image.sprite = oreData.minedOreSprite;
            textCount.text = _count > 0 ? _count.ToString() : "";
            image.color = new Color(1, 1, 1, 1);
        }
        else {
            image.sprite = null;
            textCount.text = "";
            image.color = new Color(1, 1, 1, 0);
        }
    }

    private OreData FindOreDataById(int oreId)
    {
        // ScriptableDatas 폴더에서 OreData를 로드하여 찾기
        OreData[] oreDataArray = Resources.LoadAll<OreData>("ScriptableDatas");
        foreach (OreData data in oreDataArray)
        {
            if (data.oreId == oreId)
            {
                return data;
            }
        }
        return null;
    }
}
