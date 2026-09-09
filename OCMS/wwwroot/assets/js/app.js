/**
 * OCMS - App.js
 * Global utilities: AJAX helper, Toaster, AppLoader,
 * Confirm Modal, Button Spinner
 * All AJAX calls go through OCMS.ajax() and handle
 * the unified AppResponse { Success, Message, RedirectUrl, Data }
 */

const OCMS = (function ($) {
    'use strict';

    /* ─── App Loader ─────────────────────────────────────── */
    const Loader = {
        show() { $('#app-loader').removeClass('hide'); },
        hide() { $('#app-loader').addClass('hide'); }
    };

    /* hide loader once DOM ready */
    $(function () {
        setTimeout(() => Loader.hide(), 400);
    });

    /* ─── Toaster ─────────────────────────────────────────── */
    const Toast = {
            _icons: {
                success: '<i class="bi bi-check-circle-fill text-success"></i>',
                error: '<i class="bi bi-x-circle-fill text-danger"></i>',
                warning: '<i class="bi bi-exclamation-triangle-fill text-warning"></i>',
                info: '<i class="bi bi-info-circle-fill text-info"></i>'
            },

        show(fotype = 'in', title = '', message = '', duration = 4000) {
            const id = 'toast-' + Date.now();
            const icon = this._icons[type] || this._icons.info;
            const html = `
        <div class="toast-item toast-${type}" id="${id}" role="alert">
          <div class="toast-icon">${icon}</div>
          <div class="toast-body">
            <div class="toast-title">${title}</div>
            ${message ? `<div class="toast-msg">${message}</div>` : ''}
          </div>
          <button class="toast-close" onclick="OCMS.Toast.dismiss('${id}')">&times;</button>
        </div>`;
            $('#toast-container').append(html);
            if (duration > 0) {
                setTimeout(() => this.dismiss(id), duration);
            }
        },
        
        dismiss(id) {
            const $t = $('#' + id);
            $t.addClass('toast-out');
            setTimeout(() => $t.remove(), 300);
        },

        success(title, msg) { this.show('success', title, msg); },
        error(title, msg) { this.show('error', title, msg); },
        warning(title, msg) { this.show('warning', title, msg); },
        info(title, msg) { this.show('info', title, msg); }
    };

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

    /* ─── Button Spinner ──────────────────────────────────── */
    const BtnSpinner = {
        start($btn) {
            if (!$btn || !$btn.length) return;
            $btn.data('ocms-orig-html', $btn.html());
            $btn.prop('disabled', true);
            const spinner = `<span class="btn-spinner"></span>`;
            $btn.prepend(spinner);
        },

        stop($btn) {
            if (!$btn || !$btn.length) return;
            const orig = $btn.data('ocms-orig-html');
            if (orig) $btn.html(orig);
            $btn.prop('disabled', false);
        }
    };

    /* ─── AJAX Utility ────────────────────────────────────── */
    /**
     * OCMS.ajax(options)
     *
     * Unified AJAX helper. Automatically:
     *  - Shows button spinner on $btn
     *  - Sends jQuery $.ajax
     *  - Handles AppResponse { Success, Message, RedirectUrl, Data }
     *  - Shows toaster on success / error
     *  - Stops spinner when done
     *  - Redirects if RedirectUrl present
     *
     * @param {Object} opts
     * @param {string}        opts.url          - Controller endpoint
     * @param {string}        [opts.method]     - HTTP method (default: POST)
     * @param {*}             [opts.data]       - Request data / FormData
     * @param {jQuery}        [opts.$btn]       - Submit button (for spinner)
     * @param {boolean}       [opts.toastSuccess] - Show success toast (default: true)
     * @param {boolean}       [opts.toastError]   - Show error toast (default: true)
     * @param {Function}      [opts.onSuccess]  - Callback(data) on Success=true
     * @param {Function}      [opts.onError]    - Callback(response) on Success=false
     * @param {Function}      [opts.onComplete] - Always called when done
     * @param {boolean}       [opts.isFormData] - Pass true when sending FormData
     */
    function ajax(opts = {}) {
        const {
            url,
            method = 'POST',
            data = {},
            $btn = null,
            toastSuccess = true,
            toastError = true,
            onSuccess = null,
            onError = null,
            onComplete = null,
            isFormData = false
        } = opts;

        // Start button spinner
        if ($btn) BtnSpinner.start($btn);

        const ajaxOpts = {
            url,
            type: method,
            data,
            success(res) {
                if (res && typeof res === 'object') {

                    const isSuccess = res.success;      // camelCase ✅
                    const message = res.message;      // camelCase ✅
                    const redirectUrl = res.redirectUrl;  // camelCase ✅
                    const data = res.data;         // camelCase ✅

                    if (isSuccess) {
                        if (toastSuccess && message) Toast.success('Success', message);
                        if (typeof onSuccess === 'function') onSuccess(data, res);
                        if (redirectUrl) {
                            setTimeout(() => { window.location.href = redirectUrl; }, 900);
                        }
                    } else {
                        if (toastError && message) Toast.error('Error', message);
                        if (typeof onError === 'function') onError(res);
                    }
                } else {
                    Toast.error('Error', 'Unexpected server response.');
                }
            },
            error(xhr) {
                const msg = xhr.responseJSON?.Message || xhr.statusText || 'Request failed.';
                if (toastError) Toast.error('Request Failed', msg);
                if (typeof onError === 'function') onError(xhr.responseJSON || {});
            },
            complete() {
                if ($btn) BtnSpinner.stop($btn);
                if (typeof onComplete === 'function') onComplete();
            }
        };

        if (isFormData) {
            ajaxOpts.processData = false;
            ajaxOpts.contentType = false;
        } else {
            ajaxOpts.contentType = 'application/json';
            ajaxOpts.data = typeof data === 'string' ? data : JSON.stringify(data);
        }

        $.ajax(ajaxOpts);
    }

    /* ─── Form Helpers ────────────────────────────────────── */
    const Form = {
        /** Clear all validation error states on a form */
        clearErrors($form) {
            $form.find('.is-invalid').removeClass('is-invalid');
            $form.find('.ocms-error-msg').text('');
        },

        /** Show field-level error */
        setError($input, msg) {
            $input.addClass('is-invalid');
            $input.siblings('.ocms-error-msg').text(msg);
        },

        /** Simple required-field validation */
        validate($form) {
            this.clearErrors($form);
            let valid = true;
            $form.find('[required]').each(function () {
                const $f = $(this);
                if (!$f.val().trim()) {
                    Form.setError($f, 'This field is required.');
                    valid = false;
                }
            });
            return valid;
        }
    };

    /* ─── Wire up confirm modal buttons ──────────────────── */
    $(function () {
        $(document).on('click', '#confirm-ok-btn', () => Confirm._ok());
        $(document).on('click', '#confirm-cancel-btn', () => Confirm._cancel());
        $(document).on('click', '#confirm-modal-backdrop', function (e) {
            if ($(e.target).is('#confirm-modal-backdrop')) Confirm._cancel();


        });
    });

    /* ─── Public API ──────────────────────────────────────── */
    return { ajax, Toast, Confirm, BtnSpinner, Form, Loader };

})(jQuery);
