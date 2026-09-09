//// ===== ADMIN TOAST =====
//function showAdminToast(message, type = 'success') {
//    const icons = {
//        success: 'fa-circle-check',
//        danger: 'fa-circle-xmark',
//        warning: 'fa-triangle-exclamation'
//    };

//    const toast = document.createElement('div');
//    toast.className = `admin-toast ${type}`;
//    toast.innerHTML = `
//        <i class="fas ${icons[type] || icons.success}"></i>
//        <span>${message}</span>`;

//    document.getElementById('toastContainer').appendChild(toast);

//    setTimeout(() => {
//        toast.style.animation = 'toastOut 0.3s ease forwards';
//        setTimeout(() => toast.remove(), 300);
//    }, 3000);
//}

//// ===== STATUS BADGE =====
//function GetStatusBadge(status) {
//    const map = {
//        0: ['pending', 'Pending'],
//        1: ['inprogress', 'In Progress'],
//        2: ['resolved', 'Resolved'],
//        3: ['rejected', 'Rejected']
//    };
//    const [cls, label] = map[status] ?? ['pending', 'Unknown'];
//    return `<span class="status-badge badge-${cls}">${label}</span>`;
//}

//// ===== TABLE ROW ANIMATION =====
//function AnimateRows() {
//    document.querySelectorAll('.admin-table tbody tr').forEach((row, i) => {
//        row.style.animationDelay = `${i * 40}ms`;
//    });
//}

// ===== CONFIRM MODAL =====
function ShowConfirmModal(message, onConfirm) {
    const existing = document.getElementById('adminConfirmModal');
    if (existing) existing.remove();

    const modal = document.createElement('div');
    modal.id = 'adminConfirmModal';
    modal.innerHTML = `
        <div class="modal fade" id="confirmModalBS" tabindex="-1">
            <div class="modal-dialog modal-dialog-centered">
                <div class="modal-content" style="border-radius:12px; border:none;">
                    <div class="modal-body text-center p-4">
                        <i class="fas fa-triangle-exclamation fa-2x text-warning mb-3"></i>
                        <p class="mb-4" style="font-size:0.95rem;">${message}</p>
                        <div class="d-flex justify-content-center gap-3">
                            <button class="btn btn-secondary" data-bs-dismiss="modal">
                                Cancel
                            </button>
                            <button class="btn btn-danger" id="confirmBtn">
                                Yes, Delete
                            </button>
                        </div>
                    </div>
                </div>
            </div>
        </div>`;

    document.body.appendChild(modal);

    const bsModal = new bootstrap.Modal(
        document.getElementById('confirmModalBS'));
    bsModal.show();

    document.getElementById('confirmBtn').onclick = function () {
        bsModal.hide();
        onConfirm();
    };
}


//========================================

