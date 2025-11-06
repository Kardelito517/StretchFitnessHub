document.addEventListener("DOMContentLoaded", () => {
    const logoutButton = document.getElementById("logout-button");
    const logoutModal = document.getElementById("logout-modal");
    const cancelLogoutButton = document.getElementById("cancel-logout");
    const confirmLogoutButton = document.getElementById("confirm-logout");
    const logoutForm = document.getElementById("logout-form");

    const confirmationContent = document.getElementById("logout-confirmation-content");
    const loadingContent = document.getElementById("logout-loading-content");

    function openModal(modal) {
        if (modal) {
            if (confirmationContent) confirmationContent.classList.remove("hidden");
            if (loadingContent) loadingContent.classList.add("hidden");
            modal.classList.remove("hidden");
        }
    }

    function closeModal(modal) {
        if (modal) {
            modal.classList.add("hidden");
        }
    }

    if (logoutButton) {
        logoutButton.addEventListener("click", () => {
            openModal(logoutModal);
        });
    }

    if (cancelLogoutButton) {
        cancelLogoutButton.addEventListener("click", () => closeModal(logoutModal));
    }

    if (confirmLogoutButton) {
        confirmLogoutButton.addEventListener("click", () => {

            if (confirmationContent) confirmationContent.classList.add("hidden");

            if (loadingContent) loadingContent.classList.remove("hidden");

            setTimeout(() => {
                if (logoutForm) {
                    logoutForm.submit();
                }
            }, 1000);
        });
    }

    if (logoutModal) {
        logoutModal.addEventListener("click", (e) => {
            if (e.target.id === "logout-modal") closeModal(logoutModal);
        });
    }
});



document.addEventListener("DOMContentLoaded", function () {
    const profilePictureContainer = document.getElementById("profile-picture-container");
    const profileImageInput = document.getElementById("profile-image-input");
    const uploadForm = document.getElementById("upload-form");

    profilePictureContainer.addEventListener("click", () => {
        profileImageInput.click();
    });

    profileImageInput.addEventListener("change", () => {
        if (profileImageInput.files.length > 0) {
            uploadForm.submit();
        }
    });
});


function scanQR(qrValue) {
    fetch(`/Member/GetMemberByQRCode?qr=${qrValue}`)
        .then(response => response.json())
        .then(data => displayMemberInfo(data))
        .catch(err => console.error('Error fetching member:', err));
}

function displayMemberInfo(data) {
    if (data.error) {
        alert(data.error);
        return;
    }

    document.getElementById("memberName").textContent = data.name;
    document.getElementById("memberStart").textContent = data.startDate;
    document.getElementById("memberEnd").textContent = data.endDate;

    document.getElementById("memberInfo").style.display = "block";
}


document.addEventListener("DOMContentLoaded", function () {
    const editBtn = document.getElementById("edit-profile-button");
    const modal = document.getElementById("edit-profile-modal");
    const closeBtn = document.getElementById("close-edit-profile");
    const cancelBtn = document.getElementById("cancel-edit-profile");
    const form = document.getElementById("edit-profile-form");

    editBtn.addEventListener("click", function () {
        document.getElementById("editFullName").value = document.getElementById("profile-name").textContent.trim();
        document.getElementById("editEmail").value = document.getElementById("profile-email").textContent.trim();
        document.getElementById("editPhone").value = document.getElementById("profile-contact").textContent.trim();
        document.getElementById("editPassword").value = ""; 
        modal.classList.remove("hidden");
    });

    function closeEditModal() {
        modal.classList.add("hidden");
        form.reset();
    }

    closeBtn.addEventListener("click", closeEditModal);
    cancelBtn.addEventListener("click", closeEditModal);

    modal.addEventListener("click", function (e) {
        if (e.target === modal) closeEditModal();
    });

    form.addEventListener("submit", function (e) {
        e.preventDefault();
        const data = new URLSearchParams(new FormData(form));
        fetch("/Member/EditProfile", {
            method: "POST",
            headers: { "Content-Type": "application/x-www-form-urlencoded" },
            body: data
        })
            .then(res => {
                if (res.ok) {
                    location.reload();
                } else {
                    alert("Failed to update profile.");
                }
            });
    });
});

