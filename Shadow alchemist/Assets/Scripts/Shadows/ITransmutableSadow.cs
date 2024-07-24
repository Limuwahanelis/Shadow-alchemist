using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITransmutableSadow 
{
    public void Transmutate(Vector2 directionTotakeFrom);

    public void RevertTransmutation();
}
