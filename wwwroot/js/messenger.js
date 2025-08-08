const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';
const contactsList = document.getElementById('contacts-list');
const contactButton = document.getElementsByClassName('contact')[0];

let messages = [];
let messagingPartners = new Set();
let connectionId = -1;

let conversations = [];

class Message {
    constructor(fromUserId, toUserId, message, timeSent) {
        this.fromUserId = fromUserId;
        this.toUserId = toUserId;
        this.message = message;
        this.timeSent = timeSent;

        messages.push(this);

        let userIdToAdd = this.toUserId;
        if (this.fromUserId == currentUserId) // We sent the message, add the receiver to the set of partners
        {
            messagingPartners.add(toUserId);
        }
        else // Otherwise it is us who received the message, so add the sender
        {
            messagingPartners.add(fromUserId);
            userIdToAdd = this.fromUserId;
        }

        updateConversation(userIdToAdd);
    }
};

function updateConversation(userId) {
    if (conversations.includes(userId)) {
        conversations = conversations.filter(x => x !== userId);
    }

    conversations.splice(0, 0, userId);

    UpdateUI();
}

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
    if (fromUserId == currentUserId)
        updateConversation(toUserId);
    else
        updateConversation(fromUserId);
    UpdateUI();
});

connection.on("MessageSent", (message, toUserId, timeSent) => {
    SendMessageConstruct(toUserId, message, timeSent);
    updateConversation(toUserId);
    UpdateUI();
});

connection.start().then(() => {
    connection.invoke("RegisterUser", currentUserId).then(() => {
        console.log('Registered successfuly');
    });
});

function ReceiveMessage(fromUserId, message, timeSent) {
    ReceiveMessageConstruct(fromUserId, message, timeSent);
    updateConversation(fromUserId);
    UpdateUI();
}

function UpdateUI() {
    console.log("UpdateUI called");
    while (contactsList.firstChild) {
        contactsList.removeChild(contactsList.firstChild);
    }

    conversations.forEach(userId => {
        UpdateConvoUI(userId);
    });
}

function UpdateConvoUI(userId) {
    console.log(`UpdateConvoUI: ${userId}`);
    let contactCopy = contactButton.cloneNode(true);

    let usernameToDisplay;
    connection.invoke("GetUsernameFromId", userId).then(usernameReceived => {
        usernameToDisplay = usernameReceived;

        // Updating the UI of the new contact button
        // For now only the username will be displayed
        // TODO update in the future 
        contactCopy.innerHTML = usernameToDisplay;
        contactCopy.style.display = 'block';

        contactsList.appendChild(contactCopy);
        console.log('added contact!');
    })
    .catch(err => {
        console.error("Error fetching username:", err);
    });
}

function SendMessage(toUserId, message) {
    connection.invoke("SendPrivateMessage", currentUserId, toUserId, message);
}