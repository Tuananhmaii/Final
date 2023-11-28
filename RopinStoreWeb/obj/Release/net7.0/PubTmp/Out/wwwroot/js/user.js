var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/User/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "fullName", "width": "20%" },
            { "data": "email", "width": "15%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "city", "width": "10%" },
            { "data": "state", "width": "10%" },
            { "data": "zipCode", "width": "5%" },
        ]
    });

    $('#tblData tbody').on('click', 'td', function () {
        // Get the data in the first column of the clicked row
        var rowData = dataTable.row($(this).closest('tr')).data();

        // Check if rowData is not undefined
        if (typeof rowData !== 'undefined') {
            // Navigate to the specified URL using the data from the first column
            window.location.href = `/Admin/User/Edit?id=${rowData.id}`;
        }
    });

    $('#tblData tbody').on('mouseenter', 'tr', function () {
        $(this).addClass('hovered');
    }).on('mouseleave', 'tr', function () {
        $(this).removeClass('hovered');
    });

    // Attach a click event to all cells in the table (same as previous code)
    $('#tblData tbody').on('click', 'td', function () {
        // ...
    });
}