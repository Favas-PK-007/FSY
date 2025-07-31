let isEditMode = false;

// ============ Helpers ============
function createMaterialDetailRow(header = '', value = '', readOnly = false) {
    var tr = document.createElement('tr');

    var tdCheckbox = document.createElement('td');
    var checkbox = document.createElement('input');
    checkbox.type = 'checkbox';
    checkbox.className = 'form-check-input';
    tdCheckbox.appendChild(checkbox);

    var tdHeader = document.createElement('td');
    var inputHeader = document.createElement('input');
    inputHeader.type = 'text';
    inputHeader.className = 'form-control';
    inputHeader.value = header;
    if (readOnly) inputHeader.setAttribute('readonly', true);
    tdHeader.appendChild(inputHeader);

    var tdValue = document.createElement('td');
    var inputValue = document.createElement('input');
    inputValue.type = 'text';
    inputValue.className = 'form-control';
    inputValue.value = value;
    tdValue.appendChild(inputValue);

    tr.appendChild(tdCheckbox);
    tr.appendChild(tdHeader);
    tr.appendChild(tdValue);

    return tr;
}

function addDefaultMaterialDetailRow() {
    var tableBody = document.getElementById('materialTableBody');
    var newRow = createMaterialDetailRow();
    tableBody.appendChild(newRow);
}

// ============ Load Material Form ============
function loadMaterialForm(isEdit = false) {
    isEditMode = isEdit;

    $('#addMaterialModal').modal('show');
    $('#addMaterialModelContainer').html(`
        <div class="text-center p-5" id="materialLoader">
            <div class="spinner-border text-primary" role="status">
                <span class="visually-hidden">Loading...</span>
            </div>
        </div>`);

    $.ajax({
        url: baseUrl + 'Material/_CreateMaterial',
        type: 'GET',
        data: { isEdit },
        success: function (result) {
            $('#addMaterialModelContainer').html(result);

            if (isEditMode) {
                $('#modelIdTextboxContainer').hide();
                $('#modelIdDropdownContainer').show();
                $('#ddlModelId').prop('disabled', false);
            } else {
                $('#modelIdTextboxContainer').show();
                $('#txtModelId').prop('disabled', false);
                $('#modelIdDropdownContainer').hide();
                $('#ddlModelId').prop('disabled', true);
            }

            addDefaultMaterialDetailRow();
        },
        error: function () {
            $('#addMaterialModelContainer').html("<p class='text-danger p-3'>Failed to load material form.</p>");
        }
    });
}

// ============ Reset Form ============
function resetMaterialForm() {
    $('#ddlParentMaterial').empty().append('<option value="">Select Parent Material</option>').prop('disabled', true);
    $('#ddlManufacturer').empty().append('<option value="">Select Manufacturer</option>');
    $('#txtComments').val('');
    $('#materialTableBody').empty();

    if (isEditMode) {
        $('#modelIdTextboxContainer').hide();
        $('#txtModelId').prop('disabled', true);
        $('#modelIdDropdownContainer').show();
        $('#ddlModelId').empty().append('<option value="">Select Model ID</option>').prop('disabled', false);
    } else {
        $('#modelIdTextboxContainer').show();
        $('#txtModelId').val('').prop('disabled', false);
        $('#modelIdDropdownContainer').hide();
        $('#ddlModelId').empty().append('<option value="">Select Model ID</option>').prop('disabled', true);
    }

    addDefaultMaterialDetailRow();
}

// ============ Load Parent Materials ============
function loadParentMaterials(materialType) {
    $.ajax({
        url: baseUrl + 'Material/GetParentMaterial',
        type: 'GET',
        data: { materialType },
        success: function (data) {
            var ddl = $('#ddlParentMaterial');
            ddl.empty().append('<option value="">Select Parent Material</option>');

            if (data && data.length > 0) {
                data.forEach(item => ddl.append(new Option(item.text, item.value)));
                ddl.prop('disabled', false);
            } else {
                ddl.prop('disabled', true);
                if (!isEditMode) {
                    $('#txtModelId').val('').prop('disabled', false);
                    loadDetailHeaders(materialType);
                }
                loadManufacturers(materialType);
            }
        }
    });
}

// ============ Load Child Materials ============
function loadChildMaterials(parentGuid, materialType) {
    $.ajax({
        url: baseUrl + 'Material/GetChildMaterials',
        type: 'GET',
        data: { parentGuid, materialType },
        success: function (data) {
            var ddl = $('#ddlModelId');
            ddl.empty().append('<option value="">Select Model ID</option>');

            if (data && data.length > 0) {
                data.forEach(m => ddl.append(new Option(m.text, m.value)));
                ddl.prop('disabled', false);
            } else {
                ddl.prop('disabled', true);
            }
        }
    });
}

