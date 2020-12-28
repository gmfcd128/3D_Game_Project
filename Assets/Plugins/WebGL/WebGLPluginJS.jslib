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
            unityInstance.SendMessage("LobbyUIManager", "OnNewChallenge", data);
            this.socket.emit("playerUnavailable", "");
        }.bind(this));
        this.socket.on("acceptChallenge", function(data) {
            this.serverReady = false;
            console.log("socket event: acceptChallenge");
            unityInstance.SendMessage("LobbyUIManager", "EnterGame");
        });
        this.socket.on("requestDenied", function(data) {
            unityInstance.SendMessage("LobbyUIManager", "OnRequestDenied");
        });
        this.socket.on("newMessage", function(data) {
            unityInstance.SendMessage("Chat", "UpdateChat", JSON.stringify(data));
        });
        this.socket.on("yourTurn", function(data) {
            unityInstance.SendMessage("PoolGame", "myTurn");
            unityInstance.SendMessage("GameManager", "UpdateHUD", 1);
        });
        this.socket.on("standby", function(data) {
            unityInstance.SendMessage("PoolGame", "standby");
            unityInstance.SendMessage("GameManager", "UpdateHUD", 0);
        });
        this.socket.on("timer", function(data) {
            unityInstance.SendMessage("GameManager", "updateTimer", data);
        });
        this.socket.on("serverReady", function(data) {
            this.serverReady = true;
            unityInstance.SendMessage("GameManager", "OnServerReady");
        });
        this.socket.on("opponentQuit", function(data) {
            unityInstance.SendMessage("GameManager", "OnOpponentQuit");
        });
        // the data from game event is already serlized string 
        this.socket.on("CuePositionChange", function(data) {
            unityInstance.SendMessage("PoolGame", "OnCuePositionChange", data);
        });
        this.socket.on("CameraPositionChange", function(data) {
            unityInstance.SendMessage("PoolGame", "OnCameraPositionChange", data);
        });
        this.socket.on("continue", function(data) {
            unityInstance.SendMessage("PoolGame", "OnContinue");
        });
        this.socket.on("StrikeDirectionChange", function(data) {
            unityInstance.SendMessage("PoolGame", "OnStrikeDirectionChange", data);
        });
        this.socket.on("CueBallStriked", function(data) {
            unityInstance.SendMessage("PoolGame", "OnCueBallStriked", data);
        });
        this.socket.on("endMatch", function(data) {
            unityInstance.SendMessage("PoolGame", "EndMatch");
        });

        this.socket.emit("joinGame", this.username);
    },
    
    RequestChallenge : function(data) {
        var opponentSocketID = Pointer_stringify(data);
        this.socket.emit("requestChallenge", opponentSocketID);
        this.socket.emit("playerUnavailable", "");
    },

    DenyRequest : function(data) {
        var opponentSocketID = Pointer_stringify(data);
        this.socket.emit("denyRequest", opponentSocketID);
        this.socket.emit("joinGame", this.username);
    },

    AcceptChallenge : function(data) {
        this.serverReady = false;
        var opponentSocketID = Pointer_stringify(data);
        this.socket.emit("acceptChallenge", opponentSocketID);
    },

    JoinGame : function() {
        if (this.username != null) {
            this.socket.emit("joinGame", this.username);
        }
    },

    SocketEmit : function(evt, msg) {
        var event = Pointer_stringify(evt);
        var message = Pointer_stringify(msg);
        this.socket.emit(event, message);
    },

    PlayerReady : function() {
        this.socket.emit("playerReady", "");     
    }

});