// Dynamic sticky header on scroll
window.addEventListener('scroll', function() {
    const header = document.querySelector('.main-header');
    if (window.scrollY > 50) {
        header.classList.add('scrolled');
    } else {
        header.classList.remove('scrolled');
    }
});

// Initialize elements after DOM loads
document.addEventListener('DOMContentLoaded', function () {
    // PT Slider Carousel
    if (document.querySelector('.pt-swiper')) {
        const ptSwiper = new Swiper('.pt-swiper', {
            slidesPerView: 1,
            spaceBetween: 20,
            loop: true,
            autoplay: {
                delay: 4000,
                disableOnInteraction: false,
            },
            pagination: {
                el: '.pt-swiper-pagination',
                clickable: true,
            },
            navigation: {
                nextEl: '.pt-swiper-button-next',
                prevEl: '.pt-swiper-button-prev',
            },
            breakpoints: {
                576: { slidesPerView: 2, spaceBetween: 20 },
                992: { slidesPerView: 3, spaceBetween: 30 }
            }
        });
    }

    // Facilities Slider Carousel
    if (document.querySelector('.facilities-swiper')) {
        const facSwiper = new Swiper('.facilities-swiper', {
            slidesPerView: 1,
            spaceBetween: 20,
            loop: true,
            autoplay: {
                delay: 5000,
                disableOnInteraction: false,
            },
            pagination: {
                el: '.fac-swiper-pagination',
                clickable: true,
            },
            navigation: {
                nextEl: '.fac-swiper-button-next',
                prevEl: '.fac-swiper-button-prev',
            },
            breakpoints: {
                576: { slidesPerView: 2, spaceBetween: 20 },
                992: { slidesPerView: 3, spaceBetween: 30 }
            }
        });
    }

    // Testimonials Carousel (Home page)
    if (document.querySelector('.testimonials-swiper')) {
        const testSwiper = new Swiper('.testimonials-swiper', {
            slidesPerView: 1,
            spaceBetween: 30,
            loop: true,
            autoplay: {
                delay: 4500,
                disableOnInteraction: false,
            },
            navigation: {
                nextEl: '.swiper-btn-next',
                prevEl: '.swiper-btn-prev',
            }
        });
    }

    // Registration Countdown Timer
    // Set target date: End of the current month
    const now = new Date();
    const targetDate = new Date(now.getFullYear(), now.getMonth() + 1, 0, 23, 59, 59);

    function updateCountdown() {
        const currentTime = new Date().getTime();
        const diff = targetDate.getTime() - currentTime;

        if (diff <= 0) {
            if (document.getElementById('cd-days')) {
                document.getElementById('cd-days').innerText = "00";
                document.getElementById('cd-hours').innerText = "00";
                document.getElementById('cd-mins').innerText = "00";
                document.getElementById('cd-secs').innerText = "00";
            }
            return;
        }

        const days = Math.floor(diff / (1000 * 60 * 60 * 24));
        const hours = Math.floor((diff % (1000 * 60 * 60 * 24)) / (1000 * 60 * 60));
        const mins = Math.floor((diff % (1000 * 60 * 60)) / (1000 * 60));
        const secs = Math.floor((diff % (1000 * 60)) / 1000);

        if (document.getElementById('cd-days')) {
            document.getElementById('cd-days').innerText = days < 10 ? '0' + days : days;
            document.getElementById('cd-hours').innerText = hours < 10 ? '0' + hours : hours;
            document.getElementById('cd-mins').innerText = mins < 10 ? '0' + mins : mins;
            document.getElementById('cd-secs').innerText = secs < 10 ? '0' + secs : secs;
        }
    }

    if (document.getElementById('cd-days')) {
        updateCountdown();
        setInterval(updateCountdown, 1000);
    }

    // Auto-dismiss Success Toast after 5 seconds
    const toast = document.getElementById('toastSuccess');
    if (toast) {
        setTimeout(function() {
            toast.style.transition = 'opacity 0.5s ease';
            toast.style.opacity = '0';
            setTimeout(function() {
                toast.remove();
            }, 500);
        }, 5000);
    }

    // Timetable Filtering Logic (LichHoc page)
    const scheduleTabs = document.getElementById('scheduleTabs');
    if (scheduleTabs) {
        const buttons = scheduleTabs.querySelectorAll('button');
        buttons.forEach(button => {
            button.addEventListener('click', function () {
                // Toggle active class in tab list
                buttons.forEach(btn => btn.classList.remove('active'));
                this.classList.add('active');

                const filter = this.getAttribute('data-filter');
                const slots = document.querySelectorAll('.schedule-slot-item');

                slots.forEach(slot => {
                    const classType = slot.getAttribute('data-class');
                    if (filter === 'all' || classType === filter) {
                        slot.classList.remove('dimmed');
                    } else {
                        slot.classList.add('dimmed');
                    }
                });
            });
        });
    }
});

