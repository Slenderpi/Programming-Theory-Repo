using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointPowerup : Powerup {

	int pointValue = 10;

	protected override void OnCollectPowerup() {
		pm.IncreasePoints(pointValue);
	}

}
