using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerup : Powerup {

	protected override void OnCollectPowerup() {
		pm.IncreaseHealth(1);
	}

}
