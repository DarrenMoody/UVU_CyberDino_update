using UnityEngine;
using System.Collections;

public class DinosaurHealth : Health {
	
	[SerializeField]
	private AudioSource RespawnSoundEffect;
	[SerializeField]
	private float RespawnDelay = 3.0f;

	override public void OnDamage()
	{	
		//Debug.Log (gameObject.name + " took damage.");
	}

	override public void OnHeal()
	{
		//Debug.Log (gameObject.name + " was healed.");
	}

	override public void OnDeath()
	{
		StartCoroutine(deathclock(RespawnDelay));
	}

	IEnumerator deathclock(float duration)
	{
		var dse = GetComponent<DinoStatusEffects>();
		dse.enabled = false;
		networkView.RPC("StartRagdoll", RPCMode.AllBuffered);
		var mc = GetComponent<MotionControl> ();
		mc.enabled = false;

		yield return new WaitForSeconds(duration);

		networkView.RPC("ResetRagdoll", RPCMode.AllBuffered);
		mc.enabled = false; //ragdoll.ResetRacer enabled this, so the dino starts moving before respawn finished. 
		var rm = GetComponent<RespawnManager> ();
		yield return StartCoroutine(rm.Respawn());
		mc.enabled = true;
		Heal (Total);
		
		dse.enabled = true;
	}

	[RPC]
	void StartRagdoll()
	{
		var ragdoll = GetComponent<DinoRagdoll> ();
		if(ragdoll != null) ragdoll.GoRagdoll ();
	}

	[RPC]
	void ResetRagdoll()
	{		
		if(RespawnSoundEffect != null) RespawnSoundEffect.Play();
		var ragdoll = GetComponent<DinoRagdoll> ();
		if(ragdoll != null) ragdoll.ResetRacer ();
	}

}
