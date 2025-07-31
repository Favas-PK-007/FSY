$(document).ready(function () {
    $(".navbar-toggler ").click(function () {
        $(".menu-overlay").toggleClass("active");
    });
    $(".menu-overlay").click(function () {
        $(this).toggleClass("active");
    });

    $.ajaxSetup({
        beforeSend: function () {
            $("#SpinnerOverlay").show();
        },
        error: function (xhr, status, error) {
            if (xhr.status === 401) {
                $("#register").modal("hide");
                $("#login").modal("show");
                toastrmssg({ title: "Unauthorized", message: "Please login to continue", type: 'error' });
            } else {
                var resp = jQuery.parseJSON(xhr.responseText);
                toastrmssg({ message: resp.message, type: resp.type });
            }
        },
        complete: function () {
            $("#SpinnerOverlay").hide();
        }
    });
});