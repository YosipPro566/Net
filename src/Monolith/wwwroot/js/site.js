// Global helpers for AgriculturalWebsite

function toggleFav(btn, id, type) {
    var $btn = $(btn);
    $btn.prop('disabled', true);

    $.post('/Home/ToggleFavorite', { itemId: id, itemType: type })
        .done(function (res) {
            $btn.prop('disabled', false);
            if (res && res.success) {
                if (res.isFavorite) {
                    $btn.html('❤️').removeClass('btn-outline-danger').addClass('btn-danger');
                    showToast(res.message || "Додано в обране ❤️");
                } else {
                    $btn.html('🤍').removeClass('btn-danger').addClass('btn-outline-danger');
                    showToast(res.message || "Видалено з обраного 🤍");
                }
            }
        })
        .fail(function (xhr) {
            $btn.prop('disabled', false);
            if (xhr.status === 401) {
                if (confirm("Щоб додавати в обране, потрібно увійти в систему. Перейти до входу?")) {
                    window.location.href = "/Identity/Account/Login";
                }
            } else {
                alert("Помилка при роботі з обраним. Спробуйте пізніше.");
            }
        });
}

function showToast(message) {
    var toastId = 'toast_' + Date.now();
    var toastHtml = `
        <div id="${toastId}" class="toast align-items-center text-white bg-dark border-0 shadow-lg position-fixed top-0 end-0 m-4" style="z-index: 99999; border-radius: 12px; background: rgba(35, 25, 15, 0.95) !important;" role="alert" aria-live="assertive" aria-atomic="true">
            <div class="d-flex p-2">
                <div class="toast-body fs-6 fw-bold text-white">
                    ${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast" aria-label="Close"></button>
            </div>
        </div>
    `;
    $('body').append(toastHtml);
    var toastEl = document.getElementById(toastId);
    var toast = new bootstrap.Toast(toastEl, { delay: 2000 });
    toast.show();
    toastEl.addEventListener('hidden.bs.toast', function () {
        $('#' + toastId).remove();
    });
}
