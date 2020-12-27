public interface IGameObjectState {
	void Update();
	void FixedUpdate();
	void LateUpdate();

	void OnSocketEvent(string eventName, string data);
}