/**
* OCMS Admin JS
* Sidebar toggle, charts, AJAX calls for admin pages
*/
$(function () {
    'use strict';

    /* ── Sidebar Toggle ────────────────────────────────── */
    $('#sidebar-toggle').on('click', function () {
        const $sidebar = $('#admin-sidebar');
        if ($(window).width() <= 900) {
            $sidebar.toggleClass('mobile-open');
        } else {
            $sidebar.toggleClass('collapsed');
        }
    });

    /* ── Active nav link ───────────────────────────────── */
    const path = window.location.pathname.split('/').pop();
    $('.sidebar-link').each(function () {
        const href = $(this).data('href') || '';
        if (href && path.includes(href)) $(this).addClass('active');
    });

    /* ── Init Charts (only on dashboard) ──────────────── */
    if ($('#chart-complaints-line').length) initDashboardCharts();

    function initDashboardCharts() {
        // Line chart
        const ctxLine = document.getElementById('chart-complaints-line').getContext('2d');
        new Chart(ctxLine, {
            type: 'line',
            data: {
                labels: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul'],
                datasets: [{
                    label: 'Complaints',
                    data: [32, 45, 38, 60, 55, 72, 89],
                    borderColor: '#1a56db',
                    backgroundColor: 'rgba(26,86,219,.08)',
                    borderWidth: 2.5,
                    pointBackgroundColor: '#1a56db',
                    pointRadius: 4,
                    tension: 0.4,
                    fill: true
                }, {
                    label: 'Resolved',
                    data: [20, 38, 30, 50, 45, 60, 75],
                    borderColor: '#16a34a',
                    backgroundColor: 'rgba(22,163,74,.05)',
                    borderWidth: 2.5,
                    pointBackgroundColor: '#16a34a',
                    pointRadius: 4,
                    tension: 0.4,
                    fill: true
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                plugins: { legend: { position: 'bottom', labels: { font: { family: 'Plus Jakarta Sans', size: 12 } } } },
                scales: {
                    y: { beginAtZero: true, grid: { color: '#f1f5f9' }, ticks: { font: { family: 'Plus Jakarta Sans' } } },
                    x: { grid: { display: false }, ticks: { font: { family: 'Plus Jakarta Sans' } } }
                }
            }
        });

        // Doughnut chart
        const ctxDonut = document.getElementById('chart-category-donut').getContext('2d');
        new Chart(ctxDonut, {
            type: 'doughnut',
            data: {
                labels: ['Service', 'Technical', 'Billing', 'Other'],
                datasets: [{
                    data: [38, 27, 21, 14],
                    backgroundColor: ['#1a56db', '#0891b2', '#f97316', '#8b5cf6'],
                    borderWidth: 0,
                    hoverOffset: 6
                }]
            },
            options: {
                responsive: true, maintainAspectRatio: false,
                cutout: '68%',
                plugins: {
                    legend: { position: 'bottom', labels: { font: { family: 'Plus Jakarta Sans', size: 12 }, padding: 14 } }
                }
            }
        });
    }

    /* ── AJAX: Update Complaint Status ─────────────────── */
    $(document).on('click', '.btn-update-status', async function () {
        const $btn = $(this);
        const id = $btn.data('id');
        const status = $('#status-select-' + id).val();

        const confirmed = await OCMS.Confirm.show({
            title: 'Update Status',
            text: `Change complaint #${id} status to "${status}"?`,
            icon: '🔄',
            confirmText: 'Yes, Update',
            type: 'primary'
        });
        if (!confirmed) return;

        OCMS.ajax({
            url: `/complaints/update-status`,
            method: 'POST',
            data: { id, status },
            $btn,
            onSuccess(data) { loadComplaintsTable(); }
        });
    });

    /* ── AJAX: Delete Complaint ─────────────────────────── */
    $(document).on('click', '.btn-delete-complaint', async function () {
        const $btn = $(this);
        const id = $btn.data('id');

        const confirmed = await OCMS.Confirm.show({
            title: 'Delete Complaint',
            text: `Permanently delete complaint #${id}? This cannot be undone.`,
            icon: '🗑️',
            confirmText: 'Delete',
            type: 'danger'
        });
        if (!confirmed) return;

        OCMS.ajax({
            url: `/complaints/delete/${id}`,
            method: 'DELETE',
            $btn,
            onSuccess() { $btn.closest('tr').fadeOut(300, function () { $(this).remove(); }); }
        });
    });

    /* ── AJAX: Toggle User Status ───────────────────────── */
    $(document).on('click', '.btn-toggle-user', async function () {
        const $btn = $(this);
        const id = $btn.data('id');
        const action = $btn.data('action'); // 'block' | 'unblock'

        const confirmed = await OCMS.Confirm.show({
            title: action === 'block' ? 'Block User' : 'Unblock User',
            text: `Are you sure you want to ${action} this user?`,
            icon: action === 'block' ? '🚫' : '✅',
            confirmText: action === 'block' ? 'Block' : 'Unblock',
            type: action === 'block' ? 'danger' : 'success'
        });
        if (!confirmed) return;

        OCMS.ajax({
            url: `/users/toggle-status`,
            method: 'POST',
            data: { id, action },
            $btn,
            onSuccess() { loadUsersTable(); }
        });
    });

    /* ── Status filter ──────────────────────────────────── */
    $('#status-filter').on('change', function () {
        const val = $(this).val().toLowerCase();
        $('#complaints-table tbody tr').each(function () {
            const status = $(this).find('.badge-ocms').text().toLowerCase();
            $(this).toggle(!val || status.includes(val));
        });
    });

    /* ── Search filter ──────────────────────────────────── */
    $('#table-search').on('input', function () {
        const q = $(this).val().toLowerCase();
        $('#complaints-table tbody tr, #users-table tbody tr').each(function () {
            $(this).toggle($(this).text().toLowerCase().includes(q));
        });
    });
});
