function loadCableBySplies() {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    var spliceGuid = $('input[name="splicesRadio"]:checked').data('unique-guid');

    $.ajax({
        url: baseUrl + 'Cable/_GetAllCables',
        type: 'GET',
        data: { campusGuid: campusGuid, spliceGuid: spliceGuid },
        success: function (html) {
            $('#cableController').html(html);
            if ($('input[name="campusRadio"]:checked').val() > 0 && $('input[name="splicesRadio"]:checked').val() > 0) {
                $("#btnAddCable").prop('disabled', false);
            }
        }
    });
}

function loadCableForm(campusGuid = null, cableGuid = null) {
    const data = {};
    if (campusGuid) data.campusGuid = campusGuid;
    if (cableGuid) data.cableGuid = cableGuid;

    $.ajax({
        url: baseUrl + 'Cable/_CreateCable',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addCableModelContainer').html(result);
            $('#addCableModel').modal('show');
        },
        error: function () {
            $('#addCableModelContainer').html("<p class='text-danger'>Failed to load cable form.</p>");
        }
    });
}


$(document).ready(function () {
    // Enable/Disable edit/delete buttons when a cable is selected
    $(document).on('click', 'input[name="cableRadio"]', function () {
        const isSelectedCable = $('input[name="cableRadio"]:checked').length > 0;
        $('#btnEditCable, #btnDeleteCable').prop('disabled', !isSelectedCable);
        $('#btnLinkToDuct').prop('disabled', !isSelectedCable);
    });


    // Add Cable button
    $(document).on('click', '#btnAddCable', function () {
        var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        loadCableForm(campusGuid);
    });

    // Edit Cable button
    $(document).on('click', '#btnEditCable', function () {
        const campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        const selectedCable = $('input[name="cableRadio"]:checked').data('unique-guid');
        loadCableForm(campusGuid, selectedCable);
    });

    // Delete Cable button
    $(document).on('click', '#btnDeleteCable', function () {
        const selected = $('input[name="cableRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteCableProceed").data("id", id);
            $("#btnDeleteCableProceed").data("guid", guid);
            $('#delete-cable-modal').modal('show');
        }
    });

    // Confirm delete
    $(document).on('click', '#btnDeleteCableProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'Cable/DeleteCable',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueGUID: guid }),
                success: function (response) {
                    loadCableBySplies();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-cable-modal').modal('hide');
                }
            });
        }
    });

    // Submit Cable (Add/Edit)
    $(document).on('click', '#btnSubmitCable', function (e) {
        e.preventDefault();

        if (!$('#cableForm').valid()) return;

        const cablePostData = {
            UniqueID: $('#hdnCableUniqueId').val(),
            UniqueGUID: $('#hdnCableUniqueGUID').val(),
            CableType: $('#CableType').val(),
            TypeGUID: $('#CableModel').val(),
            CableID: $('#CableTextID').val(),
            SpliceAGUID: $('#SpliceA').val(),
            SpliceBGUID: $('#SpliceB').val(),
            Fibers: $('#CableFibers').val(),
            Comments: $('#CableComments').val(),
            CampusGUID: $('input[name="campusRadio"]:checked').data('unique-guid'),
            DuctGUID: $('input[name="ductRadio"]:checked').data('unique-guid') || null
        };

        $.ajax({
            url: baseUrl + 'Cable/SaveCable',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(cablePostData),
            success: function (response) {
                toastrmssg({ message: response.message, type: response.type });
                if (response.type !== 'error') {
                    $('#addCableModel').modal('hide');
                    loadCableBySplies();
                }
            }
        });

    });

    $(document).on('click', '#btnAddFiber', function (e) {
        e.preventDefault();

        var cableModelGuid = $('#CableModel').val();
        var cableGuid = $('#hdnCableUniqueGUID').val(); // From your hidden field in the form

        if (!cableModelGuid || cableModelGuid === '') {
            toastr.warning('Please select a Cable Model before generating fibers.');
            return;
        }

        if (!cableGuid || cableGuid === '') {
            toastr.warning('Please select or save a Cable first.');
            return;
        }

        console.log("Cable Model GUID:", cableModelGuid);
        console.log("Cable GUID:", cableGuid);

        $.ajax({
            url: baseUrl + 'Cable/_GetGenerateFibers',
            type: 'GET',
            data: {
                cableModelGuid: cableModelGuid,
                cableGuid: cableGuid
            },
            success: function (html) {
                $('#GenerateFibersDiv').html(html); // <== Loads partial view content here
            }
        });
    });


});
