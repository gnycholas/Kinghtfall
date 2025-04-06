using UnityEngine;

public class Requirement : ScriptableObject
{
    public virtual bool Check(GameplayController controller)
    {
        return false;
    }
}
