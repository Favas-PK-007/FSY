function loadSplice() {
    var campusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
    var structureGuid = $('input[name="structureRadio"]:checked').data('unique-guid');

    $.ajax({
        url: baseUrl + 'Splice/_GetSplices',
        type: 'GET',
        data: { campusGuid: campusGuid, structureGuid: structureGuid },
        success: function (html) {
            $('#SpliceController').html(html);
            loadCableBySplies();
            if ($('input[name="campusRadio"]:checked').val() > 0 && $('input[name="structureRadio"]:checked').val() > 0) {
                $("#btnAddSplices").prop('disabled', false)
            }
        }
    });
}

function loadSpliceForm(uniqueGuid = null, selectedCampusGuid = null, selectedStructureGuid = null) {
    const data = {
        uniqueGuid: uniqueGuid,
        selectedCampusGuid: selectedCampusGuid,
        selectedStructureGuid: selectedStructureGuid
    };
    $.ajax({
        url: baseUrl + 'Splice/_CreateSplice',
        type: 'GET',
        data: data,
        success: function (result) {
            $('#addConnectionEnclosureModelContainer').html(result);
            $('#addConnectionEnclosureModal').modal('show');
        },
        error: function () {
            $('#addConnectionEnclosureModelContainer').html("<p class='text-danger'>Failed to load connection form.</p>");
        }
    });
}

function loadConnectionList(splice) {
    $.ajax({
        url: baseUrl + 'Splice/_GetConnectionList',
        type: 'GET',
        data: {
            spliceGuid: splice,
            reloadPorts: true 
        },
        success: function (html) {
            $('#viewConnectionsModalContainer').html(html);
            $('#viewConnectionsModal').modal('show');
        },
        error: function () {
            $('#connectionListContainer').html("<p class='text-danger'>Failed to load connection list.</p>");
        }
    });
}

function getTableDataForCSV() {
    var tableData = [];

    var table = document.getElementById("tblSplicingSlots");
    var tbody = table.querySelector("tbody");
    var rows = tbody.querySelectorAll("tr");

    rows.forEach(function (row) {
        var cells = row.querySelectorAll("td");

        // Skip rows with no input (i.e., "No slots available")
        if (cells.length < 5 || cells[0].querySelector("input[type='radio']") === null) return;

        var rowData = {
            Selected: cells[0].querySelector("input[type='radio']").checked,
            CartridgeID: cells[1].textContent.trim(),
            PortID: cells[2].textContent.trim(),
            ConnectionType: cells[3].textContent.trim(),
            FiberCircuit: cells[4].textContent.trim()
        };

        tableData.push(rowData);
    });

    return tableData;
}


