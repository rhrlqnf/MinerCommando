using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Scriptable Object�� ���� ���� �����͵��� �迭�� ����
//���̵�� �迭 �ε����� ��ġ
//�̸��� �Ŵ����� �ƴ϶� �ٸ� ������ �ٲ���ҵ�
public class ItemManager : MonoBehaviour {
    public static OreData[] ores;

    private void Start() {
        ores = Resources.LoadAll<OreData>("ScriptableDatas");
    }
}
