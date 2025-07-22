const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';

let messages = [];
let connectionId = -1;

class Message {
    constructor(fromUserId, toUserId, message, timeSent) {
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.message = message;
        this.timeSent = timeSent;

        messages.push(this);
    }
};

function SendMessageConstruct(toUserId, message, timeSent) {
    new Message(currentUserId, toUserId, message, timeSent);
}

function ReceiveMessageConstruct(fromUserId, message, timeSent) {
    new Message(fromUserId, currentUserId, message, timeSent);
}

const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveConnectionId", id => {
    connectionId = id;
});

connection.on("ReceiveMessage", (message, fromUserId, timeSent) => {
    ReceiveMessage(fromUserId, message, timeSent);
});

connection.on("MessageSent", (message, toUserId, timeSent) => {
    SendMessageConstruct(toUserId, message, timeSent);
});

connection.start().then(() => {
    connection.invoke("RegisterUser", currentUserId).then(() => {
        console.log('Registered successfuly');
    });
    
});

function ReceiveMessage(fromUserId, message, timeSent) {
    ReceiveMessageConstruct(fromUserId, message, timeSent);
    UpdateUI();
}

function UpdateUI() {
    console.log(messages); // Temporary, for debugging
}

function SendMessage(toUserId, message) {
    connection.invoke("SendPrivateMessage", currentUserId, toUserId, message);
}