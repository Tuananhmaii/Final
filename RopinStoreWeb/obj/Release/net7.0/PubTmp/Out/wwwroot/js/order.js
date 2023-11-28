var dataTable;

$(document).ready(function () {
    loadDataTable();
});

function loadDataTable(status) {
    dataTable = $('#tblData').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "id", "width": "5%" },
            { "data": "name", "width": "20%" },
            { "data": "phoneNumber", "width": "15%" },
            { "data": "applicationUser.email", "width": "15%" },
            { "data": "paymentType", "width": "10%" },
            { "data": "orderStatus", "width": "10%" },
            { "data": "totalPrice", "width": "5%" },
        ]
    });

    $('#tblData tbody').on('click', 'td', function () {
        // Get the data in the first column of the clicked row
        var rowData = dataTable.row($(this).closest('tr')).data();

        // Check if rowData is not undefined
        if (typeof rowData !== 'undefined') {
            // Navigate to the specified URL using the data from the first column
            window.location.href = `/Admin/Order/Details?orderId=${rowData.id}`;
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