using TMPro;
using UnityEngine;

public class Coin : MonoBehaviour
{

    public AudioClip coinClip;

    public int coinsToGive = 1;

    private TextMeshProUGUI cointText;

    private void Start()
    {
        cointText = GameObject.FindWithTag("CoinText").GetComponent<TextMeshProUGUI>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player player = collision.gameObject.GetComponent<Player>();
            player.coins += coinsToGive;
            player.PlaySFX(coinClip, 0.4f);
            cointText.text = player.coins.ToString();
            Destroy(gameObject);
        }
    }
}
