using System.Collections;
using System.Collections.Generic;
using VolumetricLines.Utils;
using UnityEngine;

public class PulseEffect : MonoBehaviour {
	public float PulseIntervals;
	public float PulseDelay;
	public GameObject StartPosition;
	public GameObject EndPosition;
	public VolumetricLines.VolumetricLineBehavior line;

	private Vector3 _currentPulseStart;
	private Vector3 _currentPulseEnd;
	private Vector3 _pulseLength;


	// Use this for initialization
	void Start () {
		_pulseLength = new Vector3(Mathf.Abs(StartPosition.transform.localPosition.x - EndPosition.transform.localPosition.x) / PulseIntervals, 0, 0);
	}

	public IEnumerator Pulse() {
		line.LineWidth = 1;

		_currentPulseStart = StartPosition.transform.localPosition;
		_currentPulseEnd = StartPosition.transform.localPosition + _pulseLength;

		while(_currentPulseEnd != EndPosition.transform.localPosition){
			_currentPulseStart = _currentPulseStart - _pulseLength;
			_currentPulseEnd = _currentPulseEnd - _pulseLength;

			yield return new WaitForSeconds(PulseDelay);

			line.StartPos = _currentPulseStart;
			line.EndPos = _currentPulseEnd;
		}

		line.StartPos = StartPosition.transform.localPosition;
		line.EndPos = EndPosition.transform.localPosition; 
		line.LineWidth = 0;
	}
}
