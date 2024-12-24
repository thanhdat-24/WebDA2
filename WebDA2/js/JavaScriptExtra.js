
    function togglePassword() {
        var passwordField = document.getElementById("password-field");
        if (passwordField.type === "password") {
            passwordField.type = "text";
        } else {
            passwordField.type = "password";
        }
    }
var modal = document.getElementById('id01');

window.onclick = function (event) {
    if (event.target == modal) {
        modal.style.display = "none";
    }
}
/*JS làm tròn phần trăm giá giảm*/
    document.querySelectorAll('.product_sale').forEach(function (element) {
        var percent = parseFloat(element.textContent);
        if (!isNaN(percent)) {
            element.textContent = percent.toFixed(0) + '%';
        }
    });
/*END JS làm tròn phần trăm giá giảm*/

/*Ẩn hiện mật khẩu đăng nhập*/
    function togglePassword() {
        var passwordInput = document.getElementById("passwordInput");
    var toggleIcon = document.getElementById("toggleIcon");

    if (passwordInput.type === "password") {
        passwordInput.type = "text";
    toggleIcon.classList.remove("fa-low-vision");
    toggleIcon.classList.add("fa-eye");
        } else {
        passwordInput.type = "password";
    toggleIcon.classList.remove("fa-eye");
    toggleIcon.classList.add("fa-low-vision");
        }
    }

