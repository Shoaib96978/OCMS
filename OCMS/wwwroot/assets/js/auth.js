$(function () {

    // -─ LOGIN — only runs if login form exists on this page ──
    if ($('#login-form').length) {

        $('#toggle-password').on('click', function () {
            const $inp = $('#password');
            const isText = $inp.attr('type') === 'text';
            $inp.attr('type', isText ? 'password' : 'text');
            $(this).find('i').toggleClass('bi-eye bi-eye-slash');
        });

        $('#login-form').on('submit', function (e) {
            e.preventDefault();

            const form = document.getElementById('login-form');
            const formData = new FormData(form);
            const btn = document.getElementById('login-btn');

            OCMS.ajaxPostForm('/Auth/Login', formData, btn);
        });
    }

    // ── REGISTER — only runs if register form exists ─────────
    if ($('#register-form').length) {

        $('.toggle-pw').on('click', function () {
            const target = $(this).data('target');
            const $inp = $('#' + target);
            $inp.attr('type', $inp.attr('type') === 'text' ? 'password' : 'text');
            $(this).find('i').toggleClass('bi-eye bi-eye-slash');
        });

        $('#password').on('input', function () {
            const val = $(this).val();
            if (!val) { $('#pwd-strength').hide(); return; }
            $('#pwd-strength').show();
            let score = 0;
            if (val.length >= 8) score++;
            if (/[A-Z]/.test(val)) score++;
            if (/[0-9]/.test(val)) score++;
            if (/[^A-Za-z0-9]/.test(val)) score++;
            const colors = ['#ef4444', '#f97316', '#eab308', '#16a34a'];
            const labels = ['Weak', 'Fair', 'Good', 'Strong'];
            $('#strength-bar').css({ width: (score * 25) + '%', background: colors[score - 1] || '#ef4444' });
            $('#strength-label').text(labels[score - 1] || '').css('color', colors[score - 1] || '#ef4444');    
        });

        $('#register-form').on('submit', function (e) {
            e.preventDefault();
            if (!$('#agree').is(':checked')) {
                $('#agree-error').text('You must agree to the terms.');
                return;
            }
            $('#agree-error').text('');
            if ($('#password').val() !== $('#confirmPassword').val()) {
                $('#confirmPassword').siblings('.ocms-error-msg').text('Passwords do not match.');
                return;
            }
            OCMS.ajaxPostForm('/Auth/Register', new FormData(this),
                document.getElementById('register-btn'));
        });
    }

});