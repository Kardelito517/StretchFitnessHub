
document.addEventListener('DOMContentLoaded', function () {
    const settingsLink = document.getElementById('settingsLink'); 
    const settingsModal = document.getElementById('settingsModal');
    const closeSettings = document.getElementById('closeSettings');

    if (settingsLink) {
        settingsLink.addEventListener('click', function (e) {
            e.preventDefault();
            settingsModal.classList.remove('hidden');
        });
    }

    if (closeSettings) {
        closeSettings.addEventListener('click', function () {
            settingsModal.classList.add('hidden');
        });
    }


    window.addEventListener('click', function (e) {
        if (e.target === settingsModal) {
            settingsModal.classList.add('hidden');
        }
    });
});


document.addEventListener('DOMContentLoaded', function () {
    const sidebarLinks = document.querySelectorAll('.sidebar-item > a');

    sidebarLinks.forEach(link => {
        link.addEventListener('click', function () {

            sidebarLinks.forEach(l => l.parentElement.classList.remove('active'));

            this.parentElement.classList.add('active');
        });
    });
});




document.addEventListener('DOMContentLoaded', function () {
    const logoutButton = document.getElementById('LogoutButton');
    const logoutLoadingScreen = document.getElementById('logoutLoadingScreen');
    const logoutModal = document.getElementById('logoutModal');
    const cancelLogoutBtn = document.getElementById('cancelLogoutBtn');
    const confirmLogoutBtn = document.getElementById('confirmLogoutBtn');

    if (logoutButton) {
        logoutButton.addEventListener('click', function (event) {
            event.preventDefault(); 
            logoutModal.style.display = 'block'; 
        });
    }

    if (cancelLogoutBtn) {
        cancelLogoutBtn.addEventListener('click', function () {
            logoutModal.style.display = 'none';
        });
    }

    if (confirmLogoutBtn) {
        confirmLogoutBtn.addEventListener('click', function () {
            logoutModal.style.display = 'none'; 

            if (logoutLoadingScreen) {
                logoutLoadingScreen.classList.remove('hidden'); 
            }

            setTimeout(() => {
                window.location.href = '/Account/LandingPage';
            }, 2000); 
        });
    }

    window.addEventListener('click', function (event) {
        if (event.target === logoutModal) {
            logoutModal.style.display = 'none';
        }
    });
});




    const currentDate = new Date();
    const dateDisplayElement = document.getElementById("date");
    if (dateDisplayElement) {
        const days = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];
        const options = { year: 'numeric', month: 'long', day: 'numeric' };
        dateDisplayElement.textContent = `Today is ${currentDate.toLocaleDateString('en-US', options)} | ${days[currentDate.getDay()]}`;
    }

    document.getElementById("searchInput").addEventListener("input", function () {
        let filter = this.value.toLowerCase();
        let members = document.querySelectorAll(".member-item");
        let noResults = document.getElementById("noResults");
        let found = false;

        members.forEach(member => {
            let name = member.querySelector(".member-name").textContent.toLowerCase();
            let email = member.querySelector(".member-email").textContent.toLowerCase();
            let contact = member.querySelector(".member-contact").textContent.toLowerCase();

            if (name.includes(filter) || email.includes(filter) || contact.includes(filter)) {
                member.style.display = "";
                found = true;
            } else {
                member.style.display = "none";
            }
        });

        noResults.style.display = found ? "none" : "block";
    });



document.addEventListener('DOMContentLoaded', () => {
    const notificationBell = document.getElementById('notification-bell');
    const notificationDropdown = document.getElementById('notification-dropdown');

    if (notificationBell && notificationDropdown) {
        notificationDropdown.classList.remove('show');

        notificationBell.addEventListener('click', (e) => {
            e.stopPropagation();
            notificationDropdown.classList.toggle('show');
        });

        document.addEventListener('click', () => {
            notificationDropdown.classList.remove('show');
        });

        notificationDropdown.addEventListener('click', (e) => {
            e.stopPropagation();
        });
    }
});


document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.three-dots').forEach(dot => {
        dot.addEventListener('click', function (e) {
            e.stopPropagation(); 
            const menu = this.nextElementSibling;

            document.querySelectorAll('.options-menu').forEach(m => {
                if (m !== menu) m.classList.remove('active');
            });

            menu.classList.toggle('active');
        });
    });

    document.addEventListener('click', function () {
        document.querySelectorAll('.options-menu').forEach(m => m.classList.remove('active'));
    });
});

let currentAction = null;
let currentId = null;

