const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';

let messages = [];
let connectionId = -1;

class Message {
    constructor(fromUserId, message, timeSent) {
        this.fromUserId = fromUserId;
        this.message = message;
        this.timeSent = timeSent;

        messages.push(this);
    }
};

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveConnectionId", id => {
    connectionId = id;
});

connection.on("ReceiveMessage", (fromUserId, toUserId, message, timeSent) => {
    ReceiveMessage(fromUserId, message, timeSent);
});

connection.start().then(() => {
    connection.invoke("RegisterUser", currentUserId);
});

function ReceiveMessage(fromUserId, message, timeSent) {
    new Message(fromUserId, message, timeSent);

    UpdateUI();
}

function UpdateUI() {
    console.log(messages);
}

function SendMessage(toUserId, message) {
    connection.invoke("SendPrivateMessage", toUserId, message);
}