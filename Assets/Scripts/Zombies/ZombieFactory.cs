using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct
{
    IProduct Produce();
}
/// <summary>
/// �����ֻ��Ϊ���˽⡾����ģʽ�� ��ʹ�õ�
/// </summary>
public class ZombieFactory
{

    public IProduct Produce(IProduct protype)
    {
        return protype.Produce();
    }
    public IProduct Produce(GameObject protype)
    {
        return protype.GetComponent<IProduct>().Produce();
    }
}