function showConfirmModal(action, id) {
    currentAction = action;
    currentId = id;

    let title = "";
    let message = "";

    switch (action) {
        case "archive":
            title = "Archive Member";
            message = "Are you sure you want to archive this member?";
            break;
        case "delete":
            title = "Delete Member";
            message = "Are you sure you want to delete this member?";
            break;
        case "renew":
            title = "Renew Membership";
            message = "Are you sure you want to renew this member’s membership?";
            break;
        case "unarchive":
            title = "Unarchive Member";
            message = "Are you sure you want to unarchive this member?";
            break;
    }

    document.getElementById("confirmTitle").innerText = title;
    document.getElementById("confirmMessage").innerText = message;

    document.getElementById("confirmModal").classList.remove("hidden");
}

function closeModal() {
    document.getElementById("confirmModal").classList.add("hidden");
    currentAction = null;
    currentId = null;
}

document.getElementById("confirmBtn").addEventListener("click", function () {
    if (!currentAction || !currentId) return;

    let url = "";
    switch (currentAction) {
        case "archive":
            url = "/Admin/ArchiveMember";
            break;
        case "delete":
            url = "/Admin/DeleteMember";
            break;
        case "renew":
            url = "/Admin/RenewMember";
            break;
        case "unarchive":
            url = "/Admin/UnarchiveMember";
            break;
    }

    fetch(url, {
        method: "POST",
        headers: { "Content-Type": "application/x-www-form-urlencoded" },
        body: `id=${currentId}`
    })
        .then(response => {
            if (response.ok) {
                location.reload(); 
            } else {
                alert("Something went wrong!");
            }
        })
        .catch(err => console.error(err));

    closeModal();
});

document.getElementById("cancelBtn").addEventListener("click", closeModal);

function archiveMember(id) {
    showConfirmModal("archive", id);
}

function deleteMember(id) {
    showConfirmModal("delete", id);
}

function renewMember(id) {
    showConfirmModal("renew", id);
}

function unarchiveMember(id) {
    showConfirmModal("unarchive", id);
}

document.addEventListener("click", (e) => {
    if (e.target.classList.contains("approve-btn")) {
        const id = e.target.dataset.id;

        fetch(`/Admin/ApproveRegistration/${id}`, { method: "POST" })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    const requestItem = e.target.closest(".request-item");
                    if (requestItem) requestItem.remove();

                    const membersList = document.querySelector(".member-list");
                    if (membersList) {
                        const newMember = document.createElement("div");
                        newMember.classList.add("member-item");
                        newMember.innerHTML = `
                                <div class="member-info">
                                    <p class="member-name">${data.member.fullName}</p>
                                    <p class="member-date">Ends: ${data.member.membershipEnd}</p>
                                </div>
                            `;
                        membersList.appendChild(newMember);
                    }

                    const notifList = document.querySelector(".notification-list");
                    if (notifList) {
                        const newNotif = document.createElement("div");
                        newNotif.classList.add("notification-item");
                        newNotif.textContent = `${data.member.fullName} has been approved and added as a member.`;
                        notifList.prepend(newNotif);
                    }
                }
            });
    }

    if (e.target.classList.contains("reject-btn")) {
        const id = e.target.dataset.id;

        fetch(`/Admin/RejectRegistration/${id}`, { method: "POST" })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    const requestItem = e.target.closest(".request-item");
                    if (requestItem) requestItem.remove();

                    const notifList = document.querySelector(".notification-list");
                    if (notifList) {
                        const newNotif = document.createElement("div");
                        newNotif.classList.add("notification-item");
                        newNotif.textContent = `A registration request was rejected.`;
                        notifList.prepend(newNotif);
                    }
                }
            });
    }
});


