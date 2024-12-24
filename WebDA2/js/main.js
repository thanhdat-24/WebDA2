'use strict'; // Sử dụng chế độ nghiêm ngặt

(function ($) {

    /*------------------
        Tiền tải (Preloader)
    --------------------*/
    $(window).on('load', function () {
        $(".loader").fadeOut(); // Làm mờ và ẩn bộ tải
        $("#preloder").delay(200).fadeOut("slow"); // Làm mờ tiền tải chậm hơn

        /*------------------
            Bộ lọc danh mục
        --------------------*/
        $('.featured__controls li').on('click', function () {
            $('.featured__controls li').removeClass('active'); // Xóa lớp 'active' khỏi tất cả
            $(this).addClass('active'); // Thêm lớp 'active' vào mục được nhấn
        });

        if ($('.featured__filter').length > 0) {
            var containerEl = document.querySelector('.featured__filter');
            var mixer = mixitup(containerEl); // Sử dụng thư viện mixitup cho bộ lọc
        }


    });

    /*------------------
        Đặt nền (Background Set)
    --------------------*/
    $('.set-bg').each(function () {
        var bg = $(this).data('setbg'); // Lấy hình nền từ thuộc tính `data-setbg`
        $(this).css('background-image', 'url(' + bg + ')'); // Gán hình nền
    });

    // Menu dạng thanh (Humberger Menu)
    $(".humberger__open").on('click', function () {
        $(".humberger__menu__wrapper").addClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").addClass("active");
        $("body").addClass("over_hid"); // Ẩn cuộn trang
    });

    $(".humberger__menu__overlay").on('click', function () {
        $(".humberger__menu__wrapper").removeClass("show__humberger__menu__wrapper");
        $(".humberger__menu__overlay").removeClass("active");
        $("body").removeClass("over_hid"); // Bỏ ẩn cuộn trang
    });

    /*------------------
        Điều hướng (Navigation)
    --------------------*/
    $(".mobile-menu").slicknav({
        prependTo: '#mobile-menu-wrap', // Chèn menu vào phần tử này
        allowParentLinks: true // Cho phép nhấp vào liên kết cha
    });

    /*-----------------------
        Thanh trượt danh mục
    ------------------------*/
    $(".categories__slider").owlCarousel({
        loop: true, // Lặp lại
        margin: 0, // Không có khoảng cách giữa các mục
        items: 4, // Số mục hiển thị
        dots: false,
        nav: true, // Hiển thị nút điều hướng
        navText: ["<span class='fa fa-angle-left'><span/>", "<span class='fa fa-angle-right'><span/>"],
        animateOut: 'fadeOut', // Hiệu ứng mờ dần khi chuyển mục
        animateIn: 'fadeIn', // Hiệu ứng xuất hiện khi mục vào
        smartSpeed: 1200, // Tốc độ chuyển đổi thông minh
        autoHeight: false,
        autoplay: true,
        responsive: {
            0: { items: 1 },
            480: { items: 2 },
            768: { items: 3 },
            992: { items: 4 }
        }
    });

    $('.hero__categories__all').on('click', function () {
        $('.hero__categories ul').slideToggle(400); // Hiển thị/Ẩn danh mục chính
    });

    /*--------------------------
        Thanh trượt sản phẩm mới
    ----------------------------*/
    $(".latest-product__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 1,
        dots: false,
        nav: true,
        navText: ["<span class='fa fa-angle-left'><span/>", "<span class='fa fa-angle-right'><span/>"],
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

    /*-----------------------------
        Thanh trượt sản phẩm giảm giá
    -------------------------------*/
    $(".product__discount__slider").owlCarousel({
        loop: true,
        margin: 0,
        items: 3,
        dots: true,
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true,
        responsive: {
            320: { items: 1 },
            480: { items: 2 },
            768: { items: 2 },
            992: { items: 3 }
        }
    });

    /*---------------------------------
        Thanh trượt hình ảnh chi tiết sản phẩm
    ----------------------------------*/
    $(".product__details__pic__slider").owlCarousel({
        loop: true,
        margin: 20,
        items: 4,
        dots: true,
        smartSpeed: 1200,
        autoHeight: false,
        autoplay: true
    });

    /*-----------------------
        Thanh trượt giá
    ------------------------*/
    var rangeSlider = $(".price-range"),
        minamount = $("#minamount"),
        maxamount = $("#maxamount"),
        minPrice = rangeSlider.data('min'),
        maxPrice = rangeSlider.data('max');
    rangeSlider.slider({
        range: true,
        min: minPrice,
        max: maxPrice,
        values: [minPrice, maxPrice],
        slide: function (event, ui) {
            minamount.val('$' + ui.values[0]);
            maxamount.val('$' + ui.values[1]);
        }
    });
    minamount.val('$' + rangeSlider.slider("values", 0));
    maxamount.val('$' + rangeSlider.slider("values", 1));

    /*--------------------------
        Lựa chọn (Select)
    ----------------------------*/
    $("select").niceSelect();

    /*------------------
        Sản phẩm đơn lẻ
    --------------------*/
    $('.product__details__pic__slider img').on('click', function () {
        var imgurl = $(this).data('imgbigurl');
        var bigImg = $('.product__details__pic__item--large').attr('src');
        if (imgurl != bigImg) {
            $('.product__details__pic__item--large').attr({ src: imgurl });
        }
    });

    /*-------------------
        Thay đổi số lượng
    ---------------------*/
    var proQty = $('.pro-qty');
    proQty.prepend('<span class="dec qtybtn">-</span>');
    proQty.append('<span class="inc qtybtn">+</span>');
    proQty.on('click', '.qtybtn', function () {
        var $button = $(this);
        var oldValue = $button.parent().find('input').val();
        if ($button.hasClass('inc')) {
            var newVal = parseFloat(oldValue) + 1;
        } else {
            // Không cho phép giảm xuống dưới 0
            if (oldValue > 0) {
                var newVal = parseFloat(oldValue) - 1;
            } else {
                newVal = 0;
            }
        }
        $button.parent().find('input').val(newVal);
    });

})(jQuery);
