using UnityEngine;

public class Fireplace : Interactable 
{
    [Header("Main")]
    public bool isOn;
    [SerializeField] private bool startOn = false;
    [SerializeField] private ParticleSystem flameParticle;

    private void Start() {
        if(startOn) isOn = true; else isOn = false;

        StartStopFireplace();
    }

    public override void Interact() {
        isOn = !isOn;
        StartStopFireplace();
    }
    public override void Pickup() {
    }
    public override void Drop() {
    }

    public void StartStopFireplace() {
        if(isOn) flameParticle.Play(); else flameParticle.Stop();
    }
}