using UnityEngine;

public class Fireplace : Interactable 
{
    [Header("Main")]
    public bool isOn = false;
    [SerializeField] private ParticleSystem flameParticle;

    private void Start() {
        StartStopFireplace();
    }

    public override void Interact() {
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
