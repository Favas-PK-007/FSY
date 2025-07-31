toastr.options = {
    "preventDuplicates": true,
    "closeButton": true,
    "progressBar": true
};

toastrmssg = function (result) {
    toastr.clear();
    var toastrMessage = result.message;
    var toastrTitle = result.title;
    var toastrType = result.type;
    if (!toastrTitle) {
        toastrTitle = toastrType.charAt(0).toUpperCase() + toastrType.slice(1);
    }
    var formattedMessage = toastrTitle + '<span>' + toastrMessage + '</span>';
    if (toastrType === "success") {
        toastr.success(formattedMessage);
    } else if (toastrType === "warning") {
        toastr.warning(formattedMessage);
    } else if (toastrType === "error") {
        toastr.error(formattedMessage);
    } else {
        toastr.info(formattedMessage);
    }
}