document.addEventListener("DOMContentLoaded", () => {
    const notificationsList = document.getElementById("notifications-list");
    const membersList = document.getElementById("members-list");
    const totalMembersElement = document.querySelector(".stat-value"); 

    async function fetchRegistrationRequests() {
        try {
            const response = await fetch("/Admin/GetRegistrationRequests");
            if (!response.ok) throw new Error("Failed to fetch registration requests");

            const data = await response.json();

            notificationsList.innerHTML = "";

            data.requests.forEach(req => {
                const li = document.createElement("li");
                li.innerHTML = `
                    <button onclick="approveRequest(${req.id})">Approve</button>
                    <button onclick="rejectRequest(${req.id})">Reject</button>
                `;
                notificationsList.appendChild(li);
            });
        } catch (err) {
            console.error("Error fetching registration requests:", err);
        }
    }

    document.addEventListener("DOMContentLoaded", function () {
        document.addEventListener("click", function (e) {
            if (e.target.classList.contains("approve-btn")) {
                let id = e.target.dataset.id;

                fetch(`/Admin/ApproveRegistration/${id}`, {
                    method: "POST"
                })
                    .then(res => res.json())
                    .then(data => {
                        if (data.success) {
                            document.querySelector(`#notif-${id}`)?.remove();

                            let membersContainer = document.querySelector(".member-list");
                            if (membersContainer) {
                                let newMember = document.createElement("div");
                                newMember.classList.add("member-item");
                                newMember.innerHTML = `
                            <div class="member-info">
                                <p>${data.memberName}</p>
                                <p>${data.startDate} - ${data.endDate}</p>
                            </div>
                        `;
                                membersContainer.appendChild(newMember);
                            }

                            let count = document.getElementById("membersCount");
                            if (count) count.innerText = parseInt(count.innerText) + 1;
                        }
                    });
            }

            if (e.target.classList.contains("reject-btn")) {
                let id = e.target.dataset.id;

                fetch(`/Admin/RejectRegistration/${id}`, {
                    method: "POST"
                })
                    .then(res => res.json())
                    .then(data => {
                        if (data.success) {
                            document.querySelector(`#notif-${id}`)?.remove();
                        }
                    });
            }
        });
    });



    async function fetchMembers() {
        try {
            const response = await fetch("/Admin/GetMembers");
            if (!response.ok) throw new Error("Failed to fetch members");

            const data = await response.json();

            membersList.innerHTML = "";
            data.members.forEach(m => {
                const li = document.createElement("li");
                li.textContent = `${m.fullName} (ends ${m.membershipEnd})`;
                membersList.appendChild(li);
            });

            totalMembersElement.textContent = data.totalMembers;
        } catch (err) {
            console.error("Error fetching members:", err);
        }
    }


    fetchRegistrationRequests();
    fetchMembers();


    setInterval(fetchRegistrationRequests, 5000);
});



function updateNotificationCount() {
    fetch('/Admin/GetNotificationCount')
        .then(response => response.json())
        .then(count => {
            document.getElementById("notification-count").textContent = count;
        })
        .catch(err => console.error(err));
}

updateNotificationCount();

setInterval(updateNotificationCount, 10000);


document.addEventListener("DOMContentLoaded", function () {
    const memberItems = document.querySelectorAll(".member-item");
    const modal = document.getElementById("memberProfileModal");

    const profilePic = document.getElementById("profilePic");
    const profileName = document.getElementById("profileName");
    const profileEmail = document.getElementById("profileEmail");
    const profilePhone = document.getElementById("profilePhone");
    const profileMembership = document.getElementById("profileMembership");
    const profileClass = document.getElementById("profileClass");
    const profileStatus = document.getElementById("profileStatus");
    const profilePassword = document.getElementById("profilePassword");
    const copyPasswordBtn = document.getElementById("copyPasswordBtn");

    const closeBtn = document.getElementById("closeMemberProfileModal");

    memberItems.forEach(item => {
        item.addEventListener("click", function (e) {
            if (e.target.tagName.toLowerCase() === "button" || e.target.closest(".options-menu")) {
                return;
            }

            profilePic.src = this.dataset.pic || "/images/default-profile.png";
            profileName.value = this.dataset.name || "N/A";
            profileEmail.value = this.dataset.email || "N/A";
            profilePhone.value = this.dataset.phone || "N/A";
            profileMembership.value = this.dataset.membership || "N/A";
            profileClass.value = this.dataset.class || "N/A";
            profileStatus.value = this.dataset.status || "N/A";
            profilePassword.value = this.dataset.password || "";

            modal.classList.remove("hidden");
        });
    });

    if (copyPasswordBtn) {
        copyPasswordBtn.addEventListener("click", () => {
            navigator.clipboard.writeText(profilePassword.value)
                .then(() => {
                    copyPasswordBtn.textContent = "✅ Copied!";
                    setTimeout(() => copyPasswordBtn.textContent = "Copy", 2000);
                })
                .catch(err => console.error("Failed to copy password:", err));
        });
    }

    if (closeBtn) {
        closeBtn.addEventListener("click", () => {
            modal.classList.add("hidden");
        });
    }

    window.addEventListener("click", (e) => {
        if (e.target === modal) {
            modal.classList.add("hidden");
        }
    });
});




function showTab(tab) {
    let activeBtn = document.querySelector(".tab-btn.active");
    if (activeBtn) activeBtn.classList.remove("active");

    document.querySelectorAll(".tab-btn").forEach(btn => {
        if (btn.textContent.toLowerCase().includes(tab)) {
            btn.classList.add("active");
        }
    });
        
    let hasVisible = false;

    document.querySelectorAll(".member-item").forEach(item => {
        if (tab === "active" && item.classList.contains("active")) {
            item.style.display = "flex";
            hasVisible = true;
        } else if (tab === "archived" && item.classList.contains("archived")) {
            item.style.display = "flex";
            hasVisible = true;
        } else {
            item.style.display = "none";
        }
    });

    document.getElementById("noResults").style.display =
        (tab === "active" && !hasVisible) ? "block" : "none";

    document.getElementById("noArchived").style.display =
        (tab === "archived" && !hasVisible) ? "block" : "none";
}


