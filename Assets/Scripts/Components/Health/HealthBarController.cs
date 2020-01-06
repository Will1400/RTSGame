using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[RequireComponent(typeof(HealthSystem))]
public class HealthBarController : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private Image healthBar;
    [SerializeField]
    private HealthSystem healthSystem;

    void Start()
    {
        if (healthSystem == null)
            healthSystem = GetComponent<HealthSystem>();
        healthSystem.HealthChanged += UpdateHealthBar;
        healthSystem.OnDeath += () => { canvas.gameObject.SetActive(false); };
        UpdateHealthBar();
    }

    void FixedUpdate()
    {
        if (canvas.gameObject.activeSelf)
            canvas.transform.LookAt(canvas.transform.position + Camera.main.transform.rotation * Vector3.forward, Camera.main.transform.rotation * Vector3.up);
    }

    void UpdateHealthBar()
    {
        if (healthSystem.Health < healthSystem.StartHealth)
            canvas.gameObject.SetActive(true);
        else
            canvas.gameObject.SetActive(false);

        healthBar.fillAmount = healthSystem.Health / healthSystem.StartHealth;
    }

    private void OnDestroy()
    {
        healthSystem.HealthChanged -= UpdateHealthBar;
        healthSystem.OnDeath -= () => { canvas.gameObject.SetActive(false); };

    }
}
