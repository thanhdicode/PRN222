"use strict";

var connection = new signalR.HubConnectionBuilder()
    .withUrl("/hubs/notifications")
    .withAutomaticReconnect()
    .build();

function updateNotificationBadge(count) {
    const badge = document.getElementById("notification-badge");
    if (badge) {
        badge.innerText = count > 0 ? count : "";
        badge.style.display = count > 0 ? "inline-block" : "none";
    }
}

function fetchUnreadNotifications() {
    fetch('/api/Notifications/GetUnread')
        .then(response => response.json())
        .then(data => {
            updateNotificationBadge(data.length);
            renderNotificationDropdown(data);
        })
        .catch(error => console.error('Error fetching notifications:', error));
}

function renderNotificationDropdown(notifications) {
    const container = document.getElementById("notification-dropdown-menu");
    if (!container) return;

    container.innerHTML = "";
    
    if (notifications.length === 0) {
        container.innerHTML = "<li><span class='dropdown-item text-muted'>No new notifications</span></li>";
        return;
    }

    notifications.forEach(n => {
        const li = document.createElement("li");
        li.innerHTML = `
            <div class="dropdown-item d-flex justify-content-between align-items-center">
                <div>
                    <strong>${n.title}</strong><br/>
                    <small>${n.message}</small>
                </div>
                <button class="btn btn-sm btn-light ms-2" onclick="markAsRead('${n.notificationId}')">
                    <i class="bi bi-check"></i>
                </button>
            </div>
        `;
        container.appendChild(li);
    });

    const divider = document.createElement("li");
    divider.innerHTML = "<hr class='dropdown-divider'>";
    container.appendChild(divider);

    const markAllItem = document.createElement("li");
    markAllItem.innerHTML = `<button class="dropdown-item text-center text-primary" onclick="markAllAsRead()">Mark all as read</button>`;
    container.appendChild(markAllItem);
}

function markAsRead(notificationId) {
    fetch(`/api/Notifications/MarkRead/${notificationId}`, { method: 'POST' })
        .then(response => {
            if (response.ok) fetchUnreadNotifications();
        });
}

function markAllAsRead() {
    fetch('/api/Notifications/MarkAllRead', { method: 'POST' })
        .then(response => {
            if (response.ok) fetchUnreadNotifications();
        });
}

function showToast(title, message) {
    const toastContainer = document.getElementById('toast-container');
    if (!toastContainer) return;

    const toastEl = document.createElement('div');
    toastEl.className = 'toast show';
    toastEl.setAttribute('role', 'alert');
    toastEl.setAttribute('aria-live', 'assertive');
    toastEl.setAttribute('aria-atomic', 'true');

    toastEl.innerHTML = `
        <div class="toast-header bg-primary text-white">
            <strong class="me-auto">${title}</strong>
            <button type="button" class="btn-close btn-close-white" data-bs-dismiss="toast" aria-label="Close"></button>
        </div>
        <div class="toast-body">
            ${message}
        </div>
    `;

    toastContainer.appendChild(toastEl);

    setTimeout(() => {
        toastEl.classList.remove('show');
        setTimeout(() => toastEl.remove(), 500);
    }, 5000);
}

connection.on("ReceiveNotification", function (notification) {
    showToast(notification.title, notification.message);
    fetchUnreadNotifications();
});

connection.start().then(function () {
    console.log("Connected to notification hub");
    fetchUnreadNotifications();
}).catch(function (err) {
    return console.error(err.toString());
});
