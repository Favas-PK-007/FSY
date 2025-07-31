function loadConduitPath() {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    var structureGuid = $('input[name="structureRadio"]:checked').data('unique-guid');


    $.ajax({
        url: baseUrl + 'ConduitPath/_GetConduitsAndSplices',
        type: 'GET',
        data: { campusGuid: campusGuid, structureGuid: structureGuid },
        success: function (html) {
            $('#conduitPathController').html(html);
            loadDuctsByConduitPath();
            if ($('input[name="campusRadio"]:checked').val() > 0 && $('input[name="structureRadio"]:checked').val() > 0) {
                $("#btnAddConduits").prop('disabled', false)

            }
        }
    });
}

function loadConduitPathForm(conduitGuid = null, campusGuid = null) {
    $('#addConduitPathModal').modal('show');

    const data = {};
    if (conduitGuid) data.conduitGuid = conduitGuid;
    if (campusGuid) data.campusGuid = campusGuid;

    $.ajax({
        url: baseUrl + 'ConduitPath/_CreateConduit',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addConduitPathModelContainer').html(result);
        },
        error: function () {
            $('#addConduitPathModelContainer').html("<p class='text-danger'>Failed to load conduit form.</p>");
        }
    });
}




$(document).ready(function () {
    // Enable/disable buttons based on conduit path selection
    $(document).on('change', 'input[name="ConduitPathRadio"]', function () {
        loadDuctsByConduitPath();

        const isSelected = $('input[name="ConduitPathRadio"]:checked').length > 0;

        $('#btnStructureEdit, #btnStructureDelete, #btnBuildingEdit, #btnBuildingDelete, #btnEditConduit, #btnDeleteConduit')
            .prop('disabled', !isSelected);
    });

    // Add Conduit
    $(document).on('click', '#btnAddConduits', function () {
        var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        loadConduitPathForm(null, campusGuid);
    });

    // Edit Conduit
    $(document).on('click', '#btnEditConduit', function () {
        const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
        const campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');

        if (conduitGuid) {
            loadConduitPathForm(conduitGuid, campusGuid);
        } else {
            alert("Please select a conduit path ID to edit.");
        }
    });

    // Show delete confirmation modal
    $(document).on('click', '#btnDeleteConduit', function () {
        const selected = $('input[name="ConduitPathRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteConduitProceed").data("id", id);
            $("#btnDeleteConduitProceed").data("guid", guid);
            $('#delete-conduit-modal').modal('show');
        }
    });

    // Confirm delete
    $(document).on('click', '#btnDeleteConduitProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'ConduitPath/DeleteBuilding',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueID: id, UniqueGUID: guid }),
                success: function (response) {
                    loadConduitPath();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-conduit-modal').modal('hide');
                }
            });
        }
    });

    // Submit Conduit Form
    $(document).on('click', '#btnSubmitConduit', function (e) {
        e.preventDefault();

        if (!$('#conduitPathForm').valid()) return;

        const conduitPathData = {
            UniqueId: $('#hdnConduitUniqueId').val(),
            UniqueGuid: $('#hdnConduitUniqueGuid').val(),
            CampusGuid: $('#hdnCampusGuid').val(),
            ConduitId: $('#ConduitId').val(),
            StructureAGuid: $('#ddlStructureA').val(),
            StructureBGuid: $('#ddlStructureB').val(),
            Comments: $('#ConduitComments').val()
        };

        $.ajax({
            url: baseUrl + 'ConduitPath/SaveConduit',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(conduitPathData),
            success: function (response) {
                toastrmssg({ message: response.message, type: response.type });
                $('#addConduitPathModal').modal('hide');
                loadConduitPath();
            }
        });
    });

});

