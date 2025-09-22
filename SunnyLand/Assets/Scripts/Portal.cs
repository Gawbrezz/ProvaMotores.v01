
public class Portal : MonoBehaviour
{
    public string fase2;
    
    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && fase2 != "")
        {
            SceneManager.LoadScene(fase2);          
        }
    }
}