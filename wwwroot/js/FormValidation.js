document.addEventListener('DOMContentLoaded', function () {
    const form = document.getElementById('signupForm');

    if (!form) return;

    form.addEventListener('submit', function (event) {
        if (!validateForm()) {
            event.preventDefault();
        }
    });

    const inputs = document.querySelectorAll('#Username, #FirstName, #LastName, #Email, #Password, #ConfirmPassword');
    inputs.forEach(input => {
        input.addEventListener('blur', function () {
            validateField(this.id);
        });
    });

    const confirmPasswordInput = document.getElementById('ConfirmPassword');
    if (confirmPasswordInput) {
        confirmPasswordInput.addEventListener('blur', validatePasswordConfirmation);
    }
});

function validateForm() {
    clearErrors();

    const firstName = (document.getElementById('FirstName') || {}).value || '';
    const lastName = (document.getElementById('LastName') || {}).value || '';
    const username = (document.getElementById('Username') || {}).value || '';
    const email = (document.getElementById('Email') || {}).value || '';
    const password = (document.getElementById('Password') || {}).value || '';

    const okFirst = validateName(firstName, 'FirstName');
    const okLast = validateName(lastName, 'LastName');
    const okUser = validateUsername(username);
    const okEmail = validateEmail(email);
    const okPass = validatePassword(password);
    const okConfirm = validatePasswordConfirmation();

    return okFirst && okLast && okUser && okEmail && okPass && okConfirm;
}

function validateUsername(username) {
    if (!username) { showError('Username-error-form', 'Username is required'); return false; }
    if (!/^[a-zA-Z0-9_\-]+$/.test(username)) { showError('Username-error-form', 'Allowed: letters, numbers, underscore, hyphen'); return false; }
    if (username.length < 3) { showError('Username-error-form', 'At least 3 characters'); return false; }
    return true;
}

function validateName(name, id) {
    if (!name) { showError(id + '-error-form', 'This field is required'); return false; }
    if (/\d/.test(name)) { showError(id + '-error-form', 'Name should not contain numbers'); return false; }
    return true;
}

function validateEmail(email) {
    if (!email) { showError('Email-error-form', 'Email is required'); return false; }
    if (!email.includes('@')) { showError('Email-error-form', 'Enter a valid email'); return false; }
    const local = email.split('@')[0] || '';
    if (!/^[a-zA-Z0-9]+$/.test(local)) { showError('Email-error-form', 'No symbols before @'); return false; }
    const domain = email.split('@')[1] || '';
    const allowed = ['gmail.com', 'outlook.com', 'yahoo.com', 'hotmail.com'];
    if (!allowed.includes(domain)) { showError('Email-error-form', 'Allowed: Gmail, Outlook, Yahoo, Hotmail'); return false; }
    return true;
}

function validatePassword(pwd) {
    if (!pwd) { showError('Password-error-form', 'Password is required'); return false; }
    if (pwd.length < 8) { showError('Password-error-form', 'At least 8 characters'); return false; }
    if (!/[A-Z]/.test(pwd)) { showError('Password-error-form', 'One capital letter required'); return false; }
    if (!/\d/.test(pwd)) { showError('Password-error-form', 'One number required'); return false; }
    if (!/[@$!%*?&]/.test(pwd)) { showError('Password-error-form', 'One symbol required'); return false; }
    return true;
}

function validatePasswordConfirmation() {
    const pwd = (document.getElementById('Password') || {}).value || '';
    const conf = (document.getElementById('ConfirmPassword') || {}).value || '';
    if (!conf) { showError('ConfirmPassword-error-form', 'Please confirm your password'); return false; }
    if (pwd !== conf) { showError('ConfirmPassword-error-form', 'Passwords do not match'); return false; }
    return true;
}

function validateField(id) {
    const val = (document.getElementById(id) || {}).value || '';
    if (id === 'Username') return validateUsername(val);
    if (id === 'FirstName' || id === 'LastName') return validateName(val, id);
    if (id === 'Email') return validateEmail(val);
    if (id === 'Password') return validatePassword(val);
    if (id === 'ConfirmPassword') return validatePasswordConfirmation();
}

function showError(elId, msg) {
    const el = document.getElementById(elId);
    if (el) {
        el.textContent = msg;
        el.style.display = 'block';
    }
}

function clearErrors() {
    document.querySelectorAll('[id$="-error-form"]').forEach(e => {
        e.textContent = '';
        e.style.display = 'none';
    });
}