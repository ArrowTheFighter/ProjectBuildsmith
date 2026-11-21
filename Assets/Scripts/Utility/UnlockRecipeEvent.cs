using UnityEngine;

public class UnlockRecipeEvent : MonoBehaviour
{
    [SerializeField] string recipe_id;

    public void UnlockRecipe()
    {
        ScriptRefrenceSingleton.instance.gameplayUtils.UnlockRecipe(recipe_id);
    }
}
