function loadBuildingsByCampus() {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    $.ajax({
        url: baseUrl + 'Building/_GetBuildings',
        type: 'GET',
        data: { campusGuid: campusGuid },
        success: function (html) {
            $('#buildingController').html(html);
            loadStructure();
            if ($('input[name="campusRadio"]:checked').val() > 0) {
                $("#btnAddBuilding").prop('disabled', false)
            }
           
        }
    });
}

function loadBuildingForm(id) {
    $('#addBuildingModal').modal('show');
    $.ajax({
        url: baseUrl + 'Building/_CreateBuilding',
        type: 'GET',
        data: id ? { uniqueId: id } : {},
        success: function (result) {
            $('#addBuildingModelContainer').html(result);
        },
        error: function () {
            $('#addBuildingModelContainer').html("<p class='text-danger'>Failed to load form.</p>");
        }
    });
}

$(document).ready(function () {
    // Enable/disable edit/delete buttons on radio change
    $(document).on('change', 'input[name="buildingRadio"]', function () {
        const selectedBuilding = $('input[name="buildingRadio"]:checked');
        const isValidBuilding = selectedBuilding.val() > 0;
        $('#btnEditBuilding, #btnDeleteBuilding').prop('disabled', !isValidBuilding);
        // Optionally, update structure or other UI here
        const selectedCampus = $('input[name="campusRadio"]:checked');
        const selectedBuildingGuid = selectedBuilding.data('unique-guid');
        const selectedCampusGuid = selectedCampus.data('unique-guid');
        loadStructure(selectedCampusGuid, selectedBuildingGuid);
    });

    // Add Building
    $(document).on('click', '#btnAddBuilding', function (e) {
        e.preventDefault();
        loadBuildingForm();
    });

    // Edit Building
    $(document).on('click', '#btnEditBuilding', function (e) {
        e.preventDefault();
        const id = $('input[name="buildingRadio"]:checked').val();
        if (id) {
            loadBuildingForm(id);
        }
    });

    // Show delete confirmation modal
    $(document).on('click', '#btnDeleteBuilding', function () {
        const selected = $('input[name="buildingRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteBuildingProceed").data("id", id);
            $("#btnDeleteBuildingProceed").data("guid", guid);
            $('#delete-building-modal').modal('show');
        }
    });

    // Confirm delete
    $(document).on('click', '#btnDeleteBuildingProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'Building/DeleteBuilding',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueID: id, UniqueGUID: guid }),
                success: function (response) {
                    loadBuildingsByCampus();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-building-modal').modal('hide');
                }
            });
        }
    });

    // Save (Add/Update) Building
    $(document).on('click', '#btnSubmitBuilding', function (e) {
        e.preventDefault();
        if (!$('#buildingForm').valid()) return;

        const uniqueGUIDVal = $('#hdnUniqueGUID').val();
        const parsedGUID = uniqueGUIDVal && uniqueGUIDVal.trim() !== "" ? uniqueGUIDVal : null;
        const campusGuid =$('input[name="campusRadio"]:checked').data('unique-guid');

        const buildingData = {
            UniqueId: parseInt($('#hdnUniqueId').val()) || 0,
            CampusGuid: campusGuid,
            BuildingId: $('#BuildingId').val(),
            Designation: $('#BuildingDesignation').val(),
            Latitude: parseFloat($('#BuildingLatitude').val()) || 0,
            Longitude: parseFloat($('#BuildingLongitude').val()) || 0,
            Comments: $('#BuildingComments').val(),
            UniqueGUID: parsedGUID
        };

        $.ajax({
            url: baseUrl + 'Building/SaveBuilding',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(buildingData),
            success: function (response) {
                loadBuildingsByCampus();
                toastrmssg({ message: response.message, type: response.type });
                $('#addBuildingModal').modal('hide');
            }
        });
    });
});
