using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvinciblePowerup : Powerup {

	protected override void OnCollectPowerup() {
		pm.EnableInvincibility();
	}

}
