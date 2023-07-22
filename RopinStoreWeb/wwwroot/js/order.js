var dataTable;

$(document).ready(function () {
    loadDataTable();
});

//function loadDataTable() {
//    dataTable = $('#tblData').DataTable({
//        "ajax": {
//            "url": "/Admin/Order/GetAll"
//        },
//        "columns": [
//            { "data": "id", "width": "5%" },
//            { "data": "name", "width": "15%" },
//            { "data": "phoneNumber", "width": "10%" },
//                        { "data": "applicationUser.email", sDefaultContent: '', "width": "15%" },
//            { "data": "orderStatus", "width": "15%" },
//            { "data": "paymentType", "width": "10%" },
//            { "data": "orderTotal", "width": "10%" },
//            {
//                "data": "id",
//                "render": function (data) {
//                    return `
//                        <div class="w-75 btn-group" role="group">
//                        <a href="/Admin/Order/Details?orderId=${data}"
//                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                      
//					</div>
//                        `
//                },
//                "width": "5%"
//            }
//        ]
//    });
//}

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
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                        <a href="/Admin/Order/Details?orderId=${data}"
                        class="btn btn-primary mx-2"> <i class="bi bi-pencil-square"></i></a>
                      
					</div>
                        `
                },
                "width": "5%"
            }
        ]
    });
}