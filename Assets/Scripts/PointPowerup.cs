using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPowerup : Powerup {

	int gemValue = 1;

	protected override void OnCollectPowerup() {
		pm.IncreaseGemCount(gemValue);
	}

}
