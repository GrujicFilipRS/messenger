const loginForm = document.getElementById('login-form');
const signupForm = document.getElementById('signup-form');
const errorText = document.getElementById('error-display');

const queryString = window.location.search;
const urlParams = new URLSearchParams(queryString);

let toDisplayLogin = false;
function switchLoginMode() {
    toDisplayLogin = !toDisplayLogin;
    displayCorrectForm();
}

function displayCorrectForm() {
    if (toDisplayLogin) {
        loginForm.style.display = 'flex';
        signupForm.style.display = 'none';
    } else {
        loginForm.style.display = 'none';
        signupForm.style.display = 'flex';
    }
}

document.getElementById('login-form').style.display = 'none';
const errorAtForm = urlParams.get('p');

function displayLoginError() {
    if (toDisplayLogin == false) toDisplayLogin = true;
    displayCorrectForm();

    let error = '';

    const errorId = urlParams.get('error');
    switch(errorId) {
        case '1':
            error = 'Please fill in all of the fields';
            break;
        case '2':
            error = 'There exists no user with that username and password';
            break;
        default:
            error = `Error undetected. Error code: '${errorId}'`;
            break;
    }

    errorText.innerText = error;
}

function displaySignupError() {
    if (toDisplayLogin == true) toDisplayLogin = false;
    displayCorrectForm();

    let error = '';

    const errorId = urlParams.get('error');
    switch(errorId) {
        case '1':
            error = 'Please fill in all of the fields';
            break;
        case '2':
            error = 'The 2 passwords aren\'t the same';
            break;
        case '3':
            error = 'Invalid password. A password must contain at least 8 characters, and at least one uppercase and lowercase letter';
            break;
        case '4':
            error = 'Invalid username. A username must contain at least 6 letters, and it can consist of letters, numbers, and characters \'_\' and \'-\'';
            break;
        case '5':
            error = 'There already exists a user with that username';
        default:
            error = `Error undetected. Error code: '${errorId}'`;
            break;
    }

    errorText.innerText = error;
}

switch (errorAtForm) {
    case 'login':
        displayLoginError();
        break;
    case 'signup':
        displaySignupError();
        break;
    default:
        break;
}