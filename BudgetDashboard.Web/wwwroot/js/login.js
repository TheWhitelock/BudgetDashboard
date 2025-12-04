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
