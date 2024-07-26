using UnityEngine;

public class Fireplace : Interactable 
{
    [Header("Main")]
    public bool isOn;
    [SerializeField] private bool startOn = false;
    [SerializeField] private ParticleSystem flameParticle;

    public override void Start() {
        base.Start();

        if(startOn) isOn = true; else isOn = false;

        StartStopFireplace();
    }

    // FIXME: Turning firplace on/off seems broken
    public override void Interact() {
        isOn = !isOn;
        StartStopFireplace();
    }
    public override void Pickup() {
    }
    public override void Drop(bool destroyItem) {
    }

    public void StartStopFireplace() {
        if(isOn) flameParticle.Play(); else flameParticle.Stop();
    }
}
