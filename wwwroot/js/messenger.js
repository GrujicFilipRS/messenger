const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';
const contactsList = document.getElementById('contacts-list');
const contactTemplate = document.getElementById('contact-template');

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

        // Determine which user to add to conversation list
        const userIdToAdd = (this.fromUserId === currentUserId) ? this.toUserId : this.fromUserId;
        messagingPartners.add(userIdToAdd);

        updateConversation(userIdToAdd);
    }
};

let updateUITimer;
function updateConversation(userId) {
    conversations = conversations.filter(x => x !== userId);
    conversations.unshift(userId);

    if (updateUITimer) clearTimeout(updateUITimer);
    updateUITimer = setTimeout(() => {
        UpdateUI();
        updateUITimer = null;
    }, 20);
}

function SendMessageConstruct(toUserId, message, timeSent) {
    new Message(currentUserId, toUserId, message, timeSent);
}

function ReceiveMessageConstruct(fromUserId, message, timeSent) {
    new Message(fromUserId, currentUserId, message, timeSent);
}

// SignalR connection
const connection = new signalR.HubConnectionBuilder()
    .withUrl("/chathub")
    .build();

connection.on("ReceiveConnectionId", id => {
    connectionId = id;
});

connection.on("ReceiveMessage", (message, fromUserId, timeSent) => {
    ReceiveMessageConstruct(fromUserId, message, timeSent);
});

connection.on("ReceivePreviousMessage", (message, fromUserId, toUserId, timeSent) => {
    new Message(fromUserId, toUserId, message, timeSent);
});

connection.on("MessageSent", (message, toUserId, timeSent) => {
    SendMessageConstruct(toUserId, message, timeSent);
});

connection.start().then(() => {
    connection.invoke("RegisterUser", currentUserId)
        .then(() => {
            console.log('Registered successfully');
            UpdateUI();
        });
});

function ReceiveMessage(fromUserId, message, timeSent) {
    ReceiveMessageConstruct(fromUserId, message, timeSent);
}

function SendMessage(toUserId, message) {
    connection.invoke("SendPrivateMessage", currentUserId, toUserId, message);
}

async function UpdateUI() {
    contactsList.innerHTML = '';

    const promises = conversations.map(userId => createContactElement(userId));
    const elements = await Promise.all(promises);

    elements.forEach(el => contactsList.appendChild(el));
}

async function createContactElement(userId) {
    const contactCopy = contactTemplate.content.firstElementChild.cloneNode(true);

    try {
        const username = await connection.invoke("GetUsernameFromId", userId);
        contactCopy.textContent = username;
    } catch (err) {
        console.error("Error fetching username:", err);
        contactCopy.textContent = `User ${userId}`;
    }

    contactCopy.style.display = 'block';
    return contactCopy;
}