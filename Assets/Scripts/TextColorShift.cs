using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextColorShift : MonoBehaviour {

	TMP_Text textComponent;

	int t = 0;
	byte rate = 9; // MAKE SURE RATE IS A FACTOR OF 255
	int currentValue = 0;
	short flip = 1;

	int r = 0;
	int g = 255;
	int b = 0;

	float colorRateDelay = 0.05f;
	float accumulatedTime = 0f;

	// Start is called before the first frame update
	void Start() {
		textComponent = GetComponent<TMP_Text>();
		textComponent.outlineColor = new Color32((byte)r, (byte)g, (byte)b, 255);
	}

	// Update is called once per frame
	void Update() {
		accumulatedTime += Time.deltaTime;

		if (accumulatedTime >= colorRateDelay) {
			accumulatedTime = 0f;
			t += rate;
			switch (currentValue) {
				case 0:
					r += (rate * flip);
					if (r > 255) r = 255;
					else if (r < 0) r = 0;
					break;
				case 1:
					g += (rate * flip);
					if (g > 255) g = 255;
					if (g < 0) g = 0;
					break;
				case 2:
					b += (rate * flip);
					if (b > 255) b = 255;
					if (b < 0) b = 0;
					break;
			}
			textComponent.outlineColor = new Color32((byte)r, (byte)g, (byte)b, 255);
			if (t > 254) {
				t = 0;
				flip *= -1;
				currentValue = ++currentValue % 3;
			}
		}
	}
}
