// ============================================================
//  ajax-helper.js
//  Drop in: wwwroot/js/ajax-helper.js
//  Include ONCE in _Layout.cshtml before closing </body>
// ============================================================


// ── ajaxPost ────────────────────────────────────────────────────────────────
//  url       : controller action url
//  data      : plain JS object  { key: value }
//  buttonEl  : the submit button DOM element (pass null if you don't have one)
//  onSuccess : optional callback function(response) called on success
// ────────────────────────────────────────────────────────────────────────────
const OCMS = (function ($) {

    /* ─── Confirm Modal ───────────────────────────────────── */
    const Confirm = {
        _resolve: null,

        show({ title = 'Are you sure?', text = 'This action cannot be undone.', icon = '⚠️', confirmText = 'Confirm', cancelText = 'Cancel', type = 'danger' } = {}) {
            return new Promise((resolve) => {
                this._resolve = resolve;
                const $b = $('#confirm-modal-backdrop');
                $b.find('.modal-icon').text(icon);
                $b.find('.modal-title').text(title);
                $b.find('.modal-text').text(text);
                $b.find('#confirm-ok-btn')
                    .text(confirmText)
                    .removeClass('btn btn-danger btn-primary btn-warning')
                    .addClass('btn btn-' + type);
                $b.find('#confirm-cancel-btn').text(cancelText);
                $b.addClass('show');
            });
        },

        _ok() { this._finish(true); },
        _cancel() { this._finish(false); },

        _finish(result) {
            $('#confirm-modal-backdrop').removeClass('show');
            if (this._resolve) { this._resolve(result); this._resolve = null; }
        }
    };
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


    function ajaxPost(url, data, buttonEl = null, onSuccess = null) {
        if (buttonEl) {
            buttonEl.disabled = true;
            buttonEl.dataset.original = buttonEl.innerHTML;
            buttonEl.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Please wait...';
        }

        $.ajax({
            url: url,
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(data),
            statusCode: {
                401: function () {
                    showToast('Session expired. Redirecting to login...', 'warning');
                    setTimeout(() => window.location.href = '/Auth/LoginPage', 1500);
                },
                403: function () {
                    showToast('You are not authorized to perform this action.', 'danger');
                }
            },
            success: function (response) {
                if (!response.success) {
                    showToast(response.message, 'danger');
                    return;
                }

                showToast(response.message, 'success');

                if (response.redirectUrl) {
                    setTimeout(() => window.location.href = response.redirectUrl, 800);
                    return;   // don't call onSuccess if we are redirecting
                }

                if (onSuccess) onSuccess(response);   // ← the only change from your original
            },
            error: function () {
                showToast('Server error!', 'danger');
            },
            complete: function () {
                if (buttonEl) {
                    buttonEl.disabled = false;
                    buttonEl.innerHTML = buttonEl.dataset.original;
                }
            }
        });
    }


    // ── ajaxPostForm ─────────────────────────────────────────────────────────────
    //  For file uploads / FormData (IFormFile on the server)
    //  url       : controller action url
    //  formData  : a FormData object
    //  buttonEl  : the trigger button DOM element (pass null if not applicable)
    //  onSuccess : optional callback function(response) called on success
    // ─────────────────────────────────────────────────────────────────────────────
    function ajaxPostForm(url, formData, buttonEl = null, onSuccess = null) {
        if (buttonEl) {
            buttonEl.disabled = true;
            buttonEl.dataset.original = buttonEl.innerHTML;
            buttonEl.style.minWidth = buttonEl.offsetWidth + 'px';   // lock width so it doesn't shrink
            buttonEl.innerHTML = `
            <span class="spinner-border spinner-border-sm text-light" role="status" aria-hidden="true"></span>
            <span>Please wait...</span>
        `;
        }

        $.ajax({
            url: url,
            type: 'POST',
            data: formData,
            processData: false,   // tell jQuery NOT to convert FormData to a query string
            contentType: false,   // let the browser set multipart/form-data with boundary
            statusCode: {
                401: function () {
                    showToast('Session expired. Redirecting to login...', 'warning');
                    setTimeout(() => window.location.href = '/Auth/LoginPage', 1500);
                },
                403: function () {
                    showToast('You are not authorized to perform this action.', 'danger');
                }
            },
            success: function (response) {
                if (!response.success) {
                    showToast(response.message, 'danger');
                    return;
                }

                showToast(response.message, 'success');

                if (response.redirectUrl) {
                    setTimeout(() => window.location.href = response.redirectUrl, 800);
                    return;
                }

                if (onSuccess) onSuccess(response);
            },
            error: function () {
                showToast('Server error occurred.', 'danger');
            },
            complete: function () {
                if (buttonEl) {
                    buttonEl.disabled = false;
                    buttonEl.innerHTML = buttonEl.dataset.original;
                    buttonEl.style.minWidth = '';   
                }
            }
        });
    }


    // ── showToast ────────────────────────────────────────────────────────────────
    //  message : string
    //  type    : 'success' | 'danger' | 'warning'
    // ─────────────────────────────────────────────────────────────────────────────
    function showToast(message, type = 'success') {
        document.querySelector('.ajax-toast')?.remove();

        const styles = {
            success: { background: '#16a34a', icon: 'bi-check-circle-fill' },
            danger: { background: '#dc2626', icon: 'bi-x-circle-fill' },
            warning: { background: '#d97706', icon: 'bi-exclamation-triangle-fill' },
            info: { background: '#0891b2', icon: 'bi-info-circle-fill' }
        };

        const style = styles[type] || styles.success;

        const toast = `
    <div class="ajax-toast" style="
        position: fixed;
        top: 20px;
        right: 20px;
        z-index: 9999;
        background: #fff;
        color: #0f172a;
        padding: 14px 18px;
        border-radius: 10px;
        border-left: 4px solid ${style.background};
        box-shadow: 0 8px 20px rgba(0,0,0,0.12);
        display: flex;
        align-items: center;
        gap: 12px;
        font-size: 14px;
        font-weight: 500;
        min-width: 260px;
        max-width: 380px;
        animation: slideIn 0.3s ease;
    ">
        <i class="bi ${style.icon}" style="font-size: 18px; color: ${style.background};"></i>
        <span style="flex:1;">${message}</span>
    </div>`;

        // Inject keyframes only once
        if (!document.getElementById('ajax-toast-keyframes')) {
            const styleTag = document.createElement('style');
            styleTag.id = 'ajax-toast-keyframes';
            styleTag.textContent = `
        @keyframes slideIn  { from { opacity:0; transform:translateX(50px); } to { opacity:1; transform:translateX(0); } }
        @keyframes slideOut { from { opacity:1; transform:translateX(0); }    to { opacity:0; transform:translateX(50px); } }
    `;
            document.head.appendChild(styleTag);
        }

        document.body.insertAdjacentHTML('beforeend', toast);

        setTimeout(() => {
            const t = document.querySelector('.ajax-toast');
            if (t) {
                t.style.animation = 'slideOut 0.3s ease forwards';
                setTimeout(() => t?.remove(), 300);
            }
        }, 3500);
    }

    /* ─── Wire up confirm modal buttons ──────────────────── */
    $(function () {
        $(document).on('click', '#confirm-ok-btn', () => Confirm._ok());
        $(document).on('click', '#confirm-cancel-btn', () => Confirm._cancel());
        $(document).on('click', '#confirm-modal-backdrop', function (e) {
            if ($(e.target).is('#confirm-modal-backdrop')) Confirm._cancel();
        });
    });

    // expose functions
    return {
        ajaxPost: ajaxPost,
        ajaxPostForm: ajaxPostForm,
        showToast: showToast,
        Confirm: Confirm
    };

})(jQuery);
