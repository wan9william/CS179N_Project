using UnityEngine;

public class Stim : Equippable
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void Use(ref Player player)
    {
        Debug.Log("STIM USED");
        player.TakeDamage(-25);

        int index = player.getInventory().GetSelectedSlot();
        player.getInventory().DecrementItem(index);
        //Destroy(this.gameObject);
        player.SelectEquipped();

        if (audioSource)
        {
            audioSource.Stop();
            audioSource.Play();
            Debug.Log("PLAY");
        }
    }
}
