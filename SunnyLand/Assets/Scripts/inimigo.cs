using UnityEngine;

public class Inimigo : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rigidbody2D;
    private Animator animator;
    private GameObject player;

    public float distanciaDeVisao = 10;
    public float velocidade = 5;
    public float forcaDoPulo = 7;
    public float tempoEntrePulos = 1.5f;

    private float tempoProximoPulo;
    private bool noChao;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (player != null)
        {
            // Checa se está no chão (usando velocidade vertical)
            noChao = Mathf.Abs(rigidbody2D.linearVelocity.y) < 0.05f;

            // Define animação conforme estado
            animator.SetBool("noChao", noChao);

            tempoProximoPulo -= Time.deltaTime;

            if (tempoProximoPulo <= 0 && noChao)
            {
                // Verifica se o player está dentro do alcance
                if (Mathf.Abs(player.transform.position.x - transform.position.x) < distanciaDeVisao)
                {
                    // Direita
                    if (player.transform.position.x > transform.position.x)
                    {
                        Pular(Vector2.right);
                        spriteRenderer.flipX = false;
                    }
                    // Esquerda
                    else if (player.transform.position.x < transform.position.x)
                    {
                        Pular(Vector2.left);
                        spriteRenderer.flipX = true;
                    }
                }

                tempoProximoPulo = tempoEntrePulos;
            }
        }
    }

    void Pular(Vector2 direcao)
    {
        rigidbody2D.linearVelocity = new Vector2(0, rigidbody2D.linearVelocity.y);
        rigidbody2D.AddForce(new Vector2(direcao.x * velocidade, forcaDoPulo), ForceMode2D.Impulse);
        animator.SetTrigger("pulando");
    }
}
