function loadCampusForm(id) {

    $('#addCampusModal').modal('show');
    $.ajax({
        url: baseUrl + 'Campus/_CampusCreate',
        type: 'GET',
        data: { uniqueID: id },
        success: function (result) {
            $('#addCampusModelContainer').html(result);
        },
        error: function () {
            $('#addCampusModelContainer').html("<p class='text-danger'>Failed to load form.</p>");
        }
    });
}

function loadCampusPartial() {
    $.ajax({
        url: baseUrl + 'Campus/_CampusController',
        type: 'GET',
        success: function (html) {
            $('#campusControllerDiv').html(html);
            loadBuildingsByCampus();
           
        }
    });
}

$(document).ready(function () {
    $(document).on('click', '#btnEditCampus', function (e) {
        e.preventDefault();
        const id = $('input[name="campusRadio"]:checked').val();
        loadCampusForm(id);
    });
    $(document).on('change', 'input[name="campusRadio"]', function () {

        selectRadio = $('input[name="campusRadio"]:checked');
        isValidRadio = selectRadio.val() > 0;
        $('#btnEditCampus, #btnDeleteCampus').prop('disabled', !isValidRadio);
        $('#btnViewCircuits').prop('disabled', !isValidRadio);
        loadBuildingsByCampus(selectRadio.data('unique-guid'));
    });

    $(document).on('click', '#btnAddCampus', function (e) {
        e.preventDefault();
        loadCampusForm();
    });


    $(document).on('click', '#btnDeleteCampus', function () {
        const selected = $('input[name="campusRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');

        if (id && guid) {
            $("#btnDeleteCampusProceed").data("id", id);
            $("#btnDeleteCampusProceed").data("guid", guid);
            $('#delete-campus-modal').modal('show');
        }
    });

    $(document).on('click', '#btnDeleteCampusProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");

        if (id && guid) {
            $.ajax({
                url: baseUrl+'Campus/DeleteCampus',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueID: id, UniqueGUID: guid }),
                success: function (response) {
                    loadCampusPartial();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-campus-modal').modal('hide');
                }
            });
        }
    });

    $(document).on('click', '#btnSubmitCampus', function (e) {
        e.preventDefault();
        if (!$('#campusForm').valid()) return;

        const uniqueGUIDVal = $('#hdnUniqueGUID').val();
        const parsedGUID = uniqueGUIDVal && uniqueGUIDVal.trim() !== "" ? uniqueGUIDVal : null;

        const campusData = {
            UniqueId: parseInt($('#hdnUniqueId').val()) || 0,
            CampusId: $('#CampusId').val(),
            Designation: $('#Designation').val(),
            Latitude: parseFloat($('#Latitude').val()) || 0,
            Longitude: parseFloat($('#Longitude').val()) || 0,
            Comments: $('#Comments').val(),
            UniqueGUID: parsedGUID
        };

        $.ajax({
            url: baseUrl + 'Campus/SaveCampus',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(campusData),
            success: function (response) {
                loadCampusPartial();
                toastrmssg({ message: response.message, type: response.type });
                $('#addCampusModal').modal('hide');
            }
        });
    });
});