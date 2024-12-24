
    $('.count').each(function () {
        $(this).prop('Counter', 0).animate({
            Counter: $(this).text()
        }, {
            duration: 10000,
            easing: 'swing',
            step: function (now) {
                $(this).text(Math.ceil(now));
            }
        });
    });
/* Star JS  dropdow cho phần Login*/
$(document).ready(function () {
    // Khi nhấn vào "Zeptain Linh", menu sẽ xuất hiện hoặc ẩn đi
    $('#zeptain-link').click(function (event) {
        event.preventDefault(); // Ngăn chặn việc di chuyển link
        $('#zeptain-dropdown').slideToggle(); // Hiện/ẩn menu
    });

    // Ẩn menu khi nhấn bên ngoài
    $(document).click(function (event) {
        if (!$(event.target).closest('#zeptain-link, #zeptain-dropdown').length) {
            $('#zeptain-dropdown').slideUp();
        }
    });
});
/* End JS  dropdow cho phần Login*/

/* Star JS  dropdow cho phần left menu*/
$(document).ready(function () {
    // Khi nhấn vào các thẻ "Bài viết" hoặc "Sản phẩm"
    $('li[data-target="#post"]').click(function () {
        $('#post').slideToggle();
    });

    $('li[data-target="#product"]').click(function () {
        $('#product').slideToggle();
    });

    $('li[data-target="#media"]').click(function () {
        $('#media').slideToggle();
    });
});
/* End JS  dropdow cho phần left menu*/
/*Đoạn mã JavaScript để làm tròn số và giữ lại một số thập phân*/
        document.querySelectorAll('.product_sale').forEach(function (element) {
        var percent = parseFloat(element.textContent);
        if (!isNaN(percent)) {
            element.textContent = percent.toFixed(1) + '%';
        }
        });

/*Đoạn mã js cập nhật ngày giờ*/
    // Hàm cập nhật ngày và giờ
    function updateTime() {
        var currentDate = new Date();

    // Lấy giờ, phút, giây
    var hours = currentDate.getHours();
    var minutes = currentDate.getMinutes();
    var seconds = currentDate.getSeconds();

    // Lấy ngày, tháng, năm
    var day = currentDate.getDate();
    var month = currentDate.getMonth() + 1; // Tháng trong JS bắt đầu từ 0
    var year = currentDate.getFullYear();

    // Đảm bảo giờ, phút, giây có 2 chữ số
    hours = hours < 10 ? '0' + hours : hours;
    minutes = minutes < 10 ? '0' + minutes : minutes;
    seconds = seconds < 10 ? '0' + seconds : seconds;

    // Đảm bảo ngày và tháng có 2 chữ số
    day = day < 10 ? '0' + day : day;
    month = month < 10 ? '0' + month : month;

    // Tạo chuỗi thời gian
    var timeString = hours + ':' + minutes + ':' + seconds;
    var dateString = '(' + day + '/' + month + '/' + year + ')';

    // Cập nhật nội dung vào phần tử với ID 'current-time'
    document.getElementById('current-time').innerHTML = timeString + ' <small>' + dateString + '</small>';
    }

    // Cập nhật mỗi giây
    setInterval(updateTime, 1000);

    // Chạy ngay khi trang được tải
    updateTime();