$(document).ready(function () {
    // Handle splice radio change
    $(document).on('change', 'input[name="splicesRadio"]', function () {
        loadCableBySplies();
        $("#btnViewConnection").prop('disabled', false)

        const isSelected = $('input[name="splicesRadio"]:checked').length > 0;
        $('#btnEditSplice, #btnDeleteSplice').prop('disabled', !isSelected);
    });



    // Root toggle
    $(document).on("click", "#toggleBufferList", function () {
        const $icon = $(this).find("i");
        $("#bufferList").slideToggle(200);
        $icon.toggleClass("fa-chevron-right fa-chevron-down");
    });

    // Buffer -> Ribbon toggle
    $(document).on("click", "#toggleRibbonList", function () {
        const $icon = $(this).find("i");
        $("#ribbonList").slideToggle(200);
        $icon.toggleClass("fa-chevron-right fa-chevron-down");
    });

    // Ribbon -> Fiber toggle
    $(document).on("click", "#toggleFiberList", function () {
        const $icon = $(this).find("i");
        $("#fiberList").slideToggle(200);
        $icon.toggleClass("fa-chevron-right fa-chevron-down");
    });


    $(document).on("click", "#toggleFiberList-CableA", function () {
        $("#fiberList-CableA").slideToggle(200);
        $(this).find("i").toggleClass("fa-chevron-right fa-chevron-down");
    });

    // Toggle Cable B
    $(document).on("click", "#toggleBufferList-CableB", function () {
        $("#bufferList-CableB").slideToggle(200);
        $(this).find("i").toggleClass("fa-chevron-right fa-chevron-down");
    });

    $(document).on("click", "#toggleRibbonList-CableB", function () {
        $("#ribbonList-CableB").slideToggle(200);
        $(this).find("i").toggleClass("fa-chevron-right fa-chevron-down");
    });

    $(document).on("click", "#toggleFiberList-CableB", function () {
        $("#fiberList-CableB").slideToggle(200);
        $(this).find("i").toggleClass("fa-chevron-right fa-chevron-down");
    });








    $(document).on('click', '#btnAddSplices', function () {
        const selectedCampusGuid = $('input[name="campusRadio"]:checked').data('unique-guid');
        const selectedStructureGuid = $('input[name="structureRadio"]:checked').data('unique-guid');
        loadSpliceForm(null, selectedCampusGuid, selectedStructureGuid);
    });

    $(document).on('click', '#btnEditSplice', function () {
        const selectedSpliceGuid = $('input[name="splicesRadio"]:checked').data('unique-guid');
        loadSpliceForm(selectedSpliceGuid);
    });

    $(document).on('change', '#SpliceEquipmentType', function () {
        const selectedType = $(this).val();
        console.log("Selected Type:", selectedType);

        $.ajax({
            url: baseUrl + 'Splice/GetSpliceModel',
            type: 'GET',
            data: {
                selectedEquipmentType: selectedType
            },
            success: function (data) {
                const modelDropdown = $('#SpliceEquipmentModel');
                modelDropdown.empty();

                modelDropdown.append(
                    $('<option>', {
                        value: '',
                        text: 'Select Equipment Model'
                    })
                );

                if (Array.isArray(data.equipmentModelList)) {
                    data.equipmentModelList.forEach(function (item) {
                        modelDropdown.append(
                            $('<option>', {
                                value: item.value,
                                text: item.text
                            })
                        );
                    });
                } else {
                    console.warn('No models returned.');
                }
            },
            error: function (xhr) {
                console.error("API Error:", xhr.responseText);
                toastr.error('Failed to load equipment models.');
            }
        });
    });

    $(document).on('click', '#btnSubmitSplice', function (e) {
        e.preventDefault();

        // Validate form
        if (!$('#connectionEnclosureForm').valid()) return;

        // Prepare payload
        const splicePayloadData = {
            UniqueId: parseInt($('#hdnSpliceUniqueId').val()) || 0,
            UniqueGUID: $('#hdnSpliceUniqueGUID').val(),
            SpliceType: $('#SpliceInstallationType').val(),
            StructureGUID: $('#SpliceStructureId').val(),
            SpliceID: $('#SpliceEquipmentId').val(),
            SelectedEquipmentType: $('#SpliceEquipmentType').val(),
            TypeGUID: $('#SpliceEquipmentModel').val(),
            Comments: $('#SpliceComments').val(),
            CampusGUID: $('input[name="campusRadio"]:checked').data('unique-guid') || null
        };

        // Submit via AJAX
        $.ajax({
            url: baseUrl + 'Splice/SaveSplice',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(splicePayloadData),
            success: function (response) {
                console.log('Response:', response);
                toastrmssg({ message: response.message, type: response.type });

                if (response.type !== 'error') {
                    $('#addConnectionEnclosureModal').modal('hide');
                    loadSplice();
                }
            }
        });
    });

    $(document).on('click', '#btnDeleteSplice', function () {
        const selected = $('input[name="splicesRadio"]:checked');
        const id = selected.val();
        const guid = selected.data('unique-guid');
        if (id && guid) {
            $("#btnDeleteSpliceProceed").data("id", id);
            $("#btnDeleteSpliceProceed").data("guid", guid);
            $('#delete-splice-modal').modal('show');
        }
    });

    // Confirm delete
    $(document).on('click', '#btnDeleteSpliceProceed', function () {
        const id = $(this).data("id");
        const guid = $(this).data("guid");
        if (id && guid) {
            $.ajax({
                url: baseUrl + 'Splice/DeleteSplice',
                type: 'POST',
                contentType: "application/json",
                data: JSON.stringify({ UniqueGUID: guid }),
                success: function (response) {
                    loadSplice();
                    toastrmssg({ message: response.message, type: response.type });
                    $('#delete-splice-modal').modal('hide');
                }
            });
        }
    });


    $(document).on('click', '#btnAddComponent', function () {
        const selectedTray = $('input[name="SelectedComponent"]:checked').val(); // Tray/Module radio
        const selectedModelGuid = $('#SpliceEquipmentModel').val();   

        console.log("Selected Tray:", selectedTray);
        if (!selectedModelGuid) {
            toastr.error("Please select a model to add a component.");
            return;
        }

        console.log("Calling API with:", selectedModelGuid);

        $.ajax({
            url: baseUrl + 'Splice/_CreateComponent',
            type: 'GET',
            data: {
                parentMaterialGuid: selectedModelGuid, 
                trayGuid: selectedTray                 
            },
            success: function (html) {
                $('#addComponentModal .modal-content').html(html);
                $('#addComponentModal').modal('show');
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                toastr.error("Failed to load component form.");
            }
        });
    });

    $(document).on('click', '#btnSaveComponent', function () {
        var selectedTray = $('input[name="SelectedComponent"]:checked').val(); // Tray or Module radio
        var spliceGuid = $('#SpliceEquipmentModel').val();                     // Parent model
        var selectedModelGuid = $('#ComponentModel').val();                   // Component dropdown
        var materialType = $('#hdnMaterialType').val();   

        const parentMaterialGuid = selectedTray || spliceGuid;

        if (!spliceGuid || !selectedModelGuid) {
            toastr.error("Please select a splice and model to add a component.");
            return;
        }
        $.ajax({
            url: baseUrl + 'Splice/SaveComponent',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify({
                EquipmentModelGuid: parentMaterialGuid,
                SelectedComponentModelGuid: selectedModelGuid,
                MaterialType: materialType
            }),
            success: function (response) {
                toastrmssg({ message: response.message, type: response.type });
                if (response.type !== 'error') {
                    $('#addComponentModal').modal('hide');
                    loadSplice();
                }
            },
            error: function (xhr, status, error) {
                console.error("AJAX Error:", status, error);
                toastr.error("Failed to add component.");
            }
        });
    });

    $(document).on('click', '#btnViewConnection', function () {
        const selectedSplice = $('input[name="splicesRadio"]:checked').data('unique-guid');
        if (!selectedSplice) {
            toastrmssg({ message: "Please select a Connection Enclosure to view connections.", type: fail  });
            return;
        }
        loadConnectionList(selectedSplice);
    });

    $(document).on('change', '#ddlCableA', function () {
        var cableGuid = $(this).val(); // get selected cable
        var spliceGuid = $('input[name="splicesRadio"]:checked').data('unique-guid'); // get data attribute from selected splice

        console.log("Cable GUID:", cableGuid);
        console.log("Splice GUID:", spliceGuid);

        // Validate selection
        if (!cableGuid || !spliceGuid) {
            toastrmssg({
                message: "Please select Cable A and Splice.",
                type: "fail"
            });
            $('#fiberTreeAContainer').html('');
            return;
        }

        // Call partial view to get fiber tree for Cable A
        $.ajax({
            url: baseUrl + 'Cable/_GetFiberTree',
            type: 'GET',
            data: {
                cableGuid: cableGuid,
                spliceGuid: spliceGuid,
                treeId: "Cable A"
            },
            success: function (html) {
                $('#fiberTreeAContainer').html(html);
            },
            error: function () {
                $('#fiberTreeAContainer').html("<p class='text-danger'>Failed to load fiber tree.</p>");
            }
        });
    });


    $(document).on('change', '#ddlCableB', function () {
        var cableGuid = $(this).val(); // get selected cable
        var spliceGuid = $('input[name="splicesRadio"]:checked').data('unique-guid'); // get data attribute from selected splice

        // Call partial view to get fiber tree for Cable B
        $.ajax({
            url: baseUrl + 'Cable/_GetFiberTree',
            type: 'GET',
            data: {
                cableGuid: cableGuid,
                spliceGuid: spliceGuid,
                treeId:"Cable B"
            },
            success: function (html) {
                $('#fiberTreeBContainer').html(html);
            },
            error: function () {
                $('#fiberTreeBContainer').html("<p class='text-danger'>Failed to load fiber tree.</p>");
            }
        });
    });


    // Trigger export on button click
    // Show confirmation modal when export button is clicked
    $(document).on('click', '#btnExportReport', function () {
        $('#export-confirm-modal').modal('show');
    });

    // Proceed with export if confirmed
    $(document).on('click', '#btnConfirmExport', function () {
        $('#export-confirm-modal').modal('hide');

        const tableData = getTableDataForCSV();
        if (!tableData || tableData.length === 0) {
            toastr.message("No data available to export.", "Export Failed");
            return;
        }

        $.ajax({
            url: baseUrl + 'Splice/ExportPanelReport',
            type: 'POST',
            data: JSON.stringify(tableData),
            contentType: 'application/json',
            xhrFields: { responseType: 'blob' },

            success: function (blob) {
                const url = URL.createObjectURL(blob);
                const link = document.createElement('a');
                link.href = url;
                link.download = 'PanelPortsReport.csv';
                document.body.appendChild(link);
                link.click();
                document.body.removeChild(link);
                URL.revokeObjectURL(url);
            }
        });
    });

});
