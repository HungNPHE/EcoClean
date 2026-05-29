// ── EcoClean Global JS ────────────────────────────────────────

// Toast notification helper
function showToast(message, type = 'success') {
    const id   = 'toast_' + Date.now();
    const icon = type === 'success' ? 'bi-check-circle-fill' : type === 'danger' ? 'bi-x-circle-fill' : 'bi-info-circle-fill';
    const html = `
        <div id="${id}" class="toast align-items-center text-bg-${type} border-0 mb-2" role="alert">
            <div class="d-flex">
                <div class="toast-body">
                    <i class="bi ${icon} me-2"></i>${message}
                </div>
                <button type="button" class="btn-close btn-close-white me-2 m-auto" data-bs-dismiss="toast"></button>
            </div>
        </div>`;
    let container = document.getElementById('toastContainer');
    if (!container) {
        container = document.createElement('div');
        container.id = 'toastContainer';
        container.className = 'toast-container position-fixed bottom-0 end-0 p-3';
        document.body.appendChild(container);
    }
    container.insertAdjacentHTML('beforeend', html);
    const toastEl = document.getElementById(id);
    const toast   = new bootstrap.Toast(toastEl, { delay: 3500 });
    toast.show();
    toastEl.addEventListener('hidden.bs.toast', () => toastEl.remove());
}

// Confirm delete with prettier dialog
function confirmDelete(formEl, message = 'Bạn có chắc muốn xoá?') {
    if (confirm(message)) formEl.submit();
    return false;
}

// Format number with thousand separator
function fmtNum(n) {
    return new Intl.NumberFormat('vi-VN').format(n);
}

// Auto-highlight active nav link
document.addEventListener('DOMContentLoaded', () => {
    const path = window.location.pathname.toLowerCase();
    document.querySelectorAll('.navbar-nav .nav-link').forEach(link => {
        const href = link.getAttribute('href')?.toLowerCase() ?? '';
        if (href !== '/' && path.startsWith(href)) {
            link.classList.add('fw-bold');
        }
    });

    // Animate number counters on dashboard
    document.querySelectorAll('[data-count]').forEach(el => {
        const target = parseInt(el.dataset.count, 10);
        let current  = 0;
        const step   = Math.ceil(target / 40);
        const timer  = setInterval(() => {
            current = Math.min(current + step, target);
            el.textContent = fmtNum(current);
            if (current >= target) clearInterval(timer);
        }, 20);
    });
});

// Image preview before upload
function previewImage(input, imgId) {
    const file = input.files[0];
    if (!file) return;
    const reader = new FileReader();
    reader.onload = e => {
        const img = document.getElementById(imgId);
        if (img) { img.src = e.target.result; img.classList.remove('d-none'); }
    };
    reader.readAsDataURL(file);
}

// Debounce for search inputs
function debounce(fn, delay = 400) {
    let t;
    return (...args) => { clearTimeout(t); t = setTimeout(() => fn(...args), delay); };
}

// BMI quick calculator (used in profile preview)
function calcBMI(weight, height) {
    if (!weight || !height) return null;
    return weight / Math.pow(height / 100, 2);
}

function bmiCategory(bmi) {
    if (bmi < 18.5) return { label: 'Thiếu cân',   color: 'info' };
    if (bmi < 25)   return { label: 'Bình thường', color: 'success' };
    if (bmi < 30)   return { label: 'Thừa cân',    color: 'warning' };
    return              { label: 'Béo phì',     color: 'danger' };
}