function openApproveModalReg() {
    document.getElementById("approveModalReg").classList.remove("hidden");
}
function closeApproveModalReg() {
    document.getElementById("approveModalReg").classList.add("hidden");
}

function openRejectModalReg() {
    document.getElementById("rejectModalReg").classList.remove("hidden");
}
function closeRejectModalReg() {
    document.getElementById("rejectModalReg").classList.add("hidden");
}


document.addEventListener("click", function (e) {

    if (e.target.classList.contains("approve-btn")) {
        const id = e.target.dataset.id;

        fetch(`/Admin/ApproveRegistration/${id}`, { method: "POST" })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    const requestItem = e.target.closest(".request-item");
                    if (requestItem) requestItem.remove();

                    const membersList = document.querySelector(".member-list");
                    if (membersList) {
                        const newMember = document.createElement("div");
                        newMember.classList.add("member-item");
                        newMember.innerHTML = `
                            <div class="member-info">
                                <p class="member-name">${data.member.fullName}</p>
                                <p class="member-date">Ends: ${data.member.membershipEnd}</p>
                            </div>
                        `;
                        membersList.appendChild(newMember);
                    }
                    
                    const notifList = document.querySelector(".notification-list");
                    if (notifList) {
                        const newNotif = document.createElement("div");
                        newNotif.classList.add("notification-item");
                        newNotif.textContent = `${data.member.fullName} has been approved and added as a member.`;
                        notifList.prepend(newNotif);
                    }

                    openApproveModalReg();
                }
            });
    }

    if (e.target.classList.contains("reject-btn")) {
        const id = e.target.dataset.id;

        fetch(`/Admin/RejectRegistration/${id}`, { method: "POST" })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    const requestItem = e.target.closest(".request-item");
                    if (requestItem) requestItem.remove();

                    const notifList = document.querySelector(".notification-list");
                    if (notifList) {
                        const newNotif = document.createElement("div");
                        newNotif.classList.add("notification-item");
                        newNotif.textContent = `A registration request was rejected.`;
                        notifList.prepend(newNotif);
                    }

                    openRejectModalReg();
                }
            });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const modal = document.getElementById("actionModal");
    const modalTitle = document.getElementById("actionModalTitle");
    const modalMessage = document.getElementById("actionModalMessage");

    // Read action and member name from hidden inputs
    const action = document.getElementById("actionResult")?.value;
    const memberName = document.getElementById("memberName")?.value || "this member";

    if (action) {
        modal.classList.remove("hidden");

        switch (action) {
            case "Approved":
                modalTitle.textContent = "✅ Request Approved";
                modalMessage.textContent = `${memberName}'s registration request has been approved`;
                break;
            case "Rejected":
                modalTitle.textContent = "❌ Request Rejected";
                modalMessage.textContent = `${memberName}'s registration request has been rejected`;
                break;
            case "WalkInApproved":
                modalTitle.textContent = "✅ Walk-In Approved";
                modalMessage.textContent = `${memberName}'s walk-in has been approved`;
                break;
            case "WalkInRejected":
                modalTitle.textContent = "❌ Walk-In Rejected";
                modalMessage.textContent = `${memberName}'s walk-in has been rejected`;
                break;
            default:
                modalTitle.textContent = "⚠ Action Failed";
                modalMessage.textContent = "Something went wrong. Please try again";
        }

        setTimeout(() => {
            modal.classList.add("fade-out");
            setTimeout(() => modal.classList.add("hidden"), 500);
        }, 3000);
    }
});



document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.member-item').forEach(item => {
        item.addEventListener('click', (e) => {
            if (e.target.closest('.options-container')) return;

            const memberId = item.getAttribute('data-id');
            const modal = document.getElementById(`memberProfileModal-${memberId}`);
            if (modal) modal.classList.remove('hidden');
        });
    });

    document.querySelectorAll('.profile-close-btn').forEach(btn => {
        btn.addEventListener('click', () => {
            const modalId = btn.getAttribute('data-modal-id');
            const modal = document.getElementById(modalId);
            if (modal) modal.classList.add('hidden');
        });
    });

    document.querySelectorAll('.profile-modal-overlay').forEach(overlay => {
        overlay.addEventListener('click', (e) => {
            if (e.target === overlay) overlay.classList.add('hidden');
        });
    });
});

