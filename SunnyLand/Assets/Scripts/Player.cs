using System;
using UnityEngine;

public class Player : MonoBehaviour
{
    public float velocidade = 40;
    public float forcaDoPulo = 4;
    public float forcaRolagem = 6f;
    public float tempoRolagem = 0.5f;

    private bool noChao = false;
    private bool andando = false;
    private bool rolando = false;
    private bool desequilibrado = false;

    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private Animator animator;

    private float tempoRolagemAtual = 0f;

    [Header("Configuração do Desequilíbrio (BoxCast)")]
    public Vector2 tamanhoCaixa = new Vector2(0.6f, 0.1f); // largura e altura da caixa
    public float distanciaCaixa = 0.1f; // quanto abaixo do pé do player a caixa fica
    public LayerMask camadaChao;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // ----- Controle da Rolagem -----
        if (rolando)
        {
            tempoRolagemAtual -= Time.deltaTime;
            if (tempoRolagemAtual <= 0f)
            {
                rolando = false;
            }
        }

        andando = false;

        if (!rolando && !desequilibrado) // não anda se estiver rolando ou desequilibrado
        {
            if (Input.GetKey(KeyCode.A))
            {
                transform.position += new Vector3(-velocidade * Time.deltaTime, 0, 0);
                sprite.flipX = true;
                andando = true;
            }

            if (Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(velocidade * Time.deltaTime, 0, 0);
                sprite.flipX = false;
                andando = true;
            }

            if (Input.GetKeyDown(KeyCode.Space) && noChao)
            {
                rb.AddForce(new Vector2(0, forcaDoPulo), ForceMode2D.Impulse);
            }

            if (Input.GetKeyDown(KeyCode.LeftShift) && noChao)
            {
                rolando = true;
                tempoRolagemAtual = tempoRolagem;
                float direcao = sprite.flipX ? -1 : 1;
                rb.AddForce(new Vector2(direcao * forcaRolagem, 0), ForceMode2D.Impulse);
            }
        }

        // ----- Checagem do Desequilíbrio -----
        VerificarDesequilibrio();

        // ----- Animator -----
        animator.SetBool("Andando", andando);
        animator.SetBool("Pulo", !noChao);
        animator.SetBool("Rolando", rolando);
        animator.SetBool("Desequilibrado", desequilibrado);
    }

    void VerificarDesequilibrio()
    {
        if (noChao)
        {
            // Centro da caixa logo abaixo do personagem
            Vector2 centro = (Vector2)transform.position + Vector2.down * distanciaCaixa;

            // Faz o BoxCast parado (distância 0), só pra verificar colisão
            RaycastHit2D hit = Physics2D.BoxCast(centro, tamanhoCaixa, 0f, Vector2.down, 0f, camadaChao);

            if (hit.collider != null)
            {
                // Pega a posição mais próxima do chão
                float centroChao = hit.point.x;

                // Se o centro do player está muito mais à esquerda/direita do ponto de contato
                desequilibrado = Mathf.Abs(transform.position.x - centroChao) > (tamanhoCaixa.x * 0.25f);
            }
            else
            {
                desequilibrado = false;
            }
        }
        else
        {
            desequilibrado = false;
        }
    }

    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Chao"))
        {
            noChao = true;
        }
    }

    void OnCollisionExit2D(Collision2D colisao)
    {
        if (colisao.gameObject.CompareTag("Chao"))
        {
            noChao = false;
        }
    }

    // Gizmos para ver a caixa no editor
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Vector2 centro = (Vector2)transform.position + Vector2.down * distanciaCaixa;
        Gizmos.DrawWireCube(centro, tamanhoCaixa);
    }
}
