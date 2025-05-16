using UnityEngine;

public class Requirement : ScriptableObject
{
    public virtual bool Check(GameplayController controller)
    {
        if (controller == null)
            return true;
        return false;
    }
}
