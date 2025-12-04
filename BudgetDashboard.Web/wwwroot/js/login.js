// Simple login/logout handlers for auth forms

let navigationManager = null;

window.setNavigationManager = (navMgr) => {
    navigationManager = navMgr;
};

window.performLogin = async (email, password) => {
    if (!email || !password) {
        alert('Please enter both email and password');
        return;
    }

    try {
        const fd = new FormData();
        fd.append('email', email);
        fd.append('password', password);

        const resp = await fetch('/api/auth/login', { method: 'POST', body: fd });
        const data = await resp.json();

        if (data.success) {
            window.location.href = data.redirect || '/';
        } else {
            alert('Login failed: ' + (data.error || 'Unknown error'));
        }
    } catch (err) {
        console.error('Login error:', err);
        alert('An error occurred during login');
    }
};

window.performRegister = async (email, password, confirmPassword) => {
    if (!email || !password || !confirmPassword) {
        alert('Please fill in all fields');
        return;
    }

    if (password !== confirmPassword) {
        alert('Passwords do not match');
        return;
    }

    if (password.length < 6) {
        alert('Password must be at least 6 characters');
        return;
    }

    try {
        const fd = new FormData();
        fd.append('email', email);
        fd.append('password', password);
        fd.append('confirmPassword', confirmPassword);

        const resp = await fetch('/api/auth/register', { method: 'POST', body: fd });
        const data = await resp.json();

        if (data.success) {
            window.location.href = data.redirect || '/';
        } else {
            const errorMessages = {
                'EmailAndPasswordRequired': 'Email and password are required',
                'PasswordsDoNotMatch': 'Passwords do not match',
                'PasswordTooShort': 'Password must be at least 6 characters',
                'EmailAlreadyExists': 'Email already registered',
                'InvalidEmail': 'Invalid email format'
            };
            const errorMsg = errorMessages[data.error] || data.error || 'Registration failed';
            alert('Registration failed: ' + errorMsg);
        }
    } catch (err) {
        console.error('Registration error:', err);
        alert('An error occurred during registration');
    }
};

window.performLogout = async () => {
    try {
        const resp = await fetch('/api/auth/logout', { method: 'POST' });
        const data = await resp.json();

        if (data.success) {
            window.location.href = data.redirect || '/Account/Login';
        }
    } catch (err) {
        console.error('Logout error:', err);
        alert('An error occurred during logout');
    }
};
