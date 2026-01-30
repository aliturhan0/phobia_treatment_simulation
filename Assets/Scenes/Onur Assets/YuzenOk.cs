using UnityEngine;

public class YuzenOk : MonoBehaviour
{
    public float yuzmeHizi = 2f;        // Aþaðý yukarý ne kadar hýzlý gitsin?
    public float yuzmeMesafesi = 0.5f;  // Ne kadar yukarý aþaðý oynasýn?

    Vector3 baslangicPozisyonu;

    void Start()
    {
        baslangicPozisyonu = transform.position;
    }

    void Update()
    {
        // Sadece aþaðý yukarý yüz
        float yeniY = baslangicPozisyonu.y + Mathf.Sin(Time.time * yuzmeHizi) * yuzmeMesafesi;

        transform.position = new Vector3(baslangicPozisyonu.x, yeniY, baslangicPozisyonu.z);
    }
}