document.getElementById("printMembersBtn").addEventListener("click", function () {
    const members = document.querySelectorAll(".member-item");
    if (members.length === 0) {
        alert("No members to export.");
        return;
    }

    let rows = [];
    members.forEach(member => {
        const name = member.dataset.name || "";
        const email = member.dataset.email || "";
        const phone = member.dataset.phone || "";
        const membership = member.dataset.membership || "";
        const status = member.dataset.status || "";
        rows.push([name, email, phone, membership, status]);
    });

    const { jsPDF } = window.jspdf;
    const doc = new jsPDF();

    doc.setFont("helvetica", "bold");
    doc.setFontSize(16);
    doc.text("Stretch Fitness Hub - Member Records", 14, 15);

    doc.setFontSize(11);
    doc.setFont("helvetica", "normal");
    doc.text(`Generated: ${new Date().toLocaleString()}`, 14, 22);

    const headers = [["Full Name", "Email", "Contact", "Membership", "Status"]];

    doc.autoTable({
        head: headers,
        body: rows,
        startY: 28,
        theme: "grid",
        headStyles: { fillColor: [34, 197, 94] }, 
        styles: { fontSize: 10 },
    });

    doc.save("Member_Records.pdf");
});

document.addEventListener("DOMContentLoaded", () => {
    const modal = document.getElementById("settingsModal");
    const closeBtn = document.getElementById("closeSettings");
    const cancelBtn = document.getElementById("cancelSettings");
    const saveBtn = document.getElementById("saveSettings");

    const alertModal = document.getElementById("customAlertModal");
    const alertMessage = document.getElementById("customAlertMessage");
    const alertOk = document.getElementById("customAlertOk");

    function showAlert(message) {
        alertMessage.textContent = message;
        alertModal.classList.remove("hidden");
    }

    alertOk.addEventListener("click", () => {
        alertModal.classList.add("hidden");
    });

    closeBtn.addEventListener("click", () => modal.classList.add("hidden"));
    cancelBtn.addEventListener("click", () => modal.classList.add("hidden"));

    saveBtn.addEventListener("click", () => {
        const username = document.getElementById("username").value.trim();
        const password = document.getElementById("password").value.trim();
        const confirmPassword = document.getElementById("confirmPassword").value.trim();
        const email = document.getElementById("email").value.trim();

        if (password && password !== confirmPassword) {
            showAlert("❌ Passwords do not match.");
            return;
        }

        if (!email) {
            showAlert("❌ Email cannot be empty.");
            return;
        }

        const body = `username=${encodeURIComponent(username)}&password=${encodeURIComponent(password)}&email=${encodeURIComponent(email)}`;

        fetch('/Admin/UpdateSettings', {
            method: 'POST',
            headers: { 'Content-Type': 'application/x-www-form-urlencoded' },
            body: body
        })
            .then(res => res.json())
            .then(data => {
                if (data.success) {
                    showAlert("✅ Settings updated successfully!");
                    modal.classList.add("hidden");
                    const headerUsername = document.getElementById("adminUsername");
                    if (headerUsername) headerUsername.textContent = data.username;

                } else {
                    showAlert("❌ Failed to update settings.");
                }
            })
            .catch(err => {
                console.error(err);
                showAlert("❌ An error occurred. Check console.");
            });
    });
});


// Hamburger Menu Toggle
const hamburgerBtn = document.getElementById('hamburgerBtn');
const sidebar = document.getElementById('sidebar');

hamburgerBtn?.addEventListener('click', () => {
    sidebar.classList.toggle('open');
    document.body.classList.toggle('sidebar-open');
});

// Close sidebar when clicking outside
document.addEventListener('click', (e) => {
    if (sidebar.classList.contains('open') &&
        !sidebar.contains(e.target) &&
        !hamburgerBtn.contains(e.target)) {
        sidebar.classList.remove('open');
        document.body.classList.remove('sidebar-open');
    }
});




// ======== Modal Elements ========
const openModalBtn = document.getElementById('openClassScheduleModalBtn');
const classScheduleModal = document.getElementById('classScheduleModal');
const closeModalBtn = document.getElementById('closeClassScheduleModal');
const cancelModalBtn = document.getElementById('cancelClassScheduleBtn');
const classScheduleForm = document.getElementById('classScheduleForm');

// ======== Open / Close Modal ========
openModalBtn.addEventListener('click', () => classScheduleModal.style.display = 'flex');
closeModalBtn.addEventListener('click', () => classScheduleModal.style.display = 'none');
cancelModalBtn.addEventListener('click', () => classScheduleModal.style.display = 'none');

