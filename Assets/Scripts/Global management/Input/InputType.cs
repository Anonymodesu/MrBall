using UnityEngine;

public interface InputType<K> {

	K[] DefaultKeys { get; }

	bool buttonDown(Command command);
	bool buttonUp(Command command);
	bool buttonHold(Command command);
	float xAxisMovement();
	float yAxisMovement();

	void setButton(Command command, K newKey);
	K getMapping(Command command);
	//void setXAxisMovement();
	//void setYAxisMovement();
}