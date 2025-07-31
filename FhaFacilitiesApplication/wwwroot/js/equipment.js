function loadCreateEquipmentForm(connectionTypeGuid) {
    $.ajax({
        url: baseUrl + 'Equipment/_CreateEquipment',
        type: 'GET',
        data: { connectionGuid: connectionTypeGuid },
        success: function (result) {
            $('#addEquipmentModalContainer').html(result);
            $('#addEquipmentModal').modal('show');

            $('#ddlEquipmentType').prop('disabled', true);
            $('#ddlEquipmentModel').prop('disabled', true);
            $('#ddlPortId').prop('disabled', true);

            if ($('#chkNewEquipment').is(':checked')) {
                // If "New Equipment" is checked, show textbox, hide dropdown
                $('#textboxEquipmentId').show();
                $('#ddlEquipmentId').hide();
            } else {
                // If unchecked, show dropdown, hide textbox
                $('#textboxEquipmentId').hide();
                $('#ddlEquipmentId').show();
            }
        }

    });
}



$(document).ready(function () {
    $(document).on('click', '#btnAddEquipment', function () {
        var selectedRadio = $("input[name='SelectedSlotId']:checked");

        if (selectedRadio.length === 0) {
            toastrmssg({ message: "Please select a tray", type: "fail" });
            return;
        }

        var connectionTypeGuid = selectedRadio.data("connection-guid");

        loadCreateEquipmentForm(connectionTypeGuid);
    });

   
    $(document).on('change', '#chkNewEquipment', function () {
        if ($(this).is(':checked')) {
            $('#ddlEquipmentId').closest('.col-sm-6').hide();  // Hide dropdown container
            $('#textboxEquipmentId').show();                   // ✅ Show full textbox container

            $('#ddlEquipmentType').prop('disabled', false);
            $('#ddlEquipmentModel').prop('disabled', false);
            $('#ddlPortId').prop('disabled', false);
        } else {
            $('#ddlEquipmentId').closest('.col-sm-6').show();
            $('#textboxEquipmentId').hide();

            $('#ddlEquipmentType').prop('disabled', true);
            $('#ddlEquipmentModel').prop('disabled', true);
            $('#ddlPortId').prop('disabled', true);
        }
    });

    $(document).on('change', '#ddlEquipmentType', function () {

        var selectedEquipemtType = $("#ddlEquipmentType").val();
        if (selectedEquipemtType) {
            $.ajax({
                url: baseUrl + 'Equipment/_GetEquipmentModel',
                type: 'GET',
                data: { equipmentType: selectedEquipemtType },
                success: function (result) {
                    $('#equipmentModelDropdownContainer').html(result);
                    $('#ddlEquipmentModel').prop('disabled', false);
                }
            });
        }

    });

    $(document).on('change', '#ddlEquipmentModel', function () {

        var equipmentType = $('#ddlEquipmentType').val();
        var selectedEquipmentModel = $('#ddlEquipmentModel').val();
        var equimentStructureGuid = $('#hdnEquipmentStructureGuid').val();
        var equipmentSpliceGuid = $('#hdnEquipmentSpliceGuid').val();
        var equipmentTypeGuid = $('#hdnEquipmentTypeGuid').val();
        var equipmentUniqueId = $('#hdnEquipmentUniqueId').val();
        var isChecked = $('#chkNewEquipment').is(':checked');

        var selectedEquipmentId = isChecked
            ? $('#txtNewEquipmentId').val()
            : $('#ddlEquipmentId').val();

       

        $.ajax({
            url: baseUrl + 'Equipment/_GetPortId',
            type: 'GET',
            data: {
                equipmentType: equipmentType,
                selectedEquipmentModel: selectedEquipmentModel,
                equimentStructureGuid: equimentStructureGuid,
                equipmentSpliceGuid: equipmentSpliceGuid,
                equipmentTypeGuid: equipmentTypeGuid,
                selectedEquipmentId: selectedEquipmentId,
                equipmentUniqueId: equipmentUniqueId,
                isChecked: isChecked
            },
            success: function (response) {
                $('#portDropdownContainer').html(response);
                $('#ddlPortId').prop('disabled', false);
            }
        });
    });

});