// --- BMI Calculator Interactive Logic ---
function runBmiCalculator() {
    const heightInput = document.getElementById('bmiHeight');
    const weightInput = document.getElementById('bmiWeight');
    const nameInput = document.getElementById('bmiName');
    const phoneInput = document.getElementById('bmiPhone');
    const ageInput = document.getElementById('bmiAge');
    const genderInput = document.getElementById('bmiGender');
    const errorAlert = document.getElementById('bmiErrorAlert');

    // Input values
    const h = parseFloat(heightInput.value);
    const w = parseFloat(weightInput.value);
    const name = nameInput.value.trim();
    const phone = phoneInput.value.trim();
    const age = ageInput.value.trim();
    const gender = genderInput.value;

    // Reset error
    errorAlert.classList.add('d-none');
    errorAlert.innerText = "";

    // Validation checks
    if (isNaN(h) || h <= 0 || isNaN(w) || w <= 0) {
        errorAlert.innerText = "Vui lòng nhập chiều cao và cân nặng hợp lệ!";
        errorAlert.classList.remove('d-none');
        return;
    }
    if (!name || !phone) {
        errorAlert.innerText = "Vui lòng nhập đầy đủ họ tên và số điện thoại để lưu kết quả!";
        errorAlert.classList.remove('d-none');
        return;
    }

    // Compute BMI
    const bmiScore = (w / ((h / 100) * (h / 100))).toFixed(2);
    document.getElementById('bmiScoreDisplay').innerText = bmiScore;

    // Display comparison table and highlight user's classification range
    const bmiVal = parseFloat(bmiScore);
    const tableRows = [
        { label: "Dưới 18.5", status: "Thiếu cân", min: 0, max: 18.49 },
        { label: "18.5 - 24.9", status: "Bình thường", min: 18.5, max: 24.99 },
        { label: "25.0 - 29.9", status: "Dư cân", min: 25.0, max: 29.99 },
        { label: "30.0 - Trên", status: "Béo phì", min: 30.0, max: 100 }
    ];

    const bmiTableContainer = document.getElementById('bmiTableRows');
    bmiTableContainer.innerHTML = "";

    tableRows.forEach(row => {
        const isMatched = bmiVal >= row.min && bmiVal <= row.max;
        const rowDiv = document.createElement('div');
        rowDiv.className = `row p-3 m-0 border-bottom align-items-center ${isMatched ? 'bg-danger text-white fw-bold' : 'text-muted'}`;
        
        const colVal = document.createElement('div');
        colVal.className = "col-6";
        colVal.innerText = row.label;

        const colStatus = document.createElement('div');
        colStatus.className = "col-6 d-flex align-items-center gap-2";
        if (isMatched) {
            const dot = document.createElement('span');
            dot.className = "d-inline-block bg-white rounded-circle";
            dot.style.width = "8px";
            dot.style.height = "8px";
            colStatus.appendChild(dot);
        }
        colStatus.appendChild(document.createTextNode(row.status));

        rowDiv.appendChild(colVal);
        rowDiv.appendChild(colStatus);
        bmiTableContainer.appendChild(rowDiv);
    });

    // Flip views
    document.getElementById('bmiFormWrapper').classList.add('d-none');
    document.getElementById('bmiResultWrapper').classList.remove('d-none');
}

function resetBmiCalculator() {
    document.getElementById('bmiHeight').value = "";
    document.getElementById('bmiWeight').value = "";
    document.getElementById('bmiName').value = "";
    document.getElementById('bmiPhone').value = "";
    document.getElementById('bmiAge').value = "";
    
    document.getElementById('bmiErrorAlert').classList.add('d-none');
    document.getElementById('bmiResultWrapper').classList.add('d-none');
    document.getElementById('bmiFormWrapper').classList.remove('d-none');
}
