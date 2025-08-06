const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';

let messages = [];
let messagingPartners = new Set();
let connectionId = -1;

class Message {
    constructor(fromUserId, toUserId, message, timeSent) {
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.message = message;
        this.timeSent = timeSent;

        messages.push(this);

        if (this.fromUserId == currentUserId) // We sent the message, add the receiver to the set of partners
            messagingPartners.add(toUserId);
        else
            messagingPartners.add(fromUserId); // Otherwise it is us who received the message, so add the sender
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

connection.on("ReceivePreviousMessage", (message, fromUserId, toUserId, timeSent) => {
    new Message(fromUserId, toUserId, message, timeSent);
    UpdateUI();
});

connection.on("MessageSent", (message, toUserId, timeSent) => {
    SendMessageConstruct(toUserId, message, timeSent);
    UpdateUI();
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