window.addEventListener('click', e => {
    if (e.target === classScheduleModal) classScheduleModal.style.display = 'none';
});

// ======== Submit Form ========
classScheduleForm.addEventListener('submit', async e => {
    e.preventDefault();

    const checkedDays = Array.from(
        classScheduleForm.querySelectorAll('.days-checkbox-group input[type="checkbox"]:checked')
    ).map(cb => cb.value);

    if (checkedDays.length === 0) {
        showCustomAlert('Please select at least one day.');
        return;
    }

    const newClass = {
        ClassName: document.getElementById('classNameInput').value.trim(),
        Coach: document.getElementById('coachInput').value.trim(),
        Days: checkedDays.join(','),
        StartTime: document.getElementById('startTimeInput').value + ":00",
        EndTime: document.getElementById('endTimeInput').value + ":00"
    };

    if (!newClass.ClassName || !newClass.Coach || !newClass.StartTime || !newClass.EndTime) {
        showCustomAlert('Please fill in all fields.');
        return;
    }

    try {
        const response = await fetch('/Admin/AddClass', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
            },
            body: JSON.stringify(newClass)
        });

        const result = await response.json();

        if (result.success) {
            appendClassToList({
                id: result.id,
                className: result.className,
                coach: result.coach,
                days: result.days,
                startTime: result.startTime,
                endTime: result.endTime
            });

            classScheduleModal.style.display = 'none';
            classScheduleForm.reset();
        } else {
            showCustomAlert(result.message || 'Failed to add class.');
        }
    } catch (err) {
        console.error(err);
        showCustomAlert('An error occurred while adding the class.');
    }
});

// ======== Append Class to List ========
function appendClassToList(cls) {
    const list = document.querySelector('.class-list');

    const item = document.createElement('div');
    item.classList.add('class-item');
    item.dataset.id = cls.id;
    item.dataset.name = cls.className;
    item.dataset.days = cls.days;
    item.dataset.start = cls.startTime;
    item.dataset.end = cls.endTime;
    item.dataset.coach = cls.coach;

    item.innerHTML = `
        <div class="class-color" style="background-color: #3b82f6;"></div>
        <div class="class-info">
            <div class="class-name">${cls.className}</div>
            <div class="class-details">
                <div class="class-detail">
                    <i class="ri-time-line"></i>
                    <span class="time-text">${convertDaysToText(cls.days)} ${formatTime(cls.startTime)} - ${formatTime(cls.endTime)}</span>
                </div>
                <div class="class-detail">
                    <i class="ri-user-line"></i>
                    <span class="coach-text">${cls.coach}</span>
                </div>
            </div>
        </div>
        <div class="class-actions">
            <button class="remove-class-btn" data-id="${cls.id}">🗑</button>
        </div>
    `;
    list.appendChild(item);
}


// ======== Helpers ========
function convertDaysToText(daysStr) {
    return daysStr.split(',').join(', ');
}

function formatTime(timeStr) {
    // Ensure time is hh:mm format
    const [hour, minute] = timeStr.split(':');
    return `${hour.padStart(2, '0')}:${minute.padStart(2, '0')}`;
}

// ======== Custom Alert ========
function showCustomAlert(msg) {
    alert(msg); // Replace with your custom alert modal if needed
}


function convertDaysToText(days) {
    return days.split(',').map(d => d.trim()).join(', ');
}

function formatTime(time) {
    const [hour, minute] = time.split(':');
    const h = parseInt(hour, 10);
    const suffix = h >= 12 ? 'PM' : 'AM';
    const displayHour = ((h + 11) % 12) + 1;
    return `${displayHour}:${minute} ${suffix}`;
}

let classIdToRemove = null;
let classElementToRemove = null;

function openRemoveModal(classId, className, element) {
    classIdToRemove = classId;
    classElementToRemove = element;
    const modal = document.getElementById('customAlertModal');
    const message = document.getElementById('customAlertMessage');
    message.textContent = `Are you sure you want to remove "${className}" class?`;
    modal.classList.remove('hidden');
}

function closeRemoveModal() {
    const modal = document.getElementById('customAlertModal');
    modal.classList.add('hidden');
    classIdToRemove = null;
    classElementToRemove = null;
}

document.getElementById('customAlertOk').addEventListener('click', async () => {
    if (!classIdToRemove || !classElementToRemove) return;

    try {
        const response = await fetch(`/Admin/RemoveClass?id=${classIdToRemove}`, { method: 'POST' });
        const data = await response.json();

        if (data.success) {
            classElementToRemove.remove();
            const classList = document.querySelector('.class-list');
            if (!classList.children.length) {
                classList.innerHTML = '<div class="no-classes">No classes scheduled yet.</div>';
            }
        } else {
            showCustomAlert(data.message || 'Failed to remove class.');
        }
    } catch (error) {
        console.error(error);
        showCustomAlert('Error removing class.');
    } finally {
        closeRemoveModal();
    }
});

