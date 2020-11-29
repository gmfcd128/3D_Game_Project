// Creating functions for the Unity
mergeInto(LibraryManager.library, {
   // Code will go here...
    Init : function(username) {
        this.username = Pointer_stringify(username);
        this.socket = io.connect();
        this.socket.on("playerListUpdate", function(data) {
            console.log(JSON.stringify(data));
            unityInstance.SendMessage("LobbyUIManager", "updatePlayerList", JSON.stringify(data));
        });
        this.socket.on("requestChallenge", function(data) {
            unityInstance.SendMessage("LobbyUIManager", "onChallengeRequest", data);
            this.socket.emit("playerUnavailable", "");
        });
        this.socket.emit("joinGame", this.username);
    },
    
    RequestChallenge : function() {
        this.socket.emit("requestChallenge", this.socket.id);
        this.socket.emit("playerUnavailable", "");
    },

    DenyRequest : function(opponentSocketID) {
        this.socket.emit("denyRequest", opponentSocketID);
        this.socket.emit("joinGame", this.username);
    }

    
});