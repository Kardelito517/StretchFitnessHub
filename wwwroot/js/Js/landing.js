// ==================== MODAL OPEN/CLOSE ==================== //
function openLogin() {
    const loginModal = document.getElementById("loginModal");
    if (loginModal) loginModal.style.display = "flex";
}

function closeLogin() {
    const loginModal = document.getElementById("loginModal");
    const loginLoadingScreen = document.getElementById('loginLoadingScreen');
    if (loginModal) loginModal.style.display = "none";
    if (loginLoadingScreen) loginLoadingScreen.style.display = "none";

    // Ibalik sa Login form lahat
    switchForm('loginForm');
}

function openRegister() {
    const registerModal = document.getElementById("registerModal");
    if (registerModal) registerModal.style.display = "flex";
}

function closeRegister() {
    const registerModal = document.getElementById("registerModal");
    if (registerModal) registerModal.style.display = "none";
}

// ==================== CLICK OUTSIDE MODAL ==================== //
window.onclick = function (e) {
    const loginModal = document.getElementById("loginModal");
    const registerModal = document.getElementById("registerModal");
    if (e.target === loginModal) closeLogin();
    if (e.target === registerModal) closeRegister();
};

// ==================== FORM SWITCH HANDLER ==================== //
function switchForm(formId) {
    const forms = ['loginForm', 'forgotForm', 'codeForm', 'resetForm'];
    forms.forEach(id => {
        const form = document.getElementById(id);
        if (form) form.style.display = (id === formId) ? 'block' : 'none';
    });
    console.log("Switched to:", formId);
}

// ==================== DOM READY ==================== //
document.addEventListener('DOMContentLoaded', function () {
    const loginForm = document.getElementById('loginForm');
    const forgotForm = document.getElementById('forgotForm');
    const codeForm = document.getElementById('codeForm');
    const resetForm = document.getElementById('resetForm');
    const forgotLink = document.getElementById('forgotPassLink');
    const backToLoginLinks = document.querySelectorAll('.backToLogin');
    const loginLoadingScreen = document.getElementById('loginLoadingScreen');

    // Initial hide ng spinner
    if (loginLoadingScreen) loginLoadingScreen.style.display = "none";

    // ---------- LOGIN ----------
    if (loginForm) {
        loginForm.addEventListener('submit', function () {
            // Ipakita agad ang spinner bago mag-submit
            if (loginLoadingScreen) loginLoadingScreen.style.display = "flex";
        });
    }

    // ---------- FORGOT PASSWORD LINK ----------
    if (forgotLink) {
        forgotLink.addEventListener('click', function (e) {
            e.preventDefault();
            switchForm('forgotForm');
        });
    }

    // ---------- BACK TO LOGIN ----------
    backToLoginLinks.forEach(link => {
        link.addEventListener('click', function (e) {
            e.preventDefault();
            switchForm('loginForm');
        });
    });
});

// ==================== ERROR MODAL ==================== //
function openLoginError(message) {
    const errorModal = document.getElementById("loginErrorModal");
    const errorMessage = document.getElementById("loginErrorMessage");
    if (errorMessage) errorMessage.textContent = message;
    if (errorModal) errorModal.style.display = "flex";
}

function closeLoginError() {
    const errorModal = document.getElementById("loginErrorModal");
    if (errorModal) errorModal.style.display = "none";
}

// ==================== MOBILE MENU TOGGLE ==================== //
document.addEventListener("DOMContentLoaded", function () {
    var menuBtn = document.getElementById("mobileMenuBtn");
    var mobileMenu = document.getElementById("mobileMenu");
    var logo = document.querySelector(".logo");
    var becomeMemberBtn = document.querySelector(".header-actions .butun");

    if (menuBtn && mobileMenu) {
        menuBtn.addEventListener("click", function () {
            mobileMenu.classList.toggle("active");
            // Hide logo and Become a Member button when menu is active
            if (mobileMenu.classList.contains("active")) {
                if (logo) logo.style.display = "none";
                if (becomeMemberBtn) becomeMemberBtn.style.display = "none";
                // Prevent background scroll
                document.body.style.overflow = "hidden";
            } else {
                if (logo) logo.style.display = "";
                if (becomeMemberBtn) becomeMemberBtn.style.display = "";
                document.body.style.overflow = "";
            }
        });

        // Close menu when a link is clicked and restore logo/button
        mobileMenu.querySelectorAll("a").forEach(function (link) {
            link.addEventListener("click", function () {
                mobileMenu.classList.remove("active");
                if (logo) logo.style.display = "";
                if (becomeMemberBtn) becomeMemberBtn.style.display = "";
                document.body.style.overflow = "";
            });
        });
    }
});

const membershipBtn = document.getElementById('membershipQRBtn');
const walkinBtn = document.getElementById('walkinQRBtn');
const qrImage = document.getElementById('qrImage');
const qrLabel = document.getElementById('qrLabel');
const downloadBtn = document.getElementById('downloadQRBtn');

// Default: Membership QR active
membershipBtn.style.color = '#fff';
walkinBtn.style.color = '#111827';
qrLabel.textContent = 'Membership QR';
qrImage.src = '/Admin/GenerateRegistrationQRCode';
downloadBtn.href = '/Admin/GenerateRegistrationQRCode';
downloadBtn.setAttribute('download', 'RegistrationQR.png');

// Membership QR button
membershipBtn.addEventListener('click', () => {
    qrImage.src = '/Admin/GenerateRegistrationQRCode';
    qrLabel.textContent = 'Membership QR';
    downloadBtn.href = '/Admin/GenerateRegistrationQRCode';
    downloadBtn.setAttribute('download', 'RegistrationQR.png');

    membershipBtn.style.color = '#fff';
    walkinBtn.style.color = '#111827';
});

// Walk-in QR button
walkinBtn.addEventListener('click', () => {
    qrImage.src = '/Admin/GenerateWalkinQRCode';
    qrLabel.textContent = 'Walk-ins QR';
    downloadBtn.href = '/Admin/GenerateWalkinQRCode';
    downloadBtn.setAttribute('download', 'WalkinQR.png');

    membershipBtn.style.color = '#111827';
    walkinBtn.style.color = '#fff';
});