// ============ Load Manufacturers ============
function loadManufacturers(materialType) {
    $.ajax({
        url: baseUrl + 'Material/GetManufacturers',
        type: 'GET',
        data: { materialType },
        success: function (data) {
            var ddl = $('#ddlManufacturer');
            ddl.empty().append('<option value="">Select Manufacturer</option>');
            data.forEach(name => ddl.append(new Option(name, name)));
        }
    });
}

// ============ Load Detail Headers ============
function loadDetailHeaders(materialType) {
    $.ajax({
        url: baseUrl + 'Material/GetMaterialDetailsHeaders',
        type: 'GET',
        data: { materialType },
        success: function (headers) {
            var tableBody = document.getElementById('materialTableBody');
            tableBody.innerHTML = '';

            var isTextModelVisible = $('#txtModelId').is(':visible');
            var isEquipment = materialType.toLowerCase() === 'equipment';

            if (headers && headers.length > 0) {
                headers.forEach(header => {
                    var row = createMaterialDetailRow(header, '', isTextModelVisible);
                    tableBody.appendChild(row);
                });
            }

            addDefaultMaterialDetailRow();
            $('#btnDeleteMaterialRow').prop('disabled', isTextModelVisible && !isEquipment);
        }
    });
}

// ============ Load Existing Material (Edit) ============
function loadMaterialByGuid(selectedGuid) {
    if (!selectedGuid) return;

    $.ajax({
        url: baseUrl + 'Material/GetMaterialByGuid',
        type: 'GET',
        data: { selectedModelIdGuid: selectedGuid },
        success: function (data) {
            console.log(data);
            if (!data) {
                toastr.error("Failed to fetch material data.");
                return;
            }

            $('#txtComments').val(data.comments);
            $('#hdnMaterialUniqueGuid').val(data.uniqueGUID);
            $('#hdnMaterialUniqueId').val(data.uniqueID);

            var tableBody = document.getElementById('materialTableBody');
            tableBody.innerHTML = '';

            var isTextModelVisible = $('#txtModelId').is(':visible');
            var isEquipment = data.materialType?.toLowerCase() === 'equipment';

            if (data.details) {
                Object.entries(data.details).forEach(([header, value]) => {
                    var row = createMaterialDetailRow(header, value, isTextModelVisible);
                    tableBody.appendChild(row);
                });
            }

            addDefaultMaterialDetailRow();
            $('#btnDeleteMaterialRow').prop('disabled', isTextModelVisible && !isEquipment);
        }
    });
}

// ============ Submit Material ============
$(document).on('click', '#btnSubmitMaterial', function (e) {
    e.preventDefault();

    if (!$('#materialForm').valid()) return;

    var materialDetails = [];
    $('#materialTableBody tr').each(function () {
        var header = $(this).find('td:eq(1) input').val()?.trim();
        var value = $(this).find('td:eq(2) input').val()?.trim();

        if (header) {
            materialDetails.push({ Header: header, Value: value || "" });
        }
    });

    var payload = {
        UniqueID: parseInt($('#hdnMaterialUniqueId').val()) || 0,
        UniqueGuid: $('#hdnMaterialUniqueGuid').val() || null,
        MaterialType: $('#ddlMaterialType').val(),
        ParentGUID: $('#ddlParentMaterial').val() || null,
        ModelID: $('#txtModelId').is(':visible') ? $('#txtModelId').val() : $('#ddlModelId').val(),
        ManufacturerID: $('#ddlManufacturer').val(),
        Comments: $('#txtComments').val(),
        MaterialDetails: materialDetails
    };

    $.ajax({
        url: baseUrl + 'Material/SaveMaterial',
        type: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(payload),
        success: function (res) {
            toastrmssg({ message: res.message, type: res.type });
            $('#addMaterialModal').modal('hide');
        },
        error: function () {
            toastr.error("Failed to save material.");
        }
    });
});

// ============ Event Bindings ============
$(document).ready(function () {
    $(document).on('click', '#btnAddMaterials', () => loadMaterialForm(false));
    $(document).on('click', '#btnEditMaterials', () => loadMaterialForm(true));

    $(document).on('change', '#ddlMaterialType', function () {
        resetMaterialForm();
        var materialType = $(this).val();
        if (!materialType) return;
        loadParentMaterials(materialType);
        var parentGuid = $(this).val();
        if (isEditMode) {
            loadChildMaterials(parentGuid, materialType);
        }
    });

    $(document).on('change', '#ddlParentMaterial', function () {
        var materialType = $('#ddlMaterialType').val();
        var parentGuid = $(this).val();
        if (isEditMode) {
            loadChildMaterials(parentGuid, materialType);
        } else {
            loadManufacturers(materialType);
            loadDetailHeaders(materialType);
        }
    });

    $(document).on('change', '#ddlModelId', function () {
        var selectedGuid = $(this).val();
        loadMaterialByGuid(selectedGuid);
    });

    $(document).on('click', '#btnDeleteMaterialRow', function () {
        $('#materialTableBody input.form-check-input:checked').each(function () {
            $(this).closest('tr').remove();
        });
    });
});
