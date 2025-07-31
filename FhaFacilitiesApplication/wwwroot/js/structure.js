function loadStructure() {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    var buildingGuid = $('input[name="buildingRadio"]:checked').data('unique-guid');

    $.ajax({
        url: baseUrl + 'Structure/_GetStructure',
        type: 'GET',
        data: { campusGuid: campusGuid, buildingGuid: buildingGuid },
        success: function (html) {
            $('#structureController').html(html);
            if ($('input[name="buildingRadio"]:checked').val() > 0) {
                $("#btnStructureAdd").prop('disabled', false)
            }
            loadConduitPath();
            loadSplice();
        }
    });
}

function loadStructureForm(uniqueId = null) {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    $('#addStructureModal').modal('show');

    $.ajax({
        url: baseUrl + 'Structure/_StructureCreate',
        type: 'GET',
        data: { uniqueId: uniqueId, campusGuid: campusGuid },
        success: function (result) {
            $('#addStructureModelContainer').html(result);
        },
        error: function () {
            $('#addStructureModelContainer').html("<p class='text-danger'>Failed to load form.</p>");
        }
    });
}

$(document).ready(function () {
    // Enable/disable edit/delete buttons on radio change
    $(document).on('change', 'input[name="structureRadio"]', function () {
        const selected = $('input[name="structureRadio"]:checked');
        const isValidStructureId = selected.val() > 0;
        $('#btnStructureEdit, #btnStructureDelete').prop('disabled', !isValidStructureId);
        loadConduitPath();
        loadSplice();
    });

    // Add Structure
    $(document).on('click', '#btnStructureAdd', function (e) {
        e.preventDefault();
        const campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        const buildingGuid = $('input[name="buildingRadio"]:checked').data('unique-guid');
        if (!campusGuid || !buildingGuid) {
            alert("Please select a campus and building before adding a structure.");
            return;
        }
        loadStructureForm();
    });

    // Edit Structure
    $(document).on('click', '#btnStructureEdit', function (e) {
        e.preventDefault();
        const id = $('input[name="structureRadio"]:checked').val();
        if (id) {
            loadStructureForm(id);
        }
    });

    // Show delete confirmation modal
    $(document).on('click', '#btnStructureDelete', function () {
        const selected = $('input[name="structureRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteStructureProceed").data("id", id);
            $("#btnDeleteStructureProceed").data("guid", guid);
            $('#delete-structure-modal').modal('show');
        }
    });

    // Confirm delete
    $(document).on('click', '#btnDeleteStructureProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'Structure/DeleteStructure',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueID: id, UniqueGUID: guid }),
                success: function (response) {
                    loadStructure();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-structure-modal').modal('hide');
                }
            });
        }
    });

    // Structure Type change: load models
    $(document).on('change', '#ddlStructureType', function () {
        var structureType = $(this).val();
        var selectedMaterialGuid = $('#hdnSelectedMaterialGuid').val();
        var $modelDropdown = $('#ddlStructureModel');
        $modelDropdown.empty();

        if (structureType === 'Room') {
            $modelDropdown
                .append('<option value="00000000-0000-0000-0000-000000000000">N/A</option>')
                .prop('disabled', true);
            return;
        }

        $.ajax({
            url: '/Structure/GetStructureModelByType',
            type: 'GET',
            data: { structureType: structureType },
            success: function (materials) {
                $modelDropdown.empty().append('<option value="">Select Structure Model</option>');
                $.each(materials, function (i, item) {
                    var option = $('<option>', {
                        value: item.value,
                        text: item.text
                    });
                    if (selectedMaterialGuid && item.value === selectedMaterialGuid) {
                        option.prop('selected', true);
                    }
                    $modelDropdown.append(option);
                });
                $modelDropdown.prop('disabled', false);
            },
            error: function () {
                $modelDropdown
                    .empty()
                    .append('<option value="">Error loading</option>')
                    .prop('disabled', true);
            }
        });
    });

    // Location Description change: load building IDs
    $(document).on('change', '#ddlLocationDescription', function () {
        const selectedText = $(this).find("option:selected").text().toLowerCase();
        const $buildingDropdown = $('#ddlBuildingID');
        if (selectedText === 'street') {
            $buildingDropdown
                .empty()
                .val('00000000-0000-0000-0000-000000000000')
                .prop('disabled', true);
        } else {
            $buildingDropdown.prop('readonly', false);
            const campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
            if (campusGuid) {
                $.ajax({
                    url: '/Structure/GetBuildingIds',
                    type: 'GET',
                    data: { campusGuid: campusGuid },
                    success: function (response) {
                        $buildingDropdown.empty().append('<option value="">Select Building</option>');
                        $.each(response, function (i, item) {
                            $buildingDropdown.append(
                                $('<option>', {
                                    value: item.value,
                                    text: item.text
                                })
                            );
                        });
                        $buildingDropdown.prop('disabled', false);
                    },
                    error: function () {
                        $buildingDropdown
                            .empty()
                            .append('<option value="">Failed to load buildings</option>')
                            .prop('disabled', true);
                    }
                });
            }
        }
    });

    // Save (Add/Update) Structure
    $(document).on('click', '#btnSubmitStructure', function (e) {
        e.preventDefault();
        if (!$('#structureForm').valid()) return;

        const structureData = {
            CampusGuid: $('input[name="campusRadio"]:checked').data('unique-guid'),
            StructureType: $('#ddlStructureType').val(),
            LocationDescription: $('#ddlLocationDescription').val(),
            Latitude: parseFloat($('#StructureLatitude').val()) || 0,
            Longitude: parseFloat($('#StructureLongitude').val()) || 0,
            Comments: $('#StructureComments').val(),
            StructureModel: $('#ddlStructureModel').val() || null, 
            BuildingGuid: $('#ddlBuildingID').val() || null,
            TypeGuid: $('#ddlStructureModel').val() || null, 
            StructureId: $('#StructureId').val(),
            UniqueGUID: $('#hdnStructureUniqueGuId').val(),
            UniqueId: $('#hdnStructureUniqueId').val()
        };

        $.ajax({
            url: baseUrl + 'Structure/SaveStructure',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(structureData),
            success: function (response) {
                toastrmssg({ message: response.message, type: response.type });
                if (response.type !== 'error') {
                    $('#addStructureModal').modal('hide');
                    loadStructure();
                }
            }
        });
    });
});