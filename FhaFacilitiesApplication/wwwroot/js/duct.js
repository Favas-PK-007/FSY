function loadDuctsByConduitPath() {
    var conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');


    $.ajax({
        url: baseUrl + 'Duct/_GetAllDucts',
        type: 'GET',
        data: { conduitGuid: conduitGuid },
        success: function (html) {
            $('#ductController').html(html);
            if ($('input[name="ConduitPathRadio"]:checked').val() > 0) {
                $("#btnAddDuct").prop('disabled', false)
            }
        }
    });
}


function loadDuctForm(conduitGuid = null, structureAGuid = null, ductGuid = null) {

    const data = {};
    if (conduitGuid) data.conduitGuid = conduitGuid;
    if (structureAGuid) data.structureAGuid = structureAGuid;
    if (ductGuid) data.ductGuid = ductGuid;

    $.ajax({
        url: baseUrl + 'Duct/_CreateDuct',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addDuctModelContainer').html(result);
            $('#addDuctModal').modal('show');
        },
        error: function () {
            $('#addDuctModelContainer').html("<p class='text-danger'>Failed to load duct form.</p>");
        }
    });
}

function loadSubDuctForm(conduitGuid = null, structureAGuid = null, ductGuid = null) {

    const data = {};
    if (conduitGuid) data.conduitGuid = conduitGuid;
    if (structureAGuid) data.structureAGuid = structureAGuid;
    if (ductGuid) data.ductGuid = ductGuid;

    $.ajax({
        url: baseUrl + 'Duct/_CreateSubDuct',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addSubDuctModelContainer').html(result);
            $('#addSubDuctModal').modal('show');
        },
        error: function () {
            $('#addSubDuctModelContainer').html("<p class='text-danger'>Failed to load duct form.</p>");
        }
    });
}

function loadLinkDuctForm(cableGuid = null, structureGuid = null) {
    const data = {};
    if (cableGuid) data.cableGuid = cableGuid;
    if (structureGuid) data.structureGuid = structureGuid;

    // Show modal and clone loader from template
    const loaderHtml = $('#linkDuctLoaderTemplate').clone().removeAttr('id').removeClass('d-none');
    $('#addLinkDuctModalContainer').html(loaderHtml);
    $('#addLinkDuctModal').modal('show');

    // AJAX to load actual form
    $.ajax({
        url: baseUrl + 'Duct/_CreateLinkToDuct',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addLinkDuctModalContainer').html(result);
        },
        error: function () {
            const errorHtml = $('#linkDuctErrorTemplate').clone().removeAttr('id').removeClass('d-none');
            $('#addLinkDuctModalContainer').html(errorHtml);
        }
    });
}



$(document).ready(function () {
    $(document).on('click', '#btn-add-sub-duct', function () {
        const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
        const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
        const selectedDuctGuid = $('#hdnDuctUniqueGUID').val();

        loadSubDuctForm(conduitGuid, structureAGuid, selectedDuctGuid);
    });
    $(document).on('click', 'input[name="ductRadio"]', function () {
        const isSelected = $('input[name="ductRadio"]:checked').length > 0;
        $('#btnEditDuct, #btnDeleteDuct').prop('disabled', !isSelected);
    });
    // Enable/Disable edit/delete based on selected duct
    $(document).on('click', 'input[name="ductRadio"]', function () {
        const isSelected = $('input[name="ductRadio"]:checked').length > 0;
        $('#btnEditDuct, #btnDeleteDuct').prop('disabled', !isSelected);
    });

    $(document).on('click', '#btn-delete-sub-duct', function () {
        const selectedSubDuct = $('input[name="sub_duct_radio_duct"]:checked').val();

        if (!selectedSubDuct) {
            toastrmssg({ message: "Please select a sub-duct to delete.", type: "error" });
            return;
        }

        $.ajax({
            url: baseUrl + 'Duct/DeleteDuct', 
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({ UniqueGUID: selectedSubDuct }),
            success: function (response) {
                //loadDuctsByConduitPath();
                toastrmssg({ message: response.message, type: response.type });

                const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
                const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
                const selectedDuctGuid = $('#hdnDuctUniqueGUID').val();
                loadDuctForm(conduitGuid, structureAGuid, selectedDuctGuid);
                //$('#addDuctModelContainer').modal('hide');
            }
        });
    });

    // Add Duct Button
    $(document).on('click', '#btnAddDuct', function () {
        const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
        const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');

        if (!conduitGuid || !structureAGuid) {
            toastrmssg({ message: "Please select a conduit and structure first.", type: "warning" });
            return;
        }

        loadDuctForm(conduitGuid, structureAGuid);
    });

    // Edit Duct Button
    $(document).on('click', '#btnEditDuct', function () {
        const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
        const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
        const selectedDuctGuid = $('input[name="ductRadio"]:checked').data('unique-guid');

        if (!selectedDuctGuid) {
            toastrmssg({ message: "Please select a duct to edit.", type: "warning" });
            return;
        }

        loadDuctForm(conduitGuid, structureAGuid, selectedDuctGuid);
    });

    // Save (Add/Update) Building
    $(document).on('click', '#btnSubmitDuct', function (e) {
        e.preventDefault();

        if (!$('#ductForm').valid()) return;

        const uniqueGuidRaw = $('#hdnDuctUniqueGUID').val()?.trim();
        const uniqueGUID = uniqueGuidRaw && uniqueGuidRaw !== "" ? uniqueGuidRaw : null;

        const ductData = {
            UniqueId: parseInt($('#hdnDuctUniqueId').val()) || 0,
            UniqueGuid: uniqueGUID,
            ConduitGuid: $('#hdnConduitGuid').val(),
            StructureGuid: $('#hdnStructureAGuid').val(),
            DuctId: $('#DuctId1').val(),
            DuctIdb: $('#DuctId2').val(),
            DuctTypeGuid: $('#ddlDuctType').val(),
            Comments: $('#txtComments').val(),

            //SubDucts: subDuctList.map(({ Id, ...rest }) => rest) 
        };

        $.ajax({
            url: baseUrl + 'Duct/SaveDuct',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(ductData),
            success: function (response) {
                loadDuctsByConduitPath();
                toastrmssg({ message: response.message, type: response.type });

                const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
                const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
                loadDuctForm(conduitGuid, structureAGuid, response.response);
                //$('#addDuctModelContainer').modal('hide');
            }
        });
    });

    $(document).on('click', '#btnSubmitSubDuct', function (e) {
        e.preventDefault();

        if (!$('#subDuctForm').valid()) return;

        const uniqueGuidRaw = $('#hdnDuctUniqueGUID').val()?.trim();
        const uniqueGUID = uniqueGuidRaw && uniqueGuidRaw !== "" ? uniqueGuidRaw : null;

        var ductData = {
            ConduitGuid: $('#hdnSubDuctConduitGuid').val(),
            DuctId: $('#SubDuctId1').val(),
            DuctIdb: $('#SubDuctId2').val(),
            DuctTypeGuid: $('#ddlSubDuctType').val(),
            Comments: $('#txtSubDuctComments').val(),
            ParentGuid: $('#hdnSubDuctParentGuid').val()
            //SubDucts: subDuctList.map(({ Id, ...rest }) => rest) 
        };

        $.ajax({
            url: baseUrl + 'Duct/SaveSubDuct',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(ductData),
            success: function (response) {
                loadDuctsByConduitPath();
                toastrmssg({ message: response.message, type: response.type });

                const conduitGuid = $('input[name="ConduitPathRadio"]:checked').data('unique-guid');
                const structureAGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
                const ParentGuid = $('#hdnSubDuctParentGuid').val();
                loadDuctForm(conduitGuid, structureAGuid, ParentGuid);
                $('#addSubDuctModal').modal('hide');
            }
        });
    });

    // Delete SubDuct

    $(document).on('click', '#btnDeleteDuct', function () {
        const selected = $('input[name="ductRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteDuctProceed").data("id", id);
            $("#btnDeleteDuctProceed").data("guid", guid);
            $('#delete-Duct-modal').modal('show');
        }
    });
    // Confirm delete
    $(document).on('click', '#btnDeleteDuctProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'Duct/DeleteDuct',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueGUID: guid }),
                success: function (response) {
                    loadDuctsByConduitPath();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-Duct-modal').modal('hide');
                }
            });
        }
    });

    $(document).on('click', '#btnLinkToDuct', function () {
        var cable = $('input[name="cableRadio"]:checked').data('unique-guid');
        var structure = $('input[name="structureRadio"]:checked').data('unique-guid');

        if (!cable) {
            toastrmssg({
                message: "Please select a cable before proceeding.",
                type: "fail"
            });
            return;
        }

        $('#ddlLinkDuctConduit').empty();
        $('#ddlLinkDuct').empty(); 
        $('#ddlLinkDuctSubDuct').empty();

        loadLinkDuctForm(cable, structure);
    });

    $(document).on('change', '#ddlLinkDuctConduit', function () {
        $('#ddlLinkDuct').empty(); // Clears the Duct dropdown
        $('#ddlLinkDuctSubDuct').empty(); // Clears the SubDuct dropdown
        var conduit = $('#ddlLinkDuctConduit').val();
        var cableDuct = $('#hdnLinkDuctCableDuctGUID').val();

        if (conduit) {
            $.ajax({
                url: baseUrl + 'Duct/_GetDuctsByConduit',
                type: 'GET',
                data: { conduitGuid: conduit, cableDuctGuid: cableDuct },
                success: function (html) {
                    $('#ddlLinkDuct').html(html);
                },
                error: function () {
                    toastrmssg({ message: "Failed to load ducts for the selected conduit.", type: "error" });
                }
            });
        }
    });

    $(document).on('change', '#ddlLinkDuct', function () {
        var duct = $(this).val();
        var cableDuct = $('#hdnLinkDuctCableDuctGUID').val();

        if (!duct) {
            toastrmssg({ message: "Please select a duct first.", type: "warning" });
            return;
        }

        $.ajax({
            url: baseUrl + 'Duct/_GetSubductsByDuct',
            type: 'GET',
            data: {
                ductGuid: duct,
                cableDuctGuid: cableDuct
            },
            success: function (response) {
                $('#ddlLinkDuctSubDuct').html(response); 
            },
            error: function () {
                $('#ddlLinkDuctSubDuct').html('<option disabled>Error loading subducts</option>');
            }
        });
    });

    $(document).on('click', '#btnSubmitLinkDuct', function (e) {
        e.preventDefault();

        const ductValue = $('#ddlLinkDuct').val();
        const isSubDuctEnabled = !$('#ddlLinkDuctSubDuct').is(':disabled');
        const subDuctValue = $('#ddlLinkDuctSubDuct').val();

        // Check if duct is selected
        if (!ductValue) {
            toastrmssg({ message: "Please select a valid Duct to link the cable.", type: "fail" });
            return;
        }

        // If subduct is enabled, check if it's selected
        if (isSubDuctEnabled && !subDuctValue) {
            toastrmssg({ message: "Please select a valid SubDuct to link the cable.", type: "fail" });
            return;
        }

        // Use subduct if enabled, otherwise use duct
        const ductGuid = isSubDuctEnabled ? subDuctValue : ductValue;

        const linkDuctData = {
            CableGuid: $('#hdnLinkDuctCableGUID').val(),
            DuctGuid: ductGuid,
            Comments: $('#txtLinkDuctComments').val()
        };

        // Call API to save
        $.ajax({
            url: baseUrl + 'Duct/SaveLinkToDuct',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(linkDuctData),
            success: function (response) {
                loadDuctsByConduitPath();
                toastrmssg({ message: response.message, type: response.type });
                $('#addLinkDuctModal').modal('hide');
            },
            error: function () {
                toastrmssg({ message: "Failed to link duct.", type: "fail" });
            }
        });
    });


});