document.getElementById('customAlertCancel').addEventListener('click', closeRemoveModal);

document.addEventListener('click', e => {
    const target = e.target;
    if (target.classList.contains('remove-class-btn')) {
        const classItem = target.closest('.class-item');
        openRemoveModal(classItem.dataset.id, classItem.dataset.name, classItem);
    }
});



function showTab(tab) {
    const activeItems = document.querySelectorAll('.member-item.active');
    const archivedItems = document.querySelectorAll('.member-item.archived');
    const walkinItems = document.querySelectorAll('.member-item.walkins');
    const noWalkins = document.getElementById('noWalkins');
    const walkinsHeader = document.querySelector('.walkins-header');
    const walkinsLabel = document.getElementById('walkinsLabel');
    document.querySelectorAll('.tab-btn').forEach(btn => btn.classList.remove('active'));

    activeItems.forEach(i => i.style.display = 'none');
    archivedItems.forEach(i => i.style.display = 'none');
    walkinItems.forEach(i => i.style.display = 'none');
    if (noWalkins) noWalkins.style.display = 'none';
    if (walkinsHeader) walkinsHeader.style.display = 'none';

    if (tab === 'active') {
        activeItems.forEach(i => i.style.display = 'flex');
        document.querySelector('.tab-btn:nth-child(1)').classList.add('active');
    } else if (tab === 'archived') {
        archivedItems.forEach(i => i.style.display = 'flex');
        document.querySelector('.tab-btn:nth-child(2)').classList.add('active');
    } else if (tab === 'walkins') {
        if (walkinsHeader) {
            const today = new Date();
            const options = { month: 'short', day: 'numeric', year: 'numeric' };
            if (walkinsLabel) walkinsLabel.textContent = `Walk-ins for today (${today.toLocaleDateString('en-US', options)})`;
            walkinsHeader.style.display = 'block';
        }

        if (walkinItems.length > 0) {
            walkinItems.forEach(i => i.style.display = 'flex');
        } else if (noWalkins) {
            noWalkins.style.display = 'block';
        }

        document.querySelector('.tab-btn:nth-child(3)').classList.add('active');
    }
}



let selectedMemberId = null;

// ✅ Open Add Class Modal
function openAddClassModal(memberId, memberName) {
    selectedMemberId = memberId;
    document.getElementById("addClassMemberModal").style.display = "block";
    document.getElementById("memberNameSpan").textContent = memberName;
}

// ✅ Close Modal
function closeAddClassModal() {
    document.getElementById("addClassMemberModal").style.display = "none";
    selectedMemberId = null;
    document.getElementById("classSelect").value = "";
}

// ✅ Close modal when clicking outside
window.addEventListener("click", function (e) {
    const modal = document.getElementById("addClassMemberModal");
    if (e.target === modal) closeAddClassModal();
});

// ✅ Custom Alert only for Add Class
function showAddClassAlert(message, type = "success") {
    const alertModal = document.getElementById("addClassAlertModal");
    const msg = document.getElementById("addClassAlertMessage");

    msg.textContent = message;
    alertModal.classList.remove("hidden");
    alertModal.classList.remove("error");
    if (type === "error") alertModal.classList.add("error");

    alertModal.classList.add("fade-in");

    // Auto close after 3 seconds
    setTimeout(() => {
        alertModal.classList.add("fade-out");
        setTimeout(() => {
            alertModal.classList.add("hidden");
            alertModal.classList.remove("fade-in", "fade-out");
        }, 400);
    }, 3000);
}

// ✅ Handle Add Class submission
document.getElementById("confirmAddClassBtn").addEventListener("click", function () {
    const classId = document.getElementById("classSelect").value;

    if (!selectedMemberId || !classId) {
        showAddClassAlert("⚠️ Please select a class before adding.", "error");
        return;
    }

    fetch("/Admin/AddClassToMember", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            MemberId: selectedMemberId,
            ClassId: parseInt(classId)
        })
    })
        .then(res => res.json())
        .then(data => {
            showAddClassAlert(data.message, data.success ? "success" : "error");
            if (data.success) closeAddClassModal();
        })
        .catch(err => {
            console.error(err);
            showAddClassAlert("❌ An error occurred while adding class.", "error");
        });
});



/* ===== Add Staff Form ===== */
document.getElementById("addStaffForm").addEventListener("submit", async function (e) {
    e.preventDefault();

    const form = e.target;
    const data = {
        FullName: form.fullName.value.trim(),
        Username: form.username.value.trim(),
        Password: form.password.value.trim()
    };

    try {
        const res = await fetch('/Admin/CreateStaff', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        const result = await res.json();

        // Show success modal instead of alert
        if (result.success) {
            showAddStaffModal(result.message || "New staff account created successfully!");
            form.reset(); // reset form
        } else {
            showAddStaffModal(result.message || "Failed to add staff.", true);
        }
    } catch (err) {
        console.error(err);
        showAddStaffModal("An error occurred while adding staff.", true);
    }
});

/* ===== Add Staff Modal Functions ===== */
const addStaffModal = document.getElementById('newStaffModal');
const addStaffMessage = document.getElementById('newStaffMessage');
const closeAddStaffModal = document.getElementById('closeNewStaffModal');

function showAddStaffModal(message, isError = false) {
    addStaffMessage.textContent = message;
    addStaffMessage.style.color = isError ? '#e74c3c' : '#2ecc71'; // red for error, green for success
    addStaffModal.classList.remove('hidden');

    setTimeout(() => addStaffModal.classList.add('hidden'), 2000);
}

closeAddStaffModal.addEventListener('click', () => {
    addStaffModal.classList.add('hidden');
});



// ✅ Custom modal function
function showCustomAlert(message) {
    const modal = document.getElementById('customAlertModal');
    const msg = document.getElementById('customAlertMessage');
    const okBtn = document.getElementById('customAlertOk');

    msg.textContent = message;
    modal.classList.remove('hidden');

    okBtn.onclick = () => {
        modal.classList.add('hidden');
    };
}
// ✅ Convert 24h time string ("HH:mm") to 12h format with AM/PM
function to12HourFormat(timeStr) {
    const [hourStr, min] = timeStr.split(':');
    let hour = parseInt(hourStr);
    const ampm = hour >= 12 ? 'PM' : 'AM';
    hour = hour % 12 || 12; // Convert 0/12 -> 12
    return `${hour}:${min} ${ampm}`;
}

// ✅ Helper: Capitalize first letter of each word
function capitalizeWords(str) {
    return str
        .toLowerCase() // Convert all to lower first
        .split(' ')
        .filter(word => word) // Remove empty strings from double spaces
        .map(word => word.charAt(0).toUpperCase() + word.slice(1))
        .join(' ');
}

// ✅ Add class form submit with validation
document.getElementById('addClassBtn').addEventListener('click', function (e) {
    e.preventDefault();

    let className = document.getElementById('ClassName').value.trim();
    let coach = document.getElementById('Coach').value.trim();
    const days = document.getElementById('Days').value.trim();
    const startTime = document.getElementById('StartTime').value;
    const endTime = document.getElementById('EndTime').value;

    // ✅ Empty field validation
    if (!className || !coach || !days || !startTime || !endTime) {
        showCustomAlert("❌ Please fill in all fields before submitting.");
        return;
    }

    // ✅ Start time < End time
    if (startTime >= endTime) {
        showCustomAlert("❌ Start time cannot be later than or equal to end time.");
        return;
    }

    // ✅ ClassName & Coach validations
    const invalidChars = /[^a-zA-Z\s]/; // Only letters and spaces allowed
    const doubleSpace = /\s{2,}/;

    if (invalidChars.test(className)) {
        showCustomAlert("❌ Class name cannot contain numbers or special characters.");
        return;
    }

    if (doubleSpace.test(className)) {
        showCustomAlert("❌ Class name cannot contain double spaces.");
        return;
    }

    if (invalidChars.test(coach)) {
        showCustomAlert("❌ Coach name cannot contain numbers or special characters.");
        return;
    }

    if (doubleSpace.test(coach)) {
        showCustomAlert("❌ Coach name cannot contain double spaces.");
        return;
    }

    // ✅ Auto-format names: capitalize each word
    className = capitalizeWords(className);
    coach = capitalizeWords(coach);

    // ✅ Fetch POST request
    fetch('/Admin/AddClass', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]').value
        },
        body: JSON.stringify({ ClassName: className, Coach: coach, Days: days, StartTime: startTime, EndTime: endTime })
    })
        .then(res => res.json())
        .then(data => {
            if (data.success) {
                const start = to12HourFormat(data.startTime);
                const end = to12HourFormat(data.endTime);
                showCustomAlert(`Class "${data.className}" added successfully!\nTime: ${start} - ${end}\nCoach: ${data.coach}\nDays: ${data.days}`);
            } else {
                showCustomAlert(data.message);
            }
        })
        .catch(err => {
            showCustomAlert('An error occurred: ' + err.message);
        });
});








