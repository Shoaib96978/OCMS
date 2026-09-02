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
            buttonEl.innerHTML = '<i class="fas fa-spinner fa-spin"></i> Please wait...';
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
            success: { background: '#741f9e', icon: 'fa-circle-check' },
            danger: { background: '#b53e20', icon: 'fa-circle-xmark' },
            warning: { background: '#3a7ca5', icon: 'fa-triangle-exclamation' }
        };

        const style = styles[type] || styles.success;

        const toast = `
        <div class="ajax-toast" style="
            position: fixed;
            top: 20px;
            right: 20px;
            z-index: 9999;
            background: ${style.background};
            color: white;
            padding: 14px 20px;
            border-radius: 10px;
            box-shadow: 0 8px 20px rgba(0,0,0,0.2);
            display: flex;
            align-items: center;
            gap: 10px;
            font-size: 14px;
            font-weight: 500;
            min-width: 250px;
            animation: slideIn 0.3s ease;
        ">
            <i class="fas ${style.icon}" style="font-size: 18px;"></i>
            <span>${message}</span>
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
