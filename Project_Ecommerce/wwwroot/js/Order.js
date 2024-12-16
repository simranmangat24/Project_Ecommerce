var dataTable;

$(document).ready(function () {
    loadDataTable();
})

function loadDataTable() {
    dataTable = $('#tbldata').DataTable({
        "ajax": {
            "url": "/Admin/Order/GetAll"
        },
        "columns": [
            { "data": "orderDate" },
            { "data": "name"},
            { "data": "streetAddress" },
            { "data": "state" },
            { "data": "city" },
            {"data":"orderTotal"},
            {
                "data": "id",
                "render": function (data) {
                    return `
                        <div class="text-center">
                            <a href="/Admin/Order/Summary/${data.id}"  class="btn btn-info">
                                <i class="fas fa-eye"></i>
                            </a>
                        </div>
                    `;
                }
            }
        ]
    })
}
