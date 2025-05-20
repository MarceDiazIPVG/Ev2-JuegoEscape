using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class VidaPlayer : MonoBehaviour
{
     private int hitCount = 0;
    private bool isInvulnerable = false;
    private bool juegoTerminado = false;

    [Header("Tiempo")]
    [SerializeField] private TMP_Text textoTiempo;
    [SerializeField] private float tiempoTurno = 15f;
    private float tiempoRestante;

    [Header("Partículas")]
    [SerializeField] private GameObject particulasGolpe;

    [Header("UI Panels")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject victoriaPanel;
    [SerializeField] private TMP_Text textoFinalTiempo;

    void Start()
    {
        tiempoRestante = tiempoTurno;
        gameOverPanel.SetActive(false);
        victoriaPanel.SetActive(false);
    }

    void Update()
    {
        if (juegoTerminado) return;

        tiempoRestante -= Time.deltaTime;

        // Mostrar tiempo en pantalla
        textoTiempo.text = "Tiempo: " + Mathf.CeilToInt(tiempoRestante).ToString();

        if (tiempoRestante <= 0f)
        {
            GameOverPorTiempo();
        }
    }

    public bool ReceiveHit()
    {
        if (isInvulnerable || juegoTerminado)
            return false;

        hitCount++;

        if (hitCount == 1)
        {
            Debug.Log("Primer golpe recibido");
            InstanciarParticulas();
            StartCoroutine(TemporarilyInvulnerable());
            return false;
        }
        else if (hitCount >= 2)
        {
            Debug.Log("Segundo golpe. GameOver");
            GameOver();
            return true;
        }

        return false;
    }

    void InstanciarParticulas()
    {
        if (particulasGolpe != null)
        {
            GameObject efecto = Instantiate(particulasGolpe, transform.position, Quaternion.identity);
            ParticleSystem ps = efecto.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
                Destroy(efecto, ps.main.duration + ps.main.startLifetime.constant);
            }
            else
            {
                Destroy(efecto, 2f);
            }
        }
    }

    IEnumerator TemporarilyInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(1.5f);
        isInvulnerable = false;
    }

    void GameOver()
    {
        juegoTerminado = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Over por kamikaze");
    }

    void GameOverPorTiempo()
    {
        juegoTerminado = true;
        gameOverPanel.SetActive(true);
        Time.timeScale = 0f;
        Debug.Log("Game Over por tiempo agotado");
    }

    public void ReiniciarJuego()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void VolverAlMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu"); // Asegúrate de tener esta escena en el Build Settings
    }

    void OnTriggerEnter(Collider other)
    {
        if (juegoTerminado) return;

        if (other.CompareTag("Meta"))
        {
            LlegarAMeta();
        }
    }

    void LlegarAMeta()
    {
        juegoTerminado = true;
        victoriaPanel.SetActive(true);
        Time.timeScale = 0f;

        float tiempoUsado = tiempoTurno - tiempoRestante;
        textoFinalTiempo.text = "¡Has llegado a la meta!\nTiempo usado: " + Mathf.CeilToInt(tiempoUsado) + "s";
        Debug.Log("Victoria - Tiempo usado: " + tiempoUsado);
    }
}
