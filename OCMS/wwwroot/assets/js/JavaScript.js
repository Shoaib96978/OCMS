

const MyApp = (function ($) {

    // app loader
    const Loader = {
        show() { $('#app-loader').removeClass('hide'); },
        hide() { $('#app-loader').addClass('hide'); }
    };

    // hide laoder once DOM ready 
    $(function () {
        setTimeout(() => Loader.hide(), 400);
    });


    const Toaster = {
        _icons: {
            success: '<i class="bi bi-check-circle-fill text-success"></i>',
            error: '<i class="bi bi-x-circle-fill text-danger"></i>',
            warning: '<i class="bi bi-exclamation-triangle-fill text-warning"></i>',
            info: '<i class="bi bi-info-circle-fill text-info"></i>'
        },
        show(fotype = 'in', title = '', message = '', durration = 4000) {
            id = 'toast-' + Date.now();
            icon = this._icons[type] || this._icons.info;
            const html = `
            <div class="toast-item toast-${type}" id="${id}" role="alert">
            <div class="toast-icon">${icon}</div>
            <div class="taost-body">
            <div class="toast-title">${title}</div>
            ${message ? `<div class="toast-msg">${message}</div>` : ''}
            </div>
            <button class="toast-close" onclick="OCMS.Toast.dismiss('${id}')">&times;</button>
            </div>
            `;
            $('#toast-container').append(html);
            if (durration > 0) {
                setTimeout(this.dismiss(id), durration);
            }

        },
        dismiss(id) {
            const $t = $('#' + id);
            $t.addClass('toast-out');
            setTimeout($t.remove(), 300);
        },

        success(title, msg) { this.show('success', title, msg); },
        error(title, msg) { this.show('error', title, msg); },
        warning(title, msg) { this.show('warning', title, msg); },
        info(title, msg) { this.show('info', title, msg); }


    };

});

const ConfirmModal = {
    _resolve: null,
    show(title = "Are you sure?", text = "This action can not be undone", icon = "⚠️", confirmText = "Confirm", cancelText = "Cancel", type = "danger") {
        return new Promise((resolve) => {

        })
    }
};