using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] public int hp = 100;
    public float MS = 2f;

    [Header("Pengaturan State Machine")]
    [SerializeField] private float jarakDeteksi = 6f;   // masuk CHASE 
    [SerializeField] private float jarakSerang  = 1.2f; // masuk ATTACK 
    [SerializeField] private float jedaSerang   = 1f;   // detik antar serang 
    
    [SerializeField] private float radiusPatrol = 3f; 
    private Vector2 titikAwal;      // pusat area keliling
    private Vector2 tujuanPatrol;   // titik yang sedang dituju

    // state sekarang -- mulai dari IDLE 
    private StateZombie state = StateZombie.IDLE;
    private float waktuSerangTerakhir;

    protected Transform player;
    protected virtual void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if(playerObj != null)
        {
            player = playerObj.transform;
        }

        // tambahkan di dalam Start() yang sudah ada:
        titikAwal = transform.position;
         PilihTujuanPatrolBaru();
    }

    // Update is called once per frame
    void Update()
    {
        //Kejar();

        // LANGKAH A: tentukan state (aturan pindah)
        PeriksaTransisi();

        // LANGKAH B: jalankan perilaku sesuai state sekarang
        switch (state)
        {
            case StateZombie.IDLE:   PerilakuIdle();   break;
            case StateZombie.PATROL: PerilakuPatrol(); break;
            case StateZombie.CHASE:  PerilakuChase();  break;
            case StateZombie.ATTACK: PerilakuAttack(); break;
        }
    }

    void PerilakuIdle()   { }
    void PerilakuChase()  { Kejar(); }   // method dari OOP
    void PerilakuAttack()
    {
        // menyerang berkala, tidak tiap frame
        if (Time.time >= waktuSerangTerakhir + jedaSerang)
        {
            Serang(); // Method dari OOP
            waktuSerangTerakhir = Time.time;
        }
    }

        void PerilakuPatrol()
    {
        transform.position = Vector2.MoveTowards (
            transform.position, tujuanPatrol, MS * 0.5f * Time.deltaTime);

        if (Vector2.Distance(transform.position, tujuanPatrol) < 0.1f)
            PilihTujuanPatrolBaru();
    }

    void PilihTujuanPatrolBaru()
    {
        Vector2 acak = Random.insideUnitCircle * radiusPatrol;
        tujuanPatrol = titikAwal + acak;
    }

    // void PerilakuPatrol() { Debug.Log(name + ": PATROL"); }
    // void PerilakuChase()  { Debug.Log(name + ": CHASE"); }
    // void PerilakuAttack() { Debug.Log(name + ": ATTACK"); }

    public void Kejar(){
        if (player == null) return;

        transform.position = Vector2.MoveTowards(
            transform.position,
            player.position,
            MS * Time.deltaTime
        );
    }

    public virtual void Serang()
    {
        Debug.Log("Enemy menyerang!");
    }
    
    float JarakKePlayer()
    {
        if (player == null)
            return Mathf.Infinity;

        return Vector2.Distance(transform.position, player.position);
    }

    void PeriksaTransisi ()
    {
        float jarak = JarakKePlayer();   // sudah ada dari materi OOP!

        if (jarak <= jarakSerang) 
             state = StateZombie.ATTACK;      // sangat dekat -> serang
        else if (jarak <= jarakDeteksi)
            state = StateZombie.CHASE;       // terlihat -> kejar 
        else
            state = StateZombie.PATROL;      // jauh -> keliling 
    }
}