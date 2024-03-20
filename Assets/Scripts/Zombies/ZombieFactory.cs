using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProduct
{
    IProduct Produce();
}
/// <summary>
/// 这个类只是为了了解【工厂模式】 而使用的
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
