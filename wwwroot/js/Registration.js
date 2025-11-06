
document.addEventListener("keydown", function (event) {
    const isEnter = event.key === "Enter";

    if (isEnter && event.target.tagName === "INPUT") {
        event.preventDefault();
    }
});




document.addEventListener('DOMContentLoaded', function () {

    // ===========================
    // ✅ MODAL LOGIC
    // ===========================
    const errorModal = document.getElementById('errorModal');
    const errorMessage = document.getElementById('errorMessage');
    const closeModal = document.getElementById('closeModal');

    function showModal(message) {
        errorMessage.textContent = message;
        errorModal.classList.remove('hidden');
    }

    closeModal.addEventListener('click', () => {
        errorModal.classList.add('hidden');
    });


    // ===========================
    // ✅ PASSWORD TOGGLE
    // ===========================
    document.querySelectorAll('.password-toggle').forEach(toggle => {
        toggle.addEventListener('click', function () {
            const targetId = this.dataset.target;
            const passwordInput = document.getElementById(targetId);
            const icon = this.querySelector('i');

            if (passwordInput.type === 'password') {
                passwordInput.type = 'text';
                icon.classList.remove('fa-eye');
                icon.classList.add('fa-eye-slash');
            } else {
                passwordInput.type = 'password';
                icon.classList.remove('fa-eye-slash');
                icon.classList.add('fa-eye');
            }
        });
    });


    // ===========================
    // ✅ MULTI-STEP LOGIC
    // ===========================
    const registrationForm = document.getElementById('registrationForm');
    const steps = document.querySelectorAll('.form-step');
    const nextBtn = document.querySelector('.next-btn');
    const prevBtn = document.querySelector('.prev-btn');
    let currentStep = 0;

    function validateStep1() {
        const fullName = document.getElementById('fullName').value.trim();
        const username = document.getElementById('username').value.trim();
        const email = document.getElementById('email').value.trim();
        const phoneNumber = document.getElementById('phoneNumber').value.trim();
        const password = document.getElementById('Password').value;
        const confirmPassword = document.getElementById('confirmPassword').value;


        if (/ {2,}/.test(fullName) || / {2,}/.test(username) || / {2,}/.test(email)) {
            showModal('Double spacing is not allowed. Please correct your input.');
            return false;
        }

        const nameRegex = /^[A-Za-z\s]+$/;
        if (!nameRegex.test(fullName)) {
            showModal('Full name must contain letters and spaces only.');
            return false;
        }

        // ✅ USERNAME — must start with letter, no spaces at all
        const usernameRegex = /^[A-Za-z][A-Za-z0-9_]{2,19}$/;
        if (!usernameRegex.test(username)) {
            showModal('Username must start with a letter and contain no spaces. (3–20 characters)');
            return false;
        }

        // ✅ EMAIL — must be valid and LOCAL PART cannot end with number or underscore
        const emailRegex = /^[A-Za-z0-9](?:[A-Za-z0-9._%+-]*[A-Za-z0-9])?@[A-Za-z0-9.-]+\.[A-Za-z]{2,}$/;
        if (!emailRegex.test(email)) {
            showModal('Invalid email address. Example: example@gmail.com');
            return false;
        }

        // ✅ PHONE NUMBER — must be 11 digits, start with 09, NO SPACES at all
        const phoneRegex = /^09\d{9}$/;
        if (!phoneRegex.test(phoneNumber)) {
            showModal('Phone number must start with 09 and contain exactly 11 digits with no spaces.');
            return false;
        }

        if (password.length < 6) {
            showModal('Password must be at least 6 characters long.');
            return false;
        }

        // ✅ PASSWORD — strong rules
        const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;
        if (!passwordRegex.test(password)) {
            showModal('Password must be at least 8 characters with uppercase, lowercase, and numbers.');
            return false;
        }

        if (password !== confirmPassword) {
            showModal('Passwords do not match.');
            return false;
        }

        return true;
    }

    if (nextBtn) {
        nextBtn.addEventListener('click', () => {
            if (validateStep1()) {
                steps[currentStep].classList.remove('active');
                currentStep++;
                steps[currentStep].classList.add('active');
            }
        });
    }

    if (prevBtn) {
        prevBtn.addEventListener('click', () => {
            steps[currentStep].classList.remove('active');
            currentStep--;
            steps[currentStep].classList.add('active');
        });
    }


    // ===========================
    // ✅ PREFERRED CLASS SHOW/HIDE (UPDATED VALUES)
    // ===========================
    const membershipPlan = document.getElementById('membershipPlan');
    const preferredClassContainer = document.getElementById('preferredClassContainer');

    function updatePreferredClassVisibility() {
        const plan = membershipPlan.value;

        if (plan === "GymWithClass" || plan === "ClassOnly") {
            preferredClassContainer.style.display = "block";
        } else {
            preferredClassContainer.style.display = "none";
            // Auto-select "None"
            const noneOption = document.querySelector('input[name="Request.PreferredClasses"][value="None"]');
            if (noneOption) noneOption.checked = true;
        }
    }


    updatePreferredClassVisibility();
    membershipPlan.addEventListener("change", updatePreferredClassVisibility);


    // ===========================
    // ✅ FINAL FORM VALIDATION (STEP 2)
    // ===========================
    if (registrationForm) {
        registrationForm.addEventListener('submit', function (event) {

            const plan = membershipPlan.value;

            if (!plan) {
                event.preventDefault();
                showModal('Please select a membership plan.');
                return;
            }

            // ✅ Check Preferred Class if required
            if (plan === "GymWithClass" || plan === "ClassOnly") {
                const selectedClass = document.querySelector('input[name="Request.PreferredClasses"]:checked');
                if (!selectedClass || selectedClass.value === "None") {
                    event.preventDefault();
                    showModal('Please select your preferred class for this membership plan.');
                    return;
                }
            }
        });
    } else {
        console.error("Registration form not found!");
    }

});
