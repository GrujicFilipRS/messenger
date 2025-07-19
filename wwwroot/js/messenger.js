const messageHeader = document.getElementById('message-header');
messageHeader.style.display = 'none';

// The following code will only be used to design the webpage
// After the design has been made, it will be deleted

let idToMessage = -1;

function incrementUserIdToMessage() {
    alert(idToMessage);
    if (idToMessage == -1) {
        idToMessage = 1;
    }

    idToMessage += 1;

    if (idToMessage == currentUserId) {
        idToMessage += 1;
    }
}