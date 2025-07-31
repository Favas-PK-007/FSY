$(document).ready(function () {
    // View Circuits Modal Trigger
    $(document).on('click', '#btnViewCircuits', function () {
        var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        if (!campusGuid) {
            toastrmssg({ message: "You must select a Campus in order to view its Assigned Circuits.", type: "fail" });
        }

        $.ajax({
            url: baseUrl + 'Circuit/_CreateCircuit',
            type: 'GET',
            data: { campusGuid },
            success: function (result) {
                $('#circuitListContainer').html(result);
                $('#circuitListModal').modal('show');
            },
            error: function () {
                $('#circuitListContainer').html("<p class='text-danger'>Failed to load circuit.</p>");
            }
        });
    });

    // Building dropdown changed
    $(document).on('change', '#ddlCircuitBuildingList', function () {
        var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        var buildingGuid = $(this).val();

        $.ajax({
            url: baseUrl + 'Circuit/_GetSplicesByBuilding',
            type: 'GET',
            data: { campusGuid, buildingGuid },
            success: result => $('#circuitSpliceLocationContainer').html(result),
            error: () => $('#circuitSpliceLocationContainer').html("<p class='text-danger'>Failed to load splices.</p>")
        });
    });

    // Splice location changed
    $(document).on('change', '#ddlCircuitSpliceLocation', function () {
        var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        var spliceGuid = $(this).val();

        $.ajax({
            url: baseUrl + 'Circuit/_GetCableBySplice',
            type: 'GET',
            data: { campusGuid, spliceGuid },
            success: result => $('#circuitCableContainer').html(result),
            error: () => $('#circuitCableContainer').html("<p class='text-danger'>Failed to load cable list.</p>")
        });
    });

    // Cable changed → Load Fibers
    $(document).on('change', '#ddlCircuitCable', function () {
        var selectedCable = $(this).val();
        if (!selectedCable) {
            toastrmssg({ message: "Please select a cable to continue.", type: "fail" });
            return;
        }

        $.ajax({
            url: baseUrl + 'Circuit/_GetFibersByCable',
            type: 'GET',
            data: { cableGuid: selectedCable, reloadFibers: true },
            beforeSend: () => $('#fiberTreeViewContainer').html('<p>Loading fibers...</p>'),
            success: result => $('#fiberTreeViewContainer').html(result),
            error: () => $('#fiberTreeViewContainer').html("<p class='text-danger'>Failed to load fiber tree.</p>")
        });
    });

    // Expand all on initial load
    $('.tree-view ul.child').show();
    $('.tree-view .indicator i').removeClass('fa-chevron-right').addClass('fa-chevron-down');

    // Expand/collapse toggle
    $(document).on('click', '.tree-view .indicator', function (e) {
        e.stopPropagation();
        var icon = $(this).find('i');
        var childUl = $(this).closest('li').children('ul.child');
        if (childUl.length) {
            childUl.slideToggle(200);
            icon.toggleClass('fa-chevron-right fa-chevron-down');
        }
    });

    // Propagate checkbox state to children
    $(document).on('change', '.form-check-input', function () {
        var isChecked = $(this).is(':checked');
        $(this).closest('li').find('.form-check-input').prop('checked', isChecked);
    });

    // Circuit ID generation on reserved building change
    $(document).on('change', '#ddlReservedBuilding', function () {
        var circuit = $('#txtCircuitDescription').val();
        var building = $('#ddlReservedBuilding option:selected').data('uniqueid');
        if (!building) return;

        $.ajax({
            url: baseUrl + 'Circuit/GetCircuit',
            type: 'GET',
            data: { circuitIdText: circuit, buildingReservedId: building },
            success: response => {
                if (response) {
                    $('#txtCircuitDescription').val(response);
                } else {
                    toastrmssg({ message: "Something went wrong while setting circuit id.", type: "fail" });
                }
            }
        });
    });

    // Move Right (Assign Fibers)
    $(document).on('click', '#btnMoveRight', function () {
        var selectedFiberIds = $('.fiber-checkbox:checked').map(function () {
            return $(this).val();
        }).get();
        var cable = $('#ddlCircuitCable').val();
        var selectedReservedBuilding = $('#ddlReservedBuilding option:selected').val();
        var circuitId = $('#txtCircuitDescription').val();
        // Check if no fibers are selected
        if (selectedFiberIds.length === 0) {
            toastrmssg({ message: "Please select at least one circuit/fiber to assign.", type: "fail" });
            return;
        }

        // Check if no building is selected
        if (!selectedReservedBuilding || selectedReservedBuilding === "0") {
            $('#fiber-assignment-modal').modal('show');
            return;
        }

        $.ajax({
            url: baseUrl + 'Circuit/AssignSelectedCircuits',
            type: 'POST',
            data: JSON.stringify({
                selectedFiberGuids: selectedFiberIds,
                CableGuid: cable,
                CircuitId: circuitId
            }),
            contentType: 'application/json',
            success: function (result) {
                $('#fiberTreeViewContainer').html(result);
            },
            error: function () {
                $('#fiberTreeViewContainer').html("<p class='text-danger'>Failed to update fiber tree.</p>");
            }
        });
    });

    $(document).on('click', '#btnAcknowledgeFiberWarning', function () {
        $('#fiber-assignment-modal').modal('hide');
    });


    // Move Left (Remove selected circuits)
    $(document).on('click', '#btnMoveLeft', function () {
        var selectedFiberIds = $('.fiber-checkbox:checked').map(function () {
            return $(this).val();
        }).get();

        var cable = $('#ddlCircuitCable').val();

        if (selectedFiberIds.length === 0) {
            toastrmssg({ message: "Please select at least one circuit/fiber to remove.", type: "fail" });
            return
        }

       
        $.ajax({
            url: baseUrl + 'Circuit/RemoveSelectedCircuits',
            type: 'POST',
            data: JSON.stringify({
                selectedFiberGuids: selectedFiberIds,
                CableGuid: cable
            }),
            contentType: 'application/json',
            success: function (result) {
                $('#fiberTreeViewContainer').html(result);
            },
            error: function () {
                $('#fiberTreeViewContainer').html("<p class='text-danger'>Failed to update fiber tree.</p>");
            }
        });
    });
});
