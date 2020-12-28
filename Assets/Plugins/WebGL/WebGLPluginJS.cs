using System.Runtime.InteropServices;

/// <summary>
/// Class with a JS Plugin functions for WebGL.
/// </summary>
public static class WebGLPluginJS
{
    [DllImport("__Internal")]
    public static extern void Init(string username);

    [DllImport("__Internal")]
    public static extern void RequestChallenge(string opponentSocketID);

    [DllImport("__Internal")]
    public static extern void DenyRequest(string opponentSocketID);

    [DllImport("__Internal")]
    public static extern void AcceptChallenge(string opponentSocketID);

    [DllImport("__Internal")]
    public static extern void JoinGame();

    [DllImport("__Internal")]
    public static extern void SocketEmit(string evt, string message);
    [DllImport("__Internal")]
    public static extern void PlayerReady